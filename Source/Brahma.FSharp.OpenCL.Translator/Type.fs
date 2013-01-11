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

module Brahma.FSharp.OpenCL.Translator.Type

open Brahma.FSharp.OpenCL.AST

let Translate (_type:System.Type):Type<Lang> =
    let rec go (str:string) =
        match str.ToLowerInvariant() with
        | "int"| "int32" -> PrimitiveType<Lang>(Int) :> Type<Lang>
        | "float"| "float32" | "single"-> PrimitiveType<Lang>(Float) :> Type<Lang>
        | "double" -> PrimitiveType<Lang>(Double) :> Type<Lang>        
        | "single[]" -> RefType<_>(go "single") :> Type<Lang>
        | "double[]" -> RefType<_>(go "double") :> Type<Lang>
        | "int32[]" -> RefType<_>(go "int32") :> Type<Lang>
        | x ->
            if x.StartsWith "buffer`"
            then
                let bufferType = 
                    (((_type.FullName.Split('[')).[2].Split(',')).[0].Split('.')).[1]
                    |> go
                RefType<_>(bufferType) :> Type<Lang>
                    //Brahma.OpenCL.Buffer`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
            else "Unsuported kernel type: " + x |> failwith 
    _type.Name
    |> go

