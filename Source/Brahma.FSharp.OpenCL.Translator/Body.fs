module Brahma.FSharp.OpenCL.Translator.Body

open Microsoft.FSharp.Quotations

let Translate expr =
        match expr with
        | Patterns.AddressOf expr -> "AdressOf is not suported:" + string expr|> failwith
        | Patterns.AddressSet expr -> "AdressSet is not suported:" + string expr|> failwith
        | Patterns.Application expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Call expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Coerce(expr,sType) -> "Application is not suported:" + string expr|> failwith
        | Patterns.DefaultValue sType -> "Application is not suported:" + string expr|> failwith
        | Patterns.FieldGet (exprOpt,fldInfo) -> "Application is not suported:" + string expr|> failwith
        | Patterns.FieldSet (exprOpt,fldInfo,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.ForIntegerRangeLoop (var,expr1,expr2,expr3) -> "Application is not suported:" + string expr|> failwith
        | Patterns.IfThenElse (cond,thenExpr,elseExpr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Lambda (var,expr)-> "Application is not suported:" + string expr|> failwith
        | Patterns.Let (var,expr1,expr2)-> "Application is not suported:" + string expr|> failwith
        | Patterns.LetRecursive (bindings,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewArray(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewDelegate(sType,vars,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewObject(constrInfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewRecord(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewTuple(exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewUnionCase(unionCaseinfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.PropertyGet(exprOpt,propInfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.PropertySet(exprOpt,propInfo,exprs,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Quote expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Sequential(expr1,expr2) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TryFinally(tryExpr,finallyExpr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TryWith(expr1,var1,expr2,var2,expr3) -> "Application is not suported:" + string expr|> failwith 
        | Patterns.TupleGet(expr,i) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TypeTest(expr,sType) -> "Application is not suported:" + string expr|> failwith
        | Patterns.UnionCaseTest(expr,unionCaseInfo) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Value(_obj,sTpe) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Var(var) -> "Application is not suported:" + string expr|> failwith 
        | Patterns.VarSet(var,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.WhileLoop(condExpr,bodyExpr) -> "Application is not suported:" + string expr|> failwith
        | other -> "OTHER!!! :" + string other |> failwith

