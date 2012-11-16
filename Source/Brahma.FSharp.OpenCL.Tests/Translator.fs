module Brahma.FSharp.OpenCL.Tests

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

    let checkCode kernel outFile expected =
        (kernel :> ICLKernel).Source.ToString() |> (fun text -> printfn "%s" text;System.IO.File.WriteAllText(outFile,text))
        filesAreEqual outFile (System.IO.Path.Combine(basePath,expected))

    let a = [|0..3|]

    [<Test>]
    member this.``Array item set``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 0
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkCode kernel "Array.Item.Set.gen" "Array.Item.Set.ocl"

        

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
        
        checkCode kernel "Binding.gen" "Binding.ocl"

    [<Test>]
    member this.``Binop plus``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    buf.[0] <- 1 + 2
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkCode kernel "Binop.Plus.gen" "Binop.Plus.ocl"

    [<Test>]
    member this.``If Then``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkCode kernel "If.Then.gen" "If.Then.ocl"

    [<Test>]
    member this.``If Then Else``() = 
        let command = 
            <@ 
                fun (range:_1D) (buf:array<int>) -> 
                    if 0 = 2 then buf.[0] <- 1 else buf.[0] <- 2
            @>

        let c = command:>Expr
        let kernel = provider.Compile<_1D,_> c
        
        checkCode kernel "If.Then.Else.gen" "If.Then.Else.ocl"
//[<EntryPoint>]
//let f _ =
//    (new Translator()).``Array item set``()
//    0