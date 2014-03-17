module Filter

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let arrayFilter predicate (inArr: array<_>) =
    
    let platformName = "NVIDIA*"

    let localWorkSize = 5
    let deviceType = DeviceType.Default
    let length = inArr.Length
    let outArr = Array.zeroCreate length

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider
    
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let command = 
        <@
            fun (rng:_1D) predicate (a: array<_>) (b: array<_>) ->
                let r = rng.GlobalID0
                if predicate(a.[r]) = true
                //then a.[r] <- a.[r] + 1
                then b.[r] <! a.[r]
                else b.[r] <! "f"
        @> 
    
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = new _1D(length,localWorkSize)    
    kernelPrepare d predicate inArr outArr

    let _ = commandQueue.Add(kernelRun()).Finish()

    let _ = commandQueue.Add(outArr.ToHost provider).Finish()

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
                
    outArr


                