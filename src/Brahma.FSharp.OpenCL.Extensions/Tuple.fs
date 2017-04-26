[<AutoOpen>]
module Triple 

let first x = match x with (x,_,_) -> x
let second x = match x with (_,x,_) -> x
let third x = match x with (_,_,x) -> x
