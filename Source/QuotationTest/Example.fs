// Copyright (c) 2012, 2013 Semyon Grigorev <rsdpisuy@gmail.com>
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Brahma.FSharp.OpenCL.Extensions

open Microsoft.FSharp.Quotations
open System

let deviceType = Cl.DeviceType.Default
let platformName = "*"

let provider =
    try  ComputeProvider.Create(platformName, deviceType)
    with 
    | ex -> failwith ex.Message
    
printfn "Using %A" provider

let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head );

let fn ([<ParamArray>] x) = x 

type X() =
    member this.F([<ParamArray>] x) = x 

let main () =     
    let l = 10000
    let a = [|1..l|]
    let aBuf = new Buffer<int>(provider, Operations.ReadWrite, Memory.Device,a)
   // let bBuf = new Buffer<float>(provider, Operations.ReadWrite, Memory.Device,[||])
    let xobj = new X()
//    fn (aBuf, bBuf)
  //  xobj.F(aBuf, bBuf)

    let command = 
        <@ 
            fun (range:_1D) (x) (buf1:array<int>) -> 
                (*let x = range.GlobalID0i
                if (x > 2 && x < 5) || (x > 10 && x < 15) 
                then
                    buf1.[x] <- buf1.[x] * buf1.[x] * 2
                else
                    for i in 0..x do
                        let y = buf1.[i] * 2
                        let z = y - 1
                        buf1.[x] <- buf1.[x] - z*)
               
               
//                let x = range.GlobalID0i
//                let mutable i = 0
//                while (i < 10) do 
//                    buf1.[x] <- buf1.[x] + 1
                    //i <- i + 1
                //fun (range:_1D) (buf:array<int>) ->                 
                let y = buf1.[1] + x                
                buf1.[0] <- y
        @>
    
    let kernel, kernelFunPrep, kerRun = provider.Compile command
    kernelFunPrep (new _1D(l,1)) 2 a
    let cq = commandQueue.Add(kerRun()).Finish()
    let r = Array.zeroCreate(l)
    let cq2 = commandQueue.Add(a.ToHost(provider,r)).Finish()
    ()

do main ()