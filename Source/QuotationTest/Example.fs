// Copyright (c) 2012 Semyon Grigorev <rsdpisuy@gmail.com>
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

open Brahma.Types
open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Wrapper

open Microsoft.FSharp.Quotations

let deviceType = Cl.DeviceType.Default
let platformName = "*"

let provider =
    try  ComputeProvider.Create(platformName, deviceType)
    with 
    | ex -> failwith ex.Message
    
printfn "Using %A" provider

let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head );

let main () = 
    let l = 10000
    let a = [|1..l|]
    let aBuf = new Buffer<int>(provider, Operations.ReadWrite, Memory.Device,a)
    let command = 
        <@ 
            fun (range:_1D) (buf1:Buffer<int>) -> 
                let x = range.GlobalID0                
                buf1.[x] <- buf1.[x] * buf1.[x]                
        @>

    let c = command:>Expr
    let kernel = provider.Compile<_1D,_>(c)
    let cq = commandQueue.Add(kernel.Run(new _1D(l,1), aBuf)).Finish()
    let r = Array.zeroCreate(l)
    let cq2 = commandQueue.Add(aBuf.Read(0, l, r)).Finish()
    ()

do main ()