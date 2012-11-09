namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Statement<'lang> () =
    abstract Childs : List<string>

type VarDecl<'lang> (vType,name,expr:Option<Expression<'lang>>) =
    member this.vType = vType
    member this.Name = name
    member this.Expr = expr
    