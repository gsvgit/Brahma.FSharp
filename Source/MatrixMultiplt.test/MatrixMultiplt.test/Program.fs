module MatrixMultiply.Tests

open NUnit.Framework
open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open MatrixMultiply

[<TestFixture>]
type MatMultTests() =

    [<Test>]
    member this.``test 1 ``() = 
        let m = MatrixMultiply.Main "NVIDIA*" [|2.0f; 4.0f; 4.0f; 2.0f|] [|1.0f; 2.0f; 2.0f; 1.0f|]
        let res = [|10.0f; 8.0f; 8.0f; 10.0f|]
        Assert.AreEqual(res, m, sprintf "Expected: %A, but get: %A " res m)
