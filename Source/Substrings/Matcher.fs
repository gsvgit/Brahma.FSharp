module Matcher

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks

[<Struct>]
type GPUConfig =    
    val t:int

type SearchAlgorithm =
    | Nive
    | AhoCorasick
    | Hashtable
    | RabinKarp

type Matcher(provider, config) =

    let platformName = "*"
    let deviceType = Cl.DeviceType.Default    

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head) 

    let timer = new Timer<string>()

    let maxTemplateLength = 32

    let kRef = ref 1024
    let localWorkSizeRef = ref 512

    let indexRef = ref 1
    let templatesPathRef = ref ""

    let debugMode = ref false
    let mutable buffersCreated = false
    let mutable result = null
    let mutable ready = true

    let memory,ex = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
    let maxGpuMemory = memory.CastTo<uint64>()

    let maxHostMemory = 256UL * 1024UL * 1024UL

    let additionalArgs = 2UL * (256UL + 2UL) * (uint64) maxTemplateLength * (uint64) templates + (uint64) templates +
                         (uint64) templatesSum + 13UL + 1000UL

    let additionalTempData = 2UL * (256UL + 256UL + 3UL) * (uint64) maxTemplateLength * (uint64) templates + 
                             (uint64) maxTemplateLength * (uint64) templates + 100000UL

    let availableMemory = (int) (min (maxGpuMemory - additionalArgs) (maxHostMemory - additionalArgs - additionalTempData))

    let groupSize = k * localWorkSize * (1 + 2)

    let groups = availableMemory / groupSize

    let length = k * localWorkSize * groups

    printfn "Maximum memory on GPU is %A, additional args size is %A, temp data size is %A" maxGpuMemory additionalArgs additionalTempData

    printfn "Running %A groups with %A items in each, %A in total." groups localWorkSize (localWorkSize * groups)
    printfn "Each item will process %A bytes of input, %A total on each iteration." k length
    printfn ""
    
    let initialize length k localWorkSize templates (templateLengths:array<byte>) (gpuArr:array<byte>) (templateArr:array<byte>) =
        timer.Start()
        result <- Array.zeroCreate length
        let x, y, z = provider.Compile(query=command, translatorOptions=[BoolAsBit])
        kernel <- x
        kernelPrepare <- y
        kernelRun <- z
        input <- gpuArr
        let l = (length + (k-1))/k 
        let d =(new _1D(l,localWorkSize))
        kernelPrepare d length k templates templateLengths input templateArr result
        timer.Lap(label)
        ()

    let readingTimer = new Timer<string>()
    let countingTimer = new Timer<string>()

    let offset = 0L
    let bound = 280L*1024L*1024L*1024L

    let close () =     
        provider.CloseAllBuffers()
        commandQueue.Dispose()
        provider.Dispose()
        buffersCreated <- false

    let downloader label (result:array<_>) (task:Task<unit>) =
        if ready then failwith "Not running, can't download!"
        ready <- true

        task.Wait()

        ignore (commandQueue.Add(result.ToHost provider).Finish())
        buffersCreated <- true
        Timer<string>.Global.Lap(label)
        timer.Lap(label)

        result

    let uploader (input:array<_>) kernelRun =
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

    let run initializer label counter close =
        readingTimer.Start()
        let mutable read = 0
        let mutable highBound = 0

        let matches = Array.zeroCreate 256

        let mutable current = 0L

        let handle = Raw.CreateFile(index)

        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates (int maxTemplateLength) templateLengths templateArr

        initializer next leaf

        let mutable countingBound = 0
        let mutable matchBound = 0

        let mutable task = null

        let mutable index = 0

        while (current < bound) && ((current = 0L) || (read > 0)) do
            if current > 0L then
                current <- current - 512L

            highBound <- (if int64 length < bound - current then length else (int) (bound - current))
            read <- Raw.ReadFile(handle, buffer, highBound, offset + current)

            if read <= 0 && current = 0L then
                failwith "Failed to start reading!"

            if current > 0L then
                let result = downloader task
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

                task <- uploader()
        
        if (read > 0) then
            printfn "Last read is non-zero!"
            let result = downloader task
            countingTimer.Start()
            counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound)
            countingTimer.Lap(label)

        let hex = Array.map (fun (x : byte) -> System.String.Format("{0:X2} ", x)) templateArr
        let mutable start = 0
        for i in 0..(templates - 1) do
            let pattern = System.String.Concat(Array.sub hex start ((int) templateLengths.[i]))
            printfn "%A: %A matches found by %A" pattern matches.[i] label
            start <- start + (int) templateLengths.[i]

        printfn ""

        ignore(Raw.CloseHandle(handle))
        readingTimer.Lap(label)
        close()


    member this.Search (inputStream, templates, algo)  = 
        timer.Reset()
        1
