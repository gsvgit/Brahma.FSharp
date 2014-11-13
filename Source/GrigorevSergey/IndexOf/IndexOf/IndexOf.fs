module IndexOf

open System
open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open System.IO
open System.Text

let bufferSize = 262144//1048576
let maxGpuThreads = 1024
let maxCpuThreads = 16
let quasiThreads = 1024

let prefixFunction str =
    let l = String.length str
    let arr = Array.zeroCreate l
    let rec iter i j =
        if i >= l
        then arr
        else
            if str.[i] = str.[j]
            then
                arr.[i] <- j + 1
                iter (i + 1) (j + 1)
            elif j <> 0
            then iter i arr.[j - 1]
            else iter (i + 1) 0
    iter 1 0

let serialIndexOf (stream : Stream) (substr : string) =
    let l = substr.Length
    let prefixes = prefixFunction substr
    let buf = (int) bufferSize |> Array.zeroCreate
    let search pref len =
        let str = Encoding.Default.GetString (buf)
        let rec iter i j =
            if j = l
            then (i - j, j)
            elif i >= len
            then (len, j)
            else 
                if str.[i] = substr.[j]
                then iter (i + 1) (j + 1)
                elif j <> 0
                then prefixes.[j - 1] |> iter i
                else iter (i + 1) 0
        iter 0 pref
    let rec iter index pref =
        let res = stream.Read (buf, 0, bufferSize)
        match res with
        | 0 -> -1L
        | r ->
            match search pref r with
            | i, j when i = bufferSize -> iter (index + int64 bufferSize) j
            | i, _ -> index + (int64 i)
    iter 0L 0

(*
let bestThreadNumber ssl bs =
    let max = bs / ssl
    let rec iter i m =
        if i > max
        then m
        else
            if (1. + float (i - 1) * float ssl / float bs) / float i < (1. + float (m - 1) * float ssl / float bs) / float m
            then iter (2 * i) i
            else iter (2 * i) m
    iter 1 1
*)

let parameters N n m =
    let l = n - 1
    let t = if N <= n then 1 else N / n
    let rec iter i =
        if i * 2 > t
        then i
        else iter (i * 2)
    let t = min m (iter 1)
    let tmp = N + l * (t - 1)
    let s = if tmp % t = 0 then tmp / t else tmp / t + 1
    t, s

let quasiParallelIndexOf (stream : Stream) (substr : string) =
    let l = substr.Length
    let prefixes = prefixFunction substr
    let buf = (int) bufferSize |> Array.zeroCreate
    let search (str : string) pref start len =
        let rec iter i j =
            if j = l
            then (i - j, j)
            elif i >= len + start
            then (len + start, j)
            else 
                if str.[i] = substr.[j]
                then iter (i + 1) (j + 1)
                elif j <> 0
                then prefixes.[j - 1] |> iter i
                else iter (i + 1) 0
        iter start pref
    let emulateMultiThreading pref r =
        let str = Encoding.Default.GetString (buf)
        let subProblemSize = (r + l * (quasiThreads - 1)) / quasiThreads
        let arr = Array.zeroCreate quasiThreads
        let len i = if i < quasiThreads - 1 then subProblemSize else r - subProblemSize * i
        for i in 0 .. quasiThreads - 1 do
            arr.[i] <- search str (if i = 0 then pref else 0) (subProblemSize * i) (len i)
        let rec iter i =
            if i = quasiThreads
            then (bufferSize, snd arr.[quasiThreads - 1])
            else
                match fst arr.[i] with
                | n when n < subProblemSize * i + len i -> (n, 0)
                | _ -> iter (i + 1)
        iter 0
    let rec iter index pref =
        let res = stream.Read (buf, 0, bufferSize)
        match res with
        | 0 -> -1L
        | r ->
            match emulateMultiThreading pref r with
            | i, j when i = bufferSize -> iter (index + int64 bufferSize) j
            | i, _ -> index + (int64 i)
    iter 0L 0

let pseudoParallelIndexOf (stream : Stream) (substr : string) =
    let command (r : int) (str : byte array) (len : int) (substr : byte array) (l : int) (prefixes : int array) (posArr : int array) (prefArr : int array) (fstPref : int) (lws : int) (threads : int) =
        let start = r * (lws - l + 1)
        let mutable ln = 0
        if r < threads - 1
        then ln <- lws
        else ln <- (len - start)

        let mutable i = start
        let mutable j = 0
        if r = 0
        then j <- fstPref
        while i <= start + ln do
            if j = l
            then
                posArr.[r] <- i - j
                prefArr.[r] <- j
                i <- start + ln + 1
            elif i >= ln + start
            then
                posArr.[r] <- len
                prefArr.[r] <- j
                i <- start + ln + 1
            else 
                if str.[i] = substr.[j]
                then 
                    i <- i + 1
                    j <- j + 1
                elif j <> 0
                then j <- prefixes.[j - 1]
                else i <- i + 1 
    
    let ss = Encoding.Default.GetBytes (substr)
    let l = ss.Length
    let fullBufferThreads, fullBufferLocalWorkSize = parameters bufferSize l quasiThreads
    let posArr = Array.zeroCreate fullBufferThreads
    let prefArr = Array.zeroCreate fullBufferThreads
    let prefixes = prefixFunction substr
    let buf = (int) bufferSize |> Array.zeroCreate

    let mutable index = 0L
    let mutable pref = 0
    let mutable flag = true
    let mutable result = -1L
    while flag do
        let res = stream.Read (buf, 0, bufferSize)
        match res with
        | 0 -> flag <- false
        | r ->
            let threads, lws =
                if r = bufferSize
                then fullBufferThreads, fullBufferLocalWorkSize
                else
                    let t, s = parameters r l quasiThreads
                    t, s

            for i in 0 .. threads - 1 do
                command i buf r ss l prefixes posArr prefArr pref lws threads
            
            let rr = ref (0, 0)

            let rec iter i =
                if i = threads
                then rr := (r, prefArr.[threads - 1])
                else
                    match posArr.[i] with
                    | n when n < r -> rr := (n, 0)
                    | _ -> iter (i + 1)
            iter 0

            match !rr with
            | i, j when i = r ->
                index <- index + int64 bufferSize
                pref <- j
            | i, _ ->
                result <- index + (int64 i)
                flag <- false

    result

let cpuIndexOf stream substr =
    new NotImplementedException () |> raise


let gpuIndexOf (stream : Stream) (substr : string) =
    
    let platformName = "*"
    let deviceType = DeviceType.All

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message
            
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let command =
        <@
            fun (rng : _1D) (str : byte array) (len : int) (substr : byte array) (l : int) (prefixes : int array) (posArr : int array) (prefArr : int array) (fstPref : int) (lws : int) (threads : int) ->
                let r = rng.GlobalID0
                let start = r * (lws - l + 1)
                let mutable ln = 0
                if r < threads - 1
                then ln <- lws
                else ln <- (len - start)

                let mutable i = start
                let mutable j = 0
                if r = 0
                then j <- fstPref
                while i <= start + ln do
                    if j = l
                    then
                        posArr.[r] <- i - j
                        prefArr.[r] <- j
                        i <- start + ln + 1
                    elif i >= ln + start
                    then
                        posArr.[r] <- len
                        prefArr.[r] <- j
                        i <- start + ln + 1
                    else 
                        if str.[i] = substr.[j]
                        then 
                            i <- i + 1
                            j <- j + 1
                        elif j <> 0
                        then j <- prefixes.[j - 1]
                        else i <- i + 1 
        @>
    
    let kernel, kernelPrepare, kernelRun = provider.Compile command

    let ss = Encoding.Default.GetBytes (substr)
    let l = ss.Length
    let fullBufferThreads, fullBufferLocalWorkSize = parameters bufferSize l maxGpuThreads
    let posArr = Array.zeroCreate fullBufferThreads
    let prefArr = Array.zeroCreate fullBufferThreads
    let prefixes = prefixFunction substr
    let buf = (int) bufferSize |> Array.zeroCreate

    let D = new _1D(fullBufferThreads, 1)
   
    let mutable index = 0L
    let mutable pref = 0
    let mutable flag = true
    let mutable result = -1L
    while flag do
        let res = stream.Read (buf, 0, bufferSize)
        match res with
        | 0 -> flag <- false
        | r ->
            let threads, lws, d =
                if r = bufferSize
                then fullBufferThreads, fullBufferLocalWorkSize, D
                else
                    let t, s = parameters r l maxGpuThreads
                    t, s, new _1D(t, 1)
            kernelPrepare d buf r ss l prefixes posArr prefArr pref lws threads
            //let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
            let _ = commandQueue.Add(kernelRun()).Finish()
            let _ = commandQueue.Add(posArr.ToHost(provider)).Finish()
            let _ = commandQueue.Add(prefArr.ToHost(provider)).Finish()
            //commandQueue.Dispose()
            //provider.CloseAllBuffers()

            let rr = ref (0, 0)

            let rec iter i =
                if i = threads
                then rr := (r, prefArr.[threads - 1])
                else
                    match posArr.[i] with
                    | n when n < r -> rr := (n, 0)
                    | _ -> iter (i + 1)
            iter 0

            match !rr with
            | i, j when i = r ->
                index <- index + int64 bufferSize
                pref <- j
            | i, _ ->
                result <- index + (int64 i)
                flag <- false
    commandQueue.Dispose()
    provider.CloseAllBuffers()
    provider.Dispose()
    result