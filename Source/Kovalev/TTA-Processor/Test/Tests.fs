module Tests

open Processor
open NUnit.Framework

[<TestFixture>]
type ProcessorTests() =

    [<Test>]
    member this.``BuildingForInt``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0, processor.ValueInCell 1024 1)

    [<Test>]
    member this.``BuildingForBool``() =
        let processor = new Processor<bool>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (false, processor.ValueInCell 2048 0)

    [<Test>]
    member this.``BuildingForByte``() =
        let processor = new Processor<byte>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0uy, processor.ValueInCell 4096 1)

    [<Test>]
    member this.``BuildingForStringArray``() =
        let processor = new Processor<array<string>>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (Unchecked.defaultof<array<string>>, processor.ValueInCell 0 0)