namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Expression<'lang>()=
    abstract Childs : List<string>

type FunCall<'lang>(funName,args) = 
    member this.FunName = funName
    member this.Args = args

type ArrIndex<'lang>(arr,idx:Expression<'lang>) = 
    member this.Arr = arr
    member this.Idx = idx

type BOp<'lang> =
     | Plus
     | Minus
     | Mult
     | Div
     | Pow
     | BitAnd
     | BitOr
     | And
     | Or
     | Less
     | LessEQ
     | Great
     | GreatEQ
     | EQ

type Binop<'lang>(op:BOp<'lang>,l:Expression<'lang>,r:Expression<'lang>) = 
    member this.Left = l
    member this.Right = r
    member this.Op = op

type UOp<'lang> =
    | Minus
    | Not

type Unop<'lang>(op:UOp<'lang>,expr:Expression<'lang>) = 
    member this.Expr = expr
    member this.Op = op
