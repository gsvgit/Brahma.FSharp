open NA.Elements
open NA.IL
open System.Collections.Generic

let board =
    let blockCount = 1
    let blocks = Array.init blockCount (fun _ -> new Block<_,int,int,int,int,int,int>((+),(-),(*),(/)))
    blocks

let run (commands:array<array<_>>) =
    let queues = 
        Array.init 
            commands.Length 
            (fun i -> 
                //for c in commands.[i] do board.[i].Instructions.Enqueue c
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
        |]
    |]

do 
    run commands
    |> printfn "%A"