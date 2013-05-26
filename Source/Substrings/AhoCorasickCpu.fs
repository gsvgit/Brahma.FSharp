module AhoCorasickCpu

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks

let label = ".NET/AhoCorasickCpu"
let timer = new Timer<string>()

let mutable nextArray = null;
let mutable leafArray = null

let initialize (next:array<array<int16>>) (leaf:array<int16>) =
    nextArray <- next
    leafArray <- leaf

let count l templates (lengths:array<byte>) (go:array<int16>) (exit:array<int16>) (leaf:array<int16>) maxLength (input:array<byte>) (t:array<byte>) (result:array<int16>) =
            let mutable _start = 0
            let mutable _end = l

            let localTemplateLengths = local (Array.zeroCreate 512)

            for i in _start .. (_end - 1) do
                result.[i] <- -1s

            let mutable v = 0s
            for i in _start .. (_end - 1) do
                if _start - i = 65 then
                    barrier()

                v <- go.[256 * (int) v + (int) input.[i]]
                let mutable parent = v

                while parent > 0s do
                    let mutable currentTemplate = leaf.[(int) parent]
                    if currentTemplate >= 0s then
                        let position = i - (int) localTemplateLengths.[(int) currentTemplate] + 1
                        if result.[position] < currentTemplate then
                            result.[position] <- currentTemplate
                    parent <- exit.[(int) parent]

let findMatches length maxTemplateLength templates templatesSum (templateLengths:array<byte>) (cpuArr:array<byte>) (templateArr:array<byte>) =
    timer.Start()
    
    let result = Array.init length (fun _ -> -1s)
    let go, _, exit = AhoCorasickOptimized.buildStateMachine templates maxTemplateLength nextArray leafArray

    Timer<string>.Global.Start()

    count length templates templateLengths go exit leafArray maxTemplateLength cpuArr templateArr result

    Timer<string>.Global.Lap(label)
    timer.Lap(label)
    result