module Testing

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

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
        fun (rng:_1D) (result:array<int16>) length ->
            let r = rng.GlobalID0
            barrier()
            let _start = local(r * 15)
            let mutable delta = local(1)
            delta <!+ delta + 1
            barrier()
            let _zero = local (Array.zeroCreate 3)
            _zero.[0] <!+ _zero.[0]
            _zero.[1] <!+ _zero.[1]
            _zero.[2] <!+ _zero.[2]
            barrier()
            (*let varLength = local (Array.zeroCreate length)*)
            result.[0] <- result.[0] + (int16) _zero.[0]
            result.[1] <- result.[0] + (int16) _zero.[1]
            result.[2] <- result.[0] + (int16) _zero.[2]
    @>

let findMatches () =
    let result = Array.init 123 (fun _ -> -1s)
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _1D(1000,20))
    kernelPrepare d result 10
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    result


let Main () =

    let x = findMatches ()
    printfn "%A %A %A" x.[0] x.[1] x.[2]

    ignore (System.Console.Read())

//do Main()