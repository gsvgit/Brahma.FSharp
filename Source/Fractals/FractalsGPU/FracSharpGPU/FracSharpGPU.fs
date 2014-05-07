module FracSharpGPU

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.Drawing
open System.Windows.Forms 
//let cParallel: array<int> = Array.zeroCreate 160000

let deviceType = DeviceType.Gpu

let provider =
    try  ComputeProvider.Create("NVIDIA*", deviceType)
    with 
    | ex -> failwith ex.Message

let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

let localWorkSize = 20
//let d = (new _2D(400, 400, localWorkSize, localWorkSize))

let mandelbrot =
    <@                            
        fun (r:_2D) (cx:array<_>) scaling size mx my boxwidth boxheight->                
            let x = r.GlobalID0
            let y = r.GlobalID1
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
        @>

let julia = 
        <@
            fun (r:_2D) (cx: array<_>) scaling size mx my cr ci boxwidth boxheight ->
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
                    cx.[x * boxwidth + y] <- 0
                else
                    cx.[x * boxwidth + y] <- count

        @>


let kernel, kernelPrepare, kernelRun = provider.Compile mandelbrot
let kernel2, kernelPrepare2, kernelRun2 = provider.Compile julia

let Mandelbrot () =    
    fun scaling size mx my (cParallel:array<int>) boxwidth boxheight ->
        let d = (new _2D(boxwidth, boxheight, localWorkSize, localWorkSize))
        kernelPrepare d cParallel scaling size mx my boxwidth boxheight  
        let _ = commandQueue.Add(kernelRun()).Finish()            
        commandQueue.Add(cParallel.ToHost provider).Finish()
        |> ignore

let Julia () =
    fun scaling size mx my (cParallel:array<int>) cr ci boxwidth boxheight ->
        let d = (new _2D(boxwidth, boxheight, localWorkSize, localWorkSize))
        kernelPrepare2 d cParallel scaling size mx my cr ci boxwidth boxheight  
        let _ = commandQueue.Add(kernelRun2()).Finish()            
        commandQueue.Add(cParallel.ToHost provider).Finish()
        |> ignore

let drawIm (scaling, size, mx, my, (arr1:array<int>), (boxwidth:int), (boxheight:int), cr, ci, fcode) =
    let image = new Bitmap(boxwidth, boxheight);
    if fcode = 0
    then  
        Mandelbrot () scaling size mx my arr1 boxwidth boxheight
    else
        Julia () scaling size mx my arr1 cr ci boxwidth boxheight
    let mutable t = -boxwidth; 
    for x = 0 to boxwidth-1 do
        t <- t + boxwidth    
        for y = 0 to boxheight-1 do
            let color = Color.FromArgb(10*arr1.[t+y]%255, 5*arr1.[t+y]%255, arr1.[t+y]%255)
            image.SetPixel(x, y, color)
    image

let closeAll () =
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()