module Matcher

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

[<Struct>]
type GPUConfig =    
    val t:int

type SearchAlgorithm =
    | Nive
    | AhoCorasick
    | Hashtable
    | RabinKarp

type Matcher(provider, config) =

    let maxTemplateLength = 32

    let kRef = ref 1024
    let localWorkSizeRef = ref 512

    let indexRef = ref 1
    let templatesPathRef = ref ""

    let debugMode = ref false


    let memory,ex = Cl.GetDeviceInfo(NaiveSearchGpu.provider.Devices |> Seq.head,Cl.DeviceInfo.MaxMemAllocSize)
    let maxGpuMemory = memory.CastTo<uint64>()

    let maxHostMemory = 256UL * 1024UL * 1024UL

    let additionalArgs = 2UL * (256UL + 2UL) * (uint64) maxTemplateLength * (uint64) templates + (uint64) templates +
                         (uint64) templatesSum + 13UL + 1000UL

    let additionalTempData = 2UL * (256UL + 256UL + 3UL) * (uint64) maxTemplateLength * (uint64) templates + 
                             (uint64) maxTemplateLength * (uint64) templates + 100000UL

    let availableMemory = (int) (min (maxGpuMemory - additionalArgs) (maxHostMemory - additionalArgs - additionalTempData))

    let groupSize = k * localWorkSize * (1 + 2)

    let groups = availableMemory / groupSize

    let length = k * localWorkSize * groups

    printfn "Maximum memory on GPU is %A, additional args size is %A, temp data size is %A" maxGpuMemory additionalArgs additionalTempData

    printfn "Running %A groups with %A items in each, %A in total." groups localWorkSize (localWorkSize * groups)
    printfn "Each item will process %A bytes of input, %A total on each iteration." k length
    printfn ""

    let readingTimer = new Timer<string>()
    let countingTimer = new Timer<string>()

    let offset = 0L
    let bound = 280L*1024L*1024L*1024L

    let run initializer uploader downloader label counter close =
        readingTimer.Start()
        let mutable read = 0
        let mutable highBound = 0

        let matches = Array.zeroCreate 256

        let mutable current = 0L

        let handle = Raw.CreateFile(index)

        let prefix, next, leaf, _ = Helpers.buildSyntaxTree templates (int maxTemplateLength) templateLengths templateArr

        initializer next leaf

        let mutable countingBound = 0
        let mutable matchBound = 0

        let mutable task = null

        let mutable index = 0

        while (current < bound) && ((current = 0L) || (read > 0)) do
            if current > 0L then
                current <- current - 512L

            highBound <- (if int64 length < bound - current then length else (int) (bound - current))
            read <- Raw.ReadFile(handle, buffer, highBound, offset + current)

            if read <= 0 && current = 0L then
                failwith "Failed to start reading!"

            if current > 0L then
                let result = downloader task
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

                task <- uploader()
        
        if (read > 0) then
            printfn "Last read is non-zero!"
            let result = downloader task
            countingTimer.Start()
            counter := !counter + countMatchesDetailed result maxTemplateLength countingBound matchBound templateLengths prefix matches (current - (int64) matchBound)
            countingTimer.Lap(label)

        let hex = Array.map (fun (x : byte) -> System.String.Format("{0:X2} ", x)) templateArr
        let mutable start = 0
        for i in 0..(templates - 1) do
            let pattern = System.String.Concat(Array.sub hex start ((int) templateLengths.[i]))
            printfn "%A: %A matches found by %A" pattern matches.[i] label
            start <- start + (int) templateLengths.[i]

        printfn ""

        ignore(Raw.CloseHandle(handle))
        readingTimer.Lap(label)
        close()


    member this.Search (inputStream, templates, algo)  = 1
