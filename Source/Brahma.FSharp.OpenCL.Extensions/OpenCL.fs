[<AutoOpen>]
module OpenCL

let (<+!) a b = a + b |> ignore
let (+!) a b = a + b
let (-!) a b = a - b
let (<-!) a b = a - b |> ignore
let sIncr a = a + 1
let sDecr a = a - 1
let (<->) (a:'a) (b:'a) = b
let (<->>) (a:'a) (b:'a) = b 
