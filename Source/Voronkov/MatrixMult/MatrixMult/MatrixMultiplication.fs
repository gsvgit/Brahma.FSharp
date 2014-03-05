module MatrixMultiply

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main M1 M2 =    

    let platformName = "*"
    let deviceType = DeviceType.Default
    let localWorkSize = 4

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let cParallel = Array.zeroCreate 16

    let command = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * 4 + tx]
                for k in 0 .. 3 do
                    buf <- buf + (a.[ty * 4 + k] * b.[k * 4 + tx])
                c.[ty * 4 + tx] <- buf
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(4, 4, localWorkSize, localWorkSize))
    kernelPrepare d M1 M2 cParallel
    
    let _ = commandQueue.Add(kernelRun()).Finish
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()

    cParallel