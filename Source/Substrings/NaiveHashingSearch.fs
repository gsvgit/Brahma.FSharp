module NaiveHashingSearch

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let label = ".NET/NaiveHashing"
let timer = new Timer<string>()

let computeTemplateHashes templates templatesSum (templateLengths:array<byte>) (templateArr:array<byte>) =
    let hashes = Array.zeroCreate(templatesSum)
    let mutable templateBase = 0
    for n in 0..(templates - 1) do
        if n > 0 then templateBase <- templateBase + (int) templateLengths.[n - 1]
        let templateEnd = templateBase + (int) templateLengths.[n]
        for i in templateBase..(templateEnd - 1) do
            hashes.[n] <- hashes.[n] + templateArr.[i]
    hashes

let findMatches length maxTemplateLength templates templatesSum (templateLengths:array<byte>) (cpuArr:array<byte>) (templateArr:array<byte>) =
    timer.Start()
    let templateHashes = computeTemplateHashes templates templatesSum templateLengths templateArr
    let result = Array.init length (fun _ -> -1s)

    Timer<string>.Global.Start()
    let localHashes = Array.zeroCreate((int) maxTemplateLength)
    localHashes.[0] <- cpuArr.[0]
    for i in 1..((int) maxTemplateLength - 1) do
        if i < length then localHashes.[i] <- localHashes.[i - 1] + cpuArr.[i]

    for i in 0..(length - 1) do
        if i > 0 then
            for current in 0..((int) maxTemplateLength - 2) do
                localHashes.[current] <- localHashes.[current + 1] - cpuArr.[i - 1]
            if i + (int) maxTemplateLength <= length then
                localHashes.[(int) maxTemplateLength - 1] <- localHashes.[(int) maxTemplateLength - 2] + cpuArr.[i + (int) maxTemplateLength - 1] 

        let mutable templateBase = 0
        for n in 0..(templates - 1) do
            if n > 0 then templateBase <- templateBase + (int) templateLengths.[n - 1]
                        
            let currentLength = templateLengths.[n]
            if i + (int) currentLength <= length && templateHashes.[n] = localHashes.[(int) templateLengths.[n] - 1] then
                let mutable matches = true
                let mutable j = 0
                while (matches && j < (int) currentLength) do
                    if cpuArr.[i + j] <> templateArr.[templateBase + j] then matches <- false
                    j <- j + 1

                if matches then result.[i] <- (int16) n
    Timer<string>.Global.Lap(label)
    timer.Lap(label)
    result

let Main () =
    let length = 3000000

    let maxTemplateLength = 32uy
    let templates = 512

    let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength
    let cpuArr = NaiveSearch.generateInput length

    let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

    let templateArr = NaiveSearch.generateTemplates templatesSum

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    printfn "Finding substrings in string with length %A, using %A..." length label
    let matches = NaiveSearch.countMatches (findMatches length maxTemplateLength templates templatesSum templateLengths cpuArr templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."

    printfn "Found: %A" matches
    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))
    printfn "Avg. time, %A with preparations: %A" label (timer.Average(label))

    ignore (System.Console.Read())

//do Main()