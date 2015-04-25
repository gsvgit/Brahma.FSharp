module Tests

open System
open System.Reactive.Linq
open FSharp.Control.Reactive
open Builders
open NUnit.Framework
open IR

(*let x = new Block<int, int> (Observable.Return 42, fun x -> x + 1)

let x2 = new Block<int, bool> (x.Output, fun x -> if x % 2 <> 0 then true else false)*)

(*let y = x2.Output
y.Subscribe(fun x -> x.ToString() |> printfn "%A") |> ignore*)

let x3 = new Block ([Observable.Return (Types.Int 42); Observable.Return (Types.Int 42)], fun x y -> match x, y with 
                                                                                                     Types.Int a, Types.Int b -> Types.Int (a + b)
                                                                                                    | _ -> failwith "hello" )

x3.Output.Subscribe(fun x -> match x with Types.Int a -> a.ToString() |> printfn "%A" | _ -> failwith "") |> ignore