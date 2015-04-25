module IR

open System
open QuickGraph
open HelpTypes
open FSharp.Control.Reactive
open FSharp.Control.Reactive.Builders
open System.Reactive.Linq

type Types =
    | Int of int
    | Bool of bool

type Block<'I, 'O> (input: IObservable<'I>, op: 'I -> 'O) =    
    member val Output = Observable.map op input with get

type Block<'I1, 'I2, 'O> (input1: IObservable<'I1>, input2: IObservable<'I2>, op: 'I1 * 'I2 -> 'O) =
    member val Output = Observable.zip input1 input2 |> Observable.map op with get

type Block<'I1, 'I2, 'I3, 'O> (input1: IObservable<'I1>, input2: IObservable<'I2>, input3: IObservable<'I3>, 
                               op: 'I1 * 'I2 * 'I3 -> 'O) =
    member val Output = Observable.zip3 input1 input2 input3 |> Observable.map op with get



type Block(inputs: list<IObservable<Types>>, op: Types -> Types -> Types) =    
    member val Output = Observable.zipSeqMap (fun list -> op list.[1] list.[0]) inputs
 
         

(*type VSFGraph<'T>() =

    let graph = new AdjacencyGraph<Node<'T>, TaggedEdge<Node<'T>, EdgeType>>()*)
    