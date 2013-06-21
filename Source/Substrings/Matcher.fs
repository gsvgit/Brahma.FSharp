module Brahman.Substrings.Matcher

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks

type Config =
    {
        additionalArgs: uint64
        additionalTempData: uint64
        localWorkSize: int
        chankSize: int
        groups: int
        groupSize: int
        bufLength: int
    }

type Templates = {
    number : int
    sizes : byte[]
    content : byte[]
    }

type Matcher(provider, config) =

    let mutable label = ""
    let platformName = "*"
    let deviceType = Cl.DeviceType.Default    

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head) 

    let timer = new Timer<string>()

    let maxTemplateLength = 32       

    let debugMode = ref false
    let mutable buffersCreated = false
    let mutable result = [||]
    let mutable input = [||]
    let mutable ready = true

    let memory,ex = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
    let maxGpuMemory = memory.CastTo<uint64>()

    let maxHostMemory = 256UL * 1024UL * 1024UL

    let configure (templates:array<_>) =
        let tLenghth = templates |> Array.length |> uint64
        let additionalArgs = 2UL * (256UL + 2UL) * (uint64) maxTemplateLength * tLenghth + tLenghth +
                             tLenghth + 13UL + 1000UL

        let additionalTempData = 2UL * (256UL + 256UL + 3UL) * (uint64) maxTemplateLength * tLenghth + 
                                 (uint64) maxTemplateLength * tLenghth + 100000UL

        let availableMemory = (int) (min (maxGpuMemory - additionalArgs) (maxHostMemory - additionalArgs - additionalTempData))
        let lws,ex = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxWorkGroupSize)
        let localWorkSize = int <| lws.CastTo<uint64>()
        let chankSize = 1024
        let groupSize = chankSize * localWorkSize * (1 + 2)
        let groups = availableMemory / groupSize
        let length = chankSize * localWorkSize * groups
        {
            additionalArgs = additionalArgs
            additionalTempData = additionalTempData
            localWorkSize = localWorkSize
            chankSize = chankSize
            groups = groups
            groupSize = groupSize
            bufLength = length
        }

    let printConfiguration config =
        printfn
            "Maximum memory on GPU is %A, additional args size is %A, temp data size is %A"
            maxGpuMemory config.additionalArgs config.additionalTempData

        printfn 
            "Running %A groups with %A items in each, %A in total."
            config.groups config.localWorkSize (config.localWorkSize * config.groups)
        printfn "Each item will process %A bytes of input, %A total on each iteration." config.chankSize config.bufLength
        printfn ""
    
    let initialize config templates command =
        timer.Reset()
        timer.Start()
        printConfiguration config 
        result <- Array.zeroCreate config.bufLength
        input <- Array.zeroCreate config.bufLength
        let kernel, kernelPrepare, kernelRun = provider.Compile(query=command, translatorOptions=[BoolAsBit])                
        let l = (config.bufLength + (config.chankSize-1))/config.chankSize 
        let d =(new _1D(l,config.localWorkSize))        
        timer.Lap(label)
        kernel, (kernelPrepare d), kernelRun

    let readingTimer = new Timer<string>()
    let countingTimer = new Timer<string>()

    let offset = 0L
    let bound = 280L*1024L*1024L*1024L

    let close () =     
        provider.CloseAllBuffers()
        //commandQueue.Dispose()
        //provider.Dispose()
        buffersCreated <- false

    let downloader label (task:Task<unit>) =
        if ready then failwith "Not running, can't download!"
        ready <- true

        task.Wait()

        ignore (commandQueue.Add(result.ToHost provider).Finish())
        buffersCreated <- true
        Timer<string>.Global.Lap(label)
        timer.Lap(label)

        result

    let uploader (kernelRun:_ -> Commands.Run<_>) =
        if not ready then failwith "Already running, can't upload!"
        ready <- false

        timer.Start()
        Timer<string>.Global.Start()
        if buffersCreated || (provider.AutoconfiguredBuffers <> null && provider.AutoconfiguredBuffers.ContainsKey(input)) then
            ignore (commandQueue.Add(input.ToGpu provider).Finish())
            async { ignore (commandQueue.Add(kernelRun()).Finish())}
            |> Async.StartAsTask
        else
            ignore (commandQueue.Add(kernelRun()).Finish())
            async {()} |> Async.StartAsTask

    let countMatchesDetailed (result:array<int16>) maxTemplateLength bound length (templateLengths:array<byte>) (prefix:array<int16>) (matchesArray:array<uint64>) offset =
        let mutable matches = 0
        let clearBound = min (bound - 1) (length - (int) maxTemplateLength)

        if (!debugMode) then
            printfn "%A - %A | %A" offset (offset + (int64) bound - 1L) (offset + (int64) length - 1L)
            printfn ""

        for i in 0..clearBound do
            let mutable matchIndex = result.[i]
            if matchIndex >= 0s then
                if (!debugMode) then
                    printfn "%A : %A" (offset + (int64) i) matchIndex

                matchesArray.[(int) matchIndex] <- matchesArray.[(int) matchIndex] + 1UL
                matches <- matches + 1

        for i in (clearBound + 1)..(bound - 1) do
            let mutable matchIndex = result.[i]
            while matchIndex >= 0s && i + (int) templateLengths.[(int) matchIndex] > length do
                matchIndex <- prefix.[(int) matchIndex]
            
            if matchIndex >= 0s then
                if (!debugMode) then
                    printfn "%A : %A" (offset + (int64) i) matchIndex

                matchesArray.[(int) matchIndex] <- matchesArray.[(int) matchIndex] + 1UL
                matches <- matches + 1

        if (!debugMode) then
            printfn ""

        matches

    let run command hdIndex templates config prefix label close =
        let counter = ref 0
        readingTimer.Start()
        let mutable read = 0
        let mutable highBound = 0

        let matches = Array.zeroCreate 256

        let length = config.bufLength
        let templateLengths = templates.sizes
        let templateArr = templates.content

        let mutable current = 0L

        let handle = RawIO.CreateFileW(hdIndex)        

        let mutable countingBound = 0
        let mutable matchBound = 0

        let mutable task = Unchecked.defaultof<Task<unit>>

        let mutable index = 0

        while (current < bound) && ((current = 0L) || (read > 0)) do
            if current > 0L then
                current <- current - 512L

            highBound <- (if int64 length < bound - current then length else (int) (bound - current))
            read <- RawIO.ReadFileW(handle, input, highBound, offset + current)

            if read <= 0 && current = 0L then
                failwith "Failed to start reading!"

            if current > 0L then
                let result = downloader label task
                countingTimer.Start()
                counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound + 512L)
                countingTimer.Lap(label)

            if (read > 0) then
                index <- index + 1
                current <- current + (int64) read

                if index = 50 then
                    printfn "I am %A and I've already read %A bytes!" label current
                    index <- 0

                countingBound <- read
                matchBound <- read
                if current < bound then
                    countingBound <- countingBound - 512

                task <- uploader command
        
        if (read > 0) then
            printfn "Last read is non-zero!"
            let result = downloader label task
            countingTimer.Start()
            counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound)
            countingTimer.Lap(label)

        let hex = Array.map (fun (x : byte) -> System.String.Format("{0:X2} ", x)) templateArr
        let mutable start = 0
        for i in 0..(templates.number - 1) do
            let pattern = System.String.Concat(Array.sub hex start ((int) templateLengths.[i]))
            printfn "%A: %A matches found by %A" pattern matches.[i] label
            start <- start + (int) templateLengths.[i]

        printfn ""

        printfn "Total found by %A: %A" label !counter

        ignore(RawIO.CloseHandle(handle))
        readingTimer.Lap(label)
        close()

    let prepareTemplates array = 
        let sorted = Array.sortBy (fun (a:byte[]) -> a.Length) array

        let lengths = Array.map (fun (a:byte[]) -> (byte) a.Length) sorted
        let templateBytes = Array.toSeq sorted |> Array.concat

        let readyTemplates = { number = sorted.Length; sizes = lengths; content = templateBytes;}
        readyTemplates

    let finalize () =
        close ()
        printfn "Computation time with preparations:"
        Helpers.printTime timer label

        printfn ""

        printfn "Total time with reading:"
        Helpers.printTime readingTimer label

        printfn ""

        printfn "Counting time:"
        Helpers.printTime countingTimer label

    member this.NaiveSearch (hdId, templateArr)  = 
        label <- NaiveSearch.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr NaiveSearch.command
        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content
        kernelPrepare config.bufLength config.chankSize templates.number templates.sizes input templates.content result
        run kernelRun hdId templates config prefix label close
        timer.Lap(label)
        finalize()

    member this.AhoCorasik (hdId, templateArr)  =        
        label <- AhoCorasick.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr AhoCorasick.command        
        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content
        let go, _, exit = AhoCorasick.buildStateMachine templates.number maxTemplateLength next leaf
        kernelPrepare config.bufLength config.chankSize templates.number templates.sizes  go exit leaf maxTemplateLength input templates.content result
        run kernelRun hdId templates config prefix label close
        timer.Lap(label)
        finalize()

    member this.Hashtable (hdId, templateArr)  = 
        label <- Hashtables.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr Hashtables.command        
        let prefix, _, _, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content        
        let starts = Hashtables.computeTemplateStarts templates.number templates.sizes
        let templateHashes = Helpers.computeTemplateHashes templates.number templates.content.Length templates.sizes templates.content
        let table, next = Hashtables.createHashTable templates.number templates.sizes templateHashes
        kernelPrepare 
            config.bufLength config.chankSize templates.number templates.sizes  templateHashes table next starts maxTemplateLength input templates.content result
        run kernelRun hdId templates config prefix label close
        timer.Lap(label)
        finalize()

    member this.RabinKarp (hdId, templateArr) = 
        label <- RabinKarp.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr RabinKarp.command        
        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content        
        let templateHashes = Helpers.computeTemplateHashes templates.number templates.content.Length templates.sizes templates.content
        kernelPrepare 
            config.bufLength config.chankSize templates.number templates.sizes  templateHashes maxTemplateLength input templates.content result
        run kernelRun hdId templates config prefix label close
        timer.Lap(label)
        finalize()