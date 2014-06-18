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

let t = ref 0

let matrixGeneartor x n =
    let rows = n
    let cols = n
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

let matrixMultilpier platformName n =
    let rows = n
    let cols = n
    let aValues = Array.zeroCreate(rows * cols)
    let bValues = Array.zeroCreate(rows * cols)    
    let cParallel = Array.zeroCreate(rows * cols)
    let cParallel2 = Array.zeroCreate(rows * cols)

    let localWorkSize = 32
    let deviceType = DeviceType.Default

    use provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let commandQueue = new CommandQueue(provider, (provider).Devices |> Seq.head)


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

    let kernel, kernelPrepare, kernelRun = (provider).Compile command
    let d = new _2D(rows, cols, localWorkSize, localWorkSize)
    kernelPrepare d aValues bValues cParallel
    //(!commandQueue).Add(kernelRun()) |> ignore
    MailboxProcessor.Start(fun agent ->
        let rec loop () = 
            async {                          
               let! arg = agent.Receive()
               match arg with
               | Matrix matrix ->
                    (commandQueue).Add(aValues.ToGpu(provider, matrix)).Add(bValues.ToGpu(provider, matrix)) |> ignore
                    (commandQueue).Add(kernelRun()) |> ignore
                    (commandQueue).Add(cParallel.ToHost(provider)).Finish()
                    |> ignore
                    //printfn "Matrix on %A = %A" platformName cParallel.[0]
                    (commandQueue).Add(cParallel.ToGpu(provider, cParallel2)).Finish() |> ignore
                    lock t (fun () -> incr t)
                    return! loop ()                    
               | Stop ->                     
                    //(provider).CloseAllBuffers()                    
                    //CommandQueue.Cleanup()
                    //(commandQueue).Dispose()                    
                    //(provider).Dispose()
                    //provider := null
                    for kvp in provider.AutoconfiguredBuffers do
                        kvp.Value.Dispose()
                    provider.AutoconfiguredBuffers.Clear()    
                    printfn "Disposed"                       
            }        
        loop()
    )

let main i n =
    printfn "I=%A" i
    let providers = Array.init i (fun _ -> use t = matrixMultilpier "NVIDIA*" n in t)        
    let start = System.DateTime.Now
    let mg = matrixGeneartor 1000 n       
    let rec l i =
        let msg = mg.Receive () |> Async.RunSynchronously
        match msg with
        | Stop -> for p in providers do p.Post msg
        | _ -> 
            //printfn "i=%A" i
            (providers.[i % providers.Length]).Post msg
            l (i+1)
    l 0    
    while !t <> 1000 do ()
    let res = (System.DateTime.Now - start)    
    for (a:MailboxProcessor<_>) in providers do (a :> System.IDisposable).Dispose()
    System.GC.Collect()
    System.GC.WaitForPendingFinalizers()
    System.GC.WaitForFullGCComplete() |> ignore
    System.GC.Collect()
    System.Threading.Thread.Sleep(10000)    
    t:=0
    res
    
[for n in [64; 320; 640; 1280; 1600; 3200] -> 
    [for i in 1..4 ->         
        main i n        ]]
|> List.iter 
    (fun x -> 
        printfn "_______________"
        x |> List.iter(printfn "Time=%A"))