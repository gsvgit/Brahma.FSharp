// Copyright (c) 2013 Semyon Grigorev <rsdpisuy@gmail.com>
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

module AsyncMatrixMultiply

open System.Text.RegularExpressions

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Wrapper
open Microsoft.FSharp.Quotations

let random = new System.Random()
        
let MakeMatrix rows cols =
    Array.init (rows * cols) (fun i -> float32 (random.NextDouble()))

let GetOutputMatrixDimensions aRows aCols bRows bCols =
    if aCols <> bRows
    then failwith "Cannot multiply these two matrices"
    aRows,bCols

let Multiply (a:array<_>) aRows aCols (b:array<_>) bRows bCols (c:array<_>) =    
    let cRows, cCols = GetOutputMatrixDimensions aRows aCols bRows bCols
    for i in 0 .. cRows - 1 do
        for j in 0 .. cCols - 1 do
            for k in 0 .. aCols - 1 do
                c.[i * cCols + j] <- c.[i * cCols + j] + a.[i * aCols + k] * b.[k * bCols + j]

let cpuMultiplicator iterations (a:array<_>) aRows aCols (b:array<_>) bRows bCols =
    let rows = aRows
    let columns = bCols

    printfn "Multiplying two %Ax%A matrices %A times using .NET..." rows columns iterations

    let c = Array.zeroCreate (aRows * bCols)

    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        Multiply a rows columns b rows columns c
        Timer<string>.Global.Lap(".NET")

    printfn "done."

    c

let gpuMultiplicator timerTag provider iterations (a:array<_>) aRows aCols (b:array<_>) bRows bCols =
    let rows = aRows
    let columns = bCols
    let localWorkSize = 10
    let c = Array.zeroCreate(rows * columns)
    let aBuffer = new Buffer<float32>(provider, Operations.ReadOnly, Memory.Device, a)
    let bBuffer = new Buffer<float32>(provider, Operations.ReadOnly, Memory.Device, b)
    let cBuffer = new Buffer<float32>(provider, Operations.ReadWrite, Memory.Device, c)

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let matrixMult = 
        <@
            fun (r:_2D) (a:array<float32>) (b:array<float32>) (c:array<float32>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let columns = 600
                for k in 0 .. columns - 1 do
                    c.[ty * columns + tx] <- c.[ty * columns + tx] + (a.[ty * columns + k] * b.[k * columns + tx])
        @>

    printfn "Multiplying two %Ax%A matrices %A times using Brahma.OpenCL and selected platform/device..." rows columns iterations

    let kernel = provider.Compile matrixMult
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernel.Run(new _2D(rows, columns, localWorkSize, localWorkSize), aBuffer, bBuffer, cBuffer)).Finish()            
        Timer<string>.Global.Lap(timerTag)

    let _ = commandQueue.Add(cBuffer.Read(0, rows * columns, c)).Finish()        
    printfn "done."
    aBuffer.Dispose()
    bBuffer.Dispose()
    cBuffer.Dispose()
    commandQueue.Dispose()
    c

let Main () =

    let platformName = "*"

    let rows = 600
    let columns = 600    
    let iterations = 10
    let deviceType = Cl.DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider    

    let aValues = MakeMatrix rows columns
    let bValues = MakeMatrix rows columns
    
    let matrices = Array.init 10 (fun _ -> (MakeMatrix rows columns, MakeMatrix rows columns))

    let cstart = System.DateTime.Now
    let cpuResult =
        matrices
        |> Array.Parallel.map (fun (aValues, bValues) -> cpuMultiplicator iterations aValues rows columns bValues rows columns)
    let cpuTime = System.DateTime.Now - cstart

    let gstart = System.DateTime.Now
    let gpuResult = 
        matrices
        |> Array.map (fun (aValues, bValues) -> async{return (gpuMultiplicator "gpu async" provider iterations aValues rows columns bValues rows columns)})
        |> Async.Parallel 
        |> Async.RunSynchronously
    let gpuTime = System.DateTime.Now - gstart

    let gstart2 = System.DateTime.Now
    let gpuResult2 = 
        matrices
        |> Array.map (fun (aValues, bValues) -> (gpuMultiplicator "gpu seq" provider iterations aValues rows columns bValues rows columns))        
    let gpuTime2 = System.DateTime.Now - gstart2

    printfn "Verifying results..."

    let checkResult gpuResult cpuResult =
        Array.iteri2
            (fun i (cParallel:array<float32>) (cNormal:array<_>) ->
                printfn "Result#: %A" i
                for i in 0 .. rows * columns - 1 do
                    if System.Math.Abs(float32 (cParallel.[i] - cNormal.[i])) > 0.00001f
                    then
                        sprintf "Expected: %A Actual: %A Error = %A" cNormal.[i] cParallel.[i] (System.Math.Abs(cParallel.[i] - cNormal.[i]))
                        |> failwith)
            gpuResult
            cpuResult

    printfn "Check for GPU with async."
    checkResult gpuResult cpuResult
    printfn "Check for GPU with seq."
    checkResult gpuResult2 cpuResult

    printfn "done."

    Timer<string>.Global.Average(".NET") |> printfn "Avg. time, F#: %A"
    Timer<string>.Global.Average("gpu async") |> printfn "Avg. time, OpenCL async: %A"
    Timer<string>.Global.Average("gpu seq") |> printfn "Avg. time, OpenCL seq: %A"
    cpuTime |> printfn "CPU total time: %A"
    gpuTime |> printfn "GPU total time async: %A"
    gpuTime2 |> printfn "GPU total time seq: %A"

    provider.Dispose()

do Main()