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

module Brahma.FSharp.OpenCL.Core

open Brahma.OpenCL
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation
open Brahma.FSharp.OpenCL
open OpenCL.Net
open System

type CLCodeGenerator() =
    static member KernelName = "brahmaKernel"
    static member GenerateKernel(lambda: Expr, provider: ComputeProvider, kernel:ICLKernel) =        
        let codeGenerator = new Translator.FSQuotationToOpenCLTranslator()
        let ast = codeGenerator.Translate lambda
        let code = Printer.AST.Print ast
        kernel.Provider <- provider
        kernel.Source <- kernel.Source.Append code
        kernel.SetClosures [||]
        kernel.SetParameters []
        
type ComputeProvider with

    member this.CompileQuery<'T when 'T :> ICLKernel>(lambda:Expr) =
        let kernel = System.Activator.CreateInstance<'T>()
        let r = CLCodeGenerator.GenerateKernel(lambda, this, kernel)
        let str = (kernel :> ICLKernel).Source.ToString()    
        let program, error = Cl.CreateProgramWithSource(this.Context, 1u, [|str|], null)
        let _devices = Array.ofSeq  this.Devices
        let error = Cl.BuildProgram(program, _devices.Length |> uint32, _devices, this.CompileOptionsStr, null, IntPtr.Zero)
        if error <> Cl.ErrorCode.Success
        then 
            let s = _devices |> Array.map (fun device -> Cl.GetProgramBuildInfo(program, device, Cl.ProgramBuildInfo.Log ) |> fst |> string)
            s
            |> String.concat "\n"
            |> failwith
        let clKernel,errpr = Cl.CreateKernel(program, CLCodeGenerator.KernelName)
        (kernel :> ICLKernel).ClKernel <- clKernel
            
        kernel            
                    
    member this.Compile (query: Expr<'TRange ->'a> , ?_options:CompileOptions, ?_outCode:string ref) =
        let options = defaultArg _options this.DefaultOptions_p
        this.SetCompileOptions options
        let kernel = this.CompileQuery<Kernel<'TRange>> query
        let rng = ref Unchecked.defaultof<'TRange>
        let args = ref [||]
        let run = ref Unchecked.defaultof<Commands.Run<'TRange>>
        let getStarterFuncton qExpr =
            let rec go expr vars=
                match expr with
                | Patterns.Lambda (v, body) -> 
                    Expr.Lambda(v,go body (v::vars))
                | e -> 
                    let arr =                            
                        let c = Expr.NewArray(typeof<obj>,vars |> List.rev 
                            |> List.map 
                                 (fun v -> Expr.Coerce (Expr.Var(v),typeof<obj>)))
                        <@@ 
                            let x = %%c |> List.ofArray
                            rng := (box x.Head) :?> 'TRange
                            args := x.Tail |> Array.ofList
                            run := kernel.Run(!rng, !args)
                        @@>
                    arr            
            <@ %%(go qExpr []):'TRange ->'a @>.Compile()()
        
        if _outCode.IsSome then (_outCode.Value) := (kernel :> ICLKernel).Source.ToString()

        kernel
        , getStarterFuncton query
        , fun () -> !run
    