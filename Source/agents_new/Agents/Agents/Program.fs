open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions


type GpuConfig =
    val Name: string
    val Workers: int
    new (n,w) = {Name=n;Workers=w}

type MasterConfig =
    val Workers: Option<int>
    val GpuConfigs: array<GpuConfig>
    new (w,gc) = {GpuConfigs=gc;Workers=w}

type msg<'data,'res> =
    | Die
    | Process of 'data*(array<'res> -> unit)
    | Fill of 'data*('data -> unit)
    | InitBuffers of array<'data>*AsyncReplyChannel<array<'data>>
    | Get of AsyncReplyChannel<'data>
    | Enq of 'data

type Reader (isDataEnd, fillF) =    
    let inner =
        MailboxProcessor.Start(fun inbox ->
            let rec loop n =
                async { let! msg = inbox.Receive()
                        match msg with
                        | Die -> return ()
                        | Fill (x,cont) ->
                            let filled = fillF x
                            //printfn "FILL"
                            if filled |> Option.isSome
                            then 
                                cont x 
                                return! loop n
                            else isDataEnd := true
                        | x -> 
                            printfn "unexpected message for reader: %A" x
                            return! loop n }
            loop 0)
    
    member this.Read(a, cont) = inner.Post(Fill(a, cont))
    member this.Die() = inner.Post Die    

type Worker(f) =

    let inner =
        MailboxProcessor.Start(fun inbox ->
            let rec loop n =
                async { let! msg = inbox.Receive()
                        match msg with
                        | Die -> return ()
                        | Process (x,continuation) -> 
                            //printfn "PROCESS"
                            let r = f x
                            continuation r
                            return! loop n                        
                        | x -> 
                            printfn "unexpected message for Worker: %A" x
                            return! loop n }
            loop 0)
 
    member this.Process(a, continuation) = inner.Post(Process(a,continuation))
    member this.Die() = inner.Post(Die)


type DataManager(readers:array<Reader>) =
    let dataToProcess = new System.Collections.Generic.Queue<_>()
    let dataToFill = new System.Collections.Generic.Queue<_>()    
    let inner =
        MailboxProcessor.Start(fun inbox ->
            let rec loop n =
                async { 
                        //printfn "Data to fill: %A" dataToFill.Count
                        if dataToFill.Count > 0
                        then
                            let b = dataToFill.Dequeue()
                            readers.[0].Read(b, fun a -> dataToProcess.Enqueue a)                        
                        if inbox.CurrentQueueLength > 0
                        then
                            let! msg = inbox.Receive()
                            match msg with
                            | Die -> return ()
                            | InitBuffers (bufs,ch) ->
                                ch.Reply bufs
                                bufs |> Array.iter dataToFill.Enqueue                                
                                return! loop n
                            | Get(ch) -> 
                                //printfn "GET"
                                while dataToProcess.Count = 0 do ()
                                let b = dataToProcess.Dequeue()
                                ch.Reply b
                                return! loop n
                            | Enq b -> 
                                dataToFill.Enqueue b
                                return! loop n
                            | x ->  
                                printfn "Unexpected message for Worker: %A" x
                                return! loop n
                            else return! loop n }
            loop 0)
 
    member this.InitBuffers(bufs) = inner.PostAndReply((fun reply -> InitBuffers(bufs,reply)), timeout = 200000)
    member this.GetData() = inner.PostAndReply((fun reply -> Get reply), timeout = 200000)
    member this.Enq(b) = inner.Post(Enq b)
    member this.Die() = inner.Post(Die)

let rows = 1100
let columns = 1100

type Master((*config:MasterConfig,*) workers:array<Worker>, fill, bufs:ResizeArray<_>) =        
    
    let isDataEnd = ref false
    let reader = new Reader(isDataEnd, fill)
    let instancesCount = 4
    let mutable isEnd = false
            
    let dataManager = new DataManager([|reader|])

    let bufers = dataManager.InitBuffers(bufs.ToArray())
    let freeWorkers = new System.Collections.Concurrent.ConcurrentQueue<_>(workers)
    let inner =
        MailboxProcessor.Start(fun inbox ->
            let rec loop n =
                async {
                        if not freeWorkers.IsEmpty
                        then 
                            let success,w = freeWorkers.TryDequeue()
                            if success 
                            then
                                let b = dataManager.GetData() 
                                w.Process(b, fun a -> printfn "res=%A" a; freeWorkers.Enqueue w; dataManager.Enq b)
                        if inbox.CurrentQueueLength > 0
                        then
                            let! msg = inbox.Receive()
                            match msg with
                            | Die -> 
                                workers |> Array.iter (fun w -> w.Die())
                                reader.Die()
                                dataManager.Die()
                                return ()
                            | x ->
                                printfn "unexpected message for Worker: %A" x
                                return! loop n 
                        else return! loop n}
            loop 0)

    member this.Die() = inner.Post(Die)
    member this.IsDataEnd() = !isDataEnd


let workerF platformName deviceType localWorkSize i =        

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    //printfn "Using %A" provider

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.nth i)    

    let cParallel = Array.zeroCreate(rows * columns)
    let aValues = Array.zeroCreate(rows * columns)
    let bValues = Array.zeroCreate(rows * columns)    

    let command rows columns = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * columns + tx]
                for k in 0 .. columns - 1 do
                    buf <- buf + (a.[ty * columns + k] * b.[k * columns + tx])
                c.[ty * columns + tx] <- buf
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile <| command rows columns
    let d =(new _2D(rows, columns, localWorkSize, localWorkSize))
    do kernelPrepare d aValues bValues cParallel

    let f = fun (x,y) ->
        let _ = commandQueue.Add(aValues.ToGpu(provider, x))
        let _ = commandQueue.Add(bValues.ToGpu(provider, y))
        let _ = commandQueue.Add(kernelRun())
        let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
        [|cParallel.Length|]
    f, aValues, bValues


let platformName = "NVIDIA*"

let deviceType = DeviceType.Gpu    

let bufs = new ResizeArray<_>()
for i in 0 ..5 do bufs.Add(Array.zeroCreate(rows * columns),Array.zeroCreate(rows * columns))
let workers platformName deviceType localWorkSize = 
    Array.init 4 
        (fun i -> 
            let f,a,b = workerF platformName deviceType localWorkSize (i%2)
            bufs.Add(a,b)
            new Worker(f))


let random = new System.Random()    

let count = ref 1000

let fill (x,y) =
    if !count > 0
    then
        Array.fill x 0 (rows * columns) (float32 (random.NextDouble()))
        Array.fill y 0 (rows * columns) (float32 (random.NextDouble()))
        decr count
        Some (x,y)
    else None

let start = System.DateTime.Now
let master = new Master(workers platformName deviceType 10, fill, bufs)
while not <| master.IsDataEnd() do ()
master.Die()
printfn "Time = %A" <| System.DateTime.Now - start
