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

module ArraySum

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let random = new System.Random()
        
let k = 1000

let length = 20000000

let baseArr = Array.init length (fun _ -> random.Next(10))

let cpuArr = Array.copy baseArr
let gpuArr = Array.copy baseArr

let Main () =

    let platformName = "*"
    
    let localWorkSize = 20
    let iterations = 100
    let deviceType = Cl.DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)    

    let command = 
        <@
            fun (rng:_1D) l k (a:array<_>) (b:array<int>) ->
                let r = rng.GlobalID0
                let _start = r * k
                let mutable _end = _start + k - 1
                if _end >= l then _end <- l - 1
                let mutable buf = 0
                for i in _start .. _end do
                    buf <- buf + a.[i]
                b.[0] <!+ buf
        @>

    printfn "Sum of elements of array with length %A,  %A times using .NET..." length iterations    
    let mutable cpuSum = 0
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        cpuSum <- Array.sum cpuArr
        Timer<string>.Global.Lap(".NET")

    printfn "done."

    printfn "Sum of elements of array with length %A,  %A times uling OpenCL and selected platform/device..."  length iterations

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let sum = [|0|]    
    let l =  min (length/k+1) ((length + (k-1))/k)
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k gpuArr sum
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(sum.ToHost provider).Finish()
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernelRun()).Finish()
        Timer<string>.Global.Lap("OpenCL")
    
    printfn "done."        
    
    printfn "Verifying results..."    
    if System.Math.Abs(sum.[0] - cpuSum) > 0
    then        
        printfn "Expected: %A Actual: %A" cpuSum sum.[0]

    printfn "done."

    Timer<string>.Global.Average(".NET") |> printfn "Avg. time, C#: %A"
    Timer<string>.Global.Average("OpenCL") |> printfn "Avg. time, OpenCL: %A"

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()

    ignore (System.Console.Read())

do Main()