[<AutoOpen>]
module Brahma.FSharp.OpenCL.Extensions

open System.Runtime.InteropServices
open OpenCL.Net
open Brahma.OpenCL

type ``[]``<'t> with
    member this.ToHost(kernel:ICLKernel) =
        new Commands.ReadBuffer<'t>(kernel.AutoconfiguredBuffers.[this], Marshal.SizeOf(typeof<'t>),true,0,this.Length,this)
    member this.ToHost(kernel:ICLKernel, hostArray:array<'t>) =
        new Commands.ReadBuffer<'t>(kernel.AutoconfiguredBuffers.[this], Marshal.SizeOf(typeof<'t>),true,0,hostArray.Length,hostArray)