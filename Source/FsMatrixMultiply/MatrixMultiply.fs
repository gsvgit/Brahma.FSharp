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

module FsMatrixMultiply

open System.Text.RegularExpressions

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Wrapper
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

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

let Main () =

    let platformName = "*"

    let rows = 200
    let columns = 200
    let localWorkSize = 10
    let iterations = 100
    let deviceType = Cl.DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let aValues = MakeMatrix rows columns
    let bValues = MakeMatrix rows columns
    let cParallel = Array.zeroCreate(rows * columns)

    let matrixMult = 
        <@
            fun (r:_2D) columns (a:array<float32>) (b:array<float32>) (c:array<float32>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1                
                for k in 0 .. columns - 1 do
                    c.[ty * columns + tx] <- c.[ty * columns + tx] + (a.[ty * columns + k] * b.[k * columns + tx])
        @>

    printfn "Multiplying two %Ax%A matrices %A times using .NET..." rows columns iterations
    let cNormal = Array.zeroCreate (rows * columns)
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        Multiply aValues rows columns bValues rows columns cNormal
        Timer<string>.Global.Lap(".NET")

    printfn "done."        

    printfn "Multiplying two %Ax%A matrices %A times using Brahma.OpenCL and selected platform/device..." rows columns iterations

    let kernel, kernelPrepare, kernelRun = provider.Compile matrixMult
    let d =(new _2D(rows, columns, localWorkSize, localWorkSize))
    kernelPrepare d columns aValues bValues cParallel
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernelRun()).Finish()            
        Timer<string>.Global.Lap("OpenCL")
    
    printfn "done."
    
    let _ = commandQueue.Add(cParallel.ToHost(kernel)).Finish()
    //let x = cParallel = cParallel2
    printfn "Verifying results..."    
    for i in 0 .. rows * columns - 1 do
        if System.Math.Abs(float32 (cParallel.[i] - cNormal.[i])) > 0.00001f
        then
            sprintf "Expected: %A Actual: %A Error = %A" cNormal.[i] cParallel.[i] (System.Math.Abs(cParallel.[i] - cNormal.[i]))
            |> failwith

    printfn "done."

    Timer<string>.Global.Average(".NET") |> printfn "Avg. time, C#: %A"
    Timer<string>.Global.Average("OpenCL") |> printfn "Avg. time, OpenCL: %A"    

    commandQueue.Dispose()
    provider.Dispose()

do Main()