open Processor
open NUnit.Framework
open TTA.ASM


type MyType(p: int) = 
    class
        let i = p
        member this.sout =
            printfn "%A" i
    end

type MyType1(p: int) =
    class  
        inherit MyType(p)
    end

[<TestFixture>]
type MatrixTests() =

    [<Test>]
    member this.``IntMatrix``() =
        let matrix = new Matrix<_>([|(fun x y -> x); (fun x y -> y)|])
        Assert.AreEqual (0, matrix.ValueInCell 0 1)
    
    [<Test>]
        member this.``MatrixColumns``() =
            let matrix = new Matrix<_>([|(fun x y -> x); (fun x y -> y)|])
            Assert.AreEqual (2, matrix.NumColls)

    [<Test>]
    member this.``MatrixRows``() =
        let matrix = new Matrix<_>([|(fun x y -> x); (fun x y -> y)|])
        matrix.ValueInCell 100 0 |> ignore
        Assert.AreEqual (101, matrix.NumRows)
    
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

        
    [<Test>]
    member this.``FunctionAnalisys``() =
        let matrix = new Matrix<int>([|fun x y -> x / y|])
        matrix.RunOp [|
                        [| Set ((0<ln>, 0<col>), 4)|]
                        [| Mvc((0<ln>, 0<col>), 0)|]
                        |]
        Assert.AreEqual (Unchecked.defaultof<array<string>>, matrix.ValueInCell 0 0)

//    
//    [<Test>]
//    member this.``InVarTest``() =
//        let matrix = new Matrix<MyType>([|(fun x y -> x); (fun x y -> y)|])
//        let matrix1 = new Matrix<MyType1>([|(fun x y -> x); (fun x y -> y)|])
//        let func (m:Matrix<MyType>) =
//            (m.ValueInCell 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType>, func ((Matrix<MyType>) matrix1))
//
//    [<Test>]
//    member this.``ContrVarTest``() =
//        let matrix = new Matrix<MyType>([|(fun x y -> x); (fun x y -> y)|])
//        let matrix1 = new Matrix<MyType1>([|(fun x y -> x); (fun x y -> y)|])
//        let func (m:Matrix<MyType1>) =
//            (m.ValueInCell 0 0).sout
//        Assert.AreEqual(Unchecked.defaultof<MyType1>, func  (matrix))

    [<Test>]
    member this.``SimpleTest``() =
        let matrix = new Matrix<_>([|fun x y -> x + y|])
        matrix.RunOp [| [| Set((1<ln>, 0<col>), 1) |];
                        [| Mvc((1<ln>, 0<col>), 2)|]
                        [| Mov((1<ln>, 0<col>), (0<ln>, 0<col>))|] 
                        [| Eps |]
                        |]

        Assert.AreEqual(3, matrix.ValueInCell 0 0)

    [<Test>]
    member this.``CorrectWorkFlowsTest``() =
        let matrix = new Matrix<_>([|fun x y -> x + y|])
        matrix.RunOp [| [| Set((1<ln>, 0<col>), 1); Eps |];
                        [| Mvc((1<ln>, 0<col>), 2)|]
                        [| Mov((1<ln>, 0<col>), (0<ln>, 0<col>))|] 
                        [| Eps |]
                        |]

        Assert.AreEqual(3, matrix.ValueInCell 0 0)
        
    [<Test>]
    member this.``WorkFlowsTest``() =
        let matrix = new Matrix<_>([|fun x y -> x + y|])
        matrix.RunOp [| [| Set((1<ln>, 0<col>), 1); Eps |];
                        [| Mvc((1<ln>, 0<col>), 2); Eps|]
                        [| Mov((1<ln>, 0<col>), (0<ln>, 0<col>)); Eps|] 
                        [| Eps; Eps |]
                        |]

        Assert.AreEqual(3, matrix.ValueInCell 0 0)

        
    [<Test>]
    member this.``WorkFlowsTest()``() =
        let matrix = new Matrix<_>([|fun x y -> x + y|])
        printf "fd"
        matrix.RunOp [| [| Set((14<ln>, 0<col>), 1); Eps |];
                        [| Mvc((1<ln>, 0<col>), 2); Eps|]
                        [| Mov((1<ln>, 0<col>), (0<ln>, 0<col>)); Eps|] 
                        [| Eps; Eps |]
                        |]

        Assert.AreEqual(3, matrix.ValueInCell 0 0)
        
    [<Test>]
    member this.``RealExampleTest``() =
        let matrix = new Matrix<_>([|(fun x y -> x + y); (fun x y  -> x * y)|])
        // examle 3 + 2 * 15 + 1;
        //board |+|*|
        matrix.RunOp [| [| Set((0<ln>, 0<col>), 3); Set((0<ln>, 1<col>), 2) |];
                        [| Mvc((0<ln>, 1<col>), 15) ; Mvc((0<ln>, 0<col>), 1)|]
                        [| Mov((0<ln>, 1<col>), (0<ln>, 0<col>)); Eps|] 
                        [| Eps; Eps |]
                        |]

        Assert.AreEqual(34, matrix.ValueInCell 0 0)