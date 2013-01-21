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

    let checkCode command outFile expected =
        let code = ref ""
        let _ = provider.Compile(command,_outCode = code)
        printfn "%s" !code
        System.IO.File.WriteAllText(outFile,!code)
        filesAreEqual outFile (System.IO.Path.Combine(basePath,expected))

    let a = [|0..3|]

    [<Test>]
    member this.``Array item set``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 0
            @>

        checkCode command "Array.Item.Set.gen" "Array.Item.Set.ocl"

    [<Test>]
    member this.Binding() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    buf.[0] <- x
            @>

        checkCode command "Binding.gen" "Binding.ocl"

    [<Test>]
    member this.``Binop plus``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1 + 2
            @>
        
        checkCode command "Binop.Plus.gen" "Binop.Plus.ocl"

    [<Test>]
    member this.``If Then``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1
            @>

        checkCode command "If.Then.gen" "If.Then.ocl"

    [<Test>]
    member this.``If Then Else``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1 else buf.[0] <- 2
            @>

        checkCode command "If.Then.Else.gen" "If.Then.Else.ocl"

    [<Test>]
    member this.``For Integer Loop``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 1..3 do buf.[0] <- i
            @>

        checkCode command "For.Integer.Loop.gen" "For.Integer.Loop.ocl"

    [<Test>]
    member this.``Sequential bindings``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    let x = 1
                    let y = x + 1
                    buf.[0] <- y
            @>

        checkCode command "Sequential.Bindings.gen" "Sequential.Bindings.ocl"

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

        checkCode command "Binary.Operations.Math.gen" "Binary.Operations.Math.ocl"

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

        checkCode command "Binding.In.IF.gen" "Binding.In.IF.ocl"

    [<Test>]
    member this.``Binding in FOR.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    for i in 0..3 do
                        let x = i * i
                        buf.[0] <- x
            @>

        checkCode command "Binding.In.FOR.gen" "Binding.In.FOR.ocl"
       
    [<Test>]
    member this.``Simple WHILE loop.``() = 
        let command = 
            <@
                fun (range:_1D) (buf:array<int>) ->
                    while buf.[0] < 5 do
                        buf.[0] <- buf.[0] + 1
            @>

        checkCode command "Simple.WHILE.gen" "Simple.WHILE.ocl"

    [<Test>]
    member this.``Binding in WHILE.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                 while buf.[0] < 5 do
                     let x = buf.[0] + 1
                     buf.[0] <- x * x
            @>

        checkCode command "Binding.In.WHILE.gen" "Binding.In.WHILE.ocl"

    [<Test>]
    member this.``WHILE with complex condition.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                 while buf.[0] < 5 && (buf.[1] < 6 || buf.[2] > 2) do                     
                     buf.[0] <- 1
            @>

        checkCode command "WHILE.with.complex.condition.gen" "WHILE.with.complex.condition.cl"

    [<Test>]
    member this.``Simple seq.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    buf.[0] <- 2
                    buf.[1] <- 3
            @>

        checkCode command "Simple.Seq.gen" "Simple.Seq.cl"

    [<Test>]
    member this.``Seq with bindings.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    let x = 2
                    buf.[0] <- x
                    let y = 2
                    buf.[1] <- y
            @>

        checkCode command "Seq.With.Bindings.gen" "Seq.With.Bindings.cl"

    [<Test>]
    member this.``Bindings with equal names.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    let x = 2
                    buf.[0] <- x
                    let x = 3
                    buf.[1] <- x
            @>

        checkCode command "Bindings.With.Equal.Names.gen" "Bindings.With.Equal.Names.cl"

    [<Test>]
    member this.``Binding and FOR counter conflict 1.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    let i = 2
                    for i in 1..2 do     
                        buf.[1] <- i
            @>

        checkCode command "Binding.And.FOR.Counter.Conflict.1.gen" "Binding.And.FOR.Counter.Conflict.1.cl"

    [<Test>]
    member this.``Binding and FOR counter conflict 2.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    for i in 1..2 do
                        let i = 2     
                        buf.[1] <- i
            @>

        checkCode command "Binding.And.FOR.Counter.Conflict.2.gen" "Binding.And.FOR.Counter.Conflict.2.cl"

    [<Test>]
    member this.``Binding and FOR counter conflict 3.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    for i in 0..1 do
                        let i = i + 2
                        buf.[i] <- 2
            @>

        checkCode command "Binding.And.FOR.Counter.Conflict.3.gen" "Binding.And.FOR.Counter.Conflict.3.cl"

    [<Test>]
    member this.``Binding and FOR counter conflict 4.``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    let i = 1
                    for i in 0..i+1 do
                        let i = i + 2
                        buf.[i] <- 2
            @>

        checkCode command "Binding.And.FOR.Counter.Conflict.4.gen" "Binding.And.FOR.Counter.Conflict.4.cl"

    [<Test>]
    member this.``Quotations injections 1``() = 
        let myF = <@ fun x -> x * x @>
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    buf.[0] <- (%myF) 2
                    buf.[1] <- (%myF) 4
            @>

        checkCode command "Quotations.Injections.1.gen" "Quotations.Injections.1.cl"

    [<Test>]
    member this.``Quotations injections 2``() = 
        let myF = <@ fun x y -> x - y @>
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) ->
                    buf.[0] <- (%myF) 2 3
                    buf.[1] <- (%myF) 4 5
            @>

        checkCode command "Quotations.Injections.2.gen" "Quotations.Injections.2.cl"

[<EntryPoint>]
let f _ =
    (new Translator()).``Binding and FOR counter conflict 4.``()
    //(new Translator()).``Quotations injections 2``()
    //(new Brahma.FSharp.OpenCL.Full.Translator()).``Simple seq.``()
    0