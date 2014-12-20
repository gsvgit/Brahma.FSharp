open TTA.Processor
open TTA.ASM
open TTA.Parser
open NUnit.Framework

let processor = new Processor<_>([|(fun x y -> x + y); (fun x y -> x + y)|])
for i in 0..100 do
    processor.Run [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 2)|]; 
                     [|Set((0<ln>, 1<col>), 3); Mvc((0<ln>, 1<col>), 4)|];
                     [|Eps|]|]
         
    processor.ValueInCell 0 0 |> printfn "%A"


[<TestFixture>]
type ProcessorTests() =
    
    [<Test>]
    member this.``RealExampleTest``() =
        let processor = new Processor<_>([|(fun x y -> x + y); (fun x y  -> x * y)|])
        processor.Run [| [| Set((0<ln>, 0<col>), 3); Set((0<ln>, 1<col>), 2) |];
                         [| Mvc((0<ln>, 1<col>), 15) ; Mvc((0<ln>, 0<col>), 1)|]
                         [| Mov((0<ln>, 0<col>), (0<ln>, 1<col>)); Eps|] 
                         [| Eps; Eps |]
                      |]

        Assert.AreEqual(34, processor.ValueInCell 0 0)

    [<Test>]
    member this.``ParralelInCol``() =
        let processor = new Processor<_>([|(fun x y -> x + y); (fun x y -> x + y)|])
        for i in 0..100 do
            processor.Run [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 2)|]; 
                             [|Set((0<ln>, 1<col>), 3); Mvc((0<ln>, 1<col>), 4)|];
                             [|Eps|]|]
         
        Assert.AreEqual(0, processor.ValueInCell 0 0)
    
    //проверка вариантности коллекций

    [<Test>]
    member this.``IntTest``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0, processor.ValueInCell 0 1)
    
 
    [<Test>]
    member this.``BoolTest``() =
        let processor = new Processor<bool>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (false, processor.ValueInCell 0 0)

    [<Test>]
    member this.``ByteTest``() =
        let processor = new Processor<byte>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0uy, processor.ValueInCell 0 1)

    [<Test>]
    member this.``ArrayOfStringTest``() =
        let processor = new Processor<array<string>>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (Unchecked.defaultof<array<string>>, processor.ValueInCell 0 0)