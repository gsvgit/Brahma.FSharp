namespace AsyncMatrixMultiply

// Agents
module MatrixMultiply =

    open System.Text.RegularExpressions
    
    open Brahma.Helpers
    open OpenCL.Net
    open Brahma.OpenCL
    open Brahma.FSharp.OpenCL.Core
    open Microsoft.FSharp.Quotations
    open Brahma.FSharp.OpenCL.Extensions
    
    
    let GetOutputMatrixDimensions aRows aCols bRows bCols =
        if aCols <> bRows
        then failwith "Cannot multiply these two matrices"
        aRows,bCols
    
    let rows = 200
    let columns = 200

    let Multiply (a:array<_>) aRows aCols (b:array<_>) bRows bCols (c:array<_>) =    
        let cRows, cCols = GetOutputMatrixDimensions aRows aCols bRows bCols
        for i in 0 .. cRows - 1 do
            for j in 0 .. cCols - 1 do
                for k in 0 .. aCols - 1 do
                    c.[i * cCols + j] <- c.[i * cCols + j] + a.[i * aCols + k] * b.[k * bCols + j]
    
    let MultiplyMatrixByCPU (a:float32[]) aRows aCols (b:float32[]) bRows bCols =
        let rows = aRows
        let columns = bCols
    
//        printfn "Multiplying two %Ax%A matrices using .NET..." rows columns
    
        let c = Array.zeroCreate (aRows * bCols)
    
        Timer<string>.Global.Start()
        Multiply a rows columns b rows columns c
    //    Timer<string>.Global.Lap(".NET")
    
  //      printfn "done."
    
        c
    
    let MultyplyMatrixByCpuAgents ((a:float32[]), (b:float32[])) =
        MultiplyMatrixByCPU a rows columns b rows columns

    let MultiplyMatrixByGPU timerTag provider (a:array<_>) aRows aCols (b:array<_>) bRows bCols =
        let rows = aRows
        let columns = bCols
        let localWorkSize = 10
        let c = Array.zeroCreate(rows * columns)    
        
        let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    
        let matrixMult = 
            <@
                fun (r:_2D) columns (a:array<float32>) (b:array<float32>) (c:array<float32>) -> 
                    let tx = r.GlobalID0
                    let ty = r.GlobalID1                
                    for k in 0 .. columns - 1 do
                        c.[ty * columns + tx] <- c.[ty * columns + tx] + (a.[ty * columns + k] * b.[k * columns + tx])
            @>
    
        printfn "Multiplying two %Ax%A matrices using Brahma.OpenCL and selected platform/device..." rows columns
        
        let kernel, kernelPrepare, kernelRun = provider.Compile matrixMult
        let d = new _2D(rows, columns, localWorkSize, localWorkSize)
        kernelPrepare d columns a b c
        
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernelRun()).Finish()
        Timer<string>.Global.Lap(timerTag)
    
        let _ = commandQueue.Add(c.ToHost(provider)).Finish()        
        printfn "done."
            
        commandQueue.Dispose()
        provider.CloseAllBuffers()
    
        c

    let MultiplyMatrixByGpuAgents timerTag provider (a:array<_>) (b:array<_>) =
        MultiplyMatrixByGPU timerTag provider a rows columns b rows columns