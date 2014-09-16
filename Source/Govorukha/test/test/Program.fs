open arr
open TTA.ASM
open NUnit.Framework


[<TestFixture>]
type Test() = 
     
     [<Test>]
     member this.``res``() =
        Assert.AreEqual(1, func 10 2 3 4 1 20 7 ) 


     //[<Test>]
     //member this.``res2``() =
      //  Assert.AreEqual(-14, func 1 3 4 -3 1 45 10 )