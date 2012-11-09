module Brahma.FSharp.OpenCL.Printer.Statements

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps
open Brahma.FSharp.OpenCL.Printer

let rec private printAssignment (a:Assignment<'lang>) =
    [Expressions.Print a.Name; wordL "="; Expressions.Print a.Value] |> spaceListL

and private printVarDecl (vd:VarDecl<'lang>) =
    [ yield Types.Print vd.Type
    ; yield wordL vd.Name
    ; if vd.Expr.IsSome then yield [wordL "="; Expressions.Print vd.Expr.Value] |> spaceListL
    ] |> spaceListL

and private printStmtBlock (sb:StatementBlock<'lang>) =
    sb.Statements |> ResizeArray.map (Print false) |> List.ofSeq |> aboveListL |> braceL

and Print isToplevel (stmt:Statement<'lang>) =
    let res = 
        match stmt with
        | :? StatementBlock<'lang> as sb -> printStmtBlock sb
        | :? VarDecl<'lang> as vd -> printVarDecl vd
        | :? Assignment<'lang> as a -> printAssignment a
        | t -> failwithf "Printer. Unsupported statement: %A" t
    if isToplevel
    then res
    else res ++ wordL ";"


