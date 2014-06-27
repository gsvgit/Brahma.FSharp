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

let (<!?) (a:'a) (b:'a) (c:'a) = 
    kFail ()
    b |> ignore

let (<!?>) (a:'a) (b:'a) (c:'a) = 
    kFail ()
    b

let aMax (a:'a) (b:'a) = 
    kFail ()
    b

let aMin (a:'a) (b:'a) = 
    kFail ()
    b

//let (<!>) a = 
//    kFail ()
//    a - 1

(*let (~--) a = 
    kFail ()
    a - 1
    *)
let (<!--) a = 
    kFail ()
    a - 1 |> ignore

let (<!++>) a = 
    kFail ()
    a - 1

let (<!++) a = 
    kFail ()
    a - 1 |> ignore

let local a = a
let barrier () = ignore(null)

let _byte (x:bool) = 0uy

let as_uint (b1:byte) (b2:byte) (b3:byte) (b4:byte) = uint32 1
