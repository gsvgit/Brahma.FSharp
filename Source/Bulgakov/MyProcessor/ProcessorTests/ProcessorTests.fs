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
        let ex = Assert.Throws<ParallelException>(fun() -> processor.executeArray [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 5)|] |] |> ignore)
        Assert.AreEqual(ParallelException(0,0), ex)

[<TestFixture>]
type ProcessorRunTests() =    
    [<Test>]
    member this.``EmptyArray``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.executeArray [| [||]; [||] |]
        Assert.AreEqual (0, processor.ValueAt 1024 1)
    
    [<Test>]
    member this.``OneArr``() =
        let processor = new Processor<_>([|(fun x y -> x); (fun x y -> y)|])        
        processor.executeArray [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 1<col>), 5)|] |]
        Assert.AreEqual (0, processor.ValueAt 1024 1)

    [<Test>]
    member this.``ParallelTest``() =
        let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x + y)|])
        processor.executeArray [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 1<col>), 5)|]; 
                         [|Set((0<ln>, 0<col>), 2); Mvc((0<ln>, 1<col>), 3)|];
                         [|Mov((0<ln>, 0<col>), (0<ln>, 1<col>))|]  |]
        Assert.AreEqual (10, processor.ValueAt 0 0)