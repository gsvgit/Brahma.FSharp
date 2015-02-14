open TTA.ASM
open NUnit.Framework
open Board

[<TestFixture>]
type CreateProcessor() =

    [<Test>]
    member this.``GridColumns``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (2, processor.NumberOfColls)

    [<Test>]
    member this.``GridRows``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])
        processor.ValueInCell 55 0 |> ignore
        Assert.AreEqual (56, processor.NumberOfRows)
            
    [<Test>]
    member this.``AbstractGrid``() =        
        let processor = new Processor<_>([|(fun x y -> x); (fun x y-> y);|])
        Assert.AreEqual(0, processor.ValueInCell 0 0)    

    [<Test>]
    member this.``BoolGrid``() =        
        let processor = new Processor<_>([|(fun x y -> x||y); (fun x y-> x&&y);|])
        Assert.AreEqual(false, processor.ValueInCell 0 0)

    [<Test>]
    member this.``ArrayOfStringGrid``() =        
        let processor = new Processor<array<string>>([|(fun x y -> x); (fun x y-> y);|])
        Assert.AreEqual(null, processor.ValueInCell 0 0)

    [<Test>]
    member this.``ByteGrid``() =        
        let processor = new Processor<byte>([|(fun x y -> x + y); (fun x y-> x * y);|])
        Assert.AreEqual(0uy, processor.ValueInCell 0 0)

    [<Test>]
    member this.``IntGrid``() =        
        let processor = new Processor<int>([|(fun x y -> x + 1); (fun x y-> x * x);|])
        Assert.AreEqual(0, processor.ValueInCell 0 0) 

[<TestFixture>]
type ExceptionsTests() =

    [<Test>]
    member this.``OutOfBounds``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y-> x * x);|])
        let ex = Assert.Throws<SomeException>(fun() -> processor.ExecuteProgram [| [|Mvc((0<ln>, 2<col>), 1)|] |] |> ignore)
        Assert.AreEqual(SomeException, ex)

    [<Test>]
    member this.``ParallelException``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y-> x * x);|])
        let ex = Assert.Throws<SomeException>(fun() -> processor.ExecuteProgram [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 1)|] |] |> ignore)
        Assert.AreEqual(SomeException, ex)   

[<TestFixture>]
type ExecutionTests() =    

    [<Test>]
    member this.``EmptyGrid``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.ExecuteProgram [| [||]; [||] |]
        Assert.AreEqual (0, processor.ValueInCell 100 0)

    [<Test>]
    member this.``SimpleExecutionTest``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x * y)|])
        processor.ExecuteProgram [| 
            [|Set((0<ln>, 0<col>), 1)|]; 
            [|Set((0<ln>, 0<col>), 1)|];
            [|Set((0<ln>, 1<col>), 2)|];
            [|Mvc((0<ln>, 0<col>), 3)|];
            [|Mvc((0<ln>, 1<col>), 4)|];
            [|Mov((0<ln>, 0<col>), (0<ln>, 1<col>))|] 
                |]
        Assert.AreEqual (12, processor.ValueInCell 0 0)
    
    [<Test>]
    member this.``ParallelExecutionTest``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x * y)|])
        processor.ExecuteProgram [| 
            [|Set((0<ln>, 0<col>), 1); Set((0<ln>, 1<col>), 2)|]; 
            [|Mvc((0<ln>, 0<col>), 3); Mvc((0<ln>, 1<col>), 4)|];
            [|Mov((0<ln>, 0<col>), (0<ln>, 1<col>)); Eps|] 
                |]
        Assert.AreEqual (12, processor.ValueInCell 0 0)

    [<Test>]
    member this.``RealyFullGrid``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])
        processor.ExecuteProgram [|
            [|Set((100<ln>, 0<col>), 1); Set((100<ln>, 1<col>), 5); Set ((100<ln>, 3<col>), 8)|]; 
            [|Mvc((100<ln>, 0<col>), 3); Mvc((100<ln>, 1<col>), 2); Set ((100<ln>, 2<col>), 4)|];
            [|Mov((100<ln>, 3<col>), (100<ln>, 0<col>)); Mov((100<ln>, 2<col>), (100<ln>, 1<col>)); Eps|];
            [|Mov((101<ln>, 0<col>), (100<ln>, 3<col>)); Eps; Eps|];  
            [|Mov((101<ln>, 0<col>), (100<ln>, 2<col>)); Eps; Eps|];             
                |]          
        Assert.AreEqual(14, processor.ValueInCell 101 0)

    
//type MyType(p: int) = 
//    class
//        let i = p
//        member this.sout =
//            printfn "%A" i
//    end
//
//type MyType1(p: int) =
//    class 
//        inherit MyType(p)
//    end
//[<TestFixture>]
//type Variant() = 
//     [<Test>]
//     member this.``InVarTest``() =
//        let Processor = new Processor<MyType>([|(fun x y -> x); (fun x y -> y)|])
//        let Processor1 = new Processor<MyType1>([|(fun x y -> x); (fun x y -> y)|])
//        let func (m:Processor<MyType>) =
//            (m.ValueInCell 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType>, func((Processor<MyType>) Processor1))
//
//     [<Test>]
//     member this.``ContrVarTest``() =
//        let Processor = new Processor<MyType>([|(fun x y -> x); (fun x y -> y)|])
//        let Processor1 = new Processor<MyType1>([|(fun x y -> x); (fun x y -> y)|])
//        let func (m:Processor<MyType1>) =
//            (m.ValueInCell 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType1>, func (Processor))