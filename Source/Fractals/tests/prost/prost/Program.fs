module fr

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions


let mandelbrotcpu (cx:array<_>) scaling size mx my boxwidth boxheight =                                   
    for x in 0..boxwidth-1 do
        for y in 0..boxheight-1 do
            let fx = float x / size * scaling + mx
            let fy = float y / size * scaling + my
            let cr = fx
            let ci = fy
            let iter = 4000
            let mutable flag = true
            let mutable zr1 = 0.0
            let mutable zi1 = 0.0
            let mutable count = 0;
            while flag && count < iter do
                if (zr1 * zr1 + zi1 * zi1 ) <= 4.0
                then                        
                    let t = zr1
                    zr1 <- (zr1 * zr1 - zi1 * zi1 + cr)
                    zi1 <- (2.0 * zi1 * t + ci)
                    count <- count + 1
                else
                    flag <- false
            if count = iter
            then
                cx.[x * boxwidth + y] <- 0
            else
                cx.[x * boxwidth + y] <- count
    cx

let juliacpu (cx: array<_>) scaling size mx my cr ci boxwidth boxheight =
    for x in 0..boxwidth-1 do
        for y in 0..boxheight-1 do
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
                cx.[x * boxwidth + y] <- 0
            else
                cx.[x * boxwidth + y] <- count
    cx
let cx: int[] = Array.zeroCreate 160000
//printfn "%A" (mandelbrotcpu cx 0.5 1.0 -1.5 -1.0 400 400)
//printfn "%A" (juliacpu cx 0.5 100.0 -1.5 -1.0 0.4 0.24 400 400)




let Mandelbrotgpu (cx: int[]) scaling size mx my boxwidth boxheight = 
    let localWorkSize = 20
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
            fun (r:_2D) (cx: array<_>) scaling size mx my boxwidth boxheight  ->
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
                        zr1 <- zr1 * zr1 - zi1 * zi1 
                        zi1 <- 2.0 * zi1 * t
                        count <- count + 1
                    else
                        fl <- false
                if count = iter
                then
                    cx.[x*boxwidth + y] <- 0
                else
                    cx.[x*boxwidth + y] <- count
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = (new _2D(boxwidth, boxheight, localWorkSize, localWorkSize))
    kernelPrepare d cParallel scaling size mx my boxwidth boxheight
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    cParallel

let Juliagpu (cx: int[]) scaling size mx my cr ci boxwidth boxheight = 
    let localWorkSize = 20
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
            fun (r:_2D) (cx: array<_>) scaling size mx my cr ci boxwidth boxheight->
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
                    cx.[x*boxwidth + y] <- 0
                else
                    cx.[x*boxwidth + y] <- count
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = (new _2D(boxwidth, boxheight, localWorkSize, localWorkSize))
    kernelPrepare d cParallel scaling size mx my cr ci boxwidth boxheight
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    cParallel

