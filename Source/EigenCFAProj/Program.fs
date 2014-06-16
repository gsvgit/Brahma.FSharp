module EigenCFA.EigenCFA

open EigenCFA.StaticData
open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open System.IO
open System
open System.Reflection

let sw = System.Diagnostics.Stopwatch()

let divUp num den = (num + den - 1) / den
let deviceType = DeviceType.Default
let platformName = "*"

let provider =
    try  ComputeProvider.Create(platformName, deviceType)
    with
    | ex -> failwith ex.Message

//let provider =
//    try  ComputeProvider.Create(platformName, deviceType)
//    with
//    | ex -> failwith ex.Message
//
//let initStore = 
//        <@ fun (r:_2D) (devStore:array<_>) (scaleExp) (scaleM:int) (scaleVar:int) -> 
//                let column = r.GlobalID0
//                let row = r.GlobalID1
//
//                if(row < scaleExp && column < scaleM) then 
//                    if(row < scaleVar) then
//                        if(column % scaleM = 0) then
//                            devStore.[row*scaleM + column] <- 1
//                        else
//                            devStore.[row*scaleM + column] <- -1
//                    else
//                        if(column = 0) then
//                            devStore.[row*scaleM + column] <- 2
//                        else 
//                            if(column = 1) then
//                                devStore.[row*scaleM + column] <- row - scaleVar + 1
//                            else 
//                                devStore.[row*scaleM + column] <- -1 
//        @>

let qEigenCFA = 
        <@ fun (r:_2D)
            (devFun:array<_>)
            (devArg1:array<_>)
            (devArg2:array<_>)
            (devStore:array<_>)
            (devRep:array<_>)
            devScaleM
            devScaleCall
            devScaleLam ->
                let column = r.GlobalID0
                let row = r.GlobalID1
                if(column < devScaleCall && row < 2) then
                    let numCall = column
                    let Argi index =  
                        if(index = 0) then devArg1.[numCall]
                        else devArg2.[numCall]
                    let L index = devStore.[devFun.[numCall]*devScaleM + index]
                    let Li index = devStore.[(Argi row)*devScaleM + index]
                    let rowStore row column = devStore.[row*devScaleM + column]
                    let vL j =
                        if(row = 0) then
                            (L j) - 1
                        else
                            (L j) - 1 + devScaleLam
                    for j in 1 .. ((L 0) - 1) do
                        for k in 1 .. ((Li 0) - 1) do
                            let mutable isAdd = 1
                            let addVar = (Li k)
                            for i in 1 .. ((rowStore (vL j) 0) - 1) do
                                if((rowStore (vL j) i) = addVar) then 
                                    isAdd <- 0
                            if(isAdd > 0) then
                                devRep.[0] <- devRep.[0] + 1
                                let tail = (rowStore (vL j) 0)
                                devStore.[(vL j)*devScaleM] <- devStore.[(vL j)*devScaleM] + 1
                                devStore.[(vL j)*devScaleM + tail] <- addVar
        @>

//let provider =
//    try  ComputeProvider.Create(platformName, deviceType)
//    with
//    | ex -> failwith ex.Message
//            
let kernelCFA,kernelPrepareFCFA, kernelRunF = provider.Compile qEigenCFA    
let commandQueueCFA = new CommandQueue(provider, provider.Devices |> Seq.head) 

let initCPUStore scaleExp scaleM =
    let devStore = Array.zeroCreate (scaleExp*scaleM)

    for row in 0..(scaleExp - 1) do
        for column in 0..(scaleM - 1) do
            if(row < scaleExp && column < scaleM) then 
                if(row < scaleVar) then
                    if(column % scaleM = 0) then
                        devStore.[row*scaleM + column] <- 1
                    else
                        devStore.[row*scaleM + column] <- -1
                else
                    if(column = 0) then
                        devStore.[row*scaleM + column] <- 2
                    else 
                        if(column = 1) then
                            devStore.[row*scaleM + column] <- row - scaleVar + 1
                        else 
                            devStore.[row*scaleM + column] <- -1 
    devStore

let kernelRun (ArrayFun:int[]) (ArrayArg1:int[]) (ArrayArg2:int[]) =
            let n = scaleM*scaleExp
           // let mutable dev_Store = Array.zeroCreate n
//            let dev_Fun = Array.zeroCreate (ArrayFun.Length)
//            let dev_Arg1 = Array.zeroCreate (ArrayArg1.Length)
//            let dev_Arg2 = Array.zeroCreate (ArrayArg2.Length)
            let dev_rep = Array.zeroCreate 1
            let dev_outPut = Array.zeroCreate (scaleCall*2)

//            Array.Copy(ArrayFun, dev_Fun, ArrayFun.Length)
//            Array.Copy(ArrayArg1, dev_Arg1, ArrayArg1.Length)
//            Array.Copy(ArrayArg2, dev_Arg2, ArrayArg2.Length)

            let dev_Store = initCPUStore scaleExp scaleM            
   
            sw.Restart()
           
            let mutable rep = -1
            let mutable curRep = -2
            let mutable iter = 1

            let repArray = Array.zeroCreate 1


            while(rep <> curRep) do
                iter <- iter + 1
                rep <- curRep
                let EigenCFA = 
                    kernelPrepareFCFA (new _2D(scaleCall,2)) ArrayFun ArrayArg1 ArrayArg2 dev_Store repArray scaleM scaleCall scaleLam
                    let cq = commandQueueCFA.Add(kernelRunF()).Finish()
                    let r = Array.zeroCreate 1
                    let cq2 = commandQueueCFA.Add(repArray.ToHost(provider,r)).Finish()
                   // printf "%A\n" r
                    r
                let a = EigenCFA
                curRep <- a.[0]

            let cq2 = commandQueueCFA.Add(dev_Store.ToHost(provider)).Finish()

            sw.Stop()
            
            provider.Dispose()
            (double sw.ElapsedMilliseconds)

let run Fun Arg1 Arg2 numE =
    let mutable fullTime = 0.0
    let numIter = 5
    printf "test %d lam %d call e %d\n" scaleLam scaleCall numE
    for i in 0 .. numIter do

        fullTime <- fullTime + (kernelRun Fun Arg1 Arg2)

    writeResTestInFile pathToResFile "AleaEigenCFA.txt" (fullTime/(float numIter)) (scaleLam * 3 + scaleCall) numE
    ()

let Start maxMem path = 
    let mutable a = 0
    let startAllTests = 
        for subdir in dirs path do
            if a < 100
            then
                a <- a + 1
                if (isAllFilesContains subdir) then
                    pathToFun <- System.IO.Path.Combine(subdir, nameFileFun)
                    pathToArg1 <- System.IO.Path.Combine(subdir, nameFileArg1)
                    pathToArg2 <- System.IO.Path.Combine(subdir, nameFileArg2)
                    pathToScales <- System.IO.Path.Combine(subdir, nameFileScales)

                    let scales = readMapInt pathToScales
                    scaleCall <- scales.[0]
                    scaleLam <- scales.[1]
                    scaleVar <- scales.[2]
                    scaleExp <- scaleLam + scaleVar
                    scaleM <- (int ((float (scaleLam)))) + 2

                    //printf "%A" (initArrayGpu pathToFun)
                    if(((scaleExp* scaleM) / (1024*1024)) < maxMem) then
                        try
                            run (initArrayGpu pathToFun) (initArrayGpu pathToArg1) (initArrayGpu pathToArg2) 2
                        with _ -> printf "error mem\n"
    startAllTests

[<EntryPoint>]
let Main(args) =    
    pathToResFile <- args.[1]
    Start 900 args.[0]
    0