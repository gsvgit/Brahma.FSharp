module Substrings

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let length = 300000

let maxTemplateLength = 32uy
let templates = 512

let templateLengths = NaiveSearch.computeTemplateLengths templates maxTemplateLength
let input = NaiveSearch.generateInput length

let templatesSum = NaiveSearch.computeTemplatesSum templates templateLengths

let templateArr = NaiveSearch.generateTemplates templatesSum

let k = 1000
let localWorkSize = 20

let verifyResults (expected:int) (actual:int) label =
    printfn "Verifying %A results..." label
    if System.Math.Abs(expected - actual) > 0 then        
        printfn "Expected: %A Actual: %A" expected actual
    else
        printfn "Found: %A" actual
    printfn "done."

let printTime (timer:Timer<string>) label =
    printfn "Avg. time, %A: %A" label (timer.Average(label))

let printGlobalTime = printTime (Timer<string>.Global)

let Main () =

    let prefix = NaiveSearch.findPrefixes templates maxTemplateLength templateLengths templateArr

    printfn "Finding substrings in string with length %A using .NET..." length
    let cpuMatches = NaiveSearch.countMatches (NaiveSearch.findMatches length templates templateLengths (Array.copy input) templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."

    printfn "Finding substrings in string with length %A using .NET and hashes..." length     
    let cpuMatchesHashed = NaiveSearch.countMatches (NaiveHashingSearch.findMatches length maxTemplateLength templates templatesSum templateLengths (Array.copy input) templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."

    printfn "Finding substrings in string with length %A using OpenCL and selected platform/device..."  length 
    let gpuMatches = NaiveSearch.countMatches (NaiveSearchGpu.findMatches length k localWorkSize templates templateLengths (Array.copy input) templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."

    printfn "Finding substrings in string with length %A using OpenCL, hashes and selected platform/device..."  length
    let gpuMatchesHashing = NaiveSearch.countMatches (NaiveHashingSearchGpu.findMatches length maxTemplateLength k localWorkSize templates templatesSum templateLengths (Array.copy input) templateArr) maxTemplateLength length length templateLengths prefix
    printfn "done."   
    
    printfn ""

    verifyResults cpuMatches cpuMatchesHashed NaiveHashingSearch.label
    verifyResults cpuMatches gpuMatches NaiveSearchGpu.label
    verifyResults cpuMatches gpuMatchesHashing NaiveHashingSearchGpu.label

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

    ignore (System.Console.Read())

//do Main()