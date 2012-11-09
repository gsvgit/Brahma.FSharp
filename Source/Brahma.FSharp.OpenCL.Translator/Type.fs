module Brahma.FSharp.OpenCL.Translator.Type

open Brahma.FSharp.OpenCL.AST

let Translate (_type:System.Type):Type<Lang> =
    let rec go (str:string) =
        match str.ToLowerInvariant() with
        | "int"| "int32" -> PrimitiveType<Lang>(Int32) :> Type<Lang>
        | "float"| "float32" -> PrimitiveType<Lang>(Float32) :> Type<Lang>        
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

