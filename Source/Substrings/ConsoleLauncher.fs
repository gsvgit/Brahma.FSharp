module ConsoleLauncher

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.IO
open System.Runtime.Serialization.Formatters.Binary

open TemplatesGenerator

let maxTemplateLength = 32uy

let kRef = ref 1024
let localWorkSizeRef = ref 512

let pathRef = ref InputGenerator.path
let templatesPathRef = ref TemplatesGenerator.path

let Main () =
    let commandLineSpecs =
        [
         "-k", ArgType.Int (fun i -> kRef := i), "Work amount for one work item."
         "-l", ArgType.Int (fun i -> localWorkSizeRef := i), "Work group size."
         "-input", ArgType.String (fun s -> pathRef := s), "Input file path."
         "-templates", ArgType.String (fun s -> templatesPathRef := s), "Templates file path."
         ] |> List.map (fun (shortcut, argtype, description) -> ArgInfo(shortcut, argtype, description))
    ArgParser.Parse commandLineSpecs

    let k = !kRef  
    let localWorkSize = !localWorkSizeRef

    let path = !pathRef
    let templatesPath = !templatesPathRef

    let templatesReader = File.OpenRead(templatesPath)
    let formatter = new BinaryFormatter()
    let deserialized = formatter.Deserialize(templatesReader)

    templatesReader.Close()

    let templatesRef = ref 0
    let templateLengthsRef = ref null
    let templatesSumRef = ref 0
    let templateArrRef = ref null

    match deserialized with
    | :? Templates as t -> 
        templatesRef := t.number
        templateLengthsRef := t.sizes
        templatesSumRef := t.content.Length
        templateArrRef := t.content
    | other -> failwith "Deserialized object is not a Templates struct!"

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

    let groups = 1//availableMemory / groupSize

    let length = k * localWorkSize * groups

    printfn "Maximum memory on GPU is %A, additional args size is %A, temp data size is %A" maxGpuMemory additionalArgs additionalTempData

    printfn "Running %A groups with %A items in each, %A in total." groups localWorkSize (localWorkSize * groups)
    printfn "Each item will process %A bytes of input, %A total on each iteration." k length
    printfn ""

    let buffer = Array.zeroCreate length

    let readingTimer = new Timer<string>()

    let testAlgorithm initializer getter label counter =
        readingTimer.Start()
        let mutable read = 0
        let mutable lowBound = 0
        let mutable highBound = 0

        let mutable current = 0L

        let reader = new FileStream(path, FileMode.Open)
        let bound = reader.Length

        let prefix, next, leaf, _ = NaiveSearch.buildSyntaxTree templates maxTemplateLength templateLengths templateArr

        initializer next leaf

        while current < bound do
            if current > 0L then
                System.Array.Copy(buffer, (read + lowBound - (int) maxTemplateLength), buffer, 0, (int) maxTemplateLength)
                lowBound <- (int) maxTemplateLength

            highBound <- (if (int64) (length - lowBound) < bound then (length - lowBound) else (int) bound)
            read <- reader.Read(buffer, lowBound, highBound)
            current <- current + (int64) read

            let mutable countingBound = read + lowBound
            let mutable matchBound = read + lowBound
            if current < bound then
                countingBound <- countingBound - (int) maxTemplateLength

            counter := !counter + NaiveSearch.countMatches (getter()) countingBound matchBound templateLengths prefix
    
        reader.Close()
        readingTimer.Lap(label)

    let testAlgorithmAsync initializer uploader downloader label counter close =
        readingTimer.Start()
        let mutable read = 0
        let mutable lowBound = 0
        let mutable highBound = 0

        let mutable current = 0L

        let reader = new FileStream(path, FileMode.Open)
        let bound = reader.Length

        let prefix, next, leaf, _ = NaiveSearch.buildSyntaxTree templates maxTemplateLength templateLengths templateArr

        initializer next leaf

        let mutable countingBound = 0
        let mutable matchBound = 0

        let mutable task = null

        while current < bound do
            if current > 0L then
                System.Array.Copy(buffer, (read + lowBound - (int) maxTemplateLength), buffer, 0, (int) maxTemplateLength)
                lowBound <- (int) maxTemplateLength

            highBound <- (if (int64) (length - lowBound) < bound then (length - lowBound) else (int) bound)
            read <- reader.Read(buffer, lowBound, highBound)

            if current > 0L then
                counter := !counter + NaiveSearch.countMatches (downloader task) countingBound matchBound templateLengths prefix

            current <- current + (int64) read

            countingBound <- read + lowBound
            matchBound <- read + lowBound
            if current < bound then
                countingBound <- countingBound - (int) maxTemplateLength

            task <- uploader()
        
        counter := !counter + NaiveSearch.countMatches (downloader task) countingBound matchBound templateLengths prefix

        reader.Close()
        readingTimer.Lap(label)
        close()

    let cpuInitilizer = (fun _ _ -> ())
    let cpuHashedInitilizer = (fun _ _ -> ())
    let gpuInitilizer = (fun _ _ -> NaiveSearchGpu.initialize length k localWorkSize templates templateLengths buffer templateArr)
    let gpuHashingInitilizer = (fun _ _ -> NaiveHashingSearchGpu.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashingPrivateInitilizer = (fun _ _ -> NaiveHashingSearchGpuPrivate.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashingPrivateLocalInitilizer = (fun _ _ -> NaiveHashingGpuPrivateLocal.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuHashtableInitializer = (fun _ _ -> HashtableGpuPrivateLocal.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuExpandedHashtableInitializer = (fun _ _ -> HashtableExpanded.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr)
    let gpuAhoCorasickInitializer = (fun next leaf -> AhoCorasickGpu.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr next leaf)
    let gpuAhoCorasickOptimizedInitializer = (fun next leaf -> AhoCorasickOptimized.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr next leaf)

    let cpuGetter = (fun () -> NaiveSearch.findMatches length templates templateLengths buffer templateArr)
    let cpuHashedGetter = (fun () -> NaiveHashingSearch.findMatches length maxTemplateLength templates templatesSum templateLengths buffer templateArr)
//    let gpuGetter = (fun () -> NaiveSearchGpu.getMatches())
//    let gpuHashingGetter = (fun () -> NaiveHashingSearchGpu.getMatches())
//    let gpuHashingPrivateGetter = (fun () -> NaiveHashingSearchGpuPrivate.getMatches())
//    let gpuHashingPrivateLocalGetter = (fun () -> NaiveHashingGpuPrivateLocal.getMatches())

    let gpuUploader = (fun () -> NaiveSearchGpu.upload())
    let gpuHashingUploader = (fun () -> NaiveHashingSearchGpu.upload())
    let gpuHashingPrivateUploader = (fun () -> NaiveHashingSearchGpuPrivate.upload())
    let gpuHashingPrivateLocalUploader = (fun () -> NaiveHashingGpuPrivateLocal.upload())
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
    let gpuMatches = ref 0
    let gpuMatchesHashing = ref 0
    let gpuMatchesLocal = ref 0
    let gpuMatchesHashingPrivate = ref 0
    let gpuMatchesHashingPrivateLocal = ref 0
    let gpuMatchesHashtable = ref 0
    let gpuMatchesHashtableExpanded = ref 0
    let gpuAhoCorasick = ref 0
    let gpuAhoCorasickOptimized = ref 0

    testAlgorithm cpuInitilizer cpuGetter NaiveSearch.label cpuMatches
    testAlgorithm cpuHashedInitilizer cpuHashedGetter NaiveHashingSearch.label cpuMatchesHashed
    testAlgorithmAsync gpuInitilizer gpuUploader gpuDownloader NaiveSearchGpu.label gpuMatches
        NaiveSearchGpu.close
    testAlgorithmAsync gpuHashingInitilizer gpuHashingUploader gpuHashingDownloader NaiveHashingSearchGpu.label gpuMatchesHashing
        NaiveHashingSearchGpu.close
    testAlgorithmAsync gpuHashingPrivateInitilizer gpuHashingPrivateUploader gpuHashingPrivateDownloader NaiveHashingSearchGpuPrivate.label gpuMatchesHashingPrivate
        NaiveHashingSearchGpuPrivate.close
    testAlgorithmAsync gpuHashingPrivateLocalInitilizer gpuHashingPrivateLocalUploader gpuHashingPrivateLocalDownloader NaiveHashingGpuPrivateLocal.label gpuMatchesHashingPrivateLocal
        NaiveHashingGpuPrivateLocal.close
    testAlgorithmAsync gpuHashtableInitializer gpuHashtableUploader gpuHashtableDownloader HashtableGpuPrivateLocal.label gpuMatchesHashtable
        HashtableGpuPrivateLocal.close
    testAlgorithmAsync 
        gpuExpandedHashtableInitializer gpuExpandedHashtableUploader gpuExpandedHashtableDownloader HashtableExpanded.label gpuMatchesHashtableExpanded
        HashtableExpanded.close
    testAlgorithmAsync gpuAhoCorasickInitializer gpuAhoCorasickUploader gpuAhoCorasickDownloader AhoCorasickGpu.label gpuAhoCorasick
        AhoCorasickGpu.close
    testAlgorithmAsync gpuAhoCorasickOptimizedInitializer gpuAhoCorasickOptimizedUploader gpuAhoCorasickOptimizedDownloader AhoCorasickOptimized.label gpuAhoCorasickOptimized
        AhoCorasickOptimized.close

    Substrings.verifyResults !cpuMatches !cpuMatchesHashed NaiveHashingSearch.label
    Substrings.verifyResults !cpuMatches !gpuMatches NaiveSearchGpu.label
    Substrings.verifyResults !cpuMatches !gpuMatchesHashing NaiveHashingSearchGpu.label
    Substrings.verifyResults !cpuMatches !gpuMatchesHashingPrivate NaiveHashingSearchGpuPrivate.label
    Substrings.verifyResults !cpuMatches !gpuMatchesHashingPrivateLocal NaiveHashingGpuPrivateLocal.label
    Substrings.verifyResults !cpuMatches !gpuMatchesHashtable HashtableGpuPrivateLocal.label
    Substrings.verifyResults !cpuMatches !gpuMatchesHashtableExpanded HashtableExpanded.label
    Substrings.verifyResults !cpuMatches !gpuAhoCorasick AhoCorasickGpu.label
    Substrings.verifyResults !cpuMatches !gpuAhoCorasickOptimized AhoCorasickOptimized.label

    printfn ""

    printfn "Raw computation time spent:"
    
    FileReading.printGlobalTime NaiveSearch.label
    FileReading.printGlobalTime NaiveHashingSearch.label
    FileReading.printGlobalTime NaiveSearchGpu.label
    FileReading.printGlobalTime NaiveHashingSearchGpu.label
    FileReading.printGlobalTime NaiveHashingSearchGpuPrivate.label
    FileReading.printGlobalTime NaiveHashingGpuPrivateLocal.label
    FileReading.printGlobalTime HashtableGpuPrivateLocal.label
    FileReading.printGlobalTime HashtableExpanded.label
    FileReading.printGlobalTime AhoCorasickGpu.label
    FileReading.printGlobalTime AhoCorasickOptimized.label

    printfn ""

    printfn "Computation time with preparations:"
    FileReading.printGlobalTime NaiveSearch.label
    FileReading.printTime NaiveHashingSearch.timer NaiveHashingSearch.label
    FileReading.printTime NaiveSearchGpu.timer NaiveSearchGpu.label
    FileReading.printTime NaiveHashingSearchGpu.timer NaiveHashingSearchGpu.label
    FileReading.printTime NaiveHashingSearchGpuPrivate.timer NaiveHashingSearchGpuPrivate.label
    FileReading.printTime NaiveHashingGpuPrivateLocal.timer NaiveHashingGpuPrivateLocal.label
    FileReading.printTime HashtableGpuPrivateLocal.timer HashtableGpuPrivateLocal.label
    FileReading.printTime HashtableExpanded.timer HashtableExpanded.label
    FileReading.printTime AhoCorasickGpu.timer AhoCorasickGpu.label
    FileReading.printTime AhoCorasickOptimized.timer AhoCorasickOptimized.label

    printfn ""

    printfn "Total time with reading:"
    FileReading.printTime readingTimer NaiveSearch.label
    FileReading.printTime readingTimer NaiveHashingSearch.label
    FileReading.printTime readingTimer NaiveSearchGpu.label
    FileReading.printTime readingTimer NaiveHashingSearchGpu.label
    FileReading.printTime readingTimer NaiveHashingSearchGpuPrivate.label
    FileReading.printTime readingTimer NaiveHashingGpuPrivateLocal.label
    FileReading.printTime readingTimer HashtableGpuPrivateLocal.label
    FileReading.printTime readingTimer HashtableExpanded.label
    FileReading.printTime readingTimer AhoCorasickGpu.label
    FileReading.printTime readingTimer AhoCorasickOptimized.label

    ignore (System.Console.Read())

do //TemplatesGenerator.Main() 
    Main()
