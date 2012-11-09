module Brahma.FSharp.OpenCL.Translator.Body

open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.AST

let rec private translateBinding (var:Var) (expr:Expr) targetContext = 
    let body,tContext = Translate expr targetContext
    let vType = Type.Translate(var.Type)
    new VarDecl<Lang>(vType,var.Name,Some body)

and private translateCall exprOpt (mInfo:System.Reflection.MethodInfo) args targetContext =
    mInfo.Name

and private transletaPropGet exprOpt (propInfo:System.Reflection.PropertyInfo) exprs targetContext =
    let x = exprOpt
    match propInfo.Name.ToLowerInvariant() with
    | "globalid0" -> new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int32),"0")]) :> Expression<_>
    | "globalid1" ->  new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int32),"1")]) :> Expression<_>
    | x -> "Unsupported property in kernel: " + x |> failwith

and Translate expr (targetContext:TargetContext<_,_>) =
        match expr with
        | Patterns.AddressOf expr -> "AdressOf is not suported:" + string expr|> failwith
        | Patterns.AddressSet expr -> "AdressSet is not suported:" + string expr|> failwith
        | Patterns.Application expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Call (exprOpt,mInfo,args) -> 
            let r = translateCall exprOpt mInfo args targetContext
            "Application is not suported:" + string expr|> failwith
        | Patterns.Coerce(expr,sType) -> "Application is not suported:" + string expr|> failwith
        | Patterns.DefaultValue sType -> "Application is not suported:" + string expr|> failwith
        | Patterns.FieldGet (exprOpt,fldInfo) -> "Application is not suported:" + string expr|> failwith
        | Patterns.FieldSet (exprOpt,fldInfo,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.ForIntegerRangeLoop (var,expr1,expr2,expr3) -> "Application is not suported:" + string expr|> failwith
        | Patterns.IfThenElse (cond,thenExpr,elseExpr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Lambda (var,expr)-> "Application is not suported:" + string expr|> failwith
        | Patterns.Let (var, expr, inExpr) -> 
            let vDecl = translateBinding var expr targetContext
            targetContext.VarDecls.Add vDecl
            Translate inExpr targetContext
        | Patterns.LetRecursive (bindings,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewArray(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewDelegate(sType,vars,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewObject(constrInfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewRecord(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewTuple(exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewUnionCase(unionCaseinfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.PropertyGet(exprOpt,propInfo,exprs) -> 
            let res = transletaPropGet exprOpt propInfo exprs targetContext
            res,targetContext            
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

