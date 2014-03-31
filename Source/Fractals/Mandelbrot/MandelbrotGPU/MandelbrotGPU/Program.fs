open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.Drawing
open System.Windows.Forms 
let Main =    
    let localWorkSize = 10
    let deviceType = DeviceType.Gpu

    let provider =
        try  ComputeProvider.Create("NVIDIA*", deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    let cParallel: array<int> = Array.zeroCreate 160000
    let command = 
        <@                            
            fun (r:_2D)  (cx:array<_>) ->                
                let x = r.GlobalID0
                let y = r.GlobalID1
                let scaling = 0.5 
                let size = 100.0
                let fx = float x / size * scaling +  -1.5
                let fy = float y / size * scaling +  -1.0
                let cr = fx
                let ci = fy
                let iter = 20
                let mutable flag = true
                let mutable zr1 = 0.0
                let mutable zi1 = 0.0
                for i in 0..iter do
                    if (zr1*zr1 + zi1*zi1 ) <= 4.0
                    then
                        let t = zr1
                        zr1 <- (zr1*zr1 - zi1*zi1 + cr)
                        zi1 <- (2.0 * zi1 * t + ci)
                    else
                        flag <- false                    
                if flag 
                then
                    cx.[x*400+y] <- 1
                else 
                    cx.[x*400+y] <- 0  
        @>
    printf "Start1"
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    printf "Start2"
    let d =(new _2D(400, 400, localWorkSize, localWorkSize))

    kernelPrepare d cParallel   
    let _ = commandQueue.Add(kernelRun()).Finish()            
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //ignore (System.Console.Read())
    cParallel

let draw  (image:Bitmap) (arr1:array<int>) =
    let mutable t = -400; 
    for x = 0 to 399 do
        t <- t + 400
        for y = 0 to 399 do 
            image.SetPixel(x, y,  if (arr1.[t+y] <> 0) then  Color.Black else Color.White)

let form =
    let temp = new Form()
    let image = new Bitmap(400, 400);
    draw image Main
    temp.Paint.Add(fun e -> e.Graphics.DrawImage(image, 0, 0))
    temp.SetBounds(400, 400, 410, 450)
    temp

do Application.Run(form);;