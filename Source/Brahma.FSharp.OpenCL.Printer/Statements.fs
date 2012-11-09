// Copyright (c) 2012 Semyon Grigorev <rsdpisuy@gmail.com>
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


