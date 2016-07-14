namespace AsyncMatrixMultiply

//module Program =
//    
//    open System.Collections.Generic
//    open System.Text.RegularExpressions
//    
//    open Brahma.Helpers
//    open OpenCL.Net
//    open Brahma.OpenCL
//    open Brahma.FSharp.OpenCL.Core
//    open Microsoft.FSharp.Quotations
//    open Brahma.FSharp.OpenCL.Extensions
//    open Brahma.FSharp.OpenCL.Translator.Common
//
//
//    open DataSource
//    open MatrixMultiply
//    open BrahmaAgents
//    open AgentsBase
//    open Utils
//
//    type DummyDataSource() = 
//        let mutable matricesCount = 100
//
//        interface IDataSource<float32[] * float32[]> with  
//            member dataSource.IsEnd = 
//                matricesCount <= 0
//                    
//            member dataSource.GetData() =
//                if matricesCount > 0 
//                    then
//                        matricesCount <- matricesCount - 1
//                        let res = MakeMatrix rows columns
//                        res
//                    else
//                        null, null
//
//
//    type AdditionalArgs = 
//        struct 
//            val mutable columns: int
//            val mutable a: array<float32>
//            val mutable b: array<float32>
//            val mutable c: array<float32>
//         end
//
//    [<EntryPoint>]
//    let Main args =
//        let rows = 200
//        let columns = 200
//        let gpuMatrixMult = 
//            <@
//                fun (r:_2D) (a: array<float32>) (b: array<float32>) (c: array<float32>) -> 
//                    let tx = r.GlobalID0
//                    let ty = r.GlobalID1
//                    for k in 0 .. columns - 1 do
//                        c.[ty * columns + tx] <- c.[ty * columns + tx] + (a.[ty * columns + k] * b.[k * columns + tx])
//            @>
//        let overallConfig: AgentsOverallConfiguration = 
//            {
//                AgentCpuWorkersCount = 3
//                AgentGpuWorkersCount = 3
//                AgentDataReadersCount = 5
//            } 
//        let gpuConfig: GpuConfiguration = 
//            {
//                DeviceType = DeviceType.Default
//                PlatformName = "*"
////                CompileOptions = ComputeProvider.DefaultOptions_p
//            }
//        let prePreparation _ _ = ()
//        let collectResults (queue: CommandQueue) parameters (addParam: AdditionalArgs) provider = 
//            queue.Add(addParam.c.ToHost(provider)).Finish() |> ignore
//            addParam.c
//        
//        let argsPreparation (kernelPrepare, parameters, (additionalParams: AdditionalArgs)) = 
//            kernelPrepare parameters additionalParams.a additionalParams.b additionalParams.c
//
//        let dataSource = new DummyDataSource()
//
//        let readFunction _ (dataSource: IDataSource<float32[] * float32[]>) = dataSource.GetData()
//        let readingParam = new obj()
//        let dataToCpuTaskParam data = data
//        let dataToGpuTaskParam data = 
//            let localWorkSize = 10
//            let mutable c = Array.zeroCreate(rows * columns)    
//            let d = new _2D(rows, columns, localWorkSize, localWorkSize)
//            let a, b = data
//            let mutable addArg = new AdditionalArgs()
//            addArg.a <- a;
//            addArg.b <- b;
//            addArg.c <- c;
//
//            d, addArg
//
//        let managerParam = new obj()
//
//        let manager = new AgentManager<float32[] * float32[], float32[], _2D, AdditionalArgs, float32[], obj, float32[] * float32[], obj, obj>(
//                        "AgentManager",
//                        new FileLogger("C:\\Agent.txt"),
//                        new ConsoleProgressPresenter(),
//                        overallConfig,
//                        MultyplyMatrixByCpuAgents,
//                        gpuMatrixMult,
//                        prePreparation,
//                        collectResults,
//                        argsPreparation,
//                        gpuConfig,
//                        readFunction,
//                        readingParam,
//                        dataToCpuTaskParam,
//                        dataToGpuTaskParam,
//                        managerParam,
//                        dataSource)
//
//        manager.Post(
//            MessageManager<
//                float32[] * float32[], 
//                float32[], 
//                _2D, 
//                (AdditionalArgs), 
//                float32[], 
//                obj, 
//                float32[] * float32[], 
//                obj, obj>.Start())
//
//        while true do ()
//        0