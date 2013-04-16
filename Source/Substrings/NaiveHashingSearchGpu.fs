module NaiveHashingSearchGpu

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common

let provider = NaiveSearchGpu.provider

let createQueue() = 
    new CommandQueue(provider, provider.Devices |> Seq.head) 

let commandQueue = createQueue()

let close () = 
    commandQueue.Dispose()
    provider.CloseAllBuffers()
    provider.Dispose()

let label = "OpenCL/NaiveHashing"
let timer = new Timer<string>()

let hashingCommand = 
    <@
        fun (rng:_1D) l k templates (lengths:array<byte>) (hashes:array<byte>) (localHashes:array<byte>) maxLength (input:array<byte>) (t:array<byte>) (result:array<int16>) ->
            let r = rng.GlobalID0
            let mutable _start = r * k
            let mutable _end = _start + k
            if _end > l then _end <- l

            let localHashesBase = r * (int) maxLength
            localHashes.[localHashesBase] <- input.[_start]
            for i in 1..((int) maxLength - 1) do
                if _start + i < l then localHashes.[localHashesBase + i] <- localHashes.[localHashesBase + i - 1] + input.[_start + i]

            for i in _start .. (_end - 1) do
                result.[i] <- -1s
                if i > _start then
                    for current in 0..((int) maxLength - 2) do
                        localHashes.[localHashesBase + current] <- localHashes.[localHashesBase + current + 1] - input.[i - 1]
                    if i + (int) maxLength <= l then
                        localHashes.[localHashesBase + (int) maxLength - 1] <- localHashes.[localHashesBase + (int) maxLength - 2] + input.[i + (int) maxLength - 1]

                let mutable templateBase = 0
                for n in 0..(templates - 1) do
                    if n > 0 then templateBase <- templateBase + (int) lengths.[n - 1]
                        
                    let currentLength = (int) lengths.[n]
                    if (i + (int) currentLength) <= l && hashes.[n] = localHashes.[localHashesBase + currentLength - 1] then
                        let mutable matches = 1

                        let mutable j = 0
                        while (matches = 1 && j < (int) currentLength) do
                            if input.[i + j] <> t.[templateBase + j] then  matches <- 0
                            j <- j + 1

                        if matches = 1 then result.[i] <- (int16) n
    @>

let mutable result = null
let mutable kernel = null
let mutable kernelPrepare = Unchecked.defaultof<_>
let mutable kernelRun = Unchecked.defaultof<_>
let mutable input = null
let mutable buffersCreated = false
let mutable templateHashes = null
let mutable localHashesArr = null

let initialize length maxTemplateLength k localWorkSize templates templatesSum (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
    timer.Start()
    result <- Array.zeroCreate length
    templateHashes <- NaiveHashingSearch.computeTemplateHashes templates templatesSum templateLengths templateArr
    let l = (length + (k-1))/k 
    localHashesArr <- Array.zeroCreate((int) maxTemplateLength * l)
    let x, y, z = provider.Compile(query=hashingCommand, translatorOptions=[BoolAsBit])
    kernel <- x
    kernelPrepare <- y
    kernelRun <- z
    input <- gpuArr
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths templateHashes localHashesArr maxTemplateLength input templateArr result
    timer.Lap(label)
    ()

let mutable ready = true

let upload () =
    if not ready then failwith "Already running, can't upload!"
    ready <- false

    timer.Start()
    Timer<string>.Global.Start()
    if buffersCreated || (provider.AutoconfiguredBuffers <> null && provider.AutoconfiguredBuffers.ContainsKey(input)) then
        ignore (commandQueue.Add(input.ToGpu provider).Finish())
    ignore (commandQueue.Add(kernelRun()))
    ()

let download () =
    if ready then failwith "Not running, can't download!"
    ready <- true

    ignore (commandQueue.Add(result.ToHost provider).Finish())
    buffersCreated <- true
    Timer<string>.Global.Lap(label)
    timer.Lap(label)

    result

let getMatches () =
    timer.Start()
    Timer<string>.Global.Start()
    if buffersCreated || (provider.AutoconfiguredBuffers <> null && provider.AutoconfiguredBuffers.ContainsKey(input)) then
        ignore (commandQueue.Add(input.ToGpu provider))
    let _ = commandQueue.Add(kernelRun())
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    buffersCreated <- true
    Timer<string>.Global.Lap(label)
    timer.Lap(label)
    result

let findMatches length maxTemplateLength k localWorkSize templates templatesSum (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
    timer.Start()
    
    let templateHashes = NaiveHashingSearch.computeTemplateHashes templates templatesSum templateLengths templateArr
    let result = Array.zeroCreate length
    let kernelHashed, kernelPrepareHashed, kernelRunHashed = provider.Compile hashingCommand
    let l = (length + (k-1))/k  
    let d =(new _1D(l,localWorkSize))
    let localHashesArr = Array.zeroCreate((int) maxTemplateLength * l)
    kernelPrepareHashed d length k templates templateLengths templateHashes localHashesArr maxTemplateLength (Array.copy gpuArr) templateArr result
    Timer<string>.Global.Start()
    let _ = commandQueue.Add(kernelRunHashed()).Finish()
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    Timer<string>.Global.Lap(label)
    timer.Lap(label)
    result


let Main () =
    let length = 3000000

    let maxTemplateLength = 32uy
    let templates = 512

    let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength
    let gpuArr = NaiveSearch.generateInput length

    let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

    let templateArr = NaiveSearch.generateTemplates templatesSum

    let k = 1000    
    let localWorkSize = 20

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    printfn "Finding substrings in string with length %A, using %A..." length label
    let matches = NaiveSearch.countMatches (findMatches length maxTemplateLength k localWorkSize templates templatesSum templateLengths gpuArr templateArr) length length templateLengths prefix
    printfn "done."

    printfn "Found: %A" matches
    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))
    printfn "Avg. time, %A with preparations: %A" label (timer.Average(label))

    ignore (System.Console.Read())

//do Main()