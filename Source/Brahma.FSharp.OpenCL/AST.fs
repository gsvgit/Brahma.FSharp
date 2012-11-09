namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Node<'lang>()=
    abstract Children: List<Node<'lang>>

type AST<'lang>(topDefs) = 
    member this.TopDefs = topDefs
