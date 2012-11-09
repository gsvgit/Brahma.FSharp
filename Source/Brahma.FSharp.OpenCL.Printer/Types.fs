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

