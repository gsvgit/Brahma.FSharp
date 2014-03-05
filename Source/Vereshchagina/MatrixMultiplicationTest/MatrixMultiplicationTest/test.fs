open NUnit.Framework
open MatrixMultiply

[<TestFixture>]
type MatrixMultiplyTests() =    
    
    [<Test>]
    member this.test1() =
        let rows = 3 
        let columns = 3 
        let matrix1 = Array.init (rows * columns) (fun i -> i + 1)
        let matrix2 = Array.init (rows * columns) (fun i -> if (i % 4) = 0
                                                            then 1
                                                            else 0)
        Assert.AreEqual (matrix1, MatrixMultiply.Main matrix1 matrix2 3 3)