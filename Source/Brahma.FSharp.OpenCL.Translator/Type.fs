module Brahma.FSharp.OpenCL.Translator.Type

open Brahma.FSharp.OpenCL.AST

let Translate (_type:System.Type):Type<Lang> =
    match _type.Name.ToLowerInvariant() with
    | "int"| "int32" -> PrimitiveType<Lang>(Int32) :> Type<Lang>
    | "float"| "float32" -> PrimitiveType<Lang>(Float32) :> Type<Lang>
    | x -> "Unsuported kernel type: " + x |> failwith 

