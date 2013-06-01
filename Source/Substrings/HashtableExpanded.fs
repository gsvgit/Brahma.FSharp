module HashtableExpanded

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks

let provider = NaiveSearchGpu.provider

let createQueue() = 
    new CommandQueue(provider, provider.Devices |> Seq.head) 

let commandQueue = createQueue()

let label = "OpenCL/HashtableExpanded"
let mutable timer = null

let hashingCommand = 
    <@
        fun (rng:_1D) l k templates (lengths:array<byte>) (hashes:array<byte>) (table:array<int16>) (next:array<int16>) (starts:array<int16>) maxLength (input:array<byte>) (t:array<byte>) (result:array<int16>) ->
            let r = rng.GlobalID0
            let mutable _start = r * k
            let mutable _end = _start + k
            if _end > l then _end <- l

            let privateHashes = Array.zeroCreate 32
            privateHashes.[0] <- input.[_start]
            for i in 1..((int) maxLength - 1) do
                if _start + i < l then privateHashes.[i] <- privateHashes.[i - 1] + input.[_start + i]

            let localTemplateHashes = local (Array.zeroCreate 512)
            let localTemplateLengths = local (Array.zeroCreate 512)
            let localTable = local (Array.zeroCreate 256)
            let localNext = local (Array.zeroCreate 512)
            let localStarts = local (Array.zeroCreate 512)

            let groupSize = 512
            let chunk = (512 + groupSize - 1) / groupSize
            let tableChunk = (256 + groupSize - 1) / groupSize

            let id = rng.LocalID0

            let upperBound = (id + 1) * chunk
            let mutable higherIndex = upperBound - 1
            if upperBound > templates then
                higherIndex <- templates - 1

            let upperBoundTable = (id + 1) * tableChunk
            let mutable higherIndexTable = upperBoundTable - 1
            if upperBoundTable > 256 then
                higherIndexTable <- 256 - 1

            for index in (id * chunk)..higherIndex do
                localTemplateHashes.[index] <- hashes.[index]
                localTemplateLengths.[index] <- lengths.[index]
                localNext.[index] <- next.[index]
                localStarts.[index] <- starts.[index]

            for index in (id * tableChunk)..higherIndexTable do
                localTable.[index] <- table.[index]

            barrier()

            for i in _start .. (_end - 1) do
                result.[i] <- -1s
                if i > _start then
                    for current in 0..(((int) maxLength - 1) / 4 - 1) do
                        let baseIndex = 4 * current
                        privateHashes.[baseIndex] <- privateHashes.[baseIndex + 1] - input.[i - 1]
                        privateHashes.[baseIndex + 1] <- privateHashes.[baseIndex + 2] - input.[i - 1]
                        privateHashes.[baseIndex + 2] <- privateHashes.[baseIndex + 3] - input.[i - 1]
                        privateHashes.[baseIndex + 3] <- privateHashes.[baseIndex + 4] - input.[i - 1]
                    for current in (4 * (((int) maxLength - 1) / 4))..((int) maxLength - 2) do
                        privateHashes.[current] <- privateHashes.[current + 1] - input.[i - 1]
                    if i + (int) maxLength <= l then
                        privateHashes.[(int) maxLength - 1] <- privateHashes.[(int) maxLength - 2] + input.[i + (int) maxLength - 1]
                
                let mutable maximum = (int) maxLength - 1
                if i + maximum >= l then
                    maximum <- l - i - 1
                for current in 0..((maximum + 1) / 4 - 1) do
                    let baseIndex = 4 * current
                    let hash = privateHashes.[baseIndex]
                    let number = localTable.[(int) hash]
                    let mutable templateIndex = number
                    let mutable look = 1
                    while look > 0 && templateIndex >= 0s do
                        if localTemplateLengths.[(int) templateIndex] = (byte) baseIndex + 1uy then
                            let mutable matches = 1

                            let mutable j = 0
                            let templateBase = localStarts.[(int) templateIndex]
                            while (matches = 1 && j < baseIndex + 1) do
                                if input.[i + j] <> t.[(int) templateBase + j] then  matches <- 0
                                j <- j + 1

                            if matches = 1 then
                                result.[i] <- templateIndex
                                look <- 0
                        templateIndex <- localNext.[(int) templateIndex]
                    let hash = privateHashes.[baseIndex + 1]
                    let number = localTable.[(int) hash]
                    let mutable templateIndex = number
                    let mutable look = 1
                    while look > 0 && templateIndex >= 0s do
                        if localTemplateLengths.[(int) templateIndex] = (byte) baseIndex + 2uy then
                            let mutable matches = 1

                            let mutable j = 0
                            let templateBase = localStarts.[(int) templateIndex]
                            while (matches = 1 && j < baseIndex + 2) do
                                if input.[i + j] <> t.[(int) templateBase + j] then  matches <- 0
                                j <- j + 1

                            if matches = 1 then
                                result.[i] <- templateIndex
                                look <- 0
                        templateIndex <- localNext.[(int) templateIndex]
                    let hash = privateHashes.[baseIndex + 2]
                    let number = localTable.[(int) hash]
                    let mutable templateIndex = number
                    let mutable look = 1
                    while look > 0 && templateIndex >= 0s do
                        if localTemplateLengths.[(int) templateIndex] = (byte) baseIndex + 3uy then
                            let mutable matches = 1

                            let mutable j = 0
                            let templateBase = localStarts.[(int) templateIndex]
                            while (matches = 1 && j < baseIndex + 3) do
                                if input.[i + j] <> t.[(int) templateBase + j] then  matches <- 0
                                j <- j + 1

                            if matches = 1 then
                                result.[i] <- templateIndex
                                look <- 0
                        templateIndex <- localNext.[(int) templateIndex]
                    let hash = privateHashes.[baseIndex + 3]
                    let number = localTable.[(int) hash]
                    let mutable templateIndex = number
                    let mutable look = 1
                    while look > 0 && templateIndex >= 0s do
                        if localTemplateLengths.[(int) templateIndex] = (byte) baseIndex + 4uy then
                            let mutable matches = 1

                            let mutable j = 0
                            let templateBase = localStarts.[(int) templateIndex]
                            while (matches = 1 && j < baseIndex + 4) do
                                if input.[i + j] <> t.[(int) templateBase + j] then  matches <- 0
                                j <- j + 1

                            if matches = 1 then
                                result.[i] <- templateIndex
                                look <- 0
                        templateIndex <- localNext.[(int) templateIndex]
                for current in (4 * ((maximum + 1) / 4))..maximum do
                    let hash = privateHashes.[(int) current]
                    let number = localTable.[(int) hash]
                    let mutable templateIndex = number
                    let mutable look = 1
                    while look > 0 && templateIndex >= 0s do
                        if localTemplateLengths.[(int) templateIndex] = (byte) current + 1uy then
                            let mutable matches = 1

                            let mutable j = 0
                            let templateBase = localStarts.[(int) templateIndex]
                            while (matches = 1 && j < current + 1) do
                                if input.[i + j] <> t.[(int) templateBase + j] then  matches <- 0
                                j <- j + 1

                            if matches = 1 then
                                result.[i] <- templateIndex
                                look <- 0
                        templateIndex <- localNext.[(int) templateIndex]
    @>


let createHashTable templates (templateLengths:array<byte>) (templateHashes:array<byte>) =
    let table = Array.init 256 (fun _ -> -1s)
    let next = Array.init templates (fun _ -> -1s)

    for i in 0..templates do
        let mutable current = table.[(int) templateHashes.[i]]
        
        if current = -1s then
            table.[(int) templateHashes.[i]] <- (int16) i
        else
            while next.[(int) current] >= 0s do
                current <- next.[(int) current]

            next.[(int) current] <- (int16) i

    table, next
        
let computeTemplateStarts templates (templateLengths:array<byte>) =
    let starts = Array.zeroCreate templates
    let mutable current = 0s

    for i in 0..(templates - 1) do
        starts.[i] <- current
        current <- current + (int16) templateLengths.[i]
    
    starts

let mutable result = null
let mutable kernel = null
let mutable kernelPrepare = (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> (fun _ -> ())))))))))))))
let mutable kernelRun = (fun _ -> null)
let mutable input = null
let mutable buffersCreated = false
let mutable templateHashes = null
let mutable table = null
let mutable next = null
let mutable starts = null

let close () = 
    commandQueue.Dispose()
    provider.CloseAllBuffers()
    provider.Dispose()
    buffersCreated <- false

let initialize length maxTemplateLength k localWorkSize templates templatesSum (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
    timer <- new Timer<string>()
    timer.Start()
    result <- Array.zeroCreate length

    templateHashes <- NaiveHashingSearch.computeTemplateHashes templates templatesSum templateLengths templateArr
    let t, n = createHashTable templates templateLengths templateHashes
    table <- t
    next <- n
    starts <- computeTemplateStarts templates templateLengths

    let l = (length + (k-1))/k 
    let x, y, z = provider.Compile hashingCommand
    kernel <- x
    kernelPrepare <- y
    kernelRun <- z
    input <- gpuArr
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths templateHashes table next starts maxTemplateLength input templateArr result
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
        async {
            ignore (commandQueue.Add(kernelRun()).Finish())
        } |> Async.StartAsTask
    else
        ignore (commandQueue.Add(kernelRun()).Finish())
        async {
            ()
        } |> Async.StartAsTask

let download (task:Task<unit>) =
    if ready then failwith "Not running, can't download!"
    ready <- true

    task.Wait()

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
    let table, next = createHashTable templates templateLengths templateHashes
    let starts = computeTemplateStarts templates templateLengths

    let result = Array.zeroCreate length
    let kernelHashed, kernelPrepareHashed, kernelRunHashed = provider.Compile(query=hashingCommand, translatorOptions=[BoolAsBit])
    let l = (length + (k-1))/k  
    let d =(new _1D(l,localWorkSize))
    kernelPrepareHashed d length k templates templateLengths templateHashes table next starts maxTemplateLength (Array.copy gpuArr) templateArr result
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
    let matches = NaiveSearch.countMatches (findMatches length maxTemplateLength k localWorkSize templates templatesSum templateLengths gpuArr templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."

    printfn "Found: %A" matches
    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))
    printfn "Avg. time, %A with preparations: %A" label (timer.Average(label))

    ignore (System.Console.Read())

//do Main()