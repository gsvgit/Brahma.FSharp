module FileReadingSeparately


open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.IO

let length = 4000000

let maxTemplateLength = 32uy
let templates = 512

let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength

let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

let templateArr = NaiveSearch.generateTemplates templatesSum

let k = 1000    
let localWorkSize = 500

let path = InputGenerator.path

let Main () =
    let mutable cpuMatches = 0  
    let mutable cpuMatchesHashed = 0
    let mutable gpuMatches = 0
    let mutable gpuMatchesHashing = 0
    let mutable gpuMatchesLocal = 0
    let mutable gpuMatchesHashingPrivate = 0
    let mutable gpuMatchesHashingPrivateLocal = 0

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

        cpuMatches <- cpuMatches + NaiveSearch.countMatches (NaiveSearch.findMatches length templates templateLengths buffer templateArr) countingBound matchBound templateLengths prefix
    
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

        cpuMatchesHashed <- cpuMatchesHashed + NaiveSearch.countMatches (NaiveHashingSearch.findMatches length maxTemplateLength templates templatesSum templateLengths buffer templateArr) countingBound matchBound templateLengths prefix
    
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

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    NaiveHashingSearchGpuPrivate.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr

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

        gpuMatchesHashingPrivate <- gpuMatchesHashingPrivate + NaiveSearch.countMatches (NaiveHashingSearchGpuPrivate.getMatches()) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveHashingSearchGpuPrivate.label)

    readingTimer.Start()
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    let mutable current = 0L

    let reader = new FileStream(path, FileMode.Open)
    let bound = reader.Length

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    NaiveHashingGpuPrivateLocal.initialize length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr

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

        gpuMatchesHashingPrivateLocal <- gpuMatchesHashingPrivateLocal + NaiveSearch.countMatches (NaiveHashingGpuPrivateLocal.getMatches()) countingBound matchBound templateLengths prefix
    
    reader.Close()
    readingTimer.Lap(NaiveHashingGpuPrivateLocal.label)

    Substrings.verifyResults cpuMatches cpuMatchesHashed NaiveHashingSearch.label
    Substrings.verifyResults cpuMatches gpuMatches NaiveSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesHashing NaiveHashingSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesLocal NaiveSearchGpuLocalTemplates.label
    Substrings.verifyResults cpuMatches gpuMatchesHashingPrivate NaiveHashingSearchGpuPrivate.label
    Substrings.verifyResults cpuMatches gpuMatchesHashingPrivateLocal NaiveHashingGpuPrivateLocal.label

    printfn ""

    printfn "Raw computation time spent:"
    
    FileReading.printGlobalTime NaiveSearch.label
    FileReading.printGlobalTime NaiveHashingSearch.label
    FileReading.printGlobalTime NaiveSearchGpu.label
    FileReading.printGlobalTime NaiveHashingSearchGpu.label
    FileReading.printGlobalTime NaiveSearchGpuLocalTemplates.label
    FileReading.printGlobalTime NaiveHashingSearchGpuPrivate.label
    FileReading.printGlobalTime NaiveHashingGpuPrivateLocal.label

    printfn ""

    printfn "Computation time with preparations:"
    FileReading.printGlobalTime NaiveSearch.label
    FileReading.printTime NaiveHashingSearch.timer NaiveHashingSearch.label
    FileReading.printTime NaiveSearchGpu.timer NaiveSearchGpu.label
    FileReading.printTime NaiveHashingSearchGpu.timer NaiveHashingSearchGpu.label
    FileReading.printTime NaiveSearchGpuLocalTemplates.timer NaiveSearchGpuLocalTemplates.label
    FileReading.printTime NaiveHashingSearchGpuPrivate.timer NaiveHashingSearchGpuPrivate.label
    FileReading.printTime NaiveHashingGpuPrivateLocal.timer NaiveHashingGpuPrivateLocal.label

    printfn ""

    printfn "Total time with reading:"
    FileReading.printTime readingTimer NaiveSearch.label
    FileReading.printTime readingTimer NaiveHashingSearch.label
    FileReading.printTime readingTimer NaiveSearchGpu.label
    FileReading.printTime readingTimer NaiveHashingSearchGpu.label
    FileReading.printTime readingTimer NaiveSearchGpuLocalTemplates.label
    FileReading.printTime readingTimer NaiveHashingSearchGpuPrivate.label
    FileReading.printTime readingTimer NaiveHashingGpuPrivateLocal.label

    ignore (System.Console.Read())

//do Main()