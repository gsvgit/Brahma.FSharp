open NUnit.Framework
open MatrixMultiply

[<TestFixture>]
type MatrixMulty() =    
    
    [<Test>]
    member this.test1() =
              
        Assert.AreEqual([|7; 10; 15; 22|], (MatrixMultiply.main "NVIDIA*" [|1; 2; 3; 4|] [|1; 2; 3; 4|] 2 2))

    [<Test>]
    member this.test2() =
              
        Assert.AreEqual([|0; 0; 0; 0|], (MatrixMultiply.main "NVIDIA*" [|1; 2; 3; 4|] [|0; 0; 0; 0|] 2 2))

    [<Test>]
    member this.test3() =

        let c = MatrixMultiply.main "NVIDIA*" [|1; 1; 1; 1|] [|-1; -1; -1; -1|] 2 2

        Assert.IsTrue (c.[1] < 0)

