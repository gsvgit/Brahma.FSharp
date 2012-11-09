module Brahma.FSharp.OpenCL.Printer.Types

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps

let private printPrimitiveTipe (pType:PrimitiveType<'lang>) =
    match pType.Type with
    | Int32 -> "int"
    | Float32 -> "float"
    | Int64 -> "int"
    | Float64 -> "float"
    | Void -> "void"
    |> wordL

let rec Print<'lang> (_type:Type<'lang>) =
    match _type with
    | :? PrimitiveType<'lang> as pt -> printPrimitiveTipe pt
    | :? RefType<'lang> as rt ->  Print rt.BaseType ^^ wordL "*"//] |> spaceListL
    | t -> failwithf "Printer. Unsupported type: %A" t

