// Copyright (c) 2012, 2013 Semyon Grigorev <rsdpisuy@gmail.com>
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

module Brahma.FSharp.OpenCL.Printer.Expressions

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps
open Brahma.FSharp.OpenCL.Printer

let private printConst (c:Const<'lang>) = 
    match c.Type with
    | :? PrimitiveType<'lang> as pt ->
        match pt.Type with
        | Int
        | UInt
        | Short
        | UShort
        | Float
        | Long
        | Char
        | UChar
        | Double
        | ULong -> wordL c.Val
        | Void -> wordL ""
    | c -> failwithf "Printer. Unsupported const with type: %A" c

let private printVar (varible:Variable<'lang>) =
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
    | NEQ -> "!="
    | Remainder -> "%"
    |> wordL

and private printBinop (binop:Binop<'lang>) =
    let l = Print binop.Left
    let r = Print binop.Right
    let op = printBop binop.Op
    [l;op;r] |> spaceListL |> bracketL

and private printProperty (prop:Property<'lang>) =
    match prop.Property with
    | PropertyType.Var v -> printVar v
    | PropertyType.Item i -> printItem i

and private printFunCall (fc:FunCall<'lang>) =
    wordL fc.Name ++ (fc.Args |> List.map Print |> commaListL |> bracketL)

and private printUnOp (uo:Unop<'lang>) =
    match uo.Op with
    | UOp.Minus -> wordL "-" ++ Print uo.Expr |> bracketL
    | UOp.Not -> wordL "not" ++ Print uo.Expr |> bracketL
    | UOp.Incr -> Print uo.Expr ++ wordL "++"
    | UOp.Decr -> Print uo.Expr ++ wordL "--"

and private printCast (c:Cast<'lang>) =
    let t = Types.Print c.Type
    let expr = Print c.Expr
    (t |> bracketL) ++ expr

and private printPointer (p:Pointer<'lang>) =    
    let expr = Print p.Expr
    wordL "&" ^^ expr

and Print (expr:Expression<'lang>) =
    match expr with
    | :? Const<'lang> as c -> printConst c
    | :? Variable<'lang> as v -> printVar v
    | :? Item<'lang> as itm -> printItem itm
    | :? Property<'lang> as prop -> printProperty prop
    | :? Binop<'lang> as binop -> printBinop binop
    | :? FunCall<'lang> as fc -> printFunCall fc
    | :? Unop<'lang> as uo -> printUnOp uo
    | :? Cast<'lang> as c -> printCast c
    | :? Pointer<'lang> as p -> printPointer p
    | c -> failwithf "Printer. Unsupported expression: %A" c
