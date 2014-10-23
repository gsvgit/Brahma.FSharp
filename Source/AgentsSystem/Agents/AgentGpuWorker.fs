namespace BrahmaAgents

module AgentGpuWorker =

    open System
    open System.Collections.Generic
    open System.Linq
    open Microsoft.FSharp.Quotations

    open Utils
    open Brahma.OpenCL
    open Brahma.FSharp.OpenCL.Core;
    open Brahma.FSharp.OpenCL.Translator.Common
    open OpenCL.Net
    open AgentsBase

//    type AgentGpuWorker<'GpuTaskParameter, 'GpuTaskResult 
//        when 'GpuTaskParameter: (new: unit -> 'GpuTaskParameter) and 
//            'GpuTaskParameter: struct and 
//            'GpuTaskParameter:> ValueType and 
//            'GpuTaskParameter:> INDRangeDimension> =
//        
//        inherit Agent<MessageGpuWorker<'GpuTaskParameter, 'GpuTaskResult>>
//
//        val worker: MailboxProcessor<MessageGpuWorker<'GpuTaskParameter, 'GpuTaskResult>>
//        val provider: ComputeProvider
//        val commandQueue: Brahma.OpenCL.CommandQueue
//
//        new (
//            agentId, 
//            logger, 
//            task, 
//            prePreparation: ('GpuTaskParameter -> unit) -> 'GpuTaskParameter -> unit, 
//            collectResults, 
//            gpuConfiguration,
//            resultReceiver: Agent<_>) as this = {
//            
//            inherit Agent<MessageGpuWorker<'GpuTaskParameter, 'GpuTaskResult>>(agentId, logger)
//
//            provider = ComputeProvider.Create(gpuConfiguration.PlatformName, gpuConfiguration.DeviceType)
//            commandQueue = new Brahma.OpenCL.CommandQueue(this.provider, this.provider.Devices |> Seq.head)
//
//            worker = MailboxProcessor.Start(fun inbox ->
//                async {
//                    logger.LogMessage(sprintf "%s initialized at %s time" agentId currentDateTime)
//
//                    let kernel, kernelPrepare, kernelRun = 
//                        this.provider.Compile(task, gpuConfiguration.CompileOptions, gpuConfiguration.TrasnlatorOptions, gpuConfiguration.OutCode)
//            
//                    while true do
//                        let! msg = inbox.Receive()
//
//                        match msg with
//                        | DoGpuTask(parameters) ->
//                            prePreparation kernelPrepare parameters    
//                            this.commandQueue.Add(kernelRun()).Finish() |> ignore
//                            let results = collectResults this.commandQueue parameters this.provider
//                            //resultReceiver.Post(GpuTaskDone(results)
//                            ()
//                        | _ -> raise (NotSupportedMessageException(sprintf "NotSupportedMessageException from %s" agentId))
//                }
//            )
//        }
//
//        override this.AgentType with get() = AgentType.GpuWorker
//
//        override this.Post(msg) = this.worker.Post msg
//        override this.PostAndAsyncReply(buildMessage: AsyncReplyChannel<'Reply> -> MessageGpuWorker<'GpuTaskParameter, 'GpuTaskResult>) = 
//            this.worker.PostAndAsyncReply buildMessage
//
//        override this.Dispose() =
//            this.commandQueue.Dispose()
//            this.provider.CloseAllBuffers()
//            (this.worker:> IDisposable).Dispose()