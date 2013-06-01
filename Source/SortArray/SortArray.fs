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

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Microsoft.FSharp.Linq.QuotationEvaluation

let random = new System.Random()

let platformName = "*"
let deviceType = Cl.DeviceType.Default

let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message
let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
let ing,e = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.GlobalMemSize)
let gg = ing.CastTo<uint64>()
let ing2,e2 = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
let ggg = ing2.CastTo<uint64>()
let k = 1024

let length =   12//0000000
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
    commandQueue.Dispose()
    sum.[0]


let gpuiter (arr:array<_>) f = 
    let command = 
        <@
            fun (rng:_1D) (a:array<_>) ->
                let r = rng.GlobalID0
                let x = a.[r]
                a.[r] <- (%f) x
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command    
    let d = new _1D(arr.Length,20)
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
    let _ = commandQueue.Add(kernelRun())
    kernelPrepare d 1 arr.Length arr b c
    let _ = commandQueue.Add(kernelRun()).Finish
    let _ = commandQueue.Add(c.ToHost provider).Finish()
    c

//let findSubstr (s:array<byte>) (sub:array<byte>) =
//    let command =
//        <@
//            fun (rng:_1D) (s:array<_>) (sub:array<_>) (res:array<_>) sL subL subH ->
//                let i = rng.GlobalID0
//                let q = 101
//                let d = 256
//                let mutable sH = 0                
//                if i <= sL - subL
//                then
//                    //for j in 0..subL-1 do
//                      //  sH <- (d*sH + int s.[j+i])/q
//                    //if sH = subH
//                    //then
//                        let mutable areEq = true
//                        let mutable count = 0                    
//                        while areEq && count < subL do                        
//                            areEq <- areEq && s.[i+count] = sub.[count]
//                            count <- count + 1
//                        if areEq then res.[i] <- 1uy
//        @>
//
//    let length = s.Length
//    let mutable localWorkSize = 100
//    let kernel, kernelPrepare, kernelRun = provider.Compile command
//    let dim =(new _1D(length, localWorkSize))
//    let res = Array.zeroCreate length
//    let mutable subH = 0
//    let q = 101
//    let d = 256
//    for j in 0..sub.Length-1 do
//        subH <- (d*subH + int sub.[j])/q
//    kernelPrepare dim s sub res s.Length sub.Length subH
//    let _ = commandQueue.Add(kernelRun()).Finish()
//    let _ = commandQueue.Add(res.ToHost provider).Finish()
//    res //|> Array.filter (fun x -> x <> 0uy)


let findSubstr (s:array<_>) (sub:array<_>) =

    let genComparator (pat:array<byte>) =
        let l = pat.Length
        let getmi expr =
            match expr with
            | Patterns.Call (_,mi,_) -> mi
            | _ -> failwith "It is not a call"
          
        let plusMi = getmi <@ 1 + 2 @>
        let aGetMi = getmi <@ [|1uy|].[0] @>
        let aSetMi = getmi <@ [|1uy|].[0] <- 1uy @>
        let eqMi = getmi <@ 1uy = 2uy @>

        let sV = Var("s",typeof<array<byte>>)
        let iV = Var("i",typeof<int>)
        //let resV = Var("areEq",typeof<bool>)

        let s = Expr.Var sV
        let i = Expr.Var iV

        let vars = Array.init l (fun i -> Var ("s_" + string i, typeof<byte>))

        let makeExpr n =
            let eqLeft = Expr.Call(aGetMi,[s;Expr.Call(plusMi,[i;Expr.Value(n)])])
            //let eqLeft = Expr.Var vars.[n]
            let eqRight = Expr.Value(pat.[n])
            Expr.Call(eqMi,[eqLeft;eqRight])
             
        let b = 
            let lst = Array.init l makeExpr |> List.ofArray
            let rec go lst =
                match lst with
                | hd::tl -> Expr.IfThenElse(hd,go tl,Expr.Value(false))
                | [] -> Expr.Value(true)
            //Expr.VarSet(resV,go lst)
            go lst
            
//        let lets = 
//            vars
//            |> Array.mapi (fun j v -> v,Expr.Call(aGetMi,[s;Expr.Call(plusMi,[i;Expr.Value(j)])]))
//            |> Array.fold  (fun b (v,e) -> Expr.Let(v,e,b)) b
        let r  = Expr.Lambda(sV,Expr.Lambda(iV,b))
        r// :?> Expr<array<byte> -> array<byte> -> int -> int -> bool>

    let command pat =
        <@
            fun (rng:_1D) (s:array<_>) (res:array<_>) sL subL ->
                let i = rng.GlobalID0
                let s_0 = s.[i]
                let s_1 = s.[i+1]
                let s_2 = s.[i+2]
                let s_3 = s.[i+3]
                let s_4 = s.[i+4]
                let s_5 = s.[i+5]
                let s_6 = s.[i+6]
                let s_7 = s.[i+7]
                let s_8 = s.[i+8]
                let s_9 = s.[i+9]
                let s_10 = s.[i+10]
                let s_11 = s.[i+1]
                let s_12 = s.[i+2]
                let s_13 = s.[i+3]
                let s_14 = s.[i+4]
                let s_15 = s.[i+5]
                let s_16 = s.[i+6]
                let s_17 = s.[i+7]
                let s_18 = s.[i+8]
                let s_19 = s.[i+9]
                let s_20 = s.[i+10]
                let s_30 = s.[i]
                let s_31 = s.[i+1]
                let s_32 = s.[i+2]
                let s_33 = s.[i+3]
                let s_34 = s.[i+4]
                let s_35 = s.[i+5]
                let s_36 = s.[i+6]
                let s_37 = s.[i+7]
                let s_38 = s.[i+8]
                let s_39 = s.[i+9]
                let mutable r = 0uy
                let c = ref 0
                c := !c + 2
                let dif = sL - i
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                let dif = sL - i
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                let dif = sL - i
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                let dif = sL - i
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                if subL <= dif && (((%% genComparator pat):array<byte> -> int -> bool) s i)
                then r <- 1uy
                
                if r > 0uy then res.[i] <- r
        @>

    let length = s.Length
    let mutable localWorkSize = 100    
    let kernel, kernelPrepare, kernelRun = provider.Compile (command sub)
    let dim = new _1D(length, localWorkSize)
    let res = Array.zeroCreate length
    for l in 0..10 do
        kernelPrepare dim s res length sub.Length        
        commandQueue.Add(kernelRun()).Finish() |> ignore

    //let _ = commandQueue.Add(b.ToHost provider).Finish()
    //let result = Array.zeroCreate b.[0]
    let _ = commandQueue.Add(res.ToHost(provider)).Finish()
    //result
    let r = new ResizeArray<_>()
    res |> Array.iteri (fun i x -> if x <> 0uy then r.Add i)
    r.ToArray()

let findSubstr2 (s:array<byte>) (sub:array<byte>) =

    let command =
        <@
            fun (rng:_1D) (s:array<_>) (sub:array<_>) (res:array<_>) sL subL b ->
                let i = rng.GlobalID0
                let k = i % subL + b
                if s.[i] = sub.[k]
                then res.[i] <- 1
        @>

    let cSum = 
        <@
            fun (rng:_1D) (res:array<_>) sL subL -> 
                let i = rng.GlobalID0
                let mutable e = i + subL - 1
                if e > sL - 1 then e <- sL - 1
                let mutable buf = 0
                for j in i..e do
                    buf <- buf + res.[j]
                res.[i] <- buf

        @>
    let length = s.Length
    let mutable localWorkSize = 100
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let dim = new _1D(length, localWorkSize)
    let res = Array.zeroCreate length
    let sl = sub.Length
    for i in 0 .. sl-1 do
        kernelPrepare dim s sub res length sl i
        commandQueue.Add(kernelRun()) |> ignore
    let kernel2, kernelPrepare2, kernelRun2 = provider.Compile cSum
    kernelPrepare2 dim res length sl 
    commandQueue.Add(kernelRun2()) |> ignore
    commandQueue.Finish() |> ignore
    let _ = commandQueue.Add(res.ToHost provider).Finish()
    let r = new ResizeArray<_>()
    res |> Array.iteri (fun i x -> if x = sl then r.Add i)
    r.ToArray()


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
let _gpuSum = ref 0
let l = 1950 //00000
let sl = 32
let st = 40//00
let idxs = Array.init ((l/(sl*st))-1 ) (fun i -> i*sl*st)
    //[|2; 45500; 1245; 9800; 10000; 6000; 3005; 200000; 3000445;8000;12000;14000;|]
let _sig = 
    let a = 
        //Array.init sl (fun x -> random.Next())
        Array.zeroCreate sl
    random.NextBytes a
    a
    //[|0uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;255uy;0uy;1uy;1uy;2uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;254uy;0uy;1uy;1uy;2uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;254uy;0uy;1uy;1uy;2uy
      //       ; 0uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;255uy;0uy;1uy;1uy;2uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;254uy;0uy;1uy;1uy;2uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;254uy;0uy;1uy;1uy;2uy|]
//[|0uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;100uy;1uy;66uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;56uy;1uy;2uy;254uy;0uy;1uy;1uy;2uy|]
let arr = 
    //Array.init l (fun x -> random.Next())
    Array.zeroCreate l
random.NextBytes arr
for i in idxs do
    _sig |> Array.iteri (fun j x -> arr.[j+i] <- x)

let x () =
    
    let x = ref [||]
    time (fun () ->  x:= findSubstr
                            arr _sig
        //[|0uy;1uy;0uy;1uy;1uy;0uy;1uy;1uy;5uy;0uy;1uy;1uy;2uy;0uy;1uy;1uy;0uy;1uy;1uy;0uy;1uy;2uy;1uy;0uy;1uy;1uy;2uy|] [|0uy;1uy;1uy;2uy|]
        ) |> printfn "%A"
    (!x).Length |> printfn "Count = %A"
    //!x|> Seq.iter (printf "%A; ")

do x() |> printfn "%A"
//
//(fun () -> cpuSum := Array.sum cpuArr )
//|> time
//|> printfn "cpu time: %A"  
//
//
//
//(fun () -> _gpuSum := 
//                gpuSum3 gpuArr 100)
//                //gpuSumk gpuArr k )
//                // gpuSum [|2;3;4;5;1|] )
//|> time
//|> printfn "gpu time: %A"
//
//printfn "%A" cpuSum
//printfn "%A" _gpuSum
//


//let f = <@fun x -> (x*x+x+x+x+x+x+x)/(x+1)/(x+1)@>
//let _f = f.Compile()()
//(fun () ->  cpuSum := (Array.iteri (fun i x -> cpuArr.[i] <- _f x) cpuArr; Array.iteri (fun i x -> cpuArr.[i] <- _f x) cpuArr; Array.sum cpuArr))
//|> time
//|> printfn "cpu iter time: %A"  
//
//
//fun () -> _gpuSum :=(gpuiter gpuArr f; gpuiter gpuArr f; gpuSum3 gpuArr 1000)
//|> time
//|> printfn "gpu iter time: %A"  
//
//provider.CloseAllBuffers()
//
//printfn "%A" cpuSum
//printfn "%A" _gpuSum
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