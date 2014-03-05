open NUnit.Framework
open MatrixMultiply

[<TestFixture>]
type MatrixMultiplyTests() =    
    
    [<Test>]
    member this.``test1``() =
        let rows = 5 
        let columns = 5 
        let matrix = Array.init (rows * columns) (fun i -> rows)
        Assert.AreEqual (matrix, MatrixMultiply.Main "NVIDIA*" 5 5)
