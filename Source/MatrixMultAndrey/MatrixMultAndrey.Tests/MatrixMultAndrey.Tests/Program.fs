module MatrixMultAndrey.Tests
open MatrixMultAndrey
open NUnit.Framework
[<TestFixture>]
type MatrixMultAndrey() =
    
    [<Test>]
    member this.``test 1 ``() =
        let arr = Array.init 4 (fun i -> 1)
        let check = MatrixMultAndrey.Main arr arr 2 2 
        //let check2 = [||]
        Assert.IsNotEmpty(check, "Fail")