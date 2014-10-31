// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open System

let random = new System.Random()

let createArr length = Array.init length (fun _ -> random.Next(10))

let platformName = "*"
let deviceType = DeviceType.Default

let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message


let gpuSum2 (arr:array<_>) = 
    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

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
    while k < length / 2 do
        let d =(new _1D(bufLen,1))        
        kernelPrepare d k length arr
        let _ = commandQueue.Add(kernelRun()) 
        k <- k*2
        bufLen <- (min (bufLen/2+1) ((bufLen + 1)/2))

    printfn "K = %A" k
    let _ = commandQueue.Finish()
    let sum = [|0|]
    let _ = commandQueue.Add(arr.ToHost(provider,sum)).Finish()    
    provider.CloseAllBuffers()
    sum.[0]

let printArr (arr: array<_>) =
    for elem in arr do
        printfn "%A; " elem

[<EntryPoint>]
let main argv = 
    let start = System.DateTime.Now
    let mutable i = 100
    while i > 0 do
     let arr = createArr 10000
     let res = arr |> gpuSum2
     i <- i - 1
    
    let time = System.DateTime.Now - start
    printfn "Time = %A" time
    Console.ReadLine() |> ignore
    0
