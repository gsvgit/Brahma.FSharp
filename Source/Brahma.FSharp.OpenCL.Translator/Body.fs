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
    | "op_multiply"            -> new Binop<_>(Mult,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_addition"            -> new Binop<_>(Plus,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_division"            -> new Binop<_>(Div,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_lessthan"            -> new Binop<_>(Less,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_lessthanorequal"     -> new Binop<_>(LessEQ,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_greaterthan"         -> new Binop<_>(Great,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_greaterthanorequal"  -> new Binop<_>(GreatEQ,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_equality"            -> new Binop<_>(EQ,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_inequality"          -> new Binop<_>(NEQ,args.[0],args.[1]) :> Statement<_>,tContext
    | "op_subtraction"         -> new Binop<_>(Minus,args.[0],args.[1]) :> Statement<_>,tContext
    | "todouble"               -> new Cast<_>( args.[0],new PrimitiveType<_>(PTypes<_>.Float)):> Statement<_>,tContext
    | "toint"                  -> new Cast<_>( args.[0],new PrimitiveType<_>(PTypes<_>.Int)):> Statement<_>,tContext
    | "tosingle"               -> new Cast<_>( args.[0],new PrimitiveType<_>(PTypes<_>.Float)):> Statement<_>,tContext
    | "acos" | "asin" | "atan"
    | "cos" | "cosh" | "exp"
    | "floor" | "log" | "log10"
    | "pow" | "sin" | "sinh" 
    | "tan" | "tanh" as fName ->
        if mInfo.DeclaringType.AssemblyQualifiedName.StartsWith("System.Math")
        then FunCall<_>(fName,args) :> Statement<_>,tContext
        else failwithf "Seems, thet you use math function with name %s not from System.Math." fName
    | "setarray" -> 
        let item = new Item<_>(args.[0],args.[1])
        new Assignment<_>(new Property<_>(PropertyType.Item(item)),args.[2]) :> Statement<_>
        , tContext
    | "getarray" -> new Item<_>(args.[0],args.[1]) :> Statement<_>, tContext
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
    | "globalid0i" | "globalid0" -> new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int),"0")]) :> Expression<_>, targetContext
    | "globalid1i" | "globalid1" ->  new FunCall<_>("get_global_id",[Const(PrimitiveType<_>(Int),"1")]) :> Expression<_>, targetContext
    | "item" -> 
        let idx,tContext,hVar = itemHelper exprs hostVar targetContext
        new Item<_>(hVar,idx) :> Expression<_>, tContext
    | x -> failwithf "Unsupported property in kernel: %A"  x

and private transletaPropSet exprOpt (propInfo:System.Reflection.PropertyInfo) exprs newVal targetContext =    
    let hostVar = exprOpt |> Option.map(fun e -> TranslateAsExpr e targetContext)
    let newVal,tContext = TranslateAsExpr newVal (match hostVar with Some(v,c) -> c | None -> targetContext)
    match propInfo.Name.ToLowerInvariant() with
    | "item" -> 
        let idx,tContext,hVar = itemHelper exprs hostVar tContext
        let item = new Item<_>(hVar,idx)        
        new Assignment<_>(new Property<_>(PropertyType.Item(item)),newVal) :> Statement<_>
        , tContext
    | x -> failwithf "Unsupported property in kernel: %A"  x  

and TranslateAsExpr expr (targetContext:TargetContext<_,_>) =
    let (r:Node<_>),tc = Translate expr (targetContext:TargetContext<_,_>)
    (r  :?> Expression<_>) ,tc

and translateVar (var:Var) =
    new Variable<_>(var.Name)

and translateValue (value:obj) (sType:System.Type) =
    let _type = Type.Translate sType
    new Const<_>(_type, string value)

and translateVarSet (var:Var) (expr:Expr) targetContext =
    let var = translateVar var
    let expr,tContext = TranslateAsExpr expr targetContext
    new Assignment<_>(new Property<_>(PropertyType.Var var),expr),tContext

and translateCond (cond:Expr) targetContext =
    match cond with
    | Patterns.IfThenElse(cond,_then,_else) ->
        let l,tContext = translateCond cond targetContext
        let r,tContext = translateCond _then tContext
        let e,tContext = translateCond _else tContext
        new Binop<_>(BitOr, new Binop<_>(BitAnd,l,r),e) :> Expression<_> , tContext
    | Patterns.Value (v,t) -> 
        let r = new Const<_>(new PrimitiveType<_>(PTypes.Int), (if (string v).ToLowerInvariant() = "true" then  "1" else "0"))
        r :> Expression<_>, targetContext 
    | _ -> TranslateAsExpr cond targetContext

and toStb (s:Node<_>) =
    match s with
    | :? StatementBlock<_> as s -> s
    | x -> new StatementBlock<_>(new ResizeArray<_>([x :?> Statement<_>]))

and translateIf (cond:Expr) (thenBranch:Expr) (elseBranch:Expr) targetContext =
    let cond,tContext = translateCond cond targetContext
    let _then,tContext = 
        let t,tc = Translate thenBranch (new TargetContext<_,_>())
        toStb t, tc
    let _else,tContext = 
        match elseBranch with
        | Patterns.Value(null,sType) -> None,tContext
        | _ -> 
            let r,tContext = Translate elseBranch (new TargetContext<_,_>())
            Some (toStb r), tContext
    new IfThenElse<_>(cond,_then, _else), targetContext

and translateForIntegerRangeLoop (i:Var) (from:Expr) (_to:Expr) (_do:Expr) targetContext =
    let v = translateVar i
    let var = translateBinding i from targetContext
    let condExpr,tContext = TranslateAsExpr _to targetContext
    let body,tContext = Translate _do (new TargetContext<_,_>())
    let cond = new Binop<_>(LessEQ, v, condExpr)
    let condModifier = new Unop<_>(UOp.Incr,v)   
    new ForIntegerLoop<_>(var,cond, condModifier,toStb body),targetContext

and translateWhileLoop condExpr bodyExpr targetContext =
    let nCond,tContext = TranslateAsExpr condExpr targetContext
    let nBody,tContext = Translate bodyExpr tContext
    new WhileLoop<_>(nCond, toStb nBody), tContext 

and translateSeq expr1 expr2 (targetContext:TargetContext<_,_>) =
    let decls = new ResizeArray<_>(targetContext.VarDecls)
    targetContext.VarDecls.Clear()
    let nExpr1,tContext = Translate expr1 targetContext
    let nExpr2,tContext = Translate expr2 tContext
    let stmt = 
        match (nExpr1:Node<_>),(nExpr2:Node<_>) with
        | (:? StatementBlock<Lang> as s1),(:? StatementBlock<Lang> as s2) ->
            decls.AddRange(s1.Statements)
            decls.AddRange(s2.Statements)
            new StatementBlock<Lang>(decls)
        | (:? StatementBlock<Lang> as s1),s2 ->
            decls.AddRange(s1.Statements)
            decls.Add (s2 :?> Statement<_>)
            new StatementBlock<Lang>(decls)
        | s1,(:? StatementBlock<Lang> as s2) ->
            decls.Add(s1:?> Statement<_>)
            decls.AddRange s2.Statements
            new StatementBlock<Lang>(decls)
        | s1,s2 ->
            decls.Add(s1:?> Statement<_>)
            decls.Add (s2 :?> Statement<_>)            
            new StatementBlock<Lang>(decls)
    stmt, tContext

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
        | Patterns.ForIntegerRangeLoop (i, from, _to, _do) ->
            let r,tContext = translateForIntegerRangeLoop i from _to _do targetContext
            r :> Node<_>, tContext
        | Patterns.IfThenElse (cond, thenExpr, elseExpr) ->
            let r,tContext = translateIf cond thenExpr elseExpr targetContext
            r :> Node<_>, tContext
        | Patterns.Lambda (var,expr)-> "Application is not suported:" + string expr|> failwith
        | Patterns.Let (var, expr, inExpr) -> 
            let vDecl = translateBinding var expr targetContext
            targetContext.VarDecls.Add vDecl
            let res,tContext = Translate inExpr targetContext
            let sb = new ResizeArray<_>(tContext.VarDecls |> Seq.cast<Statement<_>>)
            sb.Add (res :?> Statement<_>)
            if sb.Count > 1
            then new StatementBlock<_>(sb) :> Node<_>
            else res
            , (new TargetContext<_,_>())

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
        | Patterns.Sequential(expr1,expr2) -> 
            let res,tContext = translateSeq expr1 expr2 targetContext
            res :> Node<_>,tContext
        | Patterns.TryFinally(tryExpr,finallyExpr) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TryWith(expr1,var1,expr2,var2,expr3) -> "Application is not suported:" + string expr|> failwith 
        | Patterns.TupleGet(expr,i) -> "Application is not suported:" + string expr|> failwith
        | Patterns.TypeTest(expr,sType) -> "Application is not suported:" + string expr|> failwith
        | Patterns.UnionCaseTest(expr,unionCaseInfo) -> "Application is not suported:" + string expr|> failwith
        | Patterns.Value(_obj,sType) -> translateValue _obj sType :> Node<_>, targetContext 
        | Patterns.Var var -> translateVar var :> Node<_>, targetContext
        | Patterns.VarSet(var,expr) -> 
            let res,tContext = translateVarSet var expr targetContext
            res :> Node<_>,tContext
        | Patterns.WhileLoop(condExpr,bodyExpr) -> 
            let r,tContext = translateWhileLoop condExpr bodyExpr targetContext
            r :> Node<_>, tContext
        | other -> "OTHER!!! :" + string other |> failwith
