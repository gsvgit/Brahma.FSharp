module NUnitTests

open TTA.ASM
open NUnit.Framework
open Processor
  
[<TestFixture>]
type CreateProcessor() =
            
   [<Test>]
    member this.``Create abstract processor``() =        
        let processor = new Processor<_>([|(fun x y -> x); (fun x y-> y);|])
        Assert.AreEqual(0, processor.ValueAt 186 0)
    [<Test>]
    member this.``Create integer processor``() =        
        let processor = new Processor<int>([|(fun x y -> x+1); (fun x y-> x*x);|])
        Assert.AreEqual(0, processor.ValueAt 186 0)
    [<Test>]
    member this.``Create boolean processor``() =        
        let processor = new Processor<_>([|(fun x y -> x||y); (fun x y-> x&&y);|])
        Assert.AreEqual(false, processor.ValueAt 186 0)
    [<Test>]
    member this.``Create byte processor``() =        
        let processor = new Processor<byte>([|(fun x y -> x+y); (fun x y-> x*y);|])
        Assert.AreEqual(0uy, processor.ValueAt 186 0)
    [<Test>]
    member this.``Create array<string> processor``() =        
        let processor = new Processor<array<string>>([|(fun x y -> x); (fun x y-> y);|])
        Assert.AreEqual(null, processor.ValueAt 186 0)

[<TestFixture>]
type ExceptionTests() =
    [<Test>]
    member this.``Parallel_Exception_In_Line``() =
        let processor = new Processor<int>([|(fun x y -> x+y); (fun x y-> x*x);|])
        let ex = Assert.Throws<ParallelException>(fun() -> processor.executeProgram [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 5)|] |] |> ignore)
        Assert.AreEqual(ParallelException(0,0), ex)

    [<Test>]
    member this.``Out of Bounds``() =
        let processor = new Processor<int>([|(fun x y -> x+y); (fun x y-> x*x);|])
        let ex = Assert.Throws<IndexOutOfBounds>(fun() ->processor.executeProgram [| [|Mvc((0<ln>, 2<col>), 5)|] |] |> ignore)
        Assert.AreEqual(IndexOutOfBounds(0,2), ex)

[<TestFixture>]
type ProcessorRunTests() =    
    [<Test>]
    member this.``EmptyArray``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.executeProgram [| [||]; [||] |]
        Assert.AreEqual (0, processor.ValueAt 1024 1)
    
    [<Test>]
    member this.``OneArr``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.executeProgram [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 1<col>), 5)|] |]
        Assert.AreEqual (0, processor.ValueAt 1024 1)
    
    [<Test>]
    member this.``ParallelTest``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x + y)|])
        processor.executeProgram [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 1<col>), 5)|]; 
                         [|Set((0<ln>, 0<col>), 2); Mvc((0<ln>, 1<col>), 3)|];
                         [|Mov((0<ln>, 0<col>), (0<ln>, 1<col>))|]  |]
        Assert.AreEqual (10, processor.ValueAt 0 0)

    [<Test>]
    member this.``100500``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])
        processor.executeProgram [| [|Set((100<ln>, 2<col>), 1);|] |]
        Assert.AreEqual(1, processor.ValueAt 100 2)
    
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
//            (m.ValueAt 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType>, func((Processor<MyType>) Processor1))
//
//     [<Test>]
//     member this.``ContrVarTest``() =
//        let Processor = new Processor<MyType>([|(fun x y -> x); (fun x y -> y)|])
//        let Processor1 = new Processor<MyType1>([|(fun x y -> x); (fun x y -> y)|])
//        let func (m:Processor<MyType1>) =
//            (m.ValueAt 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType1>, func (Processor))
