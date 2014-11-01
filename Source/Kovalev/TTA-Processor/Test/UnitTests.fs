module Tests

open Processor
open ASM
open NUnit.Framework

[<TestFixture>]
type ProcessorBuildingTests() =

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

[<TestFixture>]
type ProcessorRunTests() =
    
    [<Test>]
    member this.``RunEmptyProgram``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.Run [| [||]; [||] |]
        Assert.AreEqual (0, processor.NumberOfCells)

    [<Test>]
    member this.``RunSequentialASM``() =
        let processor = new Processor<_>([|(fun x y -> x * y); (fun x y -> x + y)|])        
        processor.Run [| [|Set((0<ln>, 0<col>), 6); Set((1<ln>, 1<col>), 3); 
                           Mvc((1<ln>, 1<col>), 4); Mov((0<ln>, 0<col>), (1<ln>, 1<col>))|] |]
        Assert.AreEqual (42, processor.ValueInCell 0 0)        

    [<Test>]
    member this.``RunSeveralPrograms``() =
        let processor = new Processor<_>([|(fun x y -> x * y); (fun x y -> x + y)|])
        processor.Run [| [|Set((0<ln>, 0<col>), 10); Set((1<ln>, 1<col>), 6); 
                           Mvc((1<ln>, 1<col>), 9); Mov((0<ln>, 0<col>), (1<ln>, 1<col>))|] |]      
        processor.Run [| [|Set((0<ln>, 0<col>), 6); Set((1<ln>, 1<col>), 3); 
                           Mvc((1<ln>, 1<col>), 4); Mov((0<ln>, 0<col>), (1<ln>, 1<col>))|] |]
        Assert.AreEqual (42, processor.ValueInCell 0 0) 

    [<Test>]
    member this.``RunParallelASM_WithDifferentColumnLenghts``() =
        let processor = new Processor<_>([|(fun x y -> x + y); (fun x y -> x + y)|])
        processor.Run [| [|Set((0<ln>, 0<col>), "I"); Mvc((0<ln>, 0<col>), " work ")|]; 
                         [|Set((0<ln>, 1<col>), "with"); Mvc((0<ln>, 1<col>), " strings")|];
                         [|Eps; Eps; Mov((0<ln>, 0<col>), (0<ln>, 1<col>))|] |]
        Assert.AreEqual ("I work with strings", processor.ValueInCell 0 0)        
        
    [<Test>]
    member this.``RunParallelASM_ReadWriteOneCellAtTheSameTime``() =
        let processor = new Processor<_>([|(fun x y -> x + y); (fun x y -> x + y)|])
        processor.Run [| [|Set((0<ln>, 0<col>), 1); Mov((0<ln>, 0<col>), (0<ln>, 1<col>))|]; 
                         [|Set((0<ln>, 1<col>), 2); Set((0<ln>, 1<col>), 10)|] |]
        Assert.AreEqual (3, processor.ValueInCell 0 0)
        
    [<Test>]
    member this.``RunParallelASM_DoubleWriteIntoTheCell``() =
        let processor = new Processor<_>([|(fun x y -> x + y)|])
        let writeException = Assert.Throws<DoubleWriteIntoCell>(fun () -> processor.Run [| [|Set((0<ln>, 0<col>), 1)|]; 
                                                                                           [|Set((0<ln>, 0<col>), 2)|] |] |> ignore)
        Assert.AreEqual(writeException, DoubleWriteIntoCell(0, 0))