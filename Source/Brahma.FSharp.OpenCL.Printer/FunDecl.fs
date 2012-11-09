module Brahma.FSharp.OpenCL.Printer.FunDecl

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps
open Brahma.FSharp.OpenCL.Printer

let private printFunFormalParam (param:FunFormalArg<_>) =
    [ if param.IsGlobal then yield wordL "__global"
    ; yield Types.Print param.Type
    ; yield wordL param.Name
    ] |> spaceListL
    

let Print<'lang> (funDecl:FunDecl<'lang>) =
    let header = 
        [ if funDecl.isKernel then yield wordL "__kernel"
        ; yield Types.Print funDecl.RetType
        ; yield wordL funDecl.Name
        ] |> spaceListL
    let formalParams = funDecl.Args |> List.map printFunFormalParam |> commaListL |> bracketL
    let body = Statements.Print true funDecl.Body 
    aboveL (header ++ formalParams) body
