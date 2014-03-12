module MatrixMultiply

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let main (a: array<int>) (b: array<int>) =

    let platformName = "*"

    let localWorkSize = 5
    let deviceType = DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let cParallel = Array.zeroCreate 25

    let command = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * 5 + tx]
                for k in 0 .. 4 do
                    buf <- buf + (a.[ty * 5 + k] * b.[k * 5 + tx])
                c.[ty * 5 + tx] <- buf
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(5, 5, localWorkSize, localWorkSize))
    kernelPrepare d a b cParallel

    let _ = commandQueue.Add(kernelRun()).Finish()

    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()

    cParallel
