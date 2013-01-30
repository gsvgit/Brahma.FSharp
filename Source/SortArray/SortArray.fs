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

module SortArray

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let random = new System.Random()

let platformName = "*"
let deviceType = Cl.DeviceType.Default
let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message
let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

let k = 1024

let length =   10000000
             //110000000

let baseArr = Array.init length (fun _ -> random.Next(10))

let cpuArr = Array.copy baseArr
let gpuArr = Array.copy baseArr
let b = Array.zeroCreate (min (length/k+1) ((length + (k-1))/k))

let gpuSumk (*commandQueue:CommandQueue*) (arr:array<_>) (*b*) k = 
    let dbg =
        fun rng l k (a:array<_>) (b:array<_>) ->
            for r in rng do
                let _start = r * k
                let mutable _end = _start + k - 1
                if _end >= l then _end <- l - 1
                let mutable buf = 0
                for i in _start .. _end do
                    buf <- buf + a.[i]
                b.[r] <- buf

    let command = 
        <@
            fun (rng:_1D) l k (a:array<_>) (b:array<_>) ->
                let r = rng.GlobalID0
                let _start = r * k
                let mutable _end = _start + k - 1
                if _end >= l then _end <- l - 1
                let mutable buf = 0
                for i in _start .. _end do
                    buf <- buf + a.[i]
                b.[r] <- buf
        @>    
    let length = arr.Length    
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let mutable bufLen = min (length/k+1) ((length + (k-1))/k)
    let mutable curL = length
    let mutable flip = false    
    while curL > 1 do
        let d =(new _1D(bufLen,1))
        if flip
        then 
            //dbg [|0..bufLen-1|] curL k b arr
            kernelPrepare d curL k b arr
        else
            //dbg [|0..bufLen-1|] curL k arr b
            kernelPrepare d curL k arr b
        flip <- not flip
        let _ = commandQueue.Add(kernelRun())
        curL <- bufLen
        bufLen <- (min (bufLen/k+1) ((bufLen + (k-1))/k))//bufLen/2
    let _ = commandQueue.Finish() //Add(kernelRun()).Finish()
    let sum = [|0|]
    let _ = commandQueue.Add((if flip then b else arr).ToHost(provider,sum)).Finish()
    provider.CloseAllBuffers()
    //let _ = commandQueue.Add(arr.ToHost(kernel)).Finish()
    //let _ = commandQueue.Add(b.ToHost(kernel)).Finish()
    //let sum = (if flip then b else arr).[0]
    commandQueue.Dispose()
    sum.[0]


let gpuSum (arr:array<_>) = 
    let command = 
        <@
            fun (rng:_1D) l (a:array<_>) (b:array<_>) ->
                let r = rng.GlobalID0
                let x = r * 2
                if l - 1 <= x
                then b.[r] <- a.[x]
                else b.[r] <- a.[x] + a.[x + 1]
        @>    
    let length = arr.Length    
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let mutable bufLen = min (length/2+1) ((length + 1)/2)
    let mutable curL = length
    let mutable flip = false    
    while curL > 1 do
        let d =(new _1D(bufLen,1))
        if flip
        then kernelPrepare d curL b arr
        else kernelPrepare d curL arr b
        flip <- not flip
        let _ = commandQueue.Add(kernelRun())
        curL <- bufLen
        bufLen <- (min (bufLen/2+1) ((bufLen + 1)/2))//bufLen/2
    let _ = commandQueue.Finish() //Add(kernelRun()).Finish()
    let sum = [|0|]
    let _ = commandQueue.Add((if flip then b else arr).ToHost(provider,sum)).Finish()
    //let _ = commandQueue.Add(arr.ToHost(kernel)).Finish()
    //let _ = commandQueue.Add(b.ToHost(kernel)).Finish()
    //let sum = (if flip then b else arr).[0]
    commandQueue.Dispose()
    sum.[0]

let gpuSum2 (arr:array<_>) = 
    let command = 
        <@
            fun (rng:_1D) k l (a:array<_>) ->
                let x = rng.GlobalID0 * k * 2
                let y = x + k
                if y < l
                then a.[x] <- a.[x] + a.[y]
        @>
    let length = arr.Length    
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let mutable bufLen = (min (length/2+1) ((length + 1)/2))
    let mutable k = 1
    while k < length do
        let d =(new _1D(bufLen,1))        
        kernelPrepare d k length arr
        let _ = commandQueue.Add(kernelRun()) 
        k <- k*2
        bufLen <- (min (bufLen/2+1) ((bufLen + 1)/2))

    printfn "K = %A" k
    let _ = commandQueue.Finish()
    let sum = [|0|]
    let _ = commandQueue.Add(arr.ToHost(provider,sum)).Finish()    
    commandQueue.Dispose()
    sum.[0]

let gpuSum3 (arr:array<_>) k =
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
    let length = arr.Length
    let sum = [|0|]
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let l =  min (length/k+1) ((length + (k-1))/k)
    let d =(new _1D(l,20))
    kernelPrepare d length k arr sum
    commandQueue.Add(kernelRun()).Finish() |> ignore
    let _ = commandQueue.Finish()    
    let _ = commandQueue.Add(sum.ToHost provider).Finish()
    provider.CloseAllBuffers()    
    commandQueue.Dispose()
    sum.[0]


let gpuiter (arr:array<_>) = 
    let command = 
        <@
            fun (rng:_1D) (a:array<_>) ->
                let r = rng.GlobalID0
                let x = a.[r]
                a.[r] <- x*x*x*x*x*x*x*x                
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command    
    let d =(new _1D(arr.Length,1))    
    kernelPrepare d arr        
    let _ = commandQueue.Add(kernelRun()).Finish()    
    let _ = commandQueue.Add(arr.ToHost provider).Finish()    
    ()

let gpuSort2 (arr:array<_>) =
    let command = 
        <@
            fun (rng:_1D) id l (a:array<_>) (b:array<_>) (c:array<_>)->
                let k = rng.GlobalID0
                if id = 0
                then
                    let mutable count = 0                    
                    let cur = a.[k]
                    for i in 0..l-1 do
                        if a.[i] < cur then count <- count + 1
                    b.[k] <- count
                else c.[b.[k]] <- a.[k]
        @>
    let b = Array.zeroCreate arr.Length
    let c = Array.zeroCreate arr.Length
    let kernel, kernelPrepare, kernelRun = provider.Compile command    
    let d =(new _1D(arr.Length,1))  
    kernelPrepare d 0 arr.Length arr b c
    let _ = commandQueue.Add(kernelRun())//.Barrier()
    kernelPrepare d 1 arr.Length arr b c
    let _ = commandQueue.Add(kernelRun()).Finish
    let _ = commandQueue.Add(c.ToHost provider).Finish()
    c

let gpuSort (arr:array<_>) =
    let command = 
        <@
            fun (rng:_1D) l frame (a:array<_>) ->
                let r = rng.GlobalID0
                let first = r * frame
                let mutable mid = first + frame
                let mutable left = first
                let mutable right = mid+1
                let mutable last = first + frame
                if l - 1 - last < frame then last <- l - 1
                        
                while (left <= mid && right <= last) do
                   // Select from left:  no change, just advance left
                    if a.[left] <= a.[right]
                    then left <- left + 1
                    // Select from right:  rotate [left..right] and correct
                    else
                       let tmp = a.[right]     // Will move to [left]                       
                       for j in right - 1  .. left do a.[j + 1] <- a.[j]     
                       a.[left] <- tmp
                       // EVERYTHING has moved up by one
                       left <- left + 1
                       mid <- mid + 1
                       right <- right + 1
        @>
    let s = string command
    command |> printfn "%A"
    let mutable localWorkSize = 1
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _1D(length, 1))
    kernelPrepare d arr.Length 1 arr
    let _ = commandQueue.Add(kernelRun()).Finish()
    arr.ToHost provider
//    for i in 0 .. iterations - 1 do
//        Timer<string>.Global.Start()
//        let _ = commandQueue.Add(kernelRun()).Finish()  

let time f = 
    let start = System.DateTime.Now
    f()
    System.DateTime.Now - start

let timeGpuSumk () =
    let l = 20000000
    let baseArr = Array.init l (fun _ -> random.Next(10))

    let gpuArr = Array.copy baseArr    

    for k in Array.init 20 (fun i -> (i+1)*100) do
        let b = Array.zeroCreate (min (l/k+1) ((l + (k-1))/k))
        let cq = new CommandQueue(provider, provider.Devices |> Seq.head)
        let a = Array.copy gpuArr
        fun () ->
            let sum = 1//gpuSumk cq a b k
            printfn "k=%A" k
            printfn "Sum=%A" sum
        |> time
        |> printfn "Time: %A"

//timeGpuSumk()

let cpuSum = ref 0
(fun () -> cpuSum := Array.sum cpuArr )
|> time
|> printfn "cpu time: %A"  


let _gpuSum = ref 0
(fun () -> _gpuSum := 
                gpuSum3 gpuArr 100)
                //gpuSumk gpuArr k )
                // gpuSum [|2;3;4;5;1|] )
|> time
|> printfn "gpu time: %A"

printfn "%A" cpuSum
printfn "%A" _gpuSum




//(fun () -> Array.iteri (fun i x -> cpuArr.[i] <- x*x*x*x*x*x*x*x) cpuArr )
//|> time
//|> printfn "cpu iter time: %A"  
//
//
//(fun () -> gpuiter cpuArr )
//|> time
//|> printfn "cpu iter time: %A"  


//gpuSort [|1;2|] |> printfn "%A"


//_gpuSum = cpuSum |> printfn "%A"

//Array.init 1000 (fun i -> 1002 - i) 

//let a = Array.init 5000 (fun i -> 5002 - i)  
//
//let x = 
//    
// //[|15;14;13;12;11;10;9;8;7;6;5;4;3;2;1|]
// 
//    (fun () -> a |> gpuSort2 |> ignore) |> time |> printfn "GPU %A"
//    (fun () -> Array.sort a |> ignore) |> time |> printfn "CPU %A"

//let s1 = Array.init 10000000 (fun i -> random.Next()) |> Set.ofArray
//let s2 = Array.init 10000000 (fun i -> random.Next()) |> Set.ofArray
//
//printfn "s1 %A" s1.Count
//printfn "s2 %A" s2.Count
//
//Set.difference s1 s2 |> printfn "%A"
//
//Set.union s1 s2 |> printfn "%A"
//
//Set.intersect s1 s2 |> printfn "%A"

//printfn "%A " <|  x