module ArraySum.Tests

open NUnit.Framework

[<TestFixture>]
type ArraySumTests() =
    
    [<Test>]
    member this.``test 1 ``() =
        let arr = Array.init length (fun i -> 1)
        arr.[10] <- 2
        ArraySum.Main arr
        let checkRes = arr |> Array.forall (fun x -> x = 2)
        Assert.IsTrue(checkRes, sprintf "Item %A <> 2" (Array.tryFindIndex (fun x -> x <> 2) arr))
