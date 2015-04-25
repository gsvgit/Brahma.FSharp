module Tests

open System
open System.Reactive.Linq
open FSharp.Control.Reactive
open Builders
open NUnit.Framework
open IR


[<TestFixture>]
type TestBlocks () =
    [<Test>]
    member this.``simpleIncrement`` () =
        let value = Observable.Return 1
        
        let inc = new Block<int, int> (value, fun x -> x + 1)        

        let actual = ref 0
        inc.Output.Subscribe (fun x -> actual := x) |> ignore

        Assert.AreEqual (2, !actual)
    
    [<Test>]
    member this.``workWithPredicate`` () =
        let first = Observable.Return 1
        let second = Observable.Return 2

        let predicate = new Block<int, bool> (second, fun x -> if x % 2 = 0 then true else false)
        let usefulFunc = new Block<int, int, bool, int> (first, second, predicate.Output, fun (x, y, b) -> if b then x + y else x * y)

        let actual = ref 0
        usefulFunc.Output.Subscribe (fun x -> actual := x) |> ignore

        Assert.AreEqual (3, !actual)


    [<Test>]
    member this.``PlsNoOhMyEyesICan'tSee`` () =
        let add = new Block ([Observable.Return (Types.Int 42); Observable.Return (Types.Int 42)], fun x y -> match x, y with 
                                                                                                        Types.Int a, Types.Int b -> Types.Int (a + b)
                                                                                                        | _ -> failwith "hello" )        
        let actual = ref 0
        add.Output.Subscribe(fun x -> match x with Types.Int a -> actual := a | _ -> failwith "") |> ignore
        
        Assert.AreEqual (84, !actual)        