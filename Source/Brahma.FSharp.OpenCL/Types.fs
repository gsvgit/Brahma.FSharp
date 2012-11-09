namespace Brahma.FSharp.OpenCL.AST

type PTypes<'lang> =
    | Int32
    | Float32
    | Int64
    | Float64

[<AbstractClass>]
type Type<'lang>()=
    inherit Node<'lang>()
    abstract Size:int

type PrimitiveType<'lang>(pType:PTypes<'lang>) =
    inherit Type<'lang>()
    override this.Size = 32
    override this.Children = []

