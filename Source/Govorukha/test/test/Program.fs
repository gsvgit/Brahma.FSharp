module UnitTest

open Example.Asm
open TTA.ASM
open NUnit.Framework


[<TestFixture>]
type Test() = 
     
     [<Test>]
     member this.``res``() =
        Assert.AreEqual(1, example) 


    