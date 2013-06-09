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

let templateLengths = Helpers.computeTemplateLengths templates maxTemplateLength
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

