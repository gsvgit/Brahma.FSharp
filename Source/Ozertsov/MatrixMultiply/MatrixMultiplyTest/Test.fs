open NUnit.Framework
open MatrixMultiply

[<TestFixture>]
type MatrixMulty() =    
    
    [<Test>]
    member this.test1() =
        let array1 = [|1; 0; 0; 1|]
        let array2 = [|1; 0; 0; 1|]
        
        Assert.AreEqual([|1; 0; 0; 1|], (MatrixMultiply.Main "NVIDIA*" [|1; 0; 0; 1|] [|1; 0; 0; 1|] 2 2))