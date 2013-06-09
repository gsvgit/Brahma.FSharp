module NaiveSearch

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open System.Collections.Generic

let random = new System.Random()


let generateInput length =
    Array.init length (fun _ -> (byte) (random.Next(255)))

let computeTemplatesSum templates (templateLengths:array<byte>) =
    TemplatesGenerator.computeTemplatesSum templates templateLengths

let generateTemplates templatesSum =
    TemplatesGenerator.generateTemplates templatesSum


    
let label = ".NET/Naive"

let findMatches length templates (templateLengths:array<byte>) (cpuArr:array<byte>) (templateArr:array<byte>) =
    Timer<string>.Global.Start()

    let result = Array.init length (fun _ -> -1s)
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

                if matches then result.[i] <- (int16) n
    Timer<string>.Global.Lap(label)
    result

//let Main () =
//    let length = 3000000
//
//    let maxTemplateLength = 32uy
//    let templates = 512
//
//    let templateLengths = computeTemplateLengths templates maxTemplateLength
//    let cpuArr = generateInput length
//
//    let templatesSum = computeTemplatesSum templates templateLengths
//    let templateArr = generateTemplates templatesSum
//
//    let prefix = findPrefixes templates maxTemplateLength templateLengths templateArr
//
//    printfn "Finding substrings in string with length %A, using %A..." length label
//    let matches = countMatches (findMatches length templates templateLengths cpuArr templateArr) maxTemplateLength length length templateLengths prefix
//    printfn "done."
//
//    printfn "Found: %A" matches
//    printfn "Avg. time, %A: %A" label (Timer<string>.Global.Average(label))
//
//    ignore (System.Console.Read())

//do Main()