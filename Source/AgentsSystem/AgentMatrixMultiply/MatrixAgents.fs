namespace AsyncMatrixMultiply

module MatrixAgents =

    open System
    open System.Collections.Generic
    open System.Linq

    open Brahma.OpenCL
    open Brahma.FSharp.OpenCL.Core;
    open Brahma.FSharp.OpenCL.Extensions
    open DataSource
    open Utils

    open OpenCL.Net
    
    type Agent<'T> = MailboxProcessor<'T>

    [<AbstractClass>]
    type Agent =
        new () = {}

        abstract member Post : Messages -> unit
        abstract member PostAndAsyncReply : (AsyncReplyChannel<'Reply> -> Messages) -> Async<'Reply>

    and Messages =
    | MultiplyMatrix of (float32[] * float32[]) * AsyncReplyChannel<float32[]>
    | DataNeeded of AsyncReplyChannel<float32[] * float32[]>
    | Data of (float32[] * float32[]) * Agent
    | Start 
    
    type AgentCpuWorker =
        inherit Agent

        val worker : Agent<Messages>
    
        new (task) = {
            worker = Agent.Start(fun inbox ->
                async { 
                        ConsoleLockingHelper.WriteLine "CpuWorker started"
                        while true do 
                            let! msg = inbox.Receive()
                            
                            match msg with
                            | MultiplyMatrix(matrices, reply) -> 
                                ConsoleLockingHelper.WriteLine "Mult from CPU"
                                reply.Reply(task matrices)
                            | _ -> ()
                        }) 
        }

        override this.Post(msg : Messages) = this.worker.Post msg
        override this.PostAndAsyncReply(buildMessage : AsyncReplyChannel<'Reply> -> Messages) = 
            this.worker.PostAndAsyncReply buildMessage


    type AgentGpuWorker = 
        inherit Agent

        val worker : Agent<Messages>

        new (task) = {
            worker = Agent.Start(fun inbox ->
                async { 
                        ConsoleLockingHelper.WriteLine "GpuWorker started"
                        let platformName = "*"
                        let deviceType = DeviceType.Default
                        let provider = ComputeProvider.Create(platformName, deviceType)
                        let kernel, kernelPrepare, kernelRun = provider.Compile task
        
                        while true do
                            let! msg = inbox.Receive()
        
                            match msg with
                            | MultiplyMatrix(matrices, reply) ->
                                ConsoleLockingHelper.WriteLine "Mult from GPU"
                                let localWorkSize = 10
                                let c = Array.zeroCreate(rows * columns)    
                                let d = new _2D(rows, columns, localWorkSize, localWorkSize)
                                let a, b = matrices
        
                                kernelPrepare d columns a b c
        
                                let mutable commandQueue = new Brahma.OpenCL.CommandQueue(provider, provider.Devices |> Seq.head)
        
                                let _ = commandQueue.Add(kernelRun()).Finish()
                                let _ = commandQueue.Add(c.ToHost(provider)).Finish
                
                                commandQueue.Dispose()
                                provider.CloseAllBuffers()
        
                                reply.Reply(c)
                            | _                               -> ()
                } )

        }

        override this.Post(msg : Messages) = this.worker.Post msg
        override this.PostAndAsyncReply(buildMessage : AsyncReplyChannel<'Reply> -> Messages) = 
            this.worker.PostAndAsyncReply buildMessage

    let printArray (arr:float32[]) =
        for i in arr do ConsoleLockingHelper.Write(i.ToString())
        ConsoleLockingHelper.WriteLine("")


    type AgentDataReader =
        inherit Agent

        val dataReader : Agent<Messages>

        new (readingFunc:int -> int -> float32[] * float32[], dataGetter:Agent) as this = {
            dataReader = Agent.Start(fun inbox ->
                async { 
                    ConsoleLockingHelper.WriteLine "DataReader started"
                    while true do
                    let! msg = inbox.Receive()
                        
                    match msg with
                    | DataNeeded reply -> 
                        ConsoleLockingHelper.WriteLine "Data needed from reader"
                        Threading.Thread.Sleep(5000)
                        ConsoleLockingHelper.WriteLine "Sleep ended"
                        let res = readingFunc rows columns
                        dataGetter.Post(Messages.Data(res, this))
                    | _          -> ()
                    } )
        }

        override this.Post(msg : Messages) = this.dataReader.Post msg
        override this.PostAndAsyncReply(buildMessage : AsyncReplyChannel<'Reply> -> Messages) = 
            this.dataReader.PostAndAsyncReply buildMessage

    type AgentManager = 
        inherit Agent

        val manager: Agent<Messages>

        new (workers: List<Agent>, dataReaders) = {
            manager = Agent.Start(fun inbox ->
                async { 
                    ConsoleLockingHelper.WriteLine "Manager started"
                    let initReader (reader:AgentDataReader) = async { return reader.PostAndAsyncReply(DataNeeded) }
                    let freeReaders = new List<Agent>()
                    let freeWorkers = new List<Agent>(workers)
                    let results = new List<float32[]>()

                    while true do
                        let! msg = inbox.Receive()
                        match msg with
                        | Start                ->
                            ConsoleLockingHelper.WriteLine "Initialization started"
                            let! init = async {
                                dataReaders |> Seq.map initReader |> Async.Parallel |> Async.RunSynchronously |> ignore }
                            ConsoleLockingHelper.WriteLine "Initialization completed"
                                    
                        | Data (matrices, freeReader) ->
                            freeReaders.Add freeReader
                            ConsoleLockingHelper.WriteLine "Data from manager"
                            let! res = freeWorkers.First().PostAndAsyncReply(fun reply -> Messages.MultiplyMatrix(matrices, reply))
                            printArray res
                            results.Add res
                           
                        | DataNeeded(reply)    -> //async {
                            ConsoleLockingHelper.WriteLine "Data Needed from manager"
                            let! matrices = freeReaders.First().PostAndAsyncReply(DataNeeded)
                            let! res = freeWorkers.First().PostAndAsyncReply(fun reply -> Messages.MultiplyMatrix(matrices, reply))
                            printArray res
                            results.Add res //}
                        | _               -> ()
                        } )
        }

        override this.Post(msg : Messages) = this.manager.Post msg
        override this.PostAndAsyncReply(buildMessage : AsyncReplyChannel<'Reply> -> Messages) = 
            this.manager.PostAndAsyncReply buildMessage