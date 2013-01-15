module Brahma.FSharp.OpenCL.Full

open NUnit.Framework
open System.IO
open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Wrapper
open System
open System.Reflection
open Microsoft.FSharp.Quotations


[<TestFixture>]
type Translator() =
    
    let deviceType = Cl.DeviceType.Default
    let platformName = "*"

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with
        | ex -> failwith ex.Message

    let checkResult1Generic inArray (kernel:Kernel<_,_>) getExpected =
        let l = Array.length inArray
        let a = inArray        
        let inBuf = new Buffer<_>(provider, Operations.ReadWrite, Memory.Device, a)
        let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
        let cq = commandQueue.Add(kernel.Run(new _1D(l, 1), inBuf)).Finish()
        let r = Array.zeroCreate l
        let cq2 = commandQueue.Add(inBuf.Read(0, l, r)).Finish()
        let expected = getExpected inArray
        Assert.AreEqual(expected, r)
        commandQueue.Dispose()

    let checkResult (kernel:Kernel<_,_>) expected =        
        let a = [|0 .. 3|]
        checkResult1Generic a kernel (fun _ -> expected)

    let checkResultFloat (kernel:Kernel<_,_>) expected =
        let a = Array.init 4 (fun i -> float32 i)        
        checkResult1Generic a kernel (fun _ -> expected)

    let checkResult2 (kernel:Kernel<_,_,_>) expected =
        let l = 4
        let a = [|0 .. l-1|]
        let b = Array.zeroCreate l
        let inBuf = new Buffer<int>(provider, Operations.ReadOnly, Memory.Device,a)
        let outBuf = new Buffer<int>(provider, Operations.ReadWrite, Memory.Device,b)
        let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
        let cq = commandQueue.Add(kernel.Run(new _1D(l,1), inBuf, outBuf)).Finish()
        let r = Array.zeroCreate l
        let cq2 = commandQueue.Add(outBuf.Read(0, l, r)).Finish()
        Assert.AreEqual(expected,r)
        commandQueue.Dispose()            


    [<Test>]
    member this.``Array item set``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1
            @>

        let kernel = provider.Compile command
        
        checkResult kernel [|1;1;2;3|]

    [<Test>]
    member this.Binding() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    buf.[0] <- x
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|1;1;2;3|]

    [<Test>]
    member this.``Binop plus``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1 + 2
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|3;1;2;3|]

    [<Test>]
    member this.``If Then``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|0;1;2;3|]

    [<Test>]
    member this.``If Then Else``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1 else buf.[0] <- 2
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|2;1;2;3|]

    [<Test>]
    member this.``For Integer Loop``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 1..3 do buf.[i] <- 0
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|0;0;0;0|]

    [<Test>]
    member this.``Sequential bindings``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    let y = x + 1
                    buf.[0] <- y
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|2;1;2;3|]

    [<Test>]
    member this.``Binding in IF.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 2 = 0
                    then                        
                        let x = 1
                        buf.[0] <- x
                    else
                        let i = 2
                        buf.[0] <- i
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|2;1;2;3|]

    [<Test>]
    member this.``Binding in FOR.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 0..3 do
                        let x = i * i
                        buf.[0] <- x
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|9;1;2;3|] 

    [<Test>]
    member this.``WHILE loop simple test.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                 while buf.[0] < 5 do
                     buf.[0] <- buf.[0] + 1
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|5;1;2;3|]

    [<Test>]
    member this.``WHILE in FOR.``() = 
        let command = 
            <@
                fun (range:_1D) (buf:array<int>) ->
                 for i in 0..3 do
                     while buf.[i] < 10 do
                         buf.[i] <- buf.[i] * buf.[i] + 1
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|26;26;26;10|]

    [<Test>]
    member this.``Binding in WHILE.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    while buf.[0] < 5 do
                        let x = buf.[0] + 1
                        buf.[0] <- x * x
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|25;1;2;3|]

    [<Test>]
    member this.``Simple 1D.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    let i = range.GlobalID0
                    buf.[i] <- i + i
            @>
        
        let kernel = provider.Compile command
        
        checkResult kernel [|0;2;4;6|]

    [<Test>]
    member this.``Simple 1D with copy.``() = 
        let command = 
            <@ 
                fun (range:_1D) (inBuf:array<int>) (outBuf:array<int>) ->
                    let i = range.GlobalID0
                    outBuf.[i] <- inBuf.[i]
            @>
        
        let kernel = provider.Compile command
        
        checkResult2 kernel [|0;1;2;3|]

    [<Test>]
    member this.``Simple 1D float.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<float32>) ->
                    let i = range.GlobalID0
                    buf.[i] <- buf.[i] * buf.[i]
            @>
        
        let kernel = provider.Compile command
        
        checkResultFloat kernel [|0.0;1.0;4.0;9.0|]

    [<Test>]
    member this.``Math sin``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<float32>) ->
                    let i = range.GlobalID0
                    buf.[i] <- float32(System.Math.Sin (float buf.[i]))
            @>
        
        let kernel = provider.Compile command

        let getExpected _ = [|0.0f; 0.841471f; 0.9092974f; 0.14112f|]
        
        checkResult1Generic [|0.0f;1.0f;2.0f;3.0f|] kernel getExpected

