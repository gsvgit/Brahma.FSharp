module NA.Board

open NA.Elements
open Microsoft.FSharp.Collections

[<Class>]
type Board<'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,'a8,'a9>(edgesList:List<IBlock*IConnector*IBlock>) =
    let blocks = new ResizeArray<IBlock>()
    let run (stop) =        
        while stop() do
            for b in blocks do
                b.Step()        

    let go (c:Connector<'b0,'b1>) (b1:IBlock) (b2:IBlock) =
        if blocks.Contains b1 |> not then blocks.Add b1
        if blocks.Contains b2 |> not then blocks.Add b2
        match b1 with
        | :? Block<'b0> as b1 ->
            match b2 with
            | :? Block<'b1> as b2 ->
                b2.InConnectors <- [|fun _ -> c.OutBuf|]            
                b1.OutConnectors <- [|c.Move|]
            | _ -> failwith "Incorrect type of block 2"
        | _ -> failwith "Incorrect type of block 1"
    do
        for (b1,c,b2) in edgesList do
            match c with
            | :? Connector<'a0,'a0> as c -> go c b1 b2
            | :? Connector<'a0,'a1> as c -> go c b1 b2
            | :? Connector<'a0,'a2> as c -> go c b1 b2
            | :? Connector<'a0,'a3> as c -> go c b1 b2
            | :? Connector<'a0,'a4> as c -> go c b1 b2
            | :? Connector<'a0,'a5> as c -> go c b1 b2
            | :? Connector<'a0,'a6> as c -> go c b1 b2
            | :? Connector<'a0,'a7> as c -> go c b1 b2
            | :? Connector<'a0,'a8> as c -> go c b1 b2
            | :? Connector<'a0,'a9> as c -> go c b1 b2
            | :? Connector<'a1,'a0> as c -> go c b1 b2
            | :? Connector<'a1,'a1> as c -> go c b1 b2
            | :? Connector<'a1,'a2> as c -> go c b1 b2
            | :? Connector<'a1,'a3> as c -> go c b1 b2
            | :? Connector<'a1,'a4> as c -> go c b1 b2
            | :? Connector<'a1,'a5> as c -> go c b1 b2
            | :? Connector<'a1,'a6> as c -> go c b1 b2
            | :? Connector<'a1,'a7> as c -> go c b1 b2
            | :? Connector<'a1,'a8> as c -> go c b1 b2
            | :? Connector<'a1,'a9> as c -> go c b1 b2
            | :? Connector<'a2,'a0> as c -> go c b1 b2
            | :? Connector<'a2,'a1> as c -> go c b1 b2
            | :? Connector<'a2,'a2> as c -> go c b1 b2
            | :? Connector<'a2,'a3> as c -> go c b1 b2
            | :? Connector<'a2,'a4> as c -> go c b1 b2
            | :? Connector<'a2,'a5> as c -> go c b1 b2
            | :? Connector<'a2,'a6> as c -> go c b1 b2
            | :? Connector<'a2,'a7> as c -> go c b1 b2
            | :? Connector<'a2,'a8> as c -> go c b1 b2
            | :? Connector<'a2,'a9> as c -> go c b1 b2
            | :? Connector<'a3,'a0> as c -> go c b1 b2
            | :? Connector<'a3,'a1> as c -> go c b1 b2
            | :? Connector<'a3,'a2> as c -> go c b1 b2
            | :? Connector<'a3,'a3> as c -> go c b1 b2
            | :? Connector<'a3,'a4> as c -> go c b1 b2
            | :? Connector<'a3,'a5> as c -> go c b1 b2
            | :? Connector<'a3,'a6> as c -> go c b1 b2
            | :? Connector<'a3,'a7> as c -> go c b1 b2
            | :? Connector<'a3,'a8> as c -> go c b1 b2
            | :? Connector<'a3,'a9> as c -> go c b1 b2
            | :? Connector<'a4,'a0> as c -> go c b1 b2
            | :? Connector<'a4,'a1> as c -> go c b1 b2
            | :? Connector<'a4,'a2> as c -> go c b1 b2
            | :? Connector<'a4,'a3> as c -> go c b1 b2
            | :? Connector<'a4,'a4> as c -> go c b1 b2
            | :? Connector<'a4,'a5> as c -> go c b1 b2
            | :? Connector<'a4,'a6> as c -> go c b1 b2
            | :? Connector<'a4,'a7> as c -> go c b1 b2
            | :? Connector<'a4,'a8> as c -> go c b1 b2
            | :? Connector<'a4,'a9> as c -> go c b1 b2
            | :? Connector<'a5,'a0> as c -> go c b1 b2
            | :? Connector<'a5,'a1> as c -> go c b1 b2
            | :? Connector<'a5,'a2> as c -> go c b1 b2
            | :? Connector<'a5,'a3> as c -> go c b1 b2
            | :? Connector<'a5,'a4> as c -> go c b1 b2
            | :? Connector<'a5,'a5> as c -> go c b1 b2
            | :? Connector<'a5,'a6> as c -> go c b1 b2
            | :? Connector<'a5,'a7> as c -> go c b1 b2
            | :? Connector<'a5,'a8> as c -> go c b1 b2
            | :? Connector<'a5,'a9> as c -> go c b1 b2
            | :? Connector<'a6,'a0> as c -> go c b1 b2
            | :? Connector<'a6,'a1> as c -> go c b1 b2
            | :? Connector<'a6,'a2> as c -> go c b1 b2
            | :? Connector<'a6,'a3> as c -> go c b1 b2
            | :? Connector<'a6,'a4> as c -> go c b1 b2
            | :? Connector<'a6,'a5> as c -> go c b1 b2
            | :? Connector<'a6,'a6> as c -> go c b1 b2
            | :? Connector<'a6,'a7> as c -> go c b1 b2
            | :? Connector<'a6,'a8> as c -> go c b1 b2
            | :? Connector<'a6,'a9> as c -> go c b1 b2
            | :? Connector<'a7,'a0> as c -> go c b1 b2
            | :? Connector<'a7,'a1> as c -> go c b1 b2
            | :? Connector<'a7,'a2> as c -> go c b1 b2
            | :? Connector<'a7,'a3> as c -> go c b1 b2
            | :? Connector<'a7,'a4> as c -> go c b1 b2
            | :? Connector<'a7,'a5> as c -> go c b1 b2
            | :? Connector<'a7,'a6> as c -> go c b1 b2
            | :? Connector<'a7,'a7> as c -> go c b1 b2
            | :? Connector<'a7,'a8> as c -> go c b1 b2
            | :? Connector<'a7,'a9> as c -> go c b1 b2
            | :? Connector<'a8,'a0> as c -> go c b1 b2
            | :? Connector<'a8,'a1> as c -> go c b1 b2
            | :? Connector<'a8,'a2> as c -> go c b1 b2
            | :? Connector<'a8,'a3> as c -> go c b1 b2
            | :? Connector<'a8,'a4> as c -> go c b1 b2
            | :? Connector<'a8,'a5> as c -> go c b1 b2
            | :? Connector<'a8,'a6> as c -> go c b1 b2
            | :? Connector<'a8,'a7> as c -> go c b1 b2
            | :? Connector<'a8,'a8> as c -> go c b1 b2
            | :? Connector<'a8,'a9> as c -> go c b1 b2
            | :? Connector<'a9,'a0> as c -> go c b1 b2
            | :? Connector<'a9,'a1> as c -> go c b1 b2
            | :? Connector<'a9,'a2> as c -> go c b1 b2
            | :? Connector<'a9,'a3> as c -> go c b1 b2
            | :? Connector<'a9,'a4> as c -> go c b1 b2
            | :? Connector<'a9,'a5> as c -> go c b1 b2
            | :? Connector<'a9,'a6> as c -> go c b1 b2
            | :? Connector<'a9,'a7> as c -> go c b1 b2
            | :? Connector<'a9,'a8> as c -> go c b1 b2
            | :? Connector<'a9,'a9> as c -> go c b1 b2
            | _ -> failwith "Unexpected instance of IConnector"
            
    member this.Blocks = blocks
    member this.Run stop = run stop

