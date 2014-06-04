[<AutoOpen>]
module OpenCL

let private kFail () = failwith "OpenCL kernel function!"

let (<!+) a b =
    kFail ()
    a + b |> ignore
let (<!+>) a b = 
    kFail ()
    a + b
let (<!-) a b =
    kFail ()
    a - b |> ignore
let (<!->) a b =
    kFail ()
    a - b
let (<!) (a:'a) (b:'a) =
    kFail ()
    b |> ignore 
let (<!>) (a:'a) (b:'a) = 
    kFail ()
    b 
let aIncr a = 
    kFail ()
    a + 1
let aDecr a = 
    kFail ()
    a - 1

let local a = a
let barrier () = ignore(null)

let _byte (x:bool) = 0uy

let as_uint (b1:byte) (b2:byte) (b3:byte) (b4:byte) = uint32 1
