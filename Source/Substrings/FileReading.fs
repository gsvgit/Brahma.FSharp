module FileReading


open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.Collections.Generic
open System.IO

let length = 2000000

let maxTemplateLength = 32uy
let templates = 512

let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength

let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

let templateArr = NaiveSearch.generateTemplates templatesSum

let k = 1000    
let localWorkSize = 20

let path = InputGenerator.path

let printTime (timer:Timer<string>) label =
    if timer <> null then
        printfn "Total time, %A: %A" label (timer.Total(label))

let printGlobalTime = printTime (Timer<string>.Global)

let printTotalTime (timer:Timer<string>) (readingTimer:Timer<string>) label =
    if timer <> null then
        printfn "Total time, %A: %A" label (timer.Total(label) + readingTimer.Total("reading"))

let Main () =
    let readingTimer = new Timer<string>()

    readingTimer.Start()
    let reader = new FileStream(path, FileMode.Open)

    let buffer = Array.zeroCreate length

    let mutable current = 0L
    let bound = reader.Length

    let mutable cpuMatches = 0  
    let mutable cpuMatchesHashed = 0
    let mutable gpuMatches = 0
    let mutable gpuMatchesHashing = 0

    readingTimer.Start()
    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr
    readingTimer.Lap("counting")

    readingTimer.Lap("reading")
    let mutable read = 0
    let mutable lowBound = 0
    let mutable highBound = 0

    //let mutable count = 0
    //let mutable offset = 0L

    //let dictionary = new Dictionary<int64, int>()

    while current < bound do
        readingTimer.Start()

        if current > 0L then
            System.Array.Copy(buffer, (read + lowBound - (int) maxTemplateLength), buffer, 0, (int) maxTemplateLength)
            lowBound <- (int) maxTemplateLength
        
        //offset <- current - (int64) lowBound

        highBound <- (if (int64) (length - lowBound) < bound then (length - lowBound) else (int) bound)
        read <- reader.Read(buffer, lowBound, highBound)
        current <- current + (int64) read
        readingTimer.Lap("reading")

        let mutable countingBound = read + lowBound
        let mutable matchBound = read + lowBound
        if current < bound then
            countingBound <- countingBound - (int) maxTemplateLength

        //cpuMatches <- cpuMatches + NaiveSearch.filterMatches (NaiveSearch.findMatches length templates templateLengths buffer templateArr) countingBound matchBound templates maxTemplateLength templateLengths templateArr dictionary offset
        //count <- count + countingBound
        let cpuResult = NaiveSearch.findMatches length templates templateLengths buffer templateArr

        readingTimer.Start()
        cpuMatches <- cpuMatches + NaiveSearch.countMatches cpuResult maxTemplateLength countingBound matchBound templateLengths prefix
        readingTimer.Lap("counting")
        
        cpuMatchesHashed <- cpuMatchesHashed + NaiveSearch.countMatches (NaiveHashingSearch.findMatches length maxTemplateLength templates templatesSum templateLengths buffer templateArr) maxTemplateLength countingBound matchBound templateLengths prefix
        gpuMatches <- gpuMatches + NaiveSearch.countMatches (NaiveSearchGpu.findMatches length k localWorkSize templates templateLengths buffer templateArr) maxTemplateLength countingBound matchBound templateLengths prefix
        gpuMatchesHashing <- gpuMatchesHashing + NaiveSearch.countMatches (NaiveHashingSearchGpu.findMatches length maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr) maxTemplateLength countingBound matchBound templateLengths prefix

    reader.Close()

    Substrings.verifyResults cpuMatches cpuMatchesHashed NaiveHashingSearch.label
    Substrings.verifyResults cpuMatches gpuMatches NaiveSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesHashing NaiveHashingSearchGpu.label

    printfn ""

    printfn "Reading time: %A" (readingTimer.Total("reading"))

    printfn ""

    printfn "Counting time: %A" (readingTimer.Total("counting"))

    printfn ""

    printfn "Raw computation time spent:"
    printGlobalTime NaiveSearch.label
    printGlobalTime NaiveHashingSearch.label
    printGlobalTime NaiveSearchGpu.label
    printGlobalTime NaiveHashingSearchGpu.label

    printfn ""

    printfn "Computation time with preparations:"
    printGlobalTime NaiveSearch.label
    printTime NaiveHashingSearch.timer NaiveHashingSearch.label
    printTime NaiveSearchGpu.timer NaiveSearchGpu.label
    printTime NaiveHashingSearchGpu.timer NaiveHashingSearchGpu.label

    printfn ""

    printfn "Total time with reading:"
    printTotalTime Timer<string>.Global readingTimer NaiveSearch.label
    printTotalTime NaiveHashingSearch.timer readingTimer NaiveHashingSearch.label
    printTotalTime NaiveSearchGpu.timer readingTimer NaiveSearchGpu.label
    printTotalTime NaiveHashingSearchGpu.timer readingTimer NaiveHashingSearchGpu.label

    ignore (System.Console.Read())

//do Main()