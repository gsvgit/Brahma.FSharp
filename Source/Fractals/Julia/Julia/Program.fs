module JuliaDraw

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.Drawing
open System.Windows.Forms

let JuliaDraw scaling size mx my cr ci = 
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
    cParallel


let draw scaling size mx my cr ci = 
    let image = new Bitmap (400,400)
    let arr1 = JuliaDraw scaling size mx my cr ci
    let mutable s = -400
    for x = 0 to 399 do
        s <- s + 400
        for y = 0 to 399 do
        let color = Color.FromArgb(10 * arr1.[s + y] % 255, 5 * arr1.[s + y] % 255, arr1.[s + y] % 255)
        image.SetPixel(x, y, color)
    image

let f scaling size mx my cr ci = 
    let form = 
        let temp = new Form()
        temp.Paint.Add(fun e -> e.Graphics.DrawImage(draw scaling size mx my cr ci, 0, 0))
        temp.SetBounds(400, 400, 410, 450)
        temp
    do Application.Run(form)

f 0.75 100.0 -1.5 -1.0 -0.75 0.17
//f 0.7 100.0 -1.5 -1.5 0.4  0.24
//f 0.7 100.0 -1.5 -1.5 0.4  0.27