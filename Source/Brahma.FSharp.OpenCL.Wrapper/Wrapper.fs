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

module Brahma.FSharp.OpenCL.Wrapper

open Brahma.OpenCL
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL
open OpenCL.Net
open System

type CLCodeGenerator with
    static member GenerateKernel(lambda: Expr, provider: ComputeProvider, kernel:ICLKernel) =        
        let codeGenerator = new Translator.FSQuotationToOpenCLTranslator()
        let ast = codeGenerator.Translate(lambda)
        let code = Printer.AST.Print ast
        kernel.Source <- kernel.Source.Append(code)
        kernel.SetClosures([||])
        kernel.SetParameters([])
        
type ComputeProvider with
    member this.CompileQuery<'T,'TRange, 'T1 
                                        when 'T :> ICLKernel
                                        and 'T1 :> Brahma.IMem 
                                        and 'TRange : struct 
                                        and 'TRange :> Brahma.OpenCL.INDRangeDimension
                                        and 'TRange :> ValueType 
                                        and 'TRange : (new: unit -> 'TRange)>(lambda:Expr) =
        let kernel = System.Activator.CreateInstance<'T>()
        let r = CLCodeGenerator.GenerateKernel(lambda, this, kernel)
        let str = (kernel :> ICLKernel).Source.ToString()    
        let program, error = Cl.CreateProgramWithSource(this.Context, 1u, [|str|], null)
        let _devices = Array.ofSeq  this.Devices
        let error = Cl.BuildProgram(program, _devices.Length |> uint32, _devices, this.CompileOptionsStr, null, IntPtr.Zero);
        if error <> Cl.ErrorCode.Success
        then 
            let s = _devices |> Array.map (fun device -> Cl.GetProgramBuildInfo(program, device, Cl.ProgramBuildInfo.Log ) |> fst |> string)
            s
            |> String.concat "\n"
            |> failwith
        let clKernel,errpr = Cl.CreateKernel(program, CLCodeGenerator.KernelName)
        (kernel :> ICLKernel).ClKernel <- clKernel
            
        kernel            
                    
    member this.Compile<'TRange,'T1 when 'T1 :> Brahma.IMem                                      
                                    and 'TRange : struct 
                                    and 'TRange :> Brahma.OpenCL.INDRangeDimension
                                    and 'TRange :> ValueType 
                                    and 'TRange : (new: unit -> 'TRange)>
            (query: Expr, ?_options:CompileOptions) =
        let options = defaultArg _options this.DefaultOptions_p
        this.SetCompileOptions(options)        
        this.CompileQuery<Kernel<'TRange, 'T1>,'TRange,'T1>(query)
    