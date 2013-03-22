module FileReadingSeparately


open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

open System.IO

let length = 3000000

let maxTemplateLength = 32uy
let templates = 512

let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength
let input = NaiveSearch.generateInput length

let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

let templateArr = NaiveSearch.generateTemplates templatesSum

let k = 1000    
let localWorkSize = 20

let path = "../../random.txt"

let printTime (timer:Timer<string>) label =
    printfn "Total time, %A: %A" label (timer.Total(label))

let printGlobalTime = printTime (Timer<string>.Global)

let printTotalTime (timer:Timer<string>) (readingTimer:Timer<string>) label =
    printfn "Total time, %A: %A" label (timer.Total(label) + readingTimer.Total("reading"))

(*let Main () =
    let mutable cpuMatches = 0  
    let mutable cpuMatchesHashed = 0
    let mutable gpuMatches = 0
    let mutable gpuMatchesHashing = 0

    let readingTimer = new Timer<string>()

    readingTimer.Start()
    let reader = new FileStream(path, FileMode.Open)

    let buffer = Array.zeroCreate length

    let mutable current = 0
    let bound = (int) reader.Length

    while current < bound do
        let read = reader.Read(buffer, 0, min bound length)
        current <- current + read

        cpuMatches <- cpuMatches + NaiveSearch.findMatches read templates templateLengths buffer templateArr
    readingTimer.Lap(NaiveSearch.label)
    reader.Close()

    readingTimer.Start()
    let reader = new FileStream(path, FileMode.Open)

    let buffer = Array.zeroCreate length

    let mutable current = 0
    let bound = (int) reader.Length

    while current < bound do
        let read = reader.Read(buffer, 0, min bound length)
        current <- current + read

        cpuMatchesHashed <- cpuMatchesHashed + NaiveHashingSearch.findMatches read maxTemplateLength templates templatesSum templateLengths buffer templateArr
    readingTimer.Lap(NaiveHashingSearch.label)
    reader.Close()

    readingTimer.Start()
    let reader = new FileStream(path, FileMode.Open)

    let buffer = Array.zeroCreate length

    let mutable current = 0
    let bound = (int) reader.Length

    while current < bound do
        let read = reader.Read(buffer, 0, min bound length)
        current <- current + read

        gpuMatches <- gpuMatches + NaiveSearchGpu.findMatches read k localWorkSize templates templateLengths buffer templateArr
    readingTimer.Lap(NaiveSearchGpu.label)
    reader.Close()

    readingTimer.Start()
    let reader = new FileStream(path, FileMode.Open)

    let buffer = Array.zeroCreate length

    let mutable current = 0
    let bound = (int) reader.Length

    while current < bound do
        let read = reader.Read(buffer, 0, min bound length)
        current <- current + read

        gpuMatchesHashing <- gpuMatchesHashing + NaiveHashingSearchGpu.findMatches read maxTemplateLength k localWorkSize templates templatesSum templateLengths buffer templateArr

    readingTimer.Lap(NaiveHashingSearchGpu.label)
    reader.Close()


    Substrings.verifyResults cpuMatches cpuMatchesHashed NaiveHashingSearch.label
    Substrings.verifyResults cpuMatches gpuMatches NaiveSearchGpu.label
    Substrings.verifyResults cpuMatches gpuMatchesHashing NaiveHashingSearchGpu.label

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
    printTime readingTimer NaiveSearch.label
    printTime readingTimer NaiveHashingSearch.label
    printTime readingTimer NaiveSearchGpu.label
    printTime readingTimer NaiveHashingSearchGpu.label

    ignore (System.Console.Read())*)

do FileReading.Main()