module fr

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions



let Juliagpu scaling size mx my cr ci = 
    let localWorkSize = 10
    let deviceType = DeviceType.Default
    let provider = 
        try ComputeProvider.Create("NVIDIA*", deviceType)
        with
        | ex -> failwith ex.Message
    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    let cParallel: array<int> = Array.zeroCreate 160000
    let command = 
        <@
            fun (r:_2D) (cx: array<_>) scaling size mx my cr ci ->
                let x = r.GlobalID0
                let y = r.GlobalID1
                let fx = float x / size * scaling + mx
                let fy = float y / size * scaling + my 
                let iter = 4000
                let mutable fl =  true
                let mutable zr1 = fx
                let mutable zi1 = fy
                let mutable count =  0
                let mutable t = 0.0
                while fl && (count < iter) do
                    if zr1 * zr1 + zi1 * zi1 <= 4.0
                    then 
                        t <- zr1
                        zr1 <- zr1 * zr1 - zi1 * zi1 + cr
                        zi1 <- 2.0 * zi1 * t + ci
                        count <- count + 1
                    else
                        fl <- false
                if count = iter
                then
                    cx.[x*400 + y] <- 0
                else
                    cx.[x*400 + y] <- count
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = (new _2D(400, 400, localWorkSize, localWorkSize))
    kernelPrepare d cParallel scaling size mx my cr ci
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //cParallel



let Mandelbrotgpu scaling size mx my  = 
    let localWorkSize = 10
    let deviceType = DeviceType.Default
    let provider = 
        try ComputeProvider.Create("NVIDIA*", deviceType)
        with
        | ex -> failwith ex.Message
    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    let cParallel: array<int> = Array.zeroCreate 160000
    let command = 
        <@
            fun (r:_2D) (cx: array<_>) scaling size mx my ->
                let x = r.GlobalID0
                let y = r.GlobalID1
                let fx = float x / size * scaling + mx
                let fy = float y / size * scaling + my 
                let cr = fx
                let ci = fy
                let iter = 4000
                let mutable fl =  true
                let mutable zr1 = 0.0
                let mutable zi1 = 0.0
                let mutable count =  0
                let mutable t = 0.0
                while fl && (count < iter) do
                    if zr1 * zr1 + zi1 * zi1 <= 4.0
                    then 
                        t <- zr1
                        zr1 <- zr1 * zr1 - zi1 * zi1 + cr 
                        zi1 <- 2.0 * zi1 * t + ci
                        count <- count + 1
                    else
                        fl <- false
                if count = iter
                then
                    cx.[x*400 + y] <- 0
                else
                    cx.[x*400 + y] <- count
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = (new _2D(400, 400, localWorkSize, localWorkSize))
    kernelPrepare d cParallel scaling size mx my 
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //cParallel

let Juliacpu scaling size mx my cr ci = 
    let array = Array.zeroCreate 160000
    let main (x: int) (y: int) (array: array<_>) scaling size mx my cr ci = 
        let fx = float x / size * scaling + mx
        let fy = float y / size * scaling + my 
        let iter = 4000
        let mutable fl =  true
        let mutable zr1 = fx
        let mutable zi1 = fy
        let mutable count =  0
        let mutable t = 0.0
        while fl && (count < iter) do
                if zr1 * zr1 + zi1 * zi1 <= 4.0
                then 
                    t <- zr1
                    zr1 <- zr1 * zr1 - zi1 * zi1 + cr
                    zi1 <- 2.0 * zi1 * t + ci
                    count <- count + 1
                else
                    fl <- false
                if count = iter
                then
                    array.[x*400 + y] <- 0
                else
                    array.[x*400 + y] <- count

    let main2 scaling size mx my cr ci =
        let x = [| for i in 0 .. 399 -> i|]  
        let y = [| for i in 0 .. 399 -> i|]      
        for i in x do
            for j in y do
            main i j array scaling size mx my cr ci
    main2 scaling size mx my cr ci
    //array



let Mandelbrotcpu scaling size mx my = 
    let array = Array.zeroCreate 160000
    let main (x: int) (y: int) (array: array<_>) scaling size mx my  = 
        let fx = float x / size * scaling + mx
        let fy = float y / size * scaling + my 
        let iter = 4000
        let cr = fx
        let ci = fy
        let mutable fl =  true
        let mutable zr1 = 0.0
        let mutable zi1 = 0.0
        let mutable count =  0
        let mutable t = 0.0
        while fl && (count < iter) do
                if zr1 * zr1 + zi1 * zi1 <= 4.0
                then 
                    t <- zr1
                    zr1 <- zr1 * zr1 - zi1 * zi1 + cr
                    zi1 <- 2.0 * zi1 * t + ci
                    count <- count + 1
                else
                    fl <- false
                if count = iter
                then
                    array.[x*400 + y] <- 0
                else
                    array.[x*400 + y] <- count

    let main2 scaling size mx my  =
        let x = [| for i in 0 .. 399 -> i|]  
        let y = [| for i in 0 .. 399 -> i|]      
        for i in x do
            for j in y do
            main i j array scaling size mx my 
    main2 scaling size mx my 
    //array