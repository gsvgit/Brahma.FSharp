module RawReading

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open RawIo

open System.IO
open System.Runtime.Serialization.Formatters.Binary

let maxTemplateLength = 32//uy

let kRef = ref 1024
let localWorkSizeRef = ref 512

let indexRef = ref 1
let templatesPathRef = ref ""

let debugMode = ref false

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

let launch k localWorkSize index templatesPath =
    let templatesReader = File.OpenRead(templatesPath)
    let formatter = new BinaryFormatter()
    let deserialized = formatter.Deserialize(templatesReader)

    templatesReader.Close()

    let templatesRef = ref 0
    let templateLengthsRef = ref null
    let templatesSumRef = ref 0
    let templateArrRef = ref null

    let templates = !templatesRef
    let templateLengths = !templateLengthsRef
    let templatesSum = !templatesSumRef
    let templateArr = !templateArrRef

    let memory,ex = Cl.GetDeviceInfo(NaiveSearchGpu.provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
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

    let buffer = Array.zeroCreate length

    let readingTimer = new Timer<string>()
    let countingTimer = new Timer<string>()

    let offset = 0L
    let bound = 280L*1024L*1024L*1024L

    let testAlgorithmAsync initializer uploader downloader label counter close =
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

    let gpuInitilizer = (fun _ _ -> NaiveSearchGpu.initialize length k localWorkSize templates templateLengths buffer templateArr)
    let gpuHashingInitilizer = (fun _ _ -> NaiveHashingSearchGpu.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashingPrivateInitilizer = (fun _ _ -> NaiveHashingSearchGpuPrivate.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashingPrivateLocalInitilizer = (fun _ _ -> NaiveHashingGpuPrivateLocal.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashtableInitializer = (fun _ _ -> HashtableGpuPrivateLocal.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuExpandedHashtableInitializer = (fun _ _ -> HashtableExpanded.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuAhoCorasickInitializer = (fun next leaf -> AhoCorasickGpu.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr next leaf)
    let gpuAhoCorasickOptimizedInitializer = (fun next leaf -> AhoCorasickOptimized.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr next leaf)

   
    let gpuUploader = NaiveSearchGpu.upload
    let gpuHashingUploader = (fun () -> NaiveHashingSearchGpu.upload())
    let gpuHashingPrivateUploader = (fun () -> NaiveHashingSearchGpuPrivate.upload())
    let gpuHashtableUploader = (fun () -> HashtableGpuPrivateLocal.upload())
    let gpuExpandedHashtableUploader = (fun () -> HashtableExpanded.upload())
    let gpuAhoCorasickUploader = (fun () -> AhoCorasickGpu.upload())
    let gpuAhoCorasickOptimizedUploader = (fun () -> AhoCorasickOptimized.upload())

    let gpuDownloader = (fun t -> NaiveSearchGpu.download t)
    let gpuHashingDownloader = (fun t -> NaiveHashingSearchGpu.download t)
    let gpuHashingPrivateDownloader = (fun t -> NaiveHashingSearchGpuPrivate.download t)
    let gpuHashingPrivateLocalDownloader = (fun t -> NaiveHashingGpuPrivateLocal.download t)
    let gpuHashtableDownloader = (fun t -> HashtableGpuPrivateLocal.download t)
    let gpuExpandedHashtableDownloader = (fun t -> HashtableExpanded.download t)
    let gpuAhoCorasickDownloader = (fun t -> AhoCorasickGpu.download t)
    let gpuAhoCorasickOptimizedDownloader = (fun t -> AhoCorasickOptimized.download t)

    let cpuMatches = ref 0  
    let cpuMatchesHashed = ref 0
    let cpuMatchesAhoCorasick = ref 0
    let gpuMatches = ref 0
    let gpuMatchesHashing = ref 0
    let gpuMatchesLocal = ref 0
    let gpuMatchesHashingPrivate = ref 0
    let gpuMatchesHashingPrivateLocal = ref 0
    let gpuMatchesHashtable = ref 0
    let gpuMatchesHashtableExpanded = ref 0
    let gpuAhoCorasick = ref 0
    let gpuAhoCorasickOptimized = ref 0

    //testAlgorithm cpuInitilizer cpuGetter NaiveSearch.label cpuMatches
    //testAlgorithm cpuHashedInitilizer cpuHashedGetter NaiveHashingSearch.label cpuMatchesHashed
//    testAlgorithmAsync gpuInitilizer gpuUploader gpuDownloader NaiveSearchGpu.label gpuMatches
//        NaiveSearchGpu.close
//    testAlgorithmAsync gpuHashingInitilizer gpuHashingUploader gpuHashingDownloader NaiveHashingSearchGpu.label gpuMatchesHashing
//        NaiveHashingSearchGpu.close
//    testAlgorithmAsync gpuHashingPrivateInitilizer gpuHashingPrivateUploader gpuHashingPrivateDownloader NaiveHashingSearchGpuPrivate.label gpuMatchesHashingPrivate
//        NaiveHashingSearchGpuPrivate.close
//    testAlgorithmAsync gpuHashingPrivateLocalInitilizer gpuHashingPrivateLocalUploader gpuHashingPrivateLocalDownloader NaiveHashingGpuPrivateLocal.label gpuMatchesHashingPrivateLocal
//        NaiveHashingGpuPrivateLocal.close
    testAlgorithmAsync gpuHashtableInitializer gpuHashtableUploader gpuHashtableDownloader HashtableGpuPrivateLocal.label gpuMatchesHashtable
        HashtableGpuPrivateLocal.close
//    testAlgorithmAsync 
//        gpuExpandedHashtableInitializer gpuExpandedHashtableUploader gpuExpandedHashtableDownloader HashtableExpanded.label gpuMatchesHashtableExpanded
//        HashtableExpanded.close
//    testAlgorithmAsync gpuAhoCorasickInitializer gpuAhoCorasickUploader gpuAhoCorasickDownloader AhoCorasickGpu.label gpuAhoCorasick
//        AhoCorasickGpu.close
//    testAlgorithmAsync gpuAhoCorasickOptimizedInitializer gpuAhoCorasickOptimizedUploader gpuAhoCorasickOptimizedDownloader AhoCorasickOptimized.label gpuAhoCorasickOptimized
//        AhoCorasickOptimized.close
    //testAlgorithm cpuAhoCorasickInitializer cpuAhoCorasickGetter AhoCorasickCpu.label cpuMatchesAhoCorasick

    Helpers.verifyResults !cpuMatches !gpuMatches NaiveSearchGpu.label
    Helpers.verifyResults !cpuMatches !gpuMatchesHashing NaiveHashingSearchGpu.label
    Helpers.verifyResults !cpuMatches !gpuMatchesHashingPrivate NaiveHashingSearchGpuPrivate.label
    Helpers.verifyResults !cpuMatches !gpuMatchesHashingPrivateLocal NaiveHashingGpuPrivateLocal.label
    Helpers.verifyResults !cpuMatches !gpuMatchesHashtable HashtableGpuPrivateLocal.label
    Helpers.verifyResults !cpuMatches !gpuMatchesHashtableExpanded HashtableExpanded.label
    Helpers.verifyResults !cpuMatches !gpuAhoCorasick AhoCorasickGpu.label
    Helpers.verifyResults !cpuMatches !gpuAhoCorasickOptimized AhoCorasickOptimized.label

    printfn ""

    printfn "Raw computation time spent:"
    
    Helpers.printGlobalTime NaiveSearchGpu.label
    Helpers.printGlobalTime NaiveHashingSearchGpu.label
    Helpers.printGlobalTime NaiveHashingSearchGpuPrivate.label
    Helpers.printGlobalTime NaiveHashingGpuPrivateLocal.label
    Helpers.printGlobalTime HashtableGpuPrivateLocal.label
    Helpers.printGlobalTime HashtableExpanded.label
    Helpers.printGlobalTime AhoCorasickGpu.label
    Helpers.printGlobalTime AhoCorasickOptimized.label
    
    printfn ""

    printfn "Computation time with preparations:"
    Helpers.printTime NaiveSearchGpu.timer NaiveSearchGpu.label
    Helpers.printTime NaiveHashingSearchGpu.timer NaiveHashingSearchGpu.label
    Helpers.printTime NaiveHashingSearchGpuPrivate.timer NaiveHashingSearchGpuPrivate.label
    Helpers.printTime NaiveHashingGpuPrivateLocal.timer NaiveHashingGpuPrivateLocal.label
    Helpers.printTime HashtableGpuPrivateLocal.timer HashtableGpuPrivateLocal.label
    Helpers.printTime HashtableExpanded.timer HashtableExpanded.label
    Helpers.printTime AhoCorasickGpu.timer AhoCorasickGpu.label
    Helpers.printTime AhoCorasickOptimized.timer AhoCorasickOptimized.label

    printfn ""

    printfn "Total time with reading:"
    Helpers.printTime readingTimer NaiveSearchGpu.label
    Helpers.printTime readingTimer NaiveHashingSearchGpu.label
    Helpers.printTime readingTimer NaiveHashingSearchGpuPrivate.label
    Helpers.printTime readingTimer NaiveHashingGpuPrivateLocal.label
    Helpers.printTime readingTimer HashtableGpuPrivateLocal.label
    Helpers.printTime readingTimer HashtableExpanded.label
    Helpers.printTime readingTimer AhoCorasickGpu.label
    Helpers.printTime readingTimer AhoCorasickOptimized.label

    printfn ""

    printfn "Counting time:"
    Helpers.printTime countingTimer NaiveSearchGpu.label
    Helpers.printTime countingTimer NaiveHashingSearchGpu.label
    Helpers.printTime countingTimer NaiveHashingSearchGpuPrivate.label
    Helpers.printTime countingTimer NaiveHashingGpuPrivateLocal.label
    Helpers.printTime countingTimer HashtableGpuPrivateLocal.label
    Helpers.printTime countingTimer HashtableExpanded.label
    Helpers.printTime countingTimer AhoCorasickGpu.label
    Helpers.printTime countingTimer AhoCorasickOptimized.label

    Timer<string>.Global.Reset()
    readingTimer.Reset()
    countingTimer.Reset()

let Main () =
    let commandLineSpecs =
        [
         "-k", ArgType.Int (fun i -> kRef := i), "Work amount for one work item."
         "-l", ArgType.Int (fun i -> localWorkSizeRef := i), "Work group size."
         "-index", ArgType.Int (fun i -> indexRef := i), "Input file index."
         "-templates", ArgType.String (fun s -> templatesPathRef := s), "Templates file path."
         "-debug", ArgType.String (fun s -> if s.Equals "true" then debugMode := true), "Debug mode."
         ] |> List.map (fun (shortcut, argtype, description) -> ArgInfo(shortcut, argtype, description))
    ArgParser.Parse commandLineSpecs

    let k = !kRef  
    let localWorkSize = !localWorkSizeRef

    let index = !indexRef
    let templatesPath = !templatesPathRef

    launch k localWorkSize index templatesPath

    ignore (System.Console.Read())

do Main()
