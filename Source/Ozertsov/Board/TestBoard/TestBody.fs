open Processor
open NUnit.Framework

[<TestFixture>]
type MatrixTests() =

    [<Test>]
    member this.``IntMatrix``() =
        let matrix = new Matrix<_>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0, matrix.ValueInCell 0 1)

    [<Test>]
    member this.``BoolMatrix``() =
        let matrix = new Matrix<bool>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (false, matrix.ValueInCell 0 0)

    [<Test>]
    member this.``ByteMatrix``() =
        let matrix = new Matrix<byte>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0uy, matrix.ValueInCell 0 1)

    [<Test>]
    member this.``ArrayOfStringMatrix``() =
        let matrix = new Matrix<array<string>>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (Unchecked.defaultof<array<string>>, matrix.ValueInCell 0 0)