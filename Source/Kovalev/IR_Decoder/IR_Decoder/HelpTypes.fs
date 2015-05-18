module HelpTypes

open QuickGraph
open System.Collections.Generic 
open System.Collections

//type EdgeType = Value | State | Predicate

//type Variable = 
//    | Int of int
//    | Bool of bool
    //| Array of Queue<array<int>>

type Node =
    | IntVar of IntV
    | Int of int ref
    | Bool of bool ref
    | Pred of Predicate
    | Inc of Increment
    | Div of Division
    | Gate of Multiplexer
    | NextIter of NestedGraph//AdjacencyGraph<Node, Edge<Node>>     

and IntV (var: Node) =
    member this.Out = match var with | Int v -> !v | _ -> failwith "incorrect port"
    
and Predicate (var: Node, op: int -> bool) =
    member this.Ports = match var with | Int v -> [!v] | _ -> failwith "incorrect port"
    member this.Out = match var with | Int v -> op !v | _ -> failwith "incorrect port"

and Increment (var: Node) =
    member this.Ports = match var with | Int v -> [!v] | _ -> failwith "incorrect port"
    member this.Out = match var with | Int v -> !v + 1 | _ -> failwith "incorrect port"

and Multiplexer (f: Node, p: Node, t: Node) =
    member this.Ports = match f, p, t with | Int f, Bool p, Int t -> [!f]         //p t
                                           | _ -> failwith "incorrect port"
    member this.Out = match f, p, t with | Int f, Bool p, Int t -> if !p then !t else !f
                                         | _ -> failwith "incorrect port"
    member this.Predicate = match p with | Bool v -> !v | _ -> failwith "incorrect port"

and Division (fst: Node, snd: Node) =
    member this.Ports = match fst, snd with | Int f, Int s -> [!f; !s] | _ -> failwith "incorrect port"
    member this.Out = match fst, snd with | Int f, Int s -> !f / !s | _ -> failwith "incorrect port"                   
        
and NestedGraph (g: AdjacencyGraph<Node, Edge<Node>>, port1: Node, port2: Node, port3: Node) =
    member this.Graph = g
    member this.Ports = match port1, port2, port3 with | Int f, Int s, Int t -> [!f; !s; !t] | _ -> failwith "incorrect port"

//and Int() =
//    [<DefaultValue>] val mutable Var : int

//and BoolVar() =
//    [<DefaultValue>] val mutable Var : bool

(*type Node<'T> =
    | Predicate of 'T
    | Op of 'T
    | State_in
    | State_out
    | Variable of Var
    | Block
    //| Multiplexer of 'T
    | NestedGraph of AdjacencyGraph<Node<'T>, TaggedEdge<Node<'T>, EdgeType>>*)

(*type Function = 
    | Inc of (int -> int)
    | GetElem of (int -> array<int> -> int)
    | Gate of (int -> bool -> int -> int)

//type GetElem (index: int, arr: array<int>) =
//    member val out = arr.[index] with get
//type Foo (var: int) =
//    member val out = 616 with get

int a = 10;
for (int i = 616; ; i > 0)
    i = i/10

type Block<'T> (port: Node<'T>, func: Function) =
    member val out = match func with
                     | Function.Inc op -> match port with
                                          | Variable v -> match v with
                                                          | Int intQueue -> op (intQueue.Dequeue())
                                                          | _ -> failwith "wrong variables in block1"
                                          | _ -> failwith "no"
                     | _ -> failwith "wrong op in block1"


type Block2<'T> (port1: Node<'T>, port2: Node<'T>, func: Function) =
    member val out = match func with
                     | Function.GetElem op -> match port1, port2 with
                                              | Variable v, Variable w -> match v, w with
                                                                          | Int intQueue, Array array -> op (intQueue.Dequeue()) (array.Dequeue())
                                                                          | _ -> failwith "wrong variables in block2"
                                              | _ -> failwith "ononono"
                     | _ -> failwith "wrong op in block2"

type Multiplexer<'T> (port1: Node<'T>, port2: Node<'T>, port3: Node<'T>, func: Function) =
    member val out = match func with
                     | Function.Gate op -> match port1, port2, port3 with
                                           | Variable v, Variable w, Variable x -> match v, w, x with
                                                                                   | Int intQueue, Bool boolQueue, Int intQueue2 ->
                                                                                       op (intQueue.Dequeue()) (boolQueue.Dequeue()) (intQueue2.Dequeue())
                                                                                   | _ -> failwith "wrong var in multiplexer"
                                           | _ ->  failwith "ononononono"
                     | _ -> failwith "wrong op in multiplexer"*)
                                                                                                                     
(*let graph = new AdjacencyGraph<int, Edge<int>> ()

graph.AddVertex(5) |> ignore
graph.AddVertex(3) |> ignore
graph.AddEdge(new Edge<int>(2,3)) |> ignore*)