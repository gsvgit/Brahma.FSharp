[<AutoOpen>]
module Brahma.FSharp.OpenCL.Translator.Common

type Lang = OpenCL

type TargetContext<'lang,'vDecl>() =
    let varDecls = new ResizeArray<'vDecl>()
    member this.VarDecls
        with get() =  varDecls

