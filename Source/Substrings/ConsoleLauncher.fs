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

let groups = ref 16

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
         "-g", ArgType.Int (fun i -> groups := i), "Work groups number."
         "-input", ArgType.String (fun s -> pathRef := s), "Input file path."
         "-templates", ArgType.String (fun s -> templatesPathRef := s), "Templates file path."
         ] |> List.map (fun (shortcut, argtype, description) -> ArgInfo(shortcut, argtype, description))
    ArgParser.Parse commandLineSpecs

    let k = !kRef  
    let localWorkSize = !localWorkSizeRef

    let length = k * localWorkSize * !groups

    let path = !pathRef
    let templatesPath = !templatesPathRef

    let templatesReader = File.OpenRead(templatesPath)
    let formatter = new BinaryFormatter()
    let deserialized = formatter.Deserialize(templatesReader)

    let mutable templates = 0
    let mutable templateLengths = null
    let mutable templatesSum = 0
    let mutable templateArr = null

    match deserialized with
    | :? Templates as t -> 
        templates <- t.number
        templateLengths <- t.sizes
        templatesSum <- t.content.Length
        templateArr <- t.content
    | other -> failwith "Deserialized object is not a Templates struct!"

    printfn "Running %A groups with %A items in each, %A in total." !groups localWorkSize (localWorkSize * !groups)
    printfn "Each item will process %A bytes of input, %A total on each iteration." k (localWorkSize * !groups * k)
    printfn ""

    let mutable cpuMatches = 0  
    let mutable cpuMatchesHashed = 0
    let mutable gpuMatches = 0
    let mutable gpuMatchesHashing = 0
    let mutable gpuMatchesLocal = 0

    let buffer = Array.zeroCreate length

    let readingTimer = new Timer<string>()

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

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

        //cpuMatches <- cpuMatches + NaiveSearch.countMatches (NaiveSearch.findMatches length templates templateLengths buffer templateArr) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveSearch.label)

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

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

        //cpuMatchesHashed <- cpuMatchesHashed + NaiveSearch.countMatches (NaiveHashingSearch.findMatches length maxTemplateLength templates templatesSum templateLengths buffer templateArr) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveHashingSearch.label)

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    NaiveSearchGpu.initialize length k localWorkSize templates templateLengths buffer templateArr

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

        gpuMatches <- gpuMatches + NaiveSearch.countMatches (NaiveSearchGpu.getMatches()) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveSearchGpu.label)

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    NaiveHashingSearchGpu.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr

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

        gpuMatchesHashing <- gpuMatchesHashing + NaiveSearch.countMatches (NaiveHashingSearchGpu.getMatches()) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveHashingSearchGpu.label)

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    NaiveSearchGpuLocalTemplates.initialize length k localWorkSize templates templateLengths buffer templatesSum templateArr

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

        gpuMatchesLocal <- gpuMatchesLocal + NaiveSearch.countMatches (NaiveSearchGpuLocalTemplates.getMatches()) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveSearchGpuLocalTemplates.label)

    Substrings.verifyResults cpuMatches cpuMatchesHashed NaiveHashingSearch.label
    Substrings.verifyResults cpuMatches gpuMatches NaiveSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesHashing NaiveHashingSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesLocal NaiveSearchGpuLocalTemplates.label

    printfn ""

    printfn "Raw computation time spent:"
    
    //FileReading.printGlobalTime NaiveSearch.label
    //FileReading.printGlobalTime NaiveHashingSearch.label
    FileReading.printGlobalTime NaiveSearchGpu.label
    FileReading.printGlobalTime NaiveHashingSearchGpu.label
    FileReading.printGlobalTime NaiveSearchGpuLocalTemplates.label

    printfn ""

    printfn "Computation time with preparations:"
    //FileReading.printGlobalTime NaiveSearch.label
    //FileReading.printTime NaiveHashingSearch.timer NaiveHashingSearch.label
    FileReading.printTime NaiveSearchGpu.timer NaiveSearchGpu.label
    FileReading.printTime NaiveHashingSearchGpu.timer NaiveHashingSearchGpu.label
    FileReading.printTime NaiveSearchGpuLocalTemplates.timer NaiveSearchGpuLocalTemplates.label

    printfn ""

    printfn "Total time with reading:"
    //FileReading.printTime readingTimer NaiveSearch.label
    //FileReading.printTime readingTimer NaiveHashingSearch.label
    FileReading.printTime readingTimer NaiveSearchGpu.label
    FileReading.printTime readingTimer NaiveHashingSearchGpu.label
    FileReading.printTime readingTimer NaiveSearchGpuLocalTemplates.label

    ignore (System.Console.Read())

do Main()