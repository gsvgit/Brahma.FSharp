[<AutoOpen>]
module Triple 

let first (x,_,_) = x
let second (_,x,_) = x
let third (_,_,x) = x
