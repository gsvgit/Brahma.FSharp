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
open Brahma.FSharp.OpenCL.Translator.Type

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
            Expr.IfThenElse(go cond, go thenExpr, go elseExpr)        
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
        | other -> other

    and translateApplication expr =
        let rec _go expr =
            match expr with            
            | Patterns.Application (Patterns.Lambda (fv,e),v) ->
                e.Substitute(fun x -> if x = fv then Some v else None) |> _go
            | Patterns.Application (e,v) ->
                let fb = go e
                Expr.Application(fb,v) |> _go 
            | e -> e
        let body = _go expr
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
            Expr.IfThenElse(go cond, go thenExpr, go elseExpr)        
        | Patterns.Sequential(expr1,expr2) -> 
            Expr.Sequential(go expr1, go expr2)        
        | Patterns.VarSet(var,expr) -> 
            Expr.VarSet(var, go expr)
        | Patterns.WhileLoop(condExpr,bodyExpr) -> 
            Expr.WhileLoop(go condExpr, go bodyExpr)            
        | other -> other
    go expr

///////////////////// deploy Let///////////////////////////////

let isLetFun expr =
    match expr with
    | Patterns.Let (var, inExpr, afterExpr) -> 
        match inExpr with
        | ExprShape.ShapeLambda(lv, lb) ->
            true
        | _ ->
            false
    | _ -> false

let rec recLet expr = 
    match expr with
    | Patterns.Let(v, valExpr, inExpr1) ->
//        let e = match valExpr with
        match valExpr with
            | Patterns.Let (var, inExpr, afterExpr) -> 
                let newLet = recLet ( Expr.Let(v, afterExpr, inExpr1))
//                recLet (Expr.Let(var, inExpr, recLet newLet))
                letFunUp (Expr.Let(var, inExpr, newLet))
            | ExprShape.ShapeLambda(lv, lb) ->
                let newLet = recLet valExpr
                match newLet with
                | Patterns.Let (var, inExpr, afterExpr) -> 
                    let newLetIn = (Expr.Let(v, afterExpr , inExpr1))
//                    recLet (Expr.Let(var, inExpr, recLet newLetIn))
                    letFunUp (Expr.Let(var, inExpr, newLetIn))
                | _ ->
                    Expr.Let(v, newLet , inExpr1) 
            | _ -> 
                Expr.Let(v, recLet valExpr, inExpr1)
    | ExprShape.ShapeVar(var) ->
        expr           
    | ExprShape.ShapeLambda(lv, lb1) ->
        match lb1 with
        | ExprShape.ShapeLambda(lv1, lb) ->
            let lb = recLet lb1
            match lb with
            | Patterns.Let (var, inExpr, afterExpr) ->
                if(isLetFun lb) then
                    Expr.Let(var, inExpr, (Expr.Lambda(lv, afterExpr)))
                else
                    Expr.Lambda(lv, lb)
            | _ -> 
//                let newExpr = recLet (lb)
//                match newExpr with
//                | Patterns.Let (var, inExpr, afterExpr) -> 
//                    if(isLetFun newExpr) then
//                        Expr.Let(var, inExpr, (Expr.Lambda(lv, afterExpr)))
//                    else
//                        Expr.Lambda(lv, newExpr)
//                | _ ->
//                    Expr.Lambda(lv, newExpr)
                Expr.Lambda(lv, lb)
        | _ -> 
            let funUpLB = letFunUp lb1
            match funUpLB with
            | Patterns.Let (var, inExpr, afterExpr) ->
                if(isLetFun funUpLB) then
                    Expr.Let(var, inExpr, (Expr.Lambda(lv, afterExpr)))
                else
                    Expr.Lambda(lv, funUpLB)
            | _ -> 
                Expr.Lambda(lv, funUpLB)
//                let newExpr = recLet (funUpLB)
//                match newExpr with
//                | Patterns.Let (var, inExpr, afterExpr) -> 
//                    if(isLetFun newExpr) then
//                        Expr.Let(var, inExpr, (Expr.Lambda(lv, afterExpr)))
//                    else
//                        Expr.Lambda(lv, newExpr)
//                | _ ->
//                    Expr.Lambda(lv, newExpr)
    | ExprShape.ShapeCombination(o, args) ->
        ExprShape.RebuildShapeCombination(o, List.map (fun (e:Expr) -> recLet (e)) args)

and letFunUp expr =
    match expr with
    | Patterns.Let(var, inExpr, body) ->
        let recResOutLetFun = recLet expr
        match recResOutLetFun with
        | Patterns.Let(v, iE, b) ->
            let retFunUp = letFunUp b
            if(isLetFun recResOutLetFun) then
                Expr.Let(v, iE, retFunUp)
            else
                match retFunUp with
                | Patterns.Let(vF, iEF, bF) ->
                    if(isLetFun retFunUp) then
                        let newAfterExpr = Expr.Let(v, iE, bF)
                        Expr.Let(vF, iEF, letFunUp newAfterExpr)
                    else 
                        let fUp = letFunUp retFunUp
                        match fUp with
                        | Patterns.Let (var2, inExpr2, afterExpr2) ->
                            if(isLetFun fUp) then
                                let newAfterExpr = Expr.Let(v, iE, afterExpr2)
                                Expr.Let(var2, inExpr2, letFunUp newAfterExpr)
                            else 
                                Expr.Let (v, iE, fUp)
                        | _ -> Expr.Let (v, iE, fUp)
                | _ -> recResOutLetFun
        | _ -> recResOutLetFun
    | Patterns.IfThenElse (cond, thenExpr, elseExpr) ->
        let newCond = cond
        let newThenExpr = letFunUp thenExpr
        match newThenExpr with
        | Patterns.Let (var2, inExpr2, afterExpr2) ->
            if(isLetFun newThenExpr) then
                let newAfterExpr = Expr.IfThenElse(cond, afterExpr2, elseExpr)
                Expr.Let(var2, inExpr2, recLet newAfterExpr)
            else 
                let newElseExpr = letFunUp elseExpr
                match newElseExpr with
                | Patterns.Let (var1, inExpr1, afterExpr1) ->
                    if(isLetFun newElseExpr) then
                        let newAfterExpr = Expr.IfThenElse(cond, thenExpr, afterExpr1)
                        Expr.Let(var1, inExpr1, recLet newAfterExpr)
                    else
                        Expr.IfThenElse(cond, thenExpr,elseExpr)
                | _ ->
                    Expr.IfThenElse(cond, thenExpr,elseExpr)
        | _ -> 
            let newElseExpr = letFunUp elseExpr
            match newElseExpr with
            | Patterns.Let (var1, inExpr1, afterExpr1) ->
                if(isLetFun newElseExpr) then
                    let newAfterExpr = Expr.IfThenElse(cond, thenExpr, afterExpr1)
                    Expr.Let(var1, inExpr1, recLet newAfterExpr)
                else
                    Expr.IfThenElse(cond, thenExpr,elseExpr)
            | _ ->
                Expr.IfThenElse(cond, thenExpr,elseExpr)
    | _ -> expr
                


let rec quontationTransformerRec expr = 
    match expr with
    | ExprShape.ShapeLambda(lv, lb) ->
        Expr.Lambda(lv, quontationTransformerRec lb)
    | _ ->
        //recLet expr
        letFunUp expr

let renameAllTree expr (letScope:LetScope) (renamer1:Renamer) = 
    let rec renameRec expr =
        match expr with
        | Patterns.Lambda(param, body) ->
            let newName = renamer1.addName (param.Name)
            let NewVar = new Var(newName, param.Type, param.IsMutable)

            letScope.AddVarInLastLet param.Name newName param.Type

            let newLambda = Expr.Lambda(NewVar, renameRec body)
            newLambda
        | Patterns.Let(var, expr1, expr2) ->
            let newName = renamer1.addName (var.Name)
            let NewVar = new Var(newName, var.Type, var.IsMutable)

            let isFun = 
                match expr1 with 
                | Patterns.Lambda(param, body) ->
                    true
                | _ -> false

            let nameInFun = 
                if(isFun) then
                    newName
                else
                    letScope.GetLastInFunLet

            letScope.LetIn var.Name newName var isFun nameInFun

            let prevStateIsInLet = letScope.GetIsInLastLet
            letScope.SetIsInLastLet true
                
            let exprIn = renameRec expr1

            if(isFun) then
                letScope.FunLetOut |> ignore

            letScope.SetIsInLastLet false
            let exprAfter = renameRec expr2
            letScope.SetIsInLastLet prevStateIsInLet
            letScope.LetOut |> ignore
            let newLet = Expr.Let(NewVar, exprIn, exprAfter)
            newLet
        | Patterns.ForIntegerRangeLoop (i, from, _to, _do) ->
            let newName = renamer1.addName (i.Name)
            let NewVar = new Var(newName, i.Type, i.IsMutable)

            let nameInFun = letScope.GetLastInFunLet

            letScope.LetIn i.Name newName i false nameInFun

//            letScope.AddForVars (new VarInfo(i.Name, NewVar.Name, true, NewVar.Type))
            
            let newFrom = renameRec from
            let newTo = renameRec _to
            let newDo = renameRec _do
            letScope.LetOut |> ignore
//            letScope.RemoveForVar |> ignore
            Expr.ForIntegerRangeLoop(NewVar, newFrom, newTo, newDo)
        | Patterns.Application(expr1, expr2) ->
            Expr.Application(renameRec expr1, renameRec expr2)
        | Patterns.Call(exprOpt, methodInfo, exprList) ->
            let newExprList = [for expr in exprList -> renameRec expr]
            Expr.Call(methodInfo, newExprList)
        | Patterns.Var(var) ->
            let newName = (letScope.GetNameForVarInLet var.Name (not letScope.GetIsInLastLet)).GetNewName
            let newVar = new Var(newName, var.Type, var.IsMutable)
            Expr.Var(newVar)
        | ExprShape.ShapeCombination (o, exprs) -> ExprShape.RebuildShapeCombination (o,List.map renameRec exprs)
        | other -> "OTHER!!! :" + string other |> failwith

    let rec quontationRenamerLetRec expr =
        match expr with
        | ExprShape.ShapeLambda(lv, lb) ->
            let newName = renamer1.addName (lv.Name)
            let NewVar = new Var(newName, lv.Type, lv.IsMutable)
            letScope.AddKernelVars lv.Name newName lv.Type

            let newLambda = Expr.Lambda(NewVar, quontationRenamerLetRec lb)
            newLambda
            //Expr.Lambda(lv, quontationRenamerLetRec lb)
        | _ ->
            renameRec expr
    quontationRenamerLetRec expr


let addNeededLamAndAppicatins expr (letScope:LetScope) =
    let rec addNeededLam expr =
        match expr with
        | ExprShape.ShapeVar(var) ->
            if(letScope.ContainsInfo var.Name) then
                let listNeededVars = ((letScope.GetLetInfo var.Name).GetNeedVars)
                if(listNeededVars.Count > 0) then
                    let mutable readyLet = Expr.Var((letScope.GetLetInfo var.Name).GetOrgnVar)
                    for elem in listNeededVars do
                        readyLet <- Expr.Application(readyLet, Expr.Var(new Var(elem.GetNewName, elem.GetVarType)))
                    readyLet
                else
                    expr 
             else
                expr          
        | Patterns.Let(var, expr1, expr2) ->
            let listNeededVars = (letScope.GetLetInfo var.Name).GetNeedVars
            if(listNeededVars.Count > 0) then
                let mutable readyLet = addNeededLam expr1
                listNeededVars.Reverse()
                for elem in listNeededVars do
                    readyLet <- Expr.Lambda(new Var(elem.GetNewName, elem.GetVarType), readyLet)
                let newVar = new Var(var.Name, readyLet.Type)
                (letScope.GetLetInfo var.Name).ChangeOrgnVar newVar
                listNeededVars.Reverse()
                Expr.Let( newVar, readyLet, addNeededLam expr2)
            else
                let ex1 = addNeededLam expr1
                let ex2 = addNeededLam expr2
                let newLet = Expr.Let(var, ex1, ex2)
                newLet
        | Patterns.Application(expr1, expr2) ->
            let exp1 = addNeededLam expr1
            let exp2 = addNeededLam expr2

            let mutable readyApp = Expr.Application(exp1, exp2)
            readyApp

        | ExprShape.ShapeLambda(lv, lb) ->
            let newExpr = addNeededLam (lb)
            Expr.Lambda(lv, newExpr)
        | ExprShape.ShapeCombination(o, args) ->
            ExprShape.RebuildShapeCombination(o, List.map (fun (e:Expr) -> addNeededLam (e)) args)

    let rec run expr =
        match expr with
        | ExprShape.ShapeLambda(lv, lb) ->
            (Expr.Lambda(lv, run lb))
        | _ ->
            addNeededLam expr
    run expr


//get list expr for translation
let getListLet expr =
    let listExpr = new ResizeArray<_>()
    let rec addLetInList expr =
        match expr with
        | Patterns.Let(var, exprIn, exprAfter) ->
            //let v = Expr.Var(new Var("some", var.Type))
            if(isLetFun expr) then
                let newLet = Expr.Let(var, exprIn, Expr.Value(0)) // in  body some value
                listExpr.Add(newLet)
                addLetInList exprAfter
            else 
                expr
        | _ ->
            expr
    let rec firstLams expr =
        match expr with
        | Patterns.Lambda(lv, lb) ->
            Expr.Lambda(lv, firstLams lb)
        | _ ->
            addLetInList expr
    
    let listExpr = match expr with
        | Patterns.Lambda(lv, lb) ->
            listExpr.Add(firstLams (Expr.Lambda(lv, lb)))
            listExpr
        | _ -> 
            listExpr

    let resList = ResizeArray<_>()
    for elem in listExpr do
        match elem with
        | Patterns.Let(v, e, b) ->
            let newMethod = new Method(v,e)
            resList.Add(newMethod)
        | Patterns.Lambda(lv, lb) ->
            let newVar = new Var("brahmaKernel", lv.Type, false)
            let newMethod = new Method(newVar, elem)
            resList.Add(newMethod)
    resList

let quontationTransformer expr translatorOptions = 
////    let caller = new Caller()
//    let context = new TargetContext<_,_>()
//    context.Namer.LetIn()
//    context.TranslatorOptions.AddRange translatorOptions

    let renamer = new Renamer()
    let letScope = LetScope()
    let renamedTree = renameAllTree expr letScope renamer

//    printf "\n%A" renamedTree

//    quontationTransformerRec expr context caller

    let qTransd = quontationTransformerRec renamedTree

//    printf "\n%A" qTransd
    let addedLam = addNeededLamAndAppicatins qTransd letScope

//    printf "\n%A\n\n" addedLam

//    let qq = <@ fun (m:int) -> 
//                        let g k m = k + m + m
//                        let z t m a = m + a - t
//                        let y a n m l = 
//                            let x = 5 - n + (g 4 m)
//                            z (a + x + l) m a
//                        let x n m = 
//                            let l = m
//                            let r = 
//                                y 6 n m l
//                            r + m
//                        let p = m
//                        x 7 m
//            @>
//
//    let res =   match addedLam with
//                | Patterns.Lambda(lv, lb) ->
//                    (qq.Equals(lb))
//    printf "\n%A\n" res



    //addedLam
    //printf "\n%A" addedLam
    let listExpr = getListLet addedLam
    listExpr
//
//let b = fun m -> 
//            let x = 
//                let y a = m + 5
//                y 6
//            x
//
//
//let y' a m = m + 5
//let b1 = fun m -> 
//            let x = 
//                y 6 m
//            x
//
//
//let a = fun m -> 
//            let x n = 
//                let l = 0
//                let g k = k + m
//                let r = 
//                    let y a = 
//                        let z t = m + a - t
//                        let x = 5 - n + (g 4)
//                        z (a + x + l)
//                    y 6
//                r + m
//            x
//
//let a = fun m -> 
//            let x n = 
//                let l = 0
//                let g k = k + m + m
//                let r = 
//                    let y a = 
//                        let z t = m + a - t
//                        let x = 5 - n + (g 4)
//                        z (a + x + l)
//                    y 6
//                r + m
//            x
//
//let z t m a = m + a - t
//
//let y a m = 
//    z a m a
//
//let a = fun m -> 
//            let x = 
//                y 6 m
//            x
