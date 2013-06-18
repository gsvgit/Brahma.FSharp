module NaiveSearchGpu 

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks

let label = "OpenCL/Naive"
let mutable timer = null

let platformName = "*"

let deviceType = Cl.DeviceType.Default

let createProvider() = 
    try  ComputeProvider.Create(platformName, deviceType)
    with 
    | ex -> failwith ex.Message

let provider = createProvider()

let createQueue() = 
    new CommandQueue(provider, provider.Devices |> Seq.head) 

let commandQueue = createQueue()

let command = 
    <@
        fun (rng:_1D) l k templates (lengths:array<_>) (input:array<_>) (t:array<_>) (result:array<int16>) ->
            let r = rng.GlobalID0
            let _start = r * k
            let mutable _end = _start + k
            if _end > l then _end <- l
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
                            if input.[i + j] <> t.[templateBase + j] then matches <- 0
                            j <- j + 1

                        if matches = 1 then result.[i] <- (int16) n
    @>

let mutable result = null
let mutable kernel = null
let mutable kernelPrepare = Unchecked.defaultof<_>
let mutable kernelRun = Unchecked.defaultof<_>
let mutable input = null
let mutable buffersCreated = false

let initialize length k localWorkSize templates (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
    timer <- new Timer<string>()
    timer.Start()
    result <- Array.zeroCreate length
    let x, y, z = provider.Compile(query=command, translatorOptions=[BoolAsBit])
    kernel <- x
    kernelPrepare <- y
    kernelRun <- z
    input <- gpuArr
    let l = (length + (k-1))/k 
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths input templateArr result
    timer.Lap(label)
    ()

let mutable ready = true

let close () = 
    commandQueue.Dispose()
    provider.CloseAllBuffers()
    provider.Dispose()
    buffersCreated <- false

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

let findMatches length k localWorkSize templates (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
    timer.Start()
    let result = Array.zeroCreate length
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let l = (length + (k-1))/k 
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths (Array.copy gpuArr) templateArr result
    Timer<string>.Global.Start()
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    Timer<string>.Global.Lap(label)
    timer.Lap(label)
    result