namespace ControllerTests

open NUnit.Framework
open Controller

[<TestFixture>]
type ControllerTests () =
    [<Test>]
    member this.Test0 () =
        let c = new Controller<int> ()
        Assert.That (0, Is.EqualTo (0))