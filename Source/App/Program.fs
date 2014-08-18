open NA.Elements
open NA.IL
open System.Collections.Generic

let board =
    let blockCount = 2
    let intBocks = Array.init blockCount (fun _ -> new Block<_> [|(+);(-);(*);(/)|])
    let con1:Connector<int,int> = new Connector<_,_>(id)
    let con2:Connector<int,int> = new Connector<_,_>(id)
    intBocks.[0].InConnectors <- [|fun _ -> con1.OutBuf|]
    intBocks.[1].InConnectors <- [|fun _ -> con2.OutBuf|]
    intBocks.[0].OutConnectors <- [|con2.Move|]
    intBocks.[1].OutConnectors <- [|con1.Move|]
    let blocks = intBocks
    blocks

let run (commands:array<array<_>>) =
    let queues = 
        Array.init 
            commands.Length 
            (fun i -> 
                board.[i].Instructions <- new Queue<_>(commands.[i])
                board.[i].Instructions )
    while queues |> Array.forall (fun q -> q.Count > 0) do
        for b in board do
            b.Step()
    board.[0].GetVal(2,0)

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
        |]
        [|
        Set (Const 3, OpBlock(0,0))        
        Set (Const 4, OpBlock(2,0))
        Set (Const 2, OpBlock(0,1))
        Move (OpBlock(0,0), OpBlock(0,1))
        Move (OpBlock(0,1), OpBlock(2,0))
        Move (OpBlock(2,0), Connector(0))
        Eps
        |]
    |]

do 
    run commands
    |> printfn "%A"