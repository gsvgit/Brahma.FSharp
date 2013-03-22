module NaiveSearch

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open System.Collections.Generic

let random = new System.Random(17)

let computeTemplateLengths templates maxTemplateLength =
    Array.sort (Array.init templates (fun _ -> (byte) (random.Next((int) maxTemplateLength - 1) + 1)))

let generateInput length =
    Array.init length (fun _ -> (byte) (random.Next(255)))

let computeTemplatesSum templates (templateLengths:array<byte>) =
    let mutable l = 0
    for i in 0..(templates - 1) do
        l <- l + (int) templateLengths.[i]
    l 

let generateTemplates templatesSum = Array.init templatesSum (fun _ -> (byte) (random.Next(255)))

let countMatches (result:array<int>) bound length templates maxTemplateLength (templateLengths:array<byte>) (templateArr:array<byte>) =
    let next = Array.init (templates * (int) maxTemplateLength) (fun _ -> Array.init 256 (fun _ -> -1s))
    let leaf = Array.init (templates * (int) maxTemplateLength) (fun _ -> -1s)
    let prefix = Array.init (templates * (int) maxTemplateLength) (fun _ -> -1s)

    let mutable vertices = 0
    let mutable templateBase = 0

    for n in 0..(templates - 1) do
        let mutable v = 0
        for i in 0..((int) templateLengths.[n] - 1) do
            if next.[v].[(int) templateArr.[templateBase + i]] < 0s then
                vertices <- vertices + 1
                next.[v].[(int) templateArr.[templateBase + i]] <- (int16) vertices
            v <- (int) next.[v].[(int) templateArr.[templateBase + i]]

            if leaf.[v] >= 0s then
                prefix.[n] <- leaf.[v]
        leaf.[v] <- (int16) n
        templateBase <- templateBase + (int) templateLengths.[n]


    let mutable matches = 0
    for i in 0..(bound - 1) do
        let mutable matchIndex = result.[i]
        while matchIndex >= 0 && i + (int) templateLengths.[matchIndex] > length do
            matchIndex <- (int) prefix.[matchIndex]
            
        if matchIndex >= 0 then
            matches <- matches + 1
    matches

let filterMatches (result:array<int>) bound length templates (maxTemplateLength:byte) (templateLengths:array<byte>) (templateArr:array<byte>) (dictionary:Dictionary<int64, int>) offset =
    let next = Array.init (templates * (int) maxTemplateLength) (fun _ -> Array.init 256 (fun _ -> -1s))
    let leaf = Array.init (templates * (int) maxTemplateLength) (fun _ -> -1s)
    let prefix = Array.init (templates * (int) maxTemplateLength) (fun _ -> -1s)

    let mutable vertices = 0
    let mutable templateBase = 0

    for n in 0..(templates - 1) do
        let mutable v = 0
        for i in 0..((int) templateLengths.[n] - 1) do
            if next.[v].[(int) templateArr.[templateBase + i]] < 0s then
                vertices <- vertices + 1
                next.[v].[(int) templateArr.[templateBase + i]] <- (int16) vertices
            v <- (int) next.[v].[(int) templateArr.[templateBase + i]]

            if leaf.[v] >= 0s then
                prefix.[n] <- leaf.[v]
        leaf.[v] <- (int16) n
        templateBase <- templateBase + (int) templateLengths.[n]


    let mutable matches = 0
    for i in 0..(bound - 1) do
        let mutable matchIndex = result.[i]
        while matchIndex >= 0 && i + (int) templateLengths.[matchIndex] > length do
            matchIndex <- (int) prefix.[matchIndex]
            
        if matchIndex >= 0 then
            dictionary.Add((int64) i + offset, matchIndex)
            matches <- matches + 1
    matches
    
let label = ".NET/Naive"

let findMatches length templates (templateLengths:array<byte>) (cpuArr:array<byte>) (templateArr:array<byte>) =
    Timer<string>.Global.Start()

    let result = Array.init length (fun _ -> -1)
    for i in 0 .. (length - 1) do
        let mutable templateBase = 0
        for n in 0..(templates - 1) do
            if n > 0 then templateBase <- templateBase + (int) templateLengths.[n - 1]
                        
            let templateEnd = templateBase + (int) templateLengths.[n]
            if i + (int) templateLengths.[n] <= length then
                let mutable matches = true

                let mutable j = 0
                while (matches && templateBase + j < templateEnd) do
                    if cpuArr.[i + j] <> templateArr.[templateBase + j] then
                        matches <- false

                    j <- j + 1

                if matches then result.[i] <- n
    Timer<string>.Global.Lap(label)
    result

let Main () =
    let length = 3000000

    let maxTemplateLength = 32uy
    let templates = 512

    let templateLengths = computeTemplateLengths templates maxTemplateLength
    let cpuArr = generateInput length

    let templatesSum = computeTemplatesSum templates templateLengths
    let templateArr = generateTemplates templatesSum

    printfn "Finding substrings in string with length %A, using %A..." length label
    let matches = countMatches (findMatches length templates templateLengths cpuArr templateArr) length length templates maxTemplateLength templateLengths templateArr
    printfn "done."

    printfn "Found: %A" matches
    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))

    ignore (System.Console.Read())

//do Main()