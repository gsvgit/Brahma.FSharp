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
                let maxr = 1.0
                let maxi = 1.0
                let minr = -1.78
                let mini = -1.0
                let zr = 0.0
                let zi = 0.0
                let cr = fx
                let ci = fy
                let iter = 20
                let mutable flag = false
                let mutable flag2 = true
                let mutable zr1 = zr
                let mutable zi1 = zi
                let mutable count = 0;
                while (flag2) do
                    if (minr < zr1) && (mini < zi1) && (zr1 < maxr) && (zi1 < maxi) && (count < iter)
                    then
                        count <- count + 1
                        zr1 <- (zr1*zr1 - zi1*zi1 + cr)
                        zi1 <- (zr1*zi1 + zi1*zr1 + ci)
                    else
                        flag2 <- false 
                        if count=iter
                        then flag <- true  
                        else flag <- false                
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
//let f (a:array<int>) =
//    for i in 0..(a.Length-1) do
//        printf "%A" a.[i]
//f Main

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

//let form =
//   let scaling = 0.5 
//   let size = 100.0
//   let image = new Bitmap(400, 400)
//   for x = 0 to (image.Height-1) do
//     for y = 0 to (image.Width-1) do
//       let fx = (float x)/size * scaling + (-1.5)
//       let fy = (float y)/size * scaling + (-1.0)
//       //let t = move((float)x, (float)y,-1.5, -1.0)
//       //image.SetPixel(x, y, if (mandl (0.0, 0.0) t 20 0) then Color.Black else Color.White)
//       image.SetPixel(x, y, if (mandl 0.0 0.0 fx fy 20) then Color.Black else Color.White)
//   let temp = new Form()
//   temp.Paint.Add(fun e -> e.Graphics.DrawImage(image, 0, 0))
//   temp.SetBounds(400, 400, 410, 450)
//   temp


//    let rec mandl zr zi cr ci iter count =
//        if (minr < zr) && (mini < zi) && (zr < maxr) && (zi < maxi) && (count < iter) then
//            mandl ((zr*zr - zi*zi + cr), (zr*zi+zi*zr+ci)) (cr, ci) iter (count+1)  
//        else
//            if count = iter
//            then true
//            else false
////    let mandl =
////        <@ 
////            fun zr zi cr ci iter ->
////                let maxr = 1.0
////                let maxi = 1.0
////                let minr = -1.78
////                let mini = -1.0 
////                let mutable flag = true
////                let mutable zr1 = zr
////                let mutable zi1 = zi
////                let mutable count = 0
////                while (flag = true) && (count <= iter) do
////                    if (minr < zr1) && (mini < zi1) && (zr1 < maxr) && (zi1 < maxi)
////                    then
////                        count <- count + 1
////                        zr1 <- (zr1*zr1 - zi1*zi1 + cr)
////                        zi1 <- (zr1*zi1 + zi1*zr1 + ci)
////                    else
////                        flag <- false
////                flag
////        @>
//    let scaling = 0.5 
//    let size = 100.0
//    let move =
//        <@ fun x y mx my ->
//            let fx = x/size * scaling + mx
//            let fy = y/size * scaling + my
//            (fx, fy) @>