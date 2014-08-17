module NA.Elements

open NA.IL

let opBlockSuze = 256

type Connector<'a,'b>(onTransfer:'a ->'b) =
    member this.OnTransfer = onTransfer
    member val OutBuf = Unchecked.defaultof<'b> with get, set
    member this.Move v = this.OutBuf <- this.OnTransfer v

[<Struct>]
type ConnectorsBlock<'a0,'b0,'a1,'b1,'a2,'b2,'a3,'b3,'a4,'b4,'a5,'b5> =
    val c0:Connector<'a0,'b0>
    val c1:Connector<'a1,'b1>
    val c2:Connector<'a2,'b2>
    val c3:Connector<'a3,'b3>
    val c4:Connector<'a4,'b4>
    val c5:Connector<'a5,'b5>

type OperationBlock<'a>(op: 'a -> 'a -> 'a) =
    let data = Array.init opBlockSuze (fun _ -> Unchecked.defaultof<'a>)
    member this.Move i v = data.[i] <- op data.[i] v
    member this.Set i v = data.[i] <- v
    member this.Get i = data.[i]

type Block<'bType, 'b0, 'b1, 'b2, 'b3, 'b4, 'b5>(op0,op1,op2,op3) =
    let b0 = new OperationBlock<'bType>(op0)
    let b1 = new OperationBlock<'bType>(op1)
    let b2 = new OperationBlock<'bType>(op2)
    let b3 = new OperationBlock<'bType>(op3)
    member this.GetVal(i, j) = 
        match i with
        | 0 -> b0.Get j
        | 1 -> b1.Get j
        | 2 -> b2.Get j
        | 3 -> b3.Get j
        | _ -> failwithf "Unexpected operation block number: %A"  i
    member val Instructions = new System.Collections.Generic.Queue<Instruction<'bType>>() with get,set
    member val InConnectors = Unchecked.defaultof<ConnectorsBlock<'b0,'bType,'b1,'bType,'b2,'bType,'b3,'bType,'b4,'bType,'b5,'bType>>
    member val OutConnectors = Unchecked.defaultof<ConnectorsBlock<'bType, 'b0,'bType,'b1,'bType,'b2,'bType,'b3,'bType,'b4,'bType,'b5>>
    member this.Step() =
        let instr = this.Instructions.Dequeue()
        match instr with
        | Eps -> ()                        
        | Set (OpBlock (i,j), OpBlock (k,l)) ->
            let v = 
                match i with
                | 0 -> b0.Get j
                | 1 -> b1.Get j
                | 2 -> b2.Get j
                | 3 -> b3.Get j
                | _ -> failwithf "Unexpected operation block number: %A" i
                 
            match k with
            | 0 -> b0.Set l v
            | 1 -> b1.Set l v
            | 2 -> b2.Set l v
            | 3 -> b3.Set l v
            | _ -> failwithf "Unexpected operation block number: %A" k

        | Set (Connector i, OpBlock (j,k)) ->
            let v = 
                match i with
                | 0 -> this.InConnectors.c0.OutBuf
                | 1 -> this.InConnectors.c1.OutBuf
                | 2 -> this.InConnectors.c2.OutBuf
                | 3 -> this.InConnectors.c3.OutBuf
                | 4 -> this.InConnectors.c4.OutBuf
                | 5 -> this.InConnectors.c5.OutBuf
                | _ -> failwithf "Unexpected in connector number: %A" i                 

            match j with
            | 0 -> b0.Set k v
            | 1 -> b1.Set k v
            | 2 -> b2.Set k v
            | 3 -> b3.Set k v
            | _ -> failwithf "Unexpected operation block number: %A" j
            
        | Set (Const v, OpBlock (j,k)) ->
            match j with
            | 0 -> b0.Set k v
            | 1 -> b1.Set k v
            | 2 -> b2.Set k v
            | 3 -> b3.Set k v
            | _ -> failwithf "Unexpected operation block number: %A" j

        | Move (Connector i, Connector j) -> 
            let v = 
                match i with
                | 0 -> this.InConnectors.c0.OutBuf
                | 1 -> this.InConnectors.c1.OutBuf
                | 2 -> this.InConnectors.c2.OutBuf
                | 3 -> this.InConnectors.c3.OutBuf
                | 4 -> this.InConnectors.c4.OutBuf
                | 5 -> this.InConnectors.c5.OutBuf
                | _ -> failwithf "Unexpected in connector number: %A" i
                 
            match i with
            | 0 -> this.OutConnectors.c0.Move v
            | 1 -> this.OutConnectors.c1.Move v
            | 2 -> this.OutConnectors.c2.Move v
            | 3 -> this.OutConnectors.c3.Move v
            | 4 -> this.OutConnectors.c4.Move v
            | 5 -> this.OutConnectors.c5.Move v
            | _ -> failwithf "Unexpected out connector number: %A" i
            
        | Move (OpBlock (i,j), Connector k) ->
            let v = 
                match i with
                | 0 -> b0.Get j
                | 1 -> b1.Get j
                | 2 -> b2.Get j
                | 3 -> b3.Get j
                | _ -> failwithf "Unexpected operation block number: %A" i
                 
            match k with
            | 0 -> this.OutConnectors.c0.Move v
            | 1 -> this.OutConnectors.c1.Move v
            | 2 -> this.OutConnectors.c2.Move v
            | 3 -> this.OutConnectors.c3.Move v
            | 4 -> this.OutConnectors.c4.Move v
            | 5 -> this.OutConnectors.c5.Move v
            | _ -> failwithf "Unexpected out connector number: %A" k
            
        | Move (OpBlock (i,j), OpBlock (k,l)) ->
            let v = 
                match i with
                | 0 -> b0.Get j
                | 1 -> b1.Get j
                | 2 -> b2.Get j
                | 3 -> b3.Get j
                | _ -> failwithf "Unexpected operation block number: %A" i
                 
            match k with
            | 0 -> b0.Move l v
            | 1 -> b1.Move l v
            | 2 -> b2.Move l v
            | 3 -> b3.Move l v
            | _ -> failwithf "Unexpected operation block number: %A" k

        | Move (Connector i, OpBlock (j,k)) ->
            let v = 
                match i with
                | 0 -> this.InConnectors.c0.OutBuf
                | 1 -> this.InConnectors.c1.OutBuf
                | 2 -> this.InConnectors.c2.OutBuf
                | 3 -> this.InConnectors.c3.OutBuf
                | 4 -> this.InConnectors.c4.OutBuf
                | 5 -> this.InConnectors.c5.OutBuf
                | _ -> failwithf "Unexpected in connector number: %A" i                 

            match j with
            | 0 -> b0.Move k v
            | 1 -> b1.Move k v
            | 2 -> b2.Move k v
            | 3 -> b3.Move k v
            | _ -> failwithf "Unexpected operation block number: %A" j
            
        | Move (Const v, OpBlock (j,k)) ->
            match j with
            | 0 -> b0.Move k v
            | 1 -> b1.Move k v
            | 2 -> b2.Move k v
            | 3 -> b3.Move k v
            | _ -> failwithf "Unexpected operation block number: %A" j

        | Move (Const v, Connector i) ->
            match i with
            | 0 -> this.OutConnectors.c0.Move v
            | 1 -> this.OutConnectors.c1.Move v
            | 2 -> this.OutConnectors.c2.Move v
            | 3 -> this.OutConnectors.c3.Move v
            | 4 -> this.OutConnectors.c4.Move v
            | 5 -> this.OutConnectors.c5.Move v
            | _ -> failwithf "Unexpected out connector number: %A" i

        | _ -> failwithf "Unexpected command: %A" instr
    