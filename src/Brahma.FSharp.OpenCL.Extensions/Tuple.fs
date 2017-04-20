namespace Tuple
open System
open System.Runtime.InteropServices 
open System.Collections.Generic
open System.Collections.Concurrent
open System.Linq
open OpenCL.Net
open Brahma.OpenCL

module Triple =
    let first (x,_,_) = x
    let second (_,x,_) = x
    let third (_,_,x) = x

(*[<AutoOpen>]
module Memory =
    type ``Tuple``<'T1, 'T2> with
        member this.TupleToGpu(provider:ComputeProvider) =*)
 
