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
        fun (rng:_1D) (result:array<int>) ->
            let r = rng.GlobalID0
            barrier()
            let _start = local(r * 15)
            let mutable delta = local(1)
            delta <!+ delta + 1
            barrier()
            let _zero = local (Array.zeroCreate 1)
            barrier()
            _zero.[0] <- _zero.[0] + 1
            barrier()
            result.[0] <!+ _zero.[0]
    @>

let findMatches () =
    let result = Array.init 123 (fun _ -> -1)
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _1D(1000,20))
    kernelPrepare d result
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    result


let Main () =

    let x = findMatches ()
    printfn "%A" x.[0]

    ignore (System.Console.Read())

do Main()