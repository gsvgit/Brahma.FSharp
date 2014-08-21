open NA.Elements
open NA.IL
open NA.Board
open System.Collections.Generic

let board =
    let blockCount = 4
    let intBocks = Array.init blockCount (fun _ -> new Block<_> [|(+);(-);(*);(/)|])
    let con1 = new Connector<int,int>(id) :> IConnector
    let con2 = new Connector<int,int>(id) :> IConnector
    let con3 = new Connector<int,int>(id) :> IConnector
    let con4 = new Connector<int,int>(id) :> IConnector
    let con5 = new Connector<int,int>(id) :> IConnector
    let con6 = new Connector<int,int>(id) :> IConnector
    let con7 = new Connector<int,int>(id) :> IConnector
    let con8 = new Connector<int,int>(id) :> IConnector
    let b = 
        new Board<int,int,int,int,int,int,int,int,int,int>
            ([
                intBocks.[0] :> IBlock, con1, intBocks.[1] :> IBlock
                intBocks.[1] :> IBlock, con2, intBocks.[0] :> IBlock
                intBocks.[1] :> IBlock, con3, intBocks.[2] :> IBlock
                intBocks.[2] :> IBlock, con4, intBocks.[1] :> IBlock
                intBocks.[2] :> IBlock, con5, intBocks.[3] :> IBlock
                intBocks.[3] :> IBlock, con6, intBocks.[2] :> IBlock
                intBocks.[3] :> IBlock, con7, intBocks.[0] :> IBlock
                intBocks.[0] :> IBlock, con8, intBocks.[3] :> IBlock
            ])
    b

let run (commands:array<seq<_>>) =
    let queues = 
        Array.init 
            commands.Length 
            (fun i -> 
                board.Blocks.[i].Instructions <- new Queue<_>(commands.[i])
                board.Blocks.[i].Instructions )
    board.Run (fun () -> queues |> Array.forall (fun q -> q.Count > 0))    
    board.ToDot("dot/f.dot")
    (board.Blocks.[0] :?> Block<int>).GetVal(2,0)

let commands =
    [|
        [|
        Set (Const 3, OpBlock(0,0))        
        Set (Const 4, OpBlock(2,0))
        Set (Const 2, OpBlock(0,1))
        Move (OpBlock(0,0), OpBlock(0,1))
        Move (OpBlock(0,1), OpBlock(2,0))
        Eps
        Move (Connector 0, OpBlock(2,0))
        Eps
        Move (Connector 1, OpBlock(2,0))
        |] 
        |> Seq.cast<IInstruction>
        [|
        Set (Const 3, OpBlock(0,0))        
        Set (Const 4, OpBlock(2,0))
        Set (Const 3, OpBlock(0,1))
        Move (OpBlock(0,0), OpBlock(0,1))
        Move (OpBlock(0,1), OpBlock(2,0))
        Move (OpBlock(2,0), Connector(0))
        Eps
        Eps
        Eps
        |] |> Seq.cast<IInstruction>
        [|
        Set (Const 2, OpBlock(0,0))        
        Set (Const 4, OpBlock(2,0))
        Set (Const 1, OpBlock(0,1))
        Move (OpBlock(0,0), OpBlock(0,1))
        Move (OpBlock(0,1), OpBlock(2,0))
        Move (OpBlock(2,0), Connector(1))
        Eps
        Eps
        Eps
        |] |> Seq.cast<IInstruction>
        [|
        Set (Const 4, OpBlock(0,0))        
        Set (Const 5, OpBlock(2,0))
        Set (Const 2, OpBlock(0,1))
        Move (OpBlock(0,0), OpBlock(0,1))
        Move (OpBlock(0,1), OpBlock(2,0))
        Eps
        Move (Connector 0, OpBlock(2,0))
        Move (OpBlock(2,0), Connector(1))
        Eps
        |] |> Seq.cast<IInstruction>
    |]

do 
    run commands
    |> printfn "%A"


//type T = interface end
//
//[<Class>]
//type X<'a> () =
//    interface T
//    member this.x (t:'a) = t
//
//[<Class>]
//type Y<'a> () =
//    interface T
//    member this.x (t:'a) = t
//
//let x =
//    [
//        new X<int>() :> T
//        new X<string>() :> T
//        new Y<string>() :> T
//        new Y<bool>() :> T
//    ]
//
//x |> List.iter (printfn "%A ; ")