module Tests

open System
open System.Reactive.Linq
open System.Collections.Generic
open System.Threading.Tasks
open System.Threading
open FSharp.Control.Reactive
open Builders
open NUnit.Framework
open IR
open HelpTypes
open QuickGraph


let graph = new AdjacencyGraph<Node, Edge<Node>> ()

let i = Int (ref 616)
let a = Int (ref 10)
let c = Int (ref 0)

let portDiv1 = Int (ref -1)
let portDiv2 = Int (ref -2)
let portInc = Int (ref -3)
let portPredicate = Int (ref -4)
let portNextIter1 = Int (ref -5)
let portNextIter2 = Int (ref -6)
let portNextIter3 = Int (ref -7)
let portMult1 = Int (ref -8)
let portMult2 = Bool (ref false)
let portMult3 = Int (ref -9)

let div = Div (Division (portDiv1, portDiv2))
let inc = Inc (Increment (portInc))
let pred = Pred (Predicate (portPredicate, fun x -> if x > 0 then true else false))
let mult = Gate (Multiplexer (portMult1, portMult2, portMult3))
let nextIter = NextIter (NestedGraph (graph, portNextIter1, portNextIter2, portNextIter3))

graph.AddVerticesAndEdgeRange ([
                                 Edge<Node> (i, portDiv1); Edge<Node> (i, div);
                                 Edge<Node> (a, portDiv2); Edge<Node> (a, portNextIter2); Edge<Node> (a, div); Edge<Node> (a, nextIter);
                                 Edge<Node> (c, portMult1); Edge<Node> (c, portInc); Edge<Node> (c, mult); Edge<Node> (c, inc) 
                                 Edge<Node> (div, portPredicate); Edge<Node> (div, portNextIter1); Edge<Node> (div, pred); Edge<Node> (div, nextIter)
                                 Edge<Node> (inc, portNextIter3); Edge<Node> (inc, nextIter)
                                 Edge<Node> (pred, portMult2); Edge<Node> (pred, mult);
                                 Edge<Node> (nextIter, portMult3);
                                 Edge<Node> (portNextIter1, i); 
                                 Edge<Node> (portNextIter2, a); 
                                 Edge<Node> (portNextIter3, c);
                              ]) |> ignore

let rec decode (graph: AdjacencyGraph<Node, Edge<Node>>) (startVertices: list<Node>) = 

    (*
        Обход в ширину. Начинается с набора стартовых вершин. Вершины добавляются в очередь. 
        Хотел использовать просто set, чтобы решить проблему с повторным включением вершины, но Node не реализует IComparable.
        IEquatable он тоже не реализует, поэтому не получится использовать метод contains для проверки или что-нибудь подобное 
        у коллекций.
        Так что (не)хитрый kostyl - массив флагов, "лежит ли уже данная вершина в очереди". Таблица соответсвия флагов вершинам: 

        a i c div inc pred mult nextIter
        0 1 2  3   4    5    6     7
    *)

    let kostyl = Array.create 8 false    //because Node doesn't implement IComparable or IEquatable
    kostyl.[0] <- true                   //a, i, c - стартовые
    kostyl.[1] <- true
    kostyl.[2] <- true

    let front = new Queue<Node> ()       //та самая очередь
    for n in startVertices do
        front.Enqueue (n)
    
    let tokenSource = new CancellationTokenSource();           //всякие штуки для отмены task, не знаю, как оно работает на самом деле
    let ct = tokenSource.Token;                                //делал как в примере на msdn

    let task = new Task<int> ((fun () -> decode graph startVertices), tokenSource.Token) //создаем предварительно task, чтобы потом
                                                                                         //в разных узлах запускать\требовать результат

    let mutable out = -100        //просто чтобы возвращать значение из decode

    printfn "*"

    let tryEnqueue i node =
        if kostyl.[i]
        then ()
        else 
            kostyl.[i] <- true
            front.Enqueue (node)
    
    Thread.Sleep (2000)                   //это чтобы дочерний поток не успевал дойти до nextIter (не знаю, работает ли это)
    ct.ThrowIfCancellationRequested()     //убийство (оно находится только в этой точке, потому что ... (строка выше))
                       
    while front.Count <> 0 do            //обход
        let current = front.Dequeue ()
        match current with
        | Int ref -> for v in graph.OutEdges (current) do    
                         match v.Target with
                         | Int ref2 -> ref2 := !ref
                         | Div x as y -> tryEnqueue 3 y
                         | Inc x as y -> tryEnqueue 4 y
                         | NextIter x as y -> tryEnqueue 7 y
                         | Gate x as y -> tryEnqueue 6 y
                         | _ -> failwith "not in this graph"
        
        | Div block -> if List.exists (fun x -> x < 0) block.Ports       //<--- проверка на то, что во все порты уже пришли значения
                       then front.Enqueue (current)                      //Изначально там лежат отрицательные числа
                       else                                              //Только что осознал, что это не сработает, потому что мы передаем
                           let divOut = block.Out                        //тот же самый граф во вторую итерацию, и там уже не отрицательные
                           for v in graph.OutEdges (current) do          //числа. Но сейчас даже не в этом проблема
                               match v.Target with
                                   | Int ref -> ref := divOut
                                   | Pred p as y -> tryEnqueue 5 y
                                   | NextIter g as y -> tryEnqueue 7 y
                                   | _ -> failwith "not in this graph"
        
        | Inc block -> if List.exists (fun x -> x < 0) block.Ports
                       then front.Enqueue (current)
                       else
                           let incOut = block.Out
                           for v in graph.OutEdges (current) do
                               match v.Target with
                               | Int ref -> ref := incOut
                               | NextIter g as y -> tryEnqueue 7 y
                               | _ -> failwith "not in this graph"
        
        | Pred block -> if List.exists (fun x -> x < 0) block.Ports
                        then front.Enqueue (current)
                        else
                            let predOut = block.Out
                            for v in graph.OutEdges (current) do
                               match v.Target with
                               | Bool ref -> ref := predOut
                               | Gate g as y -> tryEnqueue 6 y
                               | _ -> failwith "not in this graph"

         | NextIter gr -> if List.exists (fun x -> x < 0) gr.Ports
                          then front.Enqueue (current)
                          else 
                              match i, a, c with                    //передаем значения из портов nextIter в блоки переменных
                              | Int ref1, Int ref2, Int ref3 -> 
                                  let ports = gr.Ports
                                  ref1 := ports.[0]
                                  ref2 := ports.[1]
                                  ref3 := ports.[2]
                              | _ -> failwith "no"
                              task.Start ()                 //запускаем следующую итерацию

         | Gate block -> if List.exists (fun x -> x < 0) block.Ports || task.Status <> TaskStatus.Running
                         then front.Enqueue (current)
                         else
                             if block.Predicate         //если true, ждем task и записываем ее результат в out
                             then out <- task.Result   
                             else                       //иначе убиваем task и выполняем операцию внутри мультиплексора,
                                tokenSource.Cancel ()   //которая еще раз смотрит на предикат и пропускает значение из порта f
                                out <- block.Out        //записываем в out
                                                                          
         | _ -> failwith "jkhkjhjh"                
    out                                                        

printfn "%A" graph.VertexCount


Console.WriteLine (decode graph ([a; i; c]))


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