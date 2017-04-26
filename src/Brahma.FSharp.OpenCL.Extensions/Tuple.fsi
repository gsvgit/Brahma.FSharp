[<AutoOpen>]
module Triple 

val first : tuple:('T1 * 'T2 * 'T3) -> 'T1
val second : tuple:('T1 * 'T2 * 'T3) -> 'T2
val third : tuple:('T1 * 'T2 * 'T3) -> 'T3


(*let first x = match x with (x,_,_) -> x
let second x = match x with (_,x,_) -> x
let third x = match x with (_,_,x) -> x
*)
(*[<AutoOpen>]
module Memory =
    type ``Tuple``<'T1, 'T2> with
        member this.TupleToGpu(provider:ComputeProvider) =*)
 
