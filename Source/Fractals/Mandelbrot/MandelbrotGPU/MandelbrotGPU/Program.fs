open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main =    
    let localWorkSize = 1
    let deviceType = DeviceType.Gpu

    let provider =
        try  ComputeProvider.Create("NVIDIA*", deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    let cParallel: array<int> = Array.zeroCreate 400
    let cParallel2: array<int> = Array.zeroCreate 400
    let command = 
        <@                            
            fun (r:_2D) (cx:array<_>) (cy:array<_>) ->                
                let x = r.GlobalID0
                let y = r.GlobalID1
                let scaling = 0.5 
                let size : float = 100.0
                let fx = float x / size * scaling + float -1.5
                let fy = float y / size * scaling + float -1.0
                let maxr : float= 1.0
                let maxi : float= 1.0
                let minr : float= -1.78
                let mini : float= -1.0
                let zr : float = 0.0
                let zi : float= 0.0
                let cr = float x  //20
                let ci = float y
                let iter = 20
                let mutable flag = true
                let mutable zr1 = zr
                let mutable zi1 = zi
                let mutable count = 0
                while flag && (count <= iter) do
                    if (minr < zr1) && (mini < zi1) && (zr1 < maxr) && (zi1 < maxi)
                    then
                        count <- count + 1
                        zr1 <- zr1 * zr1 - zi1 * zi1 + cr
                        zi1 <- zr1 * zi1 + zi1 * zr1 + ci
                    else
                        flag <- false

                if flag  //((%mandl) 0.0  0.0 (float x) (float y) 20)
                then
                    cx.[x] <- x
                    cy.[x] <- y  
                else
                    cx.[x] <- 0
                    cy.[x] <- 0  
        @>
    printf "Start1"
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    printf "Start2"
    let d =(new _2D(cParallel.Length, cParallel.Length, localWorkSize, localWorkSize))

    kernelPrepare d cParallel cParallel2
    let _ = commandQueue.Add(kernelRun()).Finish()            
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    let _ = commandQueue.Add(cParallel2.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //ignore (System.Console.Read())
    (cParallel, cParallel2)
printfn "%A" Main

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