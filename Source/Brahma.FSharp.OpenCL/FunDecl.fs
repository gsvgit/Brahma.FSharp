namespace Brahma.FSharp.OpenCL.AST

type FunFormalArg<'lang>(isGlobal:bool, name:string, _type:Type<'lang>) =
    inherit Statement<'lang>()
    override this.Children = []
    member this.IsGlobal = isGlobal
    member this.Name = name
    member this.Type = _type

type FunDecl<'lang>(isKernel:bool, name:string, retType:Type<'lang>, args:List<FunFormalArg<'lang>>, body:Statement<'lang>) =
    inherit TopDef<'lang>()
    override this.Children = []
    member this.isKernel = isKernel 
    member this.RetType = retType
    member this.Name = name
    member this.Args = args
    member this.Body = body

