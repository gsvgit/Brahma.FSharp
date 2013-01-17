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

module Brahma.FSharp.OpenCL.Wrapper

open Brahma.OpenCL
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation
open Brahma.FSharp.OpenCL
open OpenCL.Net
open System

//type t () =
//    inherit ``[]``<int>()

type CLCodeGenerator() =
    static member KernelName = "brahmaKernel"
    static member GenerateKernel(lambda: Expr, provider: ComputeProvider, kernel:ICLKernel) =        
        let codeGenerator = new Translator.FSQuotationToOpenCLTranslator()
        let ast = codeGenerator.Translate lambda
        let code = Printer.AST.Print ast
        kernel.Source <- kernel.Source.Append code
        kernel.SetClosures [||]
        kernel.SetParameters []
type TTT () = 
    member this.cBox x = box x
        
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
                    
//    member this.Compile
//            (query: Expr<'TRange -> _ -> unit>, ?_options:CompileOptions) =
//        let options = defaultArg _options this.DefaultOptions_p
//        this.SetCompileOptions options
//        this.CompileQuery<Kernel<'TRange, _>> query
//
//    member this.Compile
//            (query: Expr<'TRange -> _ -> _ -> unit>, ?_options:CompileOptions) =
//        let options = defaultArg _options this.DefaultOptions_p
//        this.SetCompileOptions options
//        this.CompileQuery<Kernel<'TRange, _, _>> query
//
//    member this.Compile
//            (query: Expr<'TRange -> _ -> _ -> _ -> unit>, ?_options:CompileOptions) =
//        let options = defaultArg _options this.DefaultOptions_p
//        this.SetCompileOptions options
//        this.CompileQuery<Kernel<'TRange, _, _, _>> query
            
    member this.Compile
            (query: Expr<'TRange ->'a> , ?_options:CompileOptions, ?_outCode:string ref) =
        //let provider = ref (Unchecked.defaultof<ComputeProvider>) 
        let configure (a:array<'tr>) = new Buffer<'tr>(this, Operations.ReadWrite, Memory.Device, a)|> box            
        let (x:array<'z> ref) = ref [||]
        let b  = <@ box !x @>
        let replaceBody qExpr =
            let garr = ref [||]
            let rec go expr vars=
                match expr with
                | Patterns.Lambda (v, body) -> 
                    Expr.Lambda(v,go body (v::vars))
                | e -> 
                    let arr =                            
                        let c = Expr.NewArray(typeof<obj>,vars |> List.rev 
                            |> List.map 
                                 (fun v ->
//                                    if v.Type.Name.EndsWith "[]"
//                                    then                                        
//                                        let e = Expr.Var(v) in  <@@ configure (%%e:array<_>) @@>
//                                    else Expr.Coerce (Expr.Var(v),typeof<obj>) ))
                                            Expr.Coerce (Expr.Var(v),typeof<obj>)))
//                                            let vv = Expr.Var v
//                                            
//                                            let mi = typeof<TTT>.GetMethod("cBox")
//                                            let p  = Expr.Call(mi,[vv])
//                                            p ))
//                                            let vv = Expr.Var v
//                                            if v.Type.Name.EndsWith "[]"
//                                            then
//                                                <@@ x := (%%vv:array<'z>) ; %b  @@>
//                                            else Expr.Coerce (Expr.Var(v),typeof<obj>)))
                                            //<@@ %b:'bb when 'bb :> obj) @@>))
                        <@@ garr := %%c @@>
                    arr
            let nb = go qExpr []
            nb,garr
        let kernel = this.CompileQuery<Kernel<'TRange>> query
        if _outCode.IsSome then (_outCode.Value) := (kernel :> ICLKernel).Source.ToString()
//let rng,vars = 
//    let v = vars |> List.rev
//    v.Head,v.Tail
//let args = Expr.NewArray(typeof<obj>, vars |> List.map (fun v -> Expr.Coerce (Expr.Var(v),typeof<obj>)))
//let rng = Expr.Var rng 
//<@@ f %%rng %%args @@>

        let ff,a = replaceBody query |> fun (x,a) -> <@ %%x:'TRange ->'a @>,a

        let f = ff.Compile()()         

        let options = defaultArg _options this.DefaultOptions_p
        this.SetCompileOptions options
        //fun tr arr -> (this.CompileQuery<Kernel<'TRange>> query).Run(tr,arr)
        f, (fun (configuredBuffers:array<_>) -> 
                let x = !a |> List.ofArray
                let count = ref 0
                let rng = (box x.Head) :?> 'TRange
                let vars = 
                    x.Tail 
                    |> List.map 
                        (fun (x:obj) ->                             
                            //if x.GetType().Name.EndsWith "[]"
                            //then 
                            let a =  try (box x) :? System.Array |> Some with _ -> None
                            //let ff = (# "!0[]" #)
                            match x with
                            | :? System.Array as ar -> 
                                let v = configuredBuffers.[!count]
                                incr count
                                v
                                //let t = ar.GetType().GetElementType()
                                //let active (c:'t) = 
                                  // ar :?> array<'t>
                                //let en = ar.GetEnumerator()
                                //en.MoveNext()|>ignore
                                //let vv = en.Current
                                //let g = active vv
                                //let ddd = System.Convert.ChangeType(box ar, typeof<array<'t>>)
                                //let arr = ar :?> array<'t> //.ofSeq (ar :> System.Collections.Generic.IList<_>)
                                //new Buffer<_>(provider, Operations.ReadWrite, Memory.Device,ar)|> box

                            //| :? <'a>``[]`` as ar-> new Buffer<'a>(provider, Operations.ReadWrite, Memory.Device, ar)|> box
                            //| :? ``[]``<'a> as ar-> new Buffer<'a>(provider, Operations.ReadWrite, Memory.Device, ar)|> box
                            | _ -> x)
                kernel.Run(rng, vars |> Array.ofList))
    