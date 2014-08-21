module NA.Elements

open NA.IL

let opBlockSuze = 256

type IConnector = interface end
type IBlock = 
    abstract Step: unit -> unit
    abstract Instructions: System.Collections.Generic.Queue<IInstruction> with get, set

type Connector<'a,'b>(onTransfer:'a ->'b) =
    interface IConnector
    member val OutBuf = Unchecked.defaultof<'b> with get, set
    member this.Move v = this.OutBuf <- onTransfer v

type OperationBlock<'a>(op: 'a -> 'a -> 'a) =
    let data = Array.init opBlockSuze (fun _ -> Unchecked.defaultof<'a>)
    member this.Move i v = data.[i] <- op data.[i] v
    member this.Set i v = data.[i] <- v
    member this.Get i = data.[i]

type Block<'bType>(ops:array<_>) =    
    let blocks = Array.init ops.Length (fun i -> new OperationBlock<'bType>(ops.[i]))

    interface IBlock with
        member val Instructions = new System.Collections.Generic.Queue<IInstruction>() with get,set
        member this.Step() =
            let instr = (this :> IBlock).Instructions.Dequeue()
            match (instr :?> Instruction<'bType>) with
            | Eps -> ()                        
            | Set (OpBlock (i,j), OpBlock (k,l)) ->
                let v = blocks.[i].Get j                 
                blocks.[k].Set l v

            | Set (Connector i, OpBlock (j,k)) ->
                let v = this.InConnectors.[i]()
                blocks.[j].Set k v
            
            | Set (Const v, OpBlock (j,k)) ->
                blocks.[j].Set k v

            | Move (Connector i, Connector j) -> 
                let v =  this.InConnectors.[i]()
                this.OutConnectors.[i] v

            | Move (OpBlock (i,j), Connector k) ->
                let v = blocks.[i].Get j
                this.OutConnectors.[k] v
            
            | Move (OpBlock (i,j), OpBlock (k,l)) ->
                let v = blocks.[i].Get j
                blocks.[k].Move l v

            | Move (Connector i, OpBlock (j,k)) ->
                let v = this.InConnectors.[i]()
                blocks.[j].Move k v
            
            | Move (Const v, OpBlock (j,k)) ->
                blocks.[j].Move k v

            | Move (Const v, Connector i) ->
                this.OutConnectors.[i] v

            | _ -> failwithf "Unexpected command: %A" instr
    
    member this.GetVal(i, j) = blocks.[i].Get j
    member this.Blocks = blocks    
    member val InConnectors:array<unit->'bType> = [||] with get,set
    member val OutConnectors:array<'bType->unit> = [||] with get,set
    