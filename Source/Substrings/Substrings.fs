module Substrings

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let random = new System.Random()
        
let k = 1000

let length = 3000000

let maxTemplateLength = 32uy
let templates = 512

let templateLengths = Array.init templates (fun _ -> (byte) (random.Next((int) maxTemplateLength - 1) + 1))
let baseArr = Array.init length (fun _ -> (byte) (random.Next(255)))

let templatesSum =
    let mutable l = 0
    for i in 0..(templates - 1) do
        l <- l + (int) templateLengths.[i]
    l

let templateArr = Array.init templatesSum (fun _ -> (byte) (random.Next(255)))

let templateHashes =
    let hashes = Array.zeroCreate(templatesSum)
    let mutable templateBase = 0
    for n in 0..(templates - 1) do
        if n > 0 then templateBase <- templateBase + (int) templateLengths.[n - 1]
        let templateEnd = templateBase + (int) templateLengths.[n]
        for i in templateBase..(templateEnd - 1) do
            hashes.[n] <- hashes.[n] + templateArr.[i]
    hashes

let l = (length + (k-1))/k

let localHashesArr = Array.zeroCreate((int) maxTemplateLength * l)

let cpuArr = Array.copy baseArr
let gpuArr = Array.copy baseArr

let Main () =

    let platformName = "*"
    
    let localWorkSize = 20
    let iterations = 1
    let deviceType = Cl.DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)    

    let command = 
        <@
            fun (rng:_1D) l k templates (lengths:array<_>) (input:array<_>) (t:array<_>) (result:array<int>) ->
                let r = rng.GlobalID0
                let _start = r * k
                let mutable _end = _start + k
                if _end > l then _end <- l
                let mutable buf = 0
                for i in _start..(_end - 1) do
                    let mutable templateBase = 0
                    for n in 0..(templates - 1) do
                        if n > 0 then templateBase <- templateBase + (int) lengths.[n - 1]
                        
                        let currentLength = lengths.[n]
                        if i + (int) currentLength <= l then
                            let mutable matches = 1
                            let mutable j = 0
                            while (matches = 1 && j < (int) currentLength) do
                                if input.[i + j] <> t.[templateBase + j] then matches <- 0
                                j <- j + 1

                            buf <- buf + matches
                result.[0] <!+ buf
        @>

    let hashingCommand = 
        <@
            fun (rng:_1D) l k templates (lengths:array<_>) (hashes:array<_>) (localHashes:array<_>) maxLength (input:array<_>) (t:array<_>) (result:array<int>) ->
                let r = rng.GlobalID0
                let _start = r * k
                let mutable _end = _start + k
                if _end > l then _end <- l
                let mutable buf = 0

                let localHashesBase = r * (int) maxLength
                localHashes.[localHashesBase] <- input.[_start]
                for i in 1..((int) maxLength - 1) do
                    if _start + i < l then localHashes.[localHashesBase + i] <- localHashes.[localHashesBase + i - 1] + input.[_start + i]

                for i in _start .. (_end - 1) do
                    if i > _start then
                        for current in 0..((int) maxLength - 2) do
                            localHashes.[localHashesBase + current] <- localHashes.[localHashesBase + current + 1] - input.[i - 1]
                        if i + (int) maxLength <= l then
                            localHashes.[localHashesBase + (int) maxLength - 1] <- localHashes.[localHashesBase + (int) maxLength - 2] + input.[i + (int) maxLength - 1]

                    let mutable templateBase = 0
                    for n in 0..(templates - 1) do
                        if n > 0 then templateBase <- templateBase + (int) lengths.[n - 1]
                        
                        let currentLength = (int) lengths.[n]
                        if (i + (int) currentLength) <= l && hashes.[n] = localHashes.[localHashesBase + currentLength - 1] then
                            let mutable matches = 1

                            let mutable j = 0
                            while (matches = 1 && j < (int) currentLength) do
                                if input.[i + j] <> t.[templateBase + j] then  matches <- 0
                                j <- j + 1

                            buf <- buf + matches

                result.[0] <!+ buf
        @>

    printfn "Finding substrings in string with length %A,  %A times using .NET..." length iterations
    let mutable cpuMatches = 0
    for i in 0 .. iterations - 1 do
        cpuMatches <- 0

        Timer<string>.Global.Start()
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

                    if matches then cpuMatches <- cpuMatches + 1
        Timer<string>.Global.Lap(".NET")
    printfn "done."

    printfn "Finding substrings in string with length %A,  %A times using .NET and hashes..." length iterations    
    let mutable cpuMatchesHashed = 0
    for i in 0 .. iterations - 1 do
        cpuMatchesHashed <- 0
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
                    let mutable matches = 1
                    let mutable j = 0
                    while (matches = 1 && j < (int) currentLength) do
                        if baseArr.[i + j] <> templateArr.[templateBase + j] then matches <- 0
                        j <- j + 1

                    cpuMatchesHashed <- cpuMatchesHashed + matches
        Timer<string>.Global.Lap(".NET-hashed")
    printfn "done."

    printfn "Finding substrings in string with length %A,  %A times uling OpenCL and selected platform/device..."  length iterations

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let matches = [|0|]    
    let d =(new _1D(l,localWorkSize))
    kernelPrepare d length k templates templateLengths gpuArr templateArr matches
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(matches.ToHost provider).Finish()
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernelRun()).Finish()
        Timer<string>.Global.Lap("OpenCL")
    
    printfn "done."

    printfn "Finding substrings in string with length %A,  %A times uling OpenCL, hashes and selected platform/device..."  length iterations

    let kernelHashed, kernelPrepareHashed, kernelRunHashed = provider.Compile hashingCommand
    let hashedMatches = [|0|]    
    let d =(new _1D(l,localWorkSize))
    kernelPrepareHashed d length k templates templateLengths templateHashes localHashesArr maxTemplateLength gpuArr templateArr hashedMatches
    let _ = commandQueue.Add(kernelRunHashed()).Finish()
    let _ = commandQueue.Add(hashedMatches.ToHost provider).Finish()
    for i in 0 .. iterations - 1 do
        Timer<string>.Global.Start()
        let _ = commandQueue.Add(kernelRunHashed()).Finish()
        Timer<string>.Global.Lap("OpenCL-hashed")
    
    printfn "done."   
    
    printfn "Verifying OpenCL results..."    
    if System.Math.Abs(matches.[0] - cpuMatches) > 0 then        
        printfn "Expected: %A Actual: %A" cpuMatches matches.[0]
    else
        printfn "Found: %A" cpuMatches
    
    printfn "Verifying .NET-hashed results..."    
    if System.Math.Abs(cpuMatchesHashed - cpuMatches) > 0 then        
        printfn "Expected: %A Actual: %A" cpuMatchesHashed cpuMatches
    else
        printfn "Found: %A" cpuMatches

    printfn "Verifying OpenCL-hashed results..."    
    if System.Math.Abs(hashedMatches.[0] - cpuMatches) > 0 then        
        printfn "Expected: %A Actual: %A" cpuMatches hashedMatches.[0]
    else
        printfn "Found: %A" cpuMatches

    printfn "done."

    Timer<string>.Global.Average(".NET") |> printfn "Avg. time, C#: %A"
    Timer<string>.Global.Average(".NET-hashed") |> printfn "Avg. time, C#-hashed: %A"
    Timer<string>.Global.Average("OpenCL") |> printfn "Avg. time, OpenCL: %A"
    Timer<string>.Global.Average("OpenCL-hashed") |> printfn "Avg. time, OpenCL-hashed: %A"

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()

    ignore (System.Console.Read())

do Main()