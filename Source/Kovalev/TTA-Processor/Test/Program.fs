open Processor
open NUnit.Framework

[<TestFixture>]
type ProcessorTests() =
    
    let config = new GridConfig(2, 3, [|((fun x y -> x * y), 2); ((fun x y -> x + y), 3)|])
    let processor = new Processor(config)

    [<Test>]
    member this.``CellConstuctedByFunction``() = 
        Assert.AreEqual(0, processor.ValueInCell 0 1)

    [<Test>]
    member this.``EmptyCell``() =
        Assert.AreEqual(0, processor.ValueInCell 1 2)