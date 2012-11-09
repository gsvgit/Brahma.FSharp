namespace Brahma.FSharp.OpenCL.AST

[<AbstractClass>]
type Node<'lang>()=
    abstract Children: List<Node<'lang>>

[<AbstractClass>]
type TopDef<'lang>()=
    inherit Node<'lang>()

