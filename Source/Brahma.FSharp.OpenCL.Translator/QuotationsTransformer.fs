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

module Brahma.FSharp.OpenCL.Translator.QuotationsTransformer

open Microsoft.FSharp.Quotations

let apply (expr:Expr) =
    let rec go expr = 
        match expr with    
        | Patterns.Application (expr1,expr2) -> translateApplication expr
        | Patterns.Call (exprOpt,mInfo,args) -> 
            match exprOpt with
            | Some e -> Expr.Call(go e, mInfo, args |> List.map go)
            | None -> Expr.Call(mInfo, args |> List.map go)
        | Patterns.ForIntegerRangeLoop (i, from, _to, _do) ->
            Expr.ForIntegerRangeLoop(i, from, _to, go _do)            
        | Patterns.IfThenElse (cond, thenExpr, elseExpr) ->
            Expr.IfThenElse(cond, go thenExpr, go elseExpr)        
        | Patterns.Let (var, expr, inExpr) ->
            Expr.Let(var, go expr, go inExpr)
        | Patterns.Sequential(expr1,expr2) -> 
            Expr.Sequential(go expr1, go expr2)        
        | Patterns.VarSet(var,expr) -> 
            Expr.VarSet(var, go expr)
        | Patterns.WhileLoop(condExpr,bodyExpr) -> 
            Expr.WhileLoop(go condExpr, go bodyExpr)
        | Patterns.Lambda(v,e) ->
            Expr.Lambda(v,go e)
        | other -> 
            other |> string |> printfn "%A"
            other

    and translateApplication expr =
        let rec go expr =
            match expr with            
            | Patterns.Application (Patterns.Lambda (fv,e),v) ->
                e.Substitute(fun x -> if x = fv then Some v else None) |> go
            | e -> e
        let body = go expr
        body

    go expr

let inlineLamdas (expr:Expr) =
    let rec go expr = 
        match expr with
        | Patterns.Let(var, expr, inExpr) ->
            match expr with
            | Patterns.Lambda _ ->
                inExpr.Substitute(fun x -> if x = var then Some expr else None) |> go
            | Patterns.Application _ -> Expr.Let(var, go expr |> apply, inExpr) |> go
            | e -> Expr.Let(var, expr, go inExpr)
        | Patterns.Application (expr1,expr2) -> Expr.Application (go expr1 |> apply, go expr2)
        | Patterns.Call (exprOpt,mInfo,args) ->
            match exprOpt with
            | Some e -> Expr.Call(go e, mInfo, args |> List.map go)
            | None -> Expr.Call(mInfo, args |> List.map go)
        | Patterns.ForIntegerRangeLoop (i, from, _to, _do) ->
            Expr.ForIntegerRangeLoop(i, from, _to, go _do)            
        | Patterns.IfThenElse (cond, thenExpr, elseExpr) ->
            Expr.IfThenElse(cond, go thenExpr, go elseExpr)        
        | Patterns.Sequential(expr1,expr2) -> 
            Expr.Sequential(go expr1, go expr2)        
        | Patterns.VarSet(var,expr) -> 
            Expr.VarSet(var, go expr)
        | Patterns.WhileLoop(condExpr,bodyExpr) -> 
            Expr.WhileLoop(go condExpr, go bodyExpr)            
        | other -> other
    go expr
