module Brahma.FSharp.OpenCL.Tests

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
    
    let basePath = "../../../../Tests/Brahma.FSharp.OpenCL/Translator/Expected/"

    let deviceType = Cl.DeviceType.Default
    let platformName = "*"

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head );

    let filesAreEqual file1 file2 =
        let all1 = File.ReadAllBytes file1
        let all2 = File.ReadAllBytes file2
        Assert.AreEqual (all1.Length, all2.Length)
        Assert.IsTrue(Array.forall2 (=) all1 all2)

    let checkCode (kernel:Kernel<_1D,_>) outFile expected =
        (kernel :> ICLKernel).Source.ToString() |> (fun text -> printfn "%s" text;  System.IO.File.WriteAllText(outFile,text))
        filesAreEqual outFile (System.IO.Path.Combine(basePath,expected))

    let a = [|0..3|]

    [<Test>]
    member this.``Array item set``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 0
            @>
        
        let kernel = provider.Compile command
        
        checkCode kernel "Array.Item.Set.gen" "Array.Item.Set.ocl"

    [<Test>]
    member this.Binding() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    buf.[0] <- x
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "Binding.gen" "Binding.ocl"

    [<Test>]
    member this.``Binop plus``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1 + 2
            @>
        
        let kernel = provider.Compile command
        
        checkCode kernel "Binop.Plus.gen" "Binop.Plus.ocl"

    [<Test>]
    member this.``If Then``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "If.Then.gen" "If.Then.ocl"

    [<Test>]
    member this.``If Then Else``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1 else buf.[0] <- 2
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "If.Then.Else.gen" "If.Then.Else.ocl"

    [<Test>]
    member this.``For Integer Loop``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 1..3 do buf.[0] <- i
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "For.Integer.Loop.gen" "For.Integer.Loop.ocl"

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
        
        checkCode kernel "Sequential.Bindings.gen" "Sequential.Bindings.ocl"

    [<Test>]
    member this.``Binary operations. Math.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 0
                    let y = x + 1
                    let z = y * 2
                    let a = z - x
                    let i = a / 2
                    buf.[0] <- i
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "Binary.Operations.Math.gen" "Binary.Operations.Math.ocl"

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
        
        checkCode kernel "Binding.In.IF.gen" "Binding.In.IF.ocl"

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
        
        checkCode kernel "Binding.In.FOR.gen" "Binding.In.FOR.ocl"
       
    [<Test>]
    member this.``Simple WHILE loop.``() = 
        let command = 
            <@
                fun (range:_1D) (buf:array<int>) ->
                    while buf.[0] < 5 do
                        buf.[0] <- buf.[0] + 1
            @>

        let kernel = provider.Compile command
        
        checkCode kernel "Simple.WHILE.gen" "Simple.WHILE.ocl"

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
        
        checkCode kernel "Binding.In.WHILE.gen" "Binding.In.WHILE.ocl"

[<EntryPoint>]
let f _ =
    //(new Translator()).``Binding in WHILE.``()
    (new Brahma.FSharp.OpenCL.Full.Translator()).``Math sin``()
    0