namespace BrahmaAgents

module AgentCpuWorker =

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

//    type AgentCpuWorker<'CpuTaskParameter, 'CpuTaskResult> =
//        inherit Agent<MessageCpuWorker<'CpuTaskParameter, 'CpuTaskResult>>
//
//        val worker: MailboxProcessor<MessageCpuWorker<'CpuTaskParameter, 'CpuTaskResult>>
//    
//        new (
//            agentId, 
//            logger, 
//            task: 'CpuTaskParameter -> 'CpuTaskResult, 
//            resultReceiver: Agent<_>) as this = {
//            
//            inherit Agent<MessageCpuWorker<'CpuTaskParameter, 'CpuTaskResult>>(agentId, logger)
//           
//            worker = MailboxProcessor.Start(fun inbox ->
//                async {
//                    logger.LogMessage(sprintf "%s initialized at %s time" agentId currentDateTime)
//                    
//                    while true do
//                        let! msg = inbox.Receive()
//                        
//                        match msg with
//                        | DoCpuTask(parameters) -> 
//                            logger.LogMessage(sprintf "%s recieved DoTask message at %s time" agentId currentDateTime)
//                            resultReceiver.Post(CpuTaskDone(task parameters))
//                            logger.LogMessage(sprintf "%s answered at %s time" agentId currentDateTime)
//                            
//                        | _ -> 
//                            raise (NotSupportedMessageException(sprintf "NotSupportedMessageException from %s" agentId))
//                }
//            )
//        }
//
//        override this.AgentType with get() = AgentType.CpuWorker
//
//        override this.Post(msg) = this.worker.Post msg
//        override this.PostAndAsyncReply(buildMessage: AsyncReplyChannel<'Reply> -> MessageCpuWorker<'CpuTaskParameter, 'CpuTaskResult>) = 
//            this.worker.PostAndAsyncReply buildMessage
//
//        override this.Dispose() = (this.worker:> IDisposable).Dispose()