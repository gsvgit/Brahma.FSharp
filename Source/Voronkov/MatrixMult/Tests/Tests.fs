module Tests

open NUnit.Framework

[<TestFixture>]
type MatrixMultiplyTests() = 

    [<Test>]
    member this.``Test №1 (Multiply by zero matrix)``() = 
        let a = Array.zeroCreate 16
        let random = new System.Random()
        let b = [|for i in 0..15 -> random.Next(30)|]
        Assert.AreEqual (a, MatrixMultiply.Main a b)

    [<Test>]
    member this.``Test №2 (Multiply by identity matrix)``() = 
        let a = [|for i in 0..15 -> if i = 0 || i = 5 || i = 10 || i = 15
                                    then 1
                                    else 0|]
        let random = new System.Random()
        let b = [|for i in 0..15 -> random.Next(40)|]
        Assert.AreEqual (b, MatrixMultiply.Main a b)