namespace BrahmaAgents

module AgentManafer =

    open System
    open System.Collections.Generic
    open System.Linq
    open Microsoft.FSharp.Quotations

    open Utils
    open Brahma.OpenCL
    open Brahma.FSharp.OpenCL.Core
    open Brahma.FSharp.OpenCL.Translator.Common
    open OpenCL.Net
    open AgentsBase
    open AgentCpuWorker
    open AgentGpuWorker
    open AgentsDataReader

//    type AgentManager<'CpuTaskParameter, 'CpuTaskResult,
//                        'GpuTaskParameter, 'GpuTaskResult,
//                        'ReadingParameter, 'Data,
//                        'ManagerParams, 'OverallResult
//                            when 'GpuTaskParameter: (new: unit -> 'GpuTaskParameter) and
//                                'GpuTaskParameter: struct and
//                                'GpuTaskParameter:> ValueType and
//                                'GpuTaskParameter:> INDRangeDimension> =
//        
//        inherit Agent<MessageManager<'CpuTaskParameter, 'CpuTaskResult,
//                                     'GpuTaskParameter, 'GpuTaskResult,
//                                     'ReadingParameter, 'Data,
//                                     'ManagerParams, 'OverallResult>>
//
//        val manager: MailboxProcessor<MessageManager<'CpuTaskParameter, 'CpuTaskResult,
//                                                     'GpuTaskParameter, 'GpuTaskResult,
//                                                     'ReadingParameter, 'Data,
//                                                     'ManagerParams, 'OverallResult>>
//        
//        val mutable cpuWorkersCount: int
//        val mutable gpuWorkersCount: int
//        val mutable dataReadersCount: int
//
//        new (
//            agentId, 
//            logger, 
//            overallConfigs: AgentsOverallConfiguration, 
//            cpuTask, 
//            gpuTask, 
//            prePreparation, 
//            collectResults, 
//            gpuConfiguration, 
//            readFunction, 
//            parameters, 
//            dataSource: IDataSource<'Data>) as this = {
//            
//            inherit Agent<MessageManager<'CpuTaskParameter, 'CpuTaskResult,'GpuTaskParameter, 'GpuTaskResult,'ReadingParameter, 'Data,'ManagerParams, 'OverallResult>>(agentId, logger)
//
//            cpuWorkersCount = 0
//            gpuWorkersCount = 0
//            dataReadersCount = 0
//            manager = MailboxProcessor.Start(fun inbox ->
//                async {
//                    logger.LogMessage(sprintf "%s initialized at %s time" agentId currentDateTime)
//
//                    let freeCpuWorkers = this.CreateCpuWorkers(overallConfigs.AgentCpuWorkersCount, cpuTask)
//                    let freeGpuWorkers = 
//                        this.CreateGpuWorkers(
//                            overallConfigs.AgentGpuWorkersCount, 
//                            gpuTask, 
//                            prePreparation, 
//                            collectResults,
//                            gpuConfiguration)
//                    let freeDataReaders = 
//                        this.CreateDataReaders(
//                            overallConfigs.AgentDataReadersCount, 
//                            readFunction, 
//                            parameters, 
//                            dataSource)
//
//                    let startReader (reader:AgentDataReader<'ReadingParameter, 'Data>) = async { return reader.PostAndAsyncReply(DataNeeded) }
//       
//                    while true do
//                        let! msg = inbox.Receive()
//
//                        match msg with
//                        | Start -> 
//                            let! startReaders = async { freeDataReaders |> Seq.map startReader |> Async.Parallel |> Async.RunSynchronously |> ignore }
//                            ()
//
//                        | Data(param, agent) -> ()
//
//                        | _ -> ()
//                }
//            )
//        }
//
//        override this.AgentType with get() = AgentType.Manager
//
//        override this.Post(msg) = this.manager.Post msg
//        override this.PostAndAsyncReply(buildMessage: AsyncReplyChannel<'Reply> -> MessageManager<'CpuTaskParameter, 'CpuTaskResult,'GpuTaskParameter, 'GpuTaskResult, 'ReadingParameter, 'Data, 'ManagerParams, 'OverallResult>) = 
//            this.manager.PostAndAsyncReply buildMessage
//
//        override this.Dispose() = (this.manager:> IDisposable).Dispose()
//
//        member private this.CreateCpuWorkers(agentsCount, task) =
//            let workers = new List<AgentCpuWorker<'CpuTaskParameter, 'CpuTaskResult>>()
//            
//            let mutable i = 0
//            while i < agentsCount do
//                workers.Add(new AgentCpuWorker<'CpuTaskParameter, 'CpuTaskResult>(this.CreateAgentId(CpuWorker), this.Logger, task, this))
//                this.cpuWorkersCount <- this.cpuWorkersCount + 1
//                i <- i + 1
//
//            workers
//
//        member private this.CreateGpuWorkers(agentsCount, task, prePreparation, collectResults, configs) =
//            let workers = new List<AgentGpuWorker<'GpuTaskParameter, 'GpuTaskResult>>()
//            
//            let mutable i = 0
//            while i < agentsCount do
//                workers.Add(new AgentGpuWorker<'GpuTaskParameter, 'GpuTaskResult>(this.CreateAgentId(GpuWorker), this.Logger, task, prePreparation, collectResults, configs, this:> Agent<unit, 'OverallResults>))
//                this.gpuWorkersCount <- this.gpuWorkersCount + 1
//                i <- i + 1
//
//            workers
//
//        member private this.CreateDataReaders(agentsCount, readFunction, parameters, dataSource) =
//            let dataReaders = new List<AgentDataReader<'ReadingParameter, 'Data>>()
//            
//            let mutable i = 0
//            while i < agentsCount do
//                dataReaders.Add(new AgentDataReader<'ReadingParameter, 'Data>(this.CreateAgentId(DataReader), this.Logger, readFunction, parameters, dataSource, this))
//                this.dataReadersCount <- this.dataReadersCount + 1
//                i <- i + 1
//
//            dataReaders
//
//        member private this.CreateAgentId(agentType: AgentType) =
//            match agentType with
//            | CpuWorker -> sprintf "%d%s" this.cpuWorkersCount (AgentType.CpuWorker.ToString())
//            | GpuWorker -> sprintf "%d%s" this.gpuWorkersCount (AgentType.GpuWorker.ToString())
//            | DataReader -> sprintf "%d%s" this.dataReadersCount (AgentType.DataReader.ToString())
//            | _ -> raise (NotSupportedMessageException(""))