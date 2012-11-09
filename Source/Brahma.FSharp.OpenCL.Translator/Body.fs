module Brahma.FSharp.OpenCL.Translator.Body

open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.AST

let rec private translateBinding (var:Var) (expr:Expr) targetContext = 
    let body,tContext = TranslateAsExpr expr targetContext
    let vType = Type.Translate(var.Type)
    new VarDecl<Lang>(vType,var.Name,Some body)

and private translateCall exprOpt (mInfo:System.Reflection.MethodInfo) args targetContext =
    let args,tContext = 
        let a,c = args |> List.fold (fun (res,tc) a -> let r,tc = TranslateAsExpr a tc in r::res,tc) ([],targetContext)
        a |> List.rev , c
    match mInfo.Name.ToLowerInvariant() with
    | "op_multiply" -> new Binop<_>(Mult,args.[0],args.[1]),tContext
    | "op_addition" -> new Binop<_>(Plus,args.[0],args.[1]),tContext
    | c -> failwithf "Unsupporte call: %s" c

and private itemHelper exprs hostVar tContext =
    let idx,tContext = 
        match exprs with
        | hd::_ -> TranslateAsExpr hd tContext
        | [] -> failwith "Array index missed!"
    let hVar = 
        match hostVar with
        | Some(v,_) -> v
        | None -> failwith "Host var missed!"
    idx,tContext,hVar

and private transletaPropGet exprOpt (propInfo:System.Reflection.PropertyInfo) exprs targetContext =
    let hostVar = exprOpt |> Option.map(fun e -> TranslateAsExpr e targetContext)
    match propInfo.Name.ToLowerInvariant() with
    | "globalid0" -> new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int32),"0")]) :> Expression<_>, targetContext
    | "globalid1" ->  new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int32),"1")]) :> Expression<_>, targetContext
    | "item" -> 
        let idx,tContext,hVar = itemHelper exprs hostVar targetContext
        new Item<_>(hVar,idx) :> Expression<_>, tContext
    | x -> "Unsupported property in kernel: " + x |> failwith

and private transletaPropSet exprOpt (propInfo:System.Reflection.PropertyInfo) exprs newVal targetContext =    
    let hostVar = exprOpt |> Option.map(fun e -> TranslateAsExpr e targetContext)
    let newVal,tContext = TranslateAsExpr newVal (match hostVar with Some(v,c) -> c | None -> targetContext)
    match propInfo.Name.ToLowerInvariant() with
    | "item" -> 
        let idx,tContext,hVar = itemHelper exprs hostVar tContext(* = 
            match exprs with
            | hd::_ -> TranslateAsExpr hd tContext
            | [] -> failwith "Array index missed!"
        let hVar = 
            match hostVar with
            | Some(v,_) -> v
            | None -> failwith "Host var missed!"*)
        let item = new Item<_>(hVar,idx)        
        new Assignment<_>(new Property<_>(PropertyType.Item(item)),newVal) :> Statement<_>
        , tContext
    | x -> "Unsupported property in kernel: " + x |> failwith

and TranslateAsExpr expr (targetContext:TargetContext<_,_>) =
    let (r:Node<_>),tc = Translate expr (targetContext:TargetContext<_,_>)
    (r  :?> Expression<_>) ,tc

and translateVar (var:Var) =
    new Variable<_>(var.Name)

and Translate expr (targetContext:TargetContext<_,_>) =
        match expr with
        | Patterns.AddressOf expr -> "AdressOf is not suported:" + string expr|> failwith
        | Patterns.AddressSet expr -> "AdressSet is not suported:" + string expr|> failwith
        | Patterns.Application expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Call (exprOpt,mInfo,args) -> 
            let r,tContext = translateCall exprOpt mInfo args targetContext
            r :> Node<_>,tContext
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
            let res,tContext = Translate inExpr targetContext
            let sb = new ResizeArray<_>(tContext.VarDecls |> Seq.cast<Statement<_>>)
            sb.Add (res :?> Statement<_>)
            new StatementBlock<_>(sb) :> Node<_>, tContext

        | Patterns.LetRecursive (bindings,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewArray(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewDelegate(sType,vars,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewObject(constrInfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewRecord(sType,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewTuple(exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.NewUnionCase(unionCaseinfo,exprs) -> "Application is not suported:" + string expr|> failwith
        | Patterns.PropertyGet(exprOpt,propInfo,exprs) -> 
            let res, tContext = transletaPropGet exprOpt propInfo exprs targetContext
            (res :> Node<_>), tContext
        | Patterns.PropertySet(exprOpt,propInfo,exprs,expr) -> 
            let res,tContext = transletaPropSet exprOpt propInfo exprs expr targetContext
            res :> Node<_>,tContext            
        | Patterns.Quote expr -> "Application is not suported:" + string expr|> failwith
        | Patterns.Sequential(expr1,expr2) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TryFinally(tryExpr,finallyExpr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TryWith(expr1,var1,expr2,var2,expr3) -> "Application is not suported:" + string expr|> failwith 
        | Patterns.TupleGet(expr,i) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TypeTest(expr,sType) -> "Application is not suported:" + string expr|> failwith
        | Patterns.UnionCaseTest(expr,unionCaseInfo) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Value(_obj,sTpe) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Var var -> translateVar var :> Node<_>, targetContext
        | Patterns.VarSet(var,expr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.WhileLoop(condExpr,bodyExpr) -> "Application is not suported:" + string expr|> failwith
        | other -> "OTHER!!! :" + string other |> failwith

