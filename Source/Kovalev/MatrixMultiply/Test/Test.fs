module Test

open NUnit.Framework

[<TestFixture>]
type MultiplyTest() = 
   
    [<Test>]
    member this.``test1``() =        
        let nullMatrix = Array.zeroCreate 25
        let random = new System.Random()
        let matrix = Array.init 25 (fun i -> random.Next(42))
        Assert.AreEqual (nullMatrix, (MatrixMultiply.main nullMatrix matrix))
