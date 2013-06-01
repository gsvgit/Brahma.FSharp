module Testing

open Brahma.Helpers
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
        fun (rng:_1D) ->
            let mutable parent = 10s

            while parent > 0s do
                let currentTemplate = 10s
                parent <- parent - 1s
                
    @>

let findMatches () =
    let result = Array.init 123 (fun _ -> -1s)
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _1D(1000,20))
    kernelPrepare d
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(result.ToHost provider).Finish()
    result


let Main () =

    let x = findMatches ()
    printfn "%A %A %A" x.[0] x.[1] x.[2]

    ignore (System.Console.Read())

//do Main()