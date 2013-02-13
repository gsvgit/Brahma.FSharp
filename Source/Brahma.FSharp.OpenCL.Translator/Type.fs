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

module Brahma.FSharp.OpenCL.Translator.Type

open Brahma.FSharp.OpenCL.AST

let Translate (_type:System.Type) isKernelArg size :Type<Lang>=
    let rec go (str:string) =
        match str.ToLowerInvariant() with
        | "int"| "int32" -> PrimitiveType<Lang>(Int) :> Type<Lang>
        | "float"| "float32" | "single"-> PrimitiveType<Lang>(Float) :> Type<Lang>
        | "byte" -> PrimitiveType<Lang>(UChar) :> Type<Lang>
        | "boolean" -> PrimitiveType<Lang>(Int) :> Type<Lang>
        | "double" -> PrimitiveType<Lang>(Double) :> Type<Lang>        
        | t when t.EndsWith "[]" ->
            let baseT = t.Substring(0,t.Length-2)
            if isKernelArg 
            then RefType<_>(go baseT) :> Type<Lang>
            else ArrayType<_>(go baseT, size |> Option.get) :> Type<Lang>
        | s when s.StartsWith "fsharpref" ->
            go (_type.GetGenericArguments().[0].Name)
        | x -> "Unsuported kernel type: " + x |> failwith 
    _type.Name
    |> go

