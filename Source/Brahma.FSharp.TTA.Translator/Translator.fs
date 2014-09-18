namespace Brahma.FSharp.TTA.Translator
open Brahma.FSharp
open QuickGraph

type Translator() = 
    let varToGraph = new System.Collections.Generic.Dictionary<_,_>()
    
    let rec translateExpr (e:OpenCL.AST.Expression<_>) =
        match e with
        | :? OpenCL.AST.Const<_> as c ->             
            new TTA.AST.AST<_>(TTA.AST.Const c.Val)

        | :? OpenCL.AST.Binop<_> as bop ->
            let l = translateExpr bop.Left
            let r = translateExpr bop.Right
            let op =
                match bop.Op with
                | OpenCL.AST.Plus      -> TTA.AST.Plus
                | OpenCL.AST.BOp.Minus -> TTA.AST.Minus
                | OpenCL.AST.Mult      -> TTA.AST.Mult
                | OpenCL.AST.Div       -> TTA.AST.Div
            let root = TTA.AST.Op op
            let res = new TTA.AST.AST<_>(root)
            res.AddVerticesAndEdge(new Edge<_>(l.Root,root)) |> ignore
            res.AddVerticesAndEdge(new Edge<_>(r.Root,root)) |> ignore
            res
        | :? OpenCL.AST.Variable<_> as v ->
            varToGraph.[v.Name]

    let translateAssign (a:OpenCL.AST.Assignment<_>) =
        let newG = translateExpr a.Value
        let name =
            match a.Name.Property with
            | OpenCL.AST.Var v -> v.Name
            | OpenCL.AST.Item i -> "item"
        varToGraph.Add(name, newG)

    let translate (oclAst:OpenCL.AST.AST<_>) =        
        let assignments = oclAst.TopDefs.[0].Children
        let resExpr = 
        //let topFunArgs = (oclAst.TopDefs.[0] :> OpenCL.AST.FunDecl<_>).Args

    let d = <@
                let x = 1 
                x + 1
            @>
    member this.X = "F#"


