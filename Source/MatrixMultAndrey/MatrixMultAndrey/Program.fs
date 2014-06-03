// Copyright (c) 2013 Semyon Grigorev <rsdpisuy@gmail.com>
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

module MatrixMultAndrey

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main (aValues:int[]) (bValues:int[]) rows columns=    

    let localWorkSize = 2
    let deviceType = DeviceType.Gpu

    let provider =
        try  ComputeProvider.Create("NVIDIA*", deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider
    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)
    let cParallel = Array.zeroCreate(rows * columns)
    let command = 
        <@
            fun (r:_2D) (a:array<_>) (b:array<_>) (c:array<_>) -> 
                let tx = r.GlobalID0
                let ty = r.GlobalID1
                let mutable buf = c.[ty * columns + tx]
                for k in 0 .. columns - 1 do
                    buf <- buf + (a.[ty * columns + k] * b.[k * columns + tx])
                c.[ty * columns + tx] <- buf
        @>
    let kernel, kernelPrepare, kernelRun = provider.Compile command
    let d =(new _2D(rows, columns, localWorkSize, localWorkSize))
    kernelPrepare d aValues bValues cParallel
    let _ = commandQueue.Add(kernelRun()).Finish()            
    let _ = commandQueue.Add(cParallel.ToHost provider).Finish()
    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    //ignore (System.Console.Read())
    cParallel
printfn "%A" (Main [|1;1;1;1|] [|1;1;1;1|] 2 2)
