module Matcher

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Brahma.FSharp.OpenCL.Translator.Common
open System.Threading.Tasks
open RawIo

type Config =
    {
        additionalArgs: uint64
        additionalTempData: uint64
        localWorkSize: int
        chankSize: int
        groups: int
        groupSize: int
        bufLength: int
    }

type Templates = {
    mutable number : int
    mutable sizes : byte[]
    mutable content : byte[]
    }

type Matcher(provider, config) =

    let mutable label = ""
    let platformName = "*"
    let deviceType = Cl.DeviceType.Default    

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head) 

    let timer = new Timer<string>()

    let maxTemplateLength = 32       

    let debugMode = ref false
    let mutable buffersCreated = false
    let mutable result = [||]
    let mutable input = [||]
    let mutable ready = true

    let memory,ex = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
    let maxGpuMemory = memory.CastTo<uint64>()

    let maxHostMemory = 256UL * 1024UL * 1024UL

    let configure (templates:array<_>) =
        let tLenghth = templates |> Array.length |> uint64
        let additionalArgs = 2UL * (256UL + 2UL) * (uint64) maxTemplateLength * tLenghth + tLenghth +
                             tLenghth + 13UL + 1000UL

        let additionalTempData = 2UL * (256UL + 256UL + 3UL) * (uint64) maxTemplateLength * tLenghth + 
                                 (uint64) maxTemplateLength * tLenghth + 100000UL

        let availableMemory = (int) (min (maxGpuMemory - additionalArgs) (maxHostMemory - additionalArgs - additionalTempData))
        let lws,ex = Cl.GetDeviceInfo(provider.Devices |> Seq.head,Cl.DeviceInfo.MaxWorkGroupSize)
        let localWorkSize = int <| lws.CastTo<uint64>()
        let chankSize = 1024
        let groupSize = chankSize * localWorkSize * (1 + 2)
        let groups = availableMemory / groupSize
        let length = chankSize * localWorkSize * groups
        {
            additionalArgs = additionalArgs
            additionalTempData = additionalTempData
            localWorkSize = localWorkSize
            chankSize = chankSize
            groups = groups
            groupSize = groupSize
            bufLength = length
        }

    let printConfiguration config =
        printfn
            "Maximum memory on GPU is %A, additional args size is %A, temp data size is %A"
            maxGpuMemory config.additionalArgs config.additionalTempData

        printfn 
            "Running %A groups with %A items in each, %A in total."
            config.groups config.localWorkSize (config.localWorkSize * config.groups)
        printfn "Each item will process %A bytes of input, %A total on each iteration." config.chankSize config.bufLength
        printfn ""
    
    let initialize config templates command =
        timer.Start()
        result <- Array.zeroCreate config.bufLength
        input <- Array.zeroCreate config.bufLength
        let kernel, kernelPrepare, kernelRun = provider.Compile(query=command, translatorOptions=[BoolAsBit])                
        let l = (config.bufLength + (config.chankSize-1))/config.chankSize 
        let d =(new _1D(l,config.localWorkSize))        
        timer.Lap(label)
        kernel, (kernelPrepare d), kernelRun

    let readingTimer = new Timer<string>()
    let countingTimer = new Timer<string>()

    let offset = 0L
    let bound = 280L*1024L*1024L*1024L

    let close () =     
        provider.CloseAllBuffers()
        commandQueue.Dispose()
        provider.Dispose()
        buffersCreated <- false

    let downloader label (task:Task<unit>) =
        if ready then failwith "Not running, can't download!"
        ready <- true

        task.Wait()

        ignore (commandQueue.Add(result.ToHost provider).Finish())
        buffersCreated <- true
        Timer<string>.Global.Lap(label)
        timer.Lap(label)

        result

    let uploader (kernelRun:_ -> Commands.Run<_>) =
        if not ready then failwith "Already running, can't upload!"
        ready <- false

        timer.Start()
        Timer<string>.Global.Start()
        if buffersCreated || (provider.AutoconfiguredBuffers <> null && provider.AutoconfiguredBuffers.ContainsKey(input)) then
            ignore (commandQueue.Add(input.ToGpu provider).Finish())
            async { ignore (commandQueue.Add(kernelRun()).Finish())}
            |> Async.StartAsTask
        else
            ignore (commandQueue.Add(kernelRun()).Finish())
            async {()} |> Async.StartAsTask

    let countMatchesDetailed (result:array<int16>) maxTemplateLength bound length (templateLengths:array<byte>) (prefix:array<int16>) (matchesArray:array<uint64>) offset =
        let mutable matches = 0
        let clearBound = min (bound - 1) (length - (int) maxTemplateLength)

        if (!debugMode) then
            printfn "%A - %A | %A" offset (offset + (int64) bound - 1L) (offset + (int64) length - 1L)
            printfn ""

        for i in 0..clearBound do
            let mutable matchIndex = result.[i]
            if matchIndex >= 0s then
                if (!debugMode) then
                    printfn "%A : %A" (offset + (int64) i) matchIndex

                matchesArray.[(int) matchIndex] <- matchesArray.[(int) matchIndex] + 1UL
                matches <- matches + 1

        for i in (clearBound + 1)..(bound - 1) do
            let mutable matchIndex = result.[i]
            while matchIndex >= 0s && i + (int) templateLengths.[(int) matchIndex] > length do
                matchIndex <- prefix.[(int) matchIndex]
            
            if matchIndex >= 0s then
                if (!debugMode) then
                    printfn "%A : %A" (offset + (int64) i) matchIndex

                matchesArray.[(int) matchIndex] <- matchesArray.[(int) matchIndex] + 1UL
                matches <- matches + 1

        if (!debugMode) then
            printfn ""

        matches

    let run command hdIndex templates config prefix label close =
        let counter = ref 0
        readingTimer.Start()
        let mutable read = 0
        let mutable highBound = 0

        let matches = Array.zeroCreate 256

        let length = config.bufLength
        let templateLengths = templates.sizes
        let templateArr = templates.content

        let mutable current = 0L

        let handle = Raw.CreateFile(hdIndex)        

        let mutable countingBound = 0
        let mutable matchBound = 0

        let mutable task = Unchecked.defaultof<Task<unit>>

        let mutable index = 0

        while (current < bound) && ((current = 0L) || (read > 0)) do
            if current > 0L then
                current <- current - 512L

            highBound <- (if int64 length < bound - current then length else (int) (bound - current))
            read <- Raw.ReadFile(handle, input, highBound, offset + current)

            if read <= 0 && current = 0L then
                failwith "Failed to start reading!"

            if current > 0L then
                let result = downloader label task
                countingTimer.Start()
                counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound + 512L)
                countingTimer.Lap(label)

            if (read > 0) then
                index <- index + 1
                current <- current + (int64) read

                if index = 50 then
                    printfn "I am %A and I've already read %A bytes!" label current
                    index <- 0

                countingBound <- read
                matchBound <- read
                if current < bound then
                    countingBound <- countingBound - 512

                task <- uploader command
        
        if (read > 0) then
            printfn "Last read is non-zero!"
            let result = downloader label task
            countingTimer.Start()
            counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound)
            countingTimer.Lap(label)

        let hex = Array.map (fun (x : byte) -> System.String.Format("{0:X2} ", x)) templateArr
        let mutable start = 0
        for i in 0..(templates.number - 1) do
            let pattern = System.String.Concat(Array.sub hex start ((int) templateLengths.[i]))
            printfn "%A: %A matches found by %A" pattern matches.[i] label
            start <- start + (int) templateLengths.[i]

        printfn ""

        ignore(Raw.CloseHandle(handle))
        readingTimer.Lap(label)
        close()

    let prepareTemplates array = 
        let sorted = Array.sortBy (fun (a:byte[]) -> a.Length) array

        let lengths = Array.map (fun (a:byte[]) -> (byte) a.Length) sorted
        let templateBytes = Array.toSeq sorted |> Array.concat

        let readyTemplates = { number = sorted.Length; sizes = lengths; content = templateBytes;}
        readyTemplates

    member this.NaiveSearch (inputStream, templateArr)  = 
        timer.Reset()
        label <- NaiveSearchGpu.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr NaiveSearchGpu.command
        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content
        kernelPrepare config.bufLength config.chankSize templates.number templates.sizes input templates.content result
        timer.Lap(label)

    member this.AhoCorasik (inputStream, templateArr)  = 
        timer.Reset()
        label <- NaiveSearchGpu.label
        let config = configure templateArr
        let templates = prepareTemplates templateArr
        let kernel, kernelPrepare, kernelRun = initialize config templateArr AhoCorasickOptimized.command        
        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates.number (int maxTemplateLength) templates.sizes templates.content
        let go, _, exit = AhoCorasickOptimized.buildStateMachine templates.number maxTemplateLength next leaf
        kernelPrepare config.bufLength config.chankSize templates.number templates.sizes  go exit leaf maxTemplateLength input templates.content result
        run kernelRun 0 templates config prefix label close
        timer.Lap(label)


let first = [|
    [|0x00uy; 0x00uy; 0x00uy; 0x14uy; 0x66uy; 0x74uy; 0x79uy; 0x70uy; 0x69uy; 0x73uy; 0x6Fuy; 0x6Duy|];//MP4
    [|0x00uy; 0x00uy; 0x00uy; 0x18uy; 0x66uy; 0x74uy; 0x79uy; 0x70uy; 0x33uy; 0x67uy; 0x70uy; 0x35uy|];//MP4
    [|0x00uy; 0x00uy; 0x00uy; 0x1Cuy; 0x66uy; 0x74uy; 0x79uy; 0x70uy; 0x4Duy; 0x53uy; 0x4Euy; 0x56uy; 0x01uy; 0x29uy; 0x00uy; 0x46uy; 0x4Duy; 0x53uy; 0x4Euy; 0x56uy; 0x6Duy; 0x70uy; 0x34uy; 0x32uy|];//MP4
    [|0x1Auy; 0x45uy; 0xDFuy; 0xA3uy; 0x93uy; 0x42uy; 0x82uy; 0x88uy; 0x6Duy; 0x61uy; 0x74uy; 0x72uy; 0x6Fuy; 0x73uy; 0x6Buy; 0x61uy|];//MKV
    [|0x25uy; 0x50uy; 0x44uy; 0x46uy|];//PDF
    [|0x00uy; 0x00uy; 0x01uy; 0xBAuy|];//MPG/VOB
    [|0x30uy; 0x26uy; 0xB2uy; 0x75uy; 0x8Euy; 0x66uy; 0xCFuy; 0x11uy; 0xA6uy; 0xD9uy; 0x00uy; 0xAAuy; 0x00uy; 0x62uy; 0xCEuy; 0x6Cuy|];//WMA/WMV
    [|0x37uy; 0x7Auy; 0xBCuy; 0xAFuy; 0x27uy; 0x1Cuy|];//7z
    [|0x38uy; 0x42uy; 0x50uy; 0x53uy|];//PSD
    [|0x42uy; 0x4Duy|];//BMP
|]

let templates = [|
    [|0x43uy; 0x57uy; 0x53uy|];//SWF
    [|0x46uy; 0x57uy; 0x53uy|];//SWF
    [|0x47uy; 0x49uy; 0x46uy; 0x38uy; 0x37uy; 0x61uy|];//GIF
    [|0x47uy; 0x49uy; 0x46uy; 0x38uy; 0x39uy; 0x61uy|];//GIF
    [|0x49uy; 0x20uy; 0x49uy|];//TIFF
    [|0x49uy; 0x44uy; 0x33uy|];//MP3
    [|0x49uy; 0x49uy; 0x2Auy; 0x00uy|];//TIFF
    [|0x4Auy; 0x41uy; 0x52uy; 0x43uy; 0x53uy; 0x00uy|];//JAR
    [|0x4Fuy; 0x67uy; 0x67uy; 0x53uy; 0x00uy; 0x02uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy|];//OGG
    [|0x50uy; 0x4Buy; 0x03uy; 0x04uy|];//ZIP
    [|0x50uy; 0x4Buy; 0x03uy; 0x04uy; 0x14uy; 0x00uy; 0x01uy; 0x00uy; 0x63uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy|];//ZIP
    [|0x50uy; 0x4Buy; 0x03uy; 0x04uy; 0x14uy; 0x00uy; 0x06uy; 0x00uy|];//DOCX
    [|0x50uy; 0x4Buy; 0x03uy; 0x04uy; 0x14uy; 0x00uy; 0x08uy; 0x00uy; 0x08uy; 0x00uy|];//JAR
    [|0x50uy; 0x4Buy; 0x05uy; 0x06uy|];//ZIP
    [|0x50uy; 0x4Buy; 0x07uy; 0x08uy|];//ZIP
    [|0x52uy; 0x61uy; 0x72uy; 0x21uy; 0x1Auy; 0x07uy; 0x00uy|];//RAR
    [|0x66uy; 0x4Cuy; 0x61uy; 0x43uy; 0x00uy; 0x00uy; 0x00uy; 0x22uy|];//FLAC
    [|0x89uy; 0x50uy; 0x4Euy; 0x47uy; 0x0Duy; 0x0Auy; 0x1Auy; 0x0Auy|];//PNG
    [|0xD0uy; 0xCFuy; 0x11uy; 0xE0uy; 0xA1uy; 0xB1uy; 0x1Auy; 0xE1uy|];//DOC
    [|0xE3uy; 0x10uy; 0x00uy; 0x01uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy|];//INFO
|]

let additional = [|
    [|0x00uy; 0x00uy; 0x00uy; 0x0Cuy; 0x6Auy; 0x50uy; 0x20uy; 0x20uy; 0x0Duy; 0x0Auy|];//JP2
    [|0x00uy; 0x00uy; 0x01uy; 0x00uy|];//ICO
    [|0x00uy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0xFFuy; 0x00uy; 0x00uy; 0x02uy; 0x00uy; 0x01uy|];//MDF
    [|0x01uy; 0x00uy; 0x09uy; 0x00uy; 0x00uy; 0x03uy|];//WMF
    [|0x09uy; 0x08uy; 0x10uy; 0x00uy; 0x00uy; 0x06uy; 0x05uy; 0x00uy|];//XLS
    [|0x0Fuy; 0x00uy; 0xE8uy; 0x03uy|];//PPT
    [|0x1Fuy; 0x8Buy; 0x08uy|];//GZ/TGZ
    [|0x3Cuy; 0x21uy; 0x64uy; 0x6Fuy; 0x63uy; 0x74uy; 0x79uy; 0x70uy|];//HTML
    [|0x3Cuy; 0x3Fuy; 0x78uy; 0x6Duy; 0x6Cuy; 0x20uy; 0x76uy; 0x65uy; 0x72uy; 0x73uy; 0x69uy; 0x6Fuy; 0x6Euy; 0x3Duy|];//MANIFEST
    [|0x3Cuy; 0x3Fuy; 0x78uy; 0x6Duy; 0x6Cuy; 0x20uy; 0x76uy; 0x65uy; 0x72uy; 0x73uy; 0x69uy; 0x6Fuy; 0x6Euy; 0x3Duy; 0x22uy; 0x31uy; 0x2Euy; 0x30uy; 0x22uy; 0x3Fuy; 0x3Euy|];//XUL
    [|0x3Fuy; 0x5Fuy; 0x03uy; 0x00uy|];//HLP
    [|0x43uy; 0x6Cuy; 0x69uy; 0x65uy; 0x6Euy; 0x74uy; 0x20uy; 0x55uy; 0x72uy; 0x6Cuy; 0x43uy; 0x61uy; 0x63uy; 0x68uy; 0x65uy; 0x20uy; 0x4Duy; 0x4Duy; 0x46uy; 0x20uy; 0x56uy; 0x65uy; 0x72uy; 0x20uy|];//DAT
    [|0x49uy; 0x53uy; 0x63uy; 0x28uy|];//CAB
    [|0x49uy; 0x54uy; 0x53uy; 0x46uy|];//CHM
    [|0x4Duy; 0x4Duy; 0x00uy; 0x2Auy|];//TIFF
    [|0x4Duy; 0x4Duy; 0x00uy; 0x2Buy|];//TIFF
    [|0x4Duy; 0x54uy; 0x68uy; 0x64uy|];//MIDI
    [|0x4Duy; 0x5Auy|];//EXE/DLL
    [|0x4Duy; 0x5Auy; 0x90uy; 0x00uy; 0x03uy; 0x00uy; 0x00uy; 0x00uy|];//API
    [|0x4Duy; 0x69uy; 0x63uy; 0x72uy; 0x6Fuy; 0x73uy; 0x6Fuy; 0x66uy; 0x74uy; 0x20uy; 0x43uy; 0x2Fuy; 0x43uy; 0x2Buy; 0x2Buy; 0x20uy|];//PDB
|]

do (new Matcher(1,1)).AhoCorasik(0, (Array.append first templates))