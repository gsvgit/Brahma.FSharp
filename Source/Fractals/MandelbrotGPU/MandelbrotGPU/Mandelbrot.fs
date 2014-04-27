module Mandelbrot
open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.Drawing
open System.Windows.Forms 
//let cParallel: array<int> = Array.zeroCreate 160000
let Mandelbrot scaling size mx my (cParallel:array<int>)=    
    let localWorkSize = 10
    let deviceType = DeviceType.Gpu

    let provider =
        try  ComputeProvider.Create("NVIDIA*", deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
//    let cParallel: array<int> = Array.zeroCreate 160000
    let command = 
        <@                            
            fun (r:_2D) (cx:array<_>) scaling size mx my->                
                let x = r.GlobalID0
                let y = r.GlobalID1
//                let scaling = 0.5 
//                let size = 100.0
                let fx = float x / size * scaling + mx //-1.5
                let fy = float y / size * scaling + my //-1.0
                let cr = fx
                let ci = fy
                let iter = 4000
                let mutable flag = true
                let mutable zr1 = 0.0
                let mutable zi1 = 0.0
                let mutable count = 0;
                while (flag) && (count<iter) do
                    if (zr1*zr1 + zi1*zi1 ) <= 4.0
                    then                        
                        let t = zr1
                        zr1 <- (zr1*zr1 - zi1*zi1 + cr)
                        zi1 <- (2.0 * zi1 * t + ci)
                        count <- count + 1
                    else
                        flag <- false
                if count = iter
                then
                    cx.[x*400+y] <- 0
                else
                    cx.[x*400+y] <- count
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(400, 400, localWorkSize, localWorkSize))

    kernelPrepare d cParallel scaling size mx my   
    let _ = commandQueue.Add(kernelRun()).Finish()            
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //ignore (System.Console.Read())
//    cParallel
let drawIm (scaling, size, mx, my, (arr1:array<int>)) =
    let image = new Bitmap(400, 400);
    Mandelbrot scaling size mx my arr1
    let mutable t = -400; 
    for x = 0 to 399 do
        t <- t + 400    
        for y = 0 to 399 do
            let color = Color.FromArgb(10*arr1.[t+y]%255, 5*arr1.[t+y]%255, arr1.[t+y]%255)
            image.SetPixel(x, y, color)
    image

//let form =
//    let temp = new Form()
//    temp.Paint.Add(fun e -> e.Graphics.DrawImage(drawIm (0.5, 100.0, -1.5, -1.0, cParallel), 0, 0))
//    temp.SetBounds(400, 400, 410, 450)
//    temp
//
//do Application.Run(form);;