namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Expression<'lang>()=
    inherit Node<'lang>()

type Const<'lang>(_type:Type<'lang>,_val:string) =
    inherit Expression<'lang>()
    override this.Children = []
    member this.Type = _type
    member this.Val = _val

type FunCall<'lang>(funName,args) = 
    inherit Expression<'lang>()
    override this.Children = []
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
    inherit Expression<'lang>()
    override this.Children = []
    member this.Left = l
    member this.Right = r
    member this.Op = op

type UOp<'lang> =
    | Minus
    | Not

type Unop<'lang>(op:UOp<'lang>,expr:Expression<'lang>) = 
    inherit Expression<'lang>()
    override this.Children = []
    member this.Expr = expr
    member this.Op = op
