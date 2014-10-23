namespace BrahmaAgents

module AgentsDataReader =

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

//    type AgentDataReader<'ReadingParameter, 'Data> =
//        inherit Agent<MessageDataReader<'ReadingParameter, 'Data>, 'ReadingParameter, 'Data>
//        
//        val dataReader: MailboxProcessor<MessageDataReader<'ReadingParameter, 'Data>>
//        
//        new (
//            agentId, 
//            logger, 
//            readFunction, 
//            parameters: 'ReadingParameter, 
//            dataSource: IDataSource<'Data>,
//            dataReceiver: Agent<MessageManager<_, _>, _, _>) as this = {
//            
//            inherit Agent<MessageDataReader<'ReadingParameter, 'Data>, 'ReadingParameter, 'Data>(agentId, logger)
//            
//            dataReader = MailboxProcessor.Start(fun inbox ->
//                async {
//                    logger.LogMessage(sprintf "%s initialized at %s time" agentId currentDateTime)
//                    
//                    while true do
//                        let! msg = inbox.Receive()
//
//                        match msg with
//                        | DataNeeded(param) ->
//                            logger.LogMessage(sprintf "%s recieved DataNeeded message at %s time" agentId currentDateTime)
//                            
//                            dataReceiver.Post(MessageManager<'ReadingParameter, 'Data>.DataFromReader((readFunction parameters dataSource), this))
//
//                        | _ -> 
//                            raise (NotSupportedMessageException(sprintf "NotSupportedMessageException from %s" agentId))
//
//                }
//            )
//        }
//
//        override this.AgentType with get() = AgentType.DataReader
//
//        override this.Post(msg) = this.dataReader.Post msg
//        override this.PostAndAsyncReply(buildMessage: AsyncReplyChannel<'Reply> -> MessageDataReader<'ReadingParameter, 'Data>) = 
//            this.dataReader.PostAndAsyncReply buildMessage
//
//        override this.Dispose() = (this.dataReader:> IDisposable).Dispose()