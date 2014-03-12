module MatrixMultiply

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let main platformName (arrMatrix1:array<_>) (arrMatrix2:array<_>) rows cols =    

    let localWorkSize = 2
    let deviceType = DeviceType.Gpu

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message         

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let cParallel = Array.zeroCreate(rows * cols)

    let command = 
        <@
            fun (r:_2D) (arrMatrix1:array<_>) (arrMatrix2:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * cols + tx]
                for k in 0 .. cols - 1 do
                    buf <- buf + (arrMatrix1.[ty * cols + k] * arrMatrix2.[k * cols + tx])
                c.[ty * cols + tx] <- buf
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(rows, cols, localWorkSize, localWorkSize))
    kernelPrepare d arrMatrix1 arrMatrix2 cParallel
    
    let _ = commandQueue.Add(kernelRun()).Finish()   
    let _ = commandQueue.Add(cParallel.ToHost provider)

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()   
    cParallel 
printfn "%A" (main "NVIDIA*" [|1; 2; 3; 4|] [|1; 2; 3; 4|] 2 2)

    

