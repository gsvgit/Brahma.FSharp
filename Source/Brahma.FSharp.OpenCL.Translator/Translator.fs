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

namespace Brahma.FSharp.OpenCL.Translator

open Microsoft.FSharp.Quotations
open  Brahma.FSharp.OpenCL.AST

type FSQuotationToOpenCLTranslator() =
   
    let CollectStructs e =
        let structs = new System.Collections.Generic.Dictionary<System.Type, _> () 
        let  add (t:System.Type) =
            if ((t.IsValueType && not t.IsPrimitive && not t.IsEnum) (*|| t.Name.StartsWith "Tuple`"*)) && not (structs.ContainsKey(t))
            then structs.Add(t, ())
        let rec go (e: Expr) = 
            add e.Type
            match e with
            | ExprShape.ShapeVar(v) -> ()
            | ExprShape.ShapeLambda(v, body) -> go body            
            | ExprShape.ShapeCombination(o, l) ->
                o.GetType() |> add 
                List.iter go l
        go e
        structs

    
    /// The parameter 'vars' is an immutable map that assigns expressions to variables
    /// (as we recursively process the tree, we replace all known variables)
    let rec expand vars expr = 

      // First recursively process & replace variables
      let expanded = 
        match expr with
        // If the variable has an assignment, then replace it with the expression
        | ExprShape.ShapeVar v when Map.containsKey v vars -> vars.[v]
        // Apply 'expand' recursively on all sub-expressions
        | ExprShape.ShapeVar v -> Expr.Var v
        | Patterns.Call(body, DerivedPatterns.MethodWithReflectedDefinition meth, args) ->
            let this = match body with Some b -> Expr.Application(meth, b) | _ -> meth
            let res = Expr.Applications(this, [ for a in args -> [a]])
            expand vars res
        | ExprShape.ShapeLambda(v, expr) -> 
            Expr.Lambda(v, expand vars expr)
        | ExprShape.ShapeCombination(o, exprs) ->
            ExprShape.RebuildShapeCombination(o, List.map (expand vars) exprs)

      // After expanding, try reducing the expression - we can replace 'let'
      // expressions and applications where the first argument is lambda
      match expanded with
      | Patterns.Application(ExprShape.ShapeLambda(v, body), assign)
      | Patterns.Let(v, assign, body) ->
          expand (Map.add v (expand vars assign) vars) body
      | _ -> expanded


    let mainKernelName = "brahmaKernel"
    let brahmaDimensionsTypes = ["_1d";"_2d";"_3d"]
    let brahmaDimensionsTypesPrefix = "brahma.opencl."
    let bdts = brahmaDimensionsTypes |> List.map (fun s -> brahmaDimensionsTypesPrefix + s)
    let buildFullAst vars types partialAst (context:TargetContext<_,_>) =
        let formalArgs = 
            vars |> List.filter (fun (v:Var) -> bdts |> List.exists((=) (v.Type.FullName.ToLowerInvariant())) |> not)
            |> List.map 
                (fun v -> 
                    let t = Type.Translate v.Type true None context
                    new FunFormalArg<_>(t :? RefType<_> , v.Name, t))
        let mainKernelFun = new FunDecl<_>(true, mainKernelName, new PrimitiveType<_>(Void), formalArgs,partialAst)
        let pragmas = 
            let res = new ResizeArray<_>()
            if context.Flags.enableAtomic then
                res.Add(new CLPragma<_>(CLGlobalInt32BaseAtomics) :> TopDef<_>)
                res.Add(new CLPragma<_>(CLLocalInt32BaseAtomics) :> TopDef<_>)
            if context.Flags.enableFP64 then
                res.Add(new CLPragma<_>(CLFP64))
            List.ofSeq res
        new AST<_>(pragmas @ types @ [mainKernelFun])

    let translate qExpr translatorOptions =
        let structs = CollectStructs qExpr
        let context = new TargetContext<_,_>()
        let translatedStructs = Type.TransleteStructDecls structs.Keys context |> Seq.cast<_> |> List.ofSeq
        //let qExpr = expand Map.empty qExpr
        let rec go expr vars =
            match expr with
            | Patterns.Lambda (v, body) -> go body (v::vars)
            | e -> 
                let body =
                    let b,context =
                        context.Namer.LetIn()
                        context.TranslatorOptions.AddRange translatorOptions
                        vars |> List.iter (fun v -> context.Namer.AddVar v.Name)
                        let newE = e |> QuotationsTransformer.inlineLamdas |> QuotationsTransformer.apply
                        printfn "%A" e
                        Body.Translate newE context
                    match b  with
                    | :? StatementBlock<Lang> as sb -> sb
                    | :? Statement<Lang> as s -> new StatementBlock<_>(new ResizeArray<_>([s]))
                    | _ -> failwithf "Incorrect function body: %A" b
                    ,context 
                vars, body

            | x -> "Incorrect OpenCL quotation: " + string x |> failwith
        let vars,(partialAst,context) = go qExpr []
        buildFullAst (List.rev vars) translatedStructs (partialAst :> Statement<_>) context
  
    member this.Translate qExpr translatorOptions = 
        let ast = translate qExpr translatorOptions
        ast