module Brahma.FSharp.OpenCL.Printer.Expressions

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps
open Brahma.FSharp.OpenCL.Printer

let printConst (c:Const<'lang>) = 
    match c.Type with
    | :? PrimitiveType<'lang> as pt ->
        match pt.Type with
        | Int32 
        | Float32
        | Int64
        | Float64 -> wordL c.Val
        | Void -> wordL ""
    | c -> failwithf "Printer. Unsupported const with type: %A" c

let printVar (varible:Variable<'lang>) =
    wordL varible.Name

let rec private printItem (itm:Item<'lang>) =
    (Print itm.Arr) ++ squareBracketL (Print itm.Idx)

and private printBop (op:BOp<'lang>) =
    match op with
    | Plus -> "+"
    | Minus -> "-"
    | Mult -> "*"
    | Div -> "/"
    | Pow -> "+"
    | BitAnd -> "&"
    | BitOr -> "|"
    | And -> "&&"
    | Or -> "||"
    | Less -> "<" 
    | LessEQ -> "<="
    | Great -> ">" 
    | GreatEQ -> ">="
    | EQ -> "=="
    |> wordL

and private printBinop (binop:Binop<'lang>) =
    let l = Print binop.Left
    let r = Print binop.Right
    let op = printBop binop.Op
    [l;op;r] |> spaceListL

and private printProperty (prop:Property<'lang>) =
    match prop.Property with
    | PropertyType.Var v -> printVar v
    | PropertyType.Item i -> printItem i

and private printFunCall (fc:FunCall<'lang>) =
    wordL fc.Name ++ (fc.Args |> List.map Print |> commaListL |> bracketL)

and Print (expr:Expression<'lang>) =
    match expr with
    | :? Const<'lang> as c -> printConst c
    | :? Variable<'lang> as v -> printVar v
    | :? Item<'lang> as itm -> printItem itm
    | :? Property<'lang> as prop -> printProperty prop
    | :? Binop<'lang> as binop -> printBinop binop
    | :? FunCall<'lang> as fc -> printFunCall fc
    | c -> failwithf "Printer. Unsupported expression: %A" c
