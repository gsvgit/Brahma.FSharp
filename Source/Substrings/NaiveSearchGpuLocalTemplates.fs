module NaiveSearchGpuLocalTemplates

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common

let label = "OpenCL/NaiveLocal"
let timer = new Timer<string>()

let provider = NaiveSearchGpu.provider

let createQueue() = 
    new CommandQueue(provider, provider.Devices |> Seq.head) 

let commandQueue = createQueue()

let command = 
    <@
        fun (rng:_1D) l k templates (lengths:array<_>) (input:array<_>) templatesSum (t:array<_>) (result:array<int16>) ->
            let r = rng.GlobalID0
            let _start = r * k
            let mutable _end = _start + k
            if _end > l then _end <- l

            let localTemplates = local (Array.zeroCreate 16336)
            let groupSize = 512
            let chunk = (templatesSum + groupSize - 1) / groupSize
            let id = rng.LocalID0

            let upperBound = (id + 1) * chunk
            let mutable higherIndex = upperBound - 1
            if upperBound > templatesSum then
                higherIndex <- templatesSum - 1

            for index in (id * chunk)..higherIndex do
                localTemplates.[index] <- t.[index]

            barrier()

            for i in _start..(_end - 1) do
                result.[i] <- -1s
                let mutable templateBase = 0
                for n in 0..(templates - 1) do
                    if n > 0 then templateBase <- templateBase + (int) lengths.[n - 1]
                        
                    let currentLength = lengths.[n]
                    if i + (int) currentLength <= l then
                        let mutable matches = 1
                        let mutable j = 0
                        while (matches = 1 && j < (int) currentLength) do
                            if input.[i + j] <> localTemplates.[templateBase + j] then matches <- 0
                            j <- j + 1

                        if matches = 1 then result.[i] <- (int16) n
    @>

let mutable result = null
let mutable kernel = null
let mutable kernelPrepare = (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> ())))))))))
let mutable kernelRun = (fun _ -> null)
let mutable input = null
let mutable buffersCreated = false

let initialize length k localWorkSize templates (templateLengths:array<byte>) (gpuArr:array<byte>) templatesSum (templateArr:array<byte>) =
    timer.Start()
    result <- Array.zeroCreate length
    let x, y, z = provider.Compile(query=command, translatorOptions=[BoolAsBit])
    kernel <- x
    kernelPrepare <- y
    kernelRun <- z
    input <- gpuArr
    let l = (length + (k-1))/k 
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths input templatesSum templateArr result
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

let findMatches length k localWorkSize templates (templateLengths:array<byte>) (gpuArr:array<byte>) templatesSum (templateArr:array<byte>) =
    timer.Start()
    let result = Array.zeroCreate length
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let l = (length + (k-1))/k 
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths (Array.copy gpuArr) templatesSum templateArr result
    Timer<string>.Global.Start()
    let _ = commandQueue.Add(kernelRun()).Finish()
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
    let matches = NaiveSearch.countMatches (findMatches length k localWorkSize templates templateLengths gpuArr templatesSum templateArr) length length templateLengths prefix
    printfn "done."

    printfn "Found: %A" matches
    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))
    printfn "Avg. time, %A with preparations: %A" label (timer.Average(label))

    ignore (System.Console.Read())

//do Main()