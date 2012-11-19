module Brahma.FSharp.OpenCL.Full

open NUnit.Framework
open System.IO
open Brahma.Types
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


    let checkResult (kernel:Kernel<_,_>) expected =
        let l = 4
        let a = [|0 .. l-1|]
        let aBuf = new Buffer<int>(provider, Operations.ReadWrite, Memory.Device,a)
        let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head );
        let cq = commandQueue.Add(kernel.Run(new _1D(l,1), aBuf)).Finish()
        let r = Array.zeroCreate(l)
        let cq2 = commandQueue.Add(aBuf.Read(0, l, r)).Finish()
        Assert.AreEqual(expected,r)
        commandQueue.Dispose();
        

    [<Test>]
    member this.``Array item set``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|1;1;2;3|]

    [<Test>]
    member this.Binding() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    buf.[0] <- x
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|1;1;2;3|]

    [<Test>]
    member this.``Binop plus``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1 + 2
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|3;1;2;3|]

    [<Test>]
    member this.``If Then``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|0;1;2;3|]

    [<Test>]
    member this.``If Then Else``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1 else buf.[0] <- 2
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|2;1;2;3|]

    [<Test>]
    member this.``For Integer Loop``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 1..3 do buf.[i] <- 0
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
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

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
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

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
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

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkResult kernel [|9;1;2;3|]