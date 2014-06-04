module Agents

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions


type MessageType =
    | Matrix of array<float32>
    | Stop

let matrixGeneartor x =
    let rows = 1000
    let cols = 1000
    let random = new System.Random()
    let makeMatrix () =
        Array.init (rows * cols) (fun i -> float32 (random.NextDouble()))
    MailboxProcessor.Start(fun agent ->
        async {
              for i in 0..x do
                makeMatrix() |> Matrix |> agent.Post
              agent.Post Stop
        }        
    )

let matrixMultilpier platformName =
    let rows = 1000
    let cols = 1000
    let aValues = Array.zeroCreate(rows * cols)
    let bValues = Array.zeroCreate(rows * cols)    
    let cParallel = Array.zeroCreate(rows * cols)
    let cParallel2 = Array.zeroCreate(rows * cols)

    let localWorkSize = 10
    let deviceType = DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)


    let command = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * cols + tx]
                for k in 0 .. cols - 1 do
                    buf <- buf + (a.[ty * cols + k] * b.[k * cols + tx])
                c.[ty * cols + tx] <- buf
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d = new _2D(rows, cols, localWorkSize, localWorkSize)
    kernelPrepare d aValues bValues cParallel
    commandQueue.Add(kernelRun()) |> ignore
    MailboxProcessor.Start(fun agent ->
        let rec loop () = 
            async {                          
               let! arg = agent.Receive()
               match arg with
               | Matrix matrix ->
                    commandQueue.Add(aValues.ToGpu(provider, matrix)).Add(bValues.ToGpu(provider, matrix)).Finish() |> ignore
                    commandQueue.Add(kernelRun()) |> ignore
                    commandQueue.Add(cParallel.ToHost(provider)).Finish()
                    |> ignore
                    printfn "Matrix on %A = %A" platformName cParallel.[0]
                    commandQueue.Add(cParallel.ToGpu(provider, cParallel2)).Finish() |> ignore
                    return! loop ()
               | Stop -> ()
            }        
        loop()
    )

let main () =
    let mg = matrixGeneartor 10
    let mm1 = matrixMultilpier "AMD*"
    let mm2 = matrixMultilpier "NVIDIA*"
    let exit = ref false
    //let mm2 = matrixMultilpier "AMD*"
    let rec l i =
        let msg = mg.Receive () |> Async.RunSynchronously
        match msg with
        | Stop -> 
            exit := true
            mm1.Post msg
            mm2.Post msg
        | _ -> (if i % 2 = 0 then mm1 else mm2).Post msg
               l (i+1)
    l 0    
    //System.Threading.Thread.CurrentThread.Join()
main ()