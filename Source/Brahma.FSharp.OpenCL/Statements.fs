namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Statement<'lang> () =
    inherit Node<'lang>()    

type VarDecl<'lang> (vType:Type<'lang>,name:string ,expr:Option<Expression<'lang>>) =
    inherit Statement<'lang>()
    override this.Children = []
    member this.Type = vType
    member this.Name = name
    member this.Expr = expr

type Assignment<'lang> (vName:Property<'lang>,value:Expression<'lang>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Name = vName
    member this.Value = value

type StatementBlock<'lang> (statements:ResizeArray<Statement<'lang>>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Statements = statements    
    