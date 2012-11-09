namespace Brahma.FSharp.OpenCL.AST

type AST<'lang>(topDefs:List<TopDef<'lang>>) = 
    member this.TopDefs = topDefs
