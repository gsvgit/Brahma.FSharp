namespace Brahma.FSharp.OpenCL.AST

type FunDecl<'lang>(isKernel, name, retType, args, body) =
    member this.isKernel = isKernel 
    member this.RetType = retType
    member this.Name = name
    member this.Args = args
    member this.Body = body

