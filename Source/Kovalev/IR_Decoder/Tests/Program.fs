module Tests

open System
open System.Reactive.Linq
open FSharp.Control.Reactive
open Builders
open NUnit.Framework
open IR
open HelpTypes
open QuickGraph
(*
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
    member this.``arithmeticExpression`` () =
        //(2 + 3) * (2 - 7)

        let channel2 = Observable.Return 2
        let channel3 = Observable.Return 3        
        let channel7 = Observable.Return 7

        let addition = new Block<int, int, int> (channel2, channel3, fun (x, y) -> x + y)
        let subtraction = new Block<int, int, int> (channel2, channel7, fun (x, y) -> x - y)
        let multiplication = new Block<int, int, int> (addition.Output, subtraction.Output, fun (x, y) -> x * y)

        let actual = ref 0
        multiplication.Output.Subscribe (fun x -> actual := x) |> ignore

        Assert.AreEqual (-25, !actual)
    
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
        
        Assert.AreEqual (84, !actual) *)

let graph = new AdjacencyGraph<Node, Edge<Node>> ()

let i = Int (ref 616)
let a = Int (ref 10)
let c = Int (ref 0)

let portDiv1 = Int (ref 1)
let portDiv2 = Int (ref 2)
let portInc = Int (ref 3)
let portPredicate = Int (ref 4)
let portNextIter1 = Int (ref 5)
let portNextIter2 = Int (ref 6)
let portNextIter3 = Int (ref 7)
let portMult1 = Int (ref 8)
let portMult2 = Bool (ref false)
let portMult3 = Int (ref 9)

let div = Div (Division (portDiv1, portDiv2))
let inc = Inc (Increment (portInc))
let pred = Pred (Predicate (portPredicate, fun x -> if x > 0 then true else false))
let mult = Gate (Multiplexer (portMult1, portMult2, portMult3))
let nextIter = NestedGraph (graph)

graph.AddVerticesAndEdgeRange ([
                                 Edge<Node> (i, portDiv1); Edge<Node> (i, div);
                                 Edge<Node> (a, portDiv2); Edge<Node> (a, portNextIter2); Edge<Node> (a, div); Edge<Node> (a, nextIter);
                                 Edge<Node> (c, portMult1); Edge<Node> (c, portInc); Edge<Node> (c, mult); Edge<Node> (c, inc) 
                                 Edge<Node> (div, portPredicate); Edge<Node> (div, portNextIter1); Edge<Node> (div, pred); Edge<Node> (div, nextIter)
                                 Edge<Node> (inc, portNextIter3); Edge<Node> (inc, nextIter)
                                 Edge<Node> (pred, portMult2); Edge<Node> (pred, mult);
                                 Edge<Node> (nextIter, portMult3);
                                 Edge<Node> (portNextIter1, i); Edge<Node> (portNextIter2, a); Edge<Node> (portNextIter3, c);
                              ]) |> ignore

printfn "%A" graph.VertexCount