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

namespace Brahma.FSharp.OpenCL.Translator

open Microsoft.FSharp.Quotations
open  Brahma.FSharp.OpenCL.AST

type FSQuotationToOpenCLTranslator() =
   
    let mainKernelName = "brahmaKernel"
    let brahmaDimensionsTypes = ["_1d";"_2d";"_3d"]
    let brahmaDimensionsTypesPrefix = "brahma.opencl."
    let bdts = brahmaDimensionsTypes |> List.map (fun s -> brahmaDimensionsTypesPrefix + s)
    let buildFullAst vars partialAst context =
        let formalArgs = 
            vars |> List.filter (fun (v:Var) -> bdts |> List.exists((=) (v.Type.FullName.ToLowerInvariant())) |> not)
            |> List.map (fun v -> new FunFormalArg<_>(true, v.Name, Type.Translate(v.Type)))
        let mainKernelFun = new FunDecl<_>(true, mainKernelName, new PrimitiveType<_>(Void), formalArgs,partialAst)
        new AST<_>([mainKernelFun])

    let translate qExpr =
        let rec go expr vars =
            match expr with
            | Patterns.Lambda (v, (Patterns.Lambda (_) as body)) -> 
                go body (v::vars)
            | Patterns.Lambda (v, e) -> v::vars, Body.Translate e (new TargetContext<Lang,_>())
            | x -> "Incorrect OpenCL quotation: " + string x |> failwith
        let vars,(partialAst,context) = go qExpr []
        buildFullAst (List.rev vars) (partialAst :?> Statement<_>) context

  
    member this.Translate qExpr = 
        let ast = translate qExpr
        ast
//        []
////          , "__kernel void brahmaKernel(__global float* a,__global float* b,__global float* c,int columns) "
////            + "\n{int tx = get_global_id(0);"
////            + "\nint ty = get_global_id(1);"
////            + "\nfloat value = ((float)0);"
////            + "\nfor (int k = ((int)0);"
////            + "\n k < columns;"
////            + "\n k++) "
////            + "\n{float elementA = a[((ty * columns) + k)];"
////            + "\nfloat elementB = b[((k * columns) + tx)];"
////            + "\n(value = (value + (elementA * elementB)));;;};"
////            + "\n(c[((ty * columns) + tx)] = value);;}" 
//        ,"__kernel void brahmaKernel(__global int* a) {int tx = get_global_id(0); a[tx] = a[tx] * 2;}" 

