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
        
        let outFile = "Array.Item.Set.gen"

        (kernel :> ICLKernel).Source.ToString() |> (fun text -> System.IO.File.WriteAllText(outFile,text))
        filesAreEqual "Array.Item.Set.gen" (System.IO.Path.Combine(basePath,"Array.Item.Set.ocl"))

//[<EntryPoint>]
//let f _ =
//    (new Translator()).``Array item set``()
//    0