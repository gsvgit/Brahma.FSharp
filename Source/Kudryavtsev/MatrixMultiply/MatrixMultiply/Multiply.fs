module MatrixMultiply

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main platformName rows columns =   
    
    let localWorkSize = 1
    let deviceType = DeviceType.Gpu
    let a = Array.init (rows * columns) (fun i -> 1)
    let b = Array.init (rows * columns) (fun i -> 1)

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message
        
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)


    let cParallel = Array.zeroCreate(rows * columns)

    let command = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * columns + tx]
                for k in 0 .. columns - 1 do
                    buf <- buf + (a.[ty * columns + k] * b.[k * columns + tx])
                c.[ty * columns + tx] <- buf
        @>

   
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(rows, columns, localWorkSize, localWorkSize))
    kernelPrepare d a b cParallel
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(cParallel.ToHost provider)

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    cParallel

printfn "%A" (Main "NVIDIA*" 5 5)