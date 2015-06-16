module TTA.VSFG

open QuickGraph

type IVSFGNode = interface end

type Port () = 
    interface IVSFGNode

type InPort (inputs:ResizeArray<OutPort>) =
    inherit Port ()
    member this.Inputs = inputs
    member this.ConnectWith p = this.Inputs.Add p
    new () = InPort (new ResizeArray<_>())
    new (x:OutPort) = InPort (new ResizeArray<_>([|x|]))

and OutPort (targets:ResizeArray<InPort>) =
    inherit Port ()
    member this.Targets = targets
    member this.ConnectWith p = this.Targets.Add p
    new () = OutPort (new ResizeArray<_>())
    new (x:InPort) = OutPort (new ResizeArray<_>([|x|]))

//type VarPort = 
//    inherit Port
//
//type StatePort = 
//    inherit Port
//
//type PredPort = 
//    inherit Port

type VSFGBlock (inPorts:ResizeArray<InPort>, outPort:OutPort) = 
    interface IVSFGNode    
    member this.OutPort = outPort 
    member this.InPorts = inPorts

type Multiplexor (predicate:InPort, _true:InPort, _false:InPort, out) = 
    inherit VSFGBlock(new ResizeArray<_>([|predicate; _true; _false|]) , out)
    member this.Predicate = predicate
    member this.True = _true
    member this.False = _false

type UnOp = 
    | Minus
    | AddC of int

type BinOp =
    | Sum
    | Div
    | Mult
    | Eq
    | Less
    | Gr
    | LEq
    | GEq

type BinOpBlock(l:InPort, r:InPort, op:BinOp, out) =
    inherit VSFGBlock(new ResizeArray<_>([|l;r|]), out)

type UnOpBlock(input:InPort, op:UnOp, out) =
    inherit VSFGBlock(new ResizeArray<_>([|input|]), out)