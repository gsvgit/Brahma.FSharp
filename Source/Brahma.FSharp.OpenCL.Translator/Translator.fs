namespace Brahma.FSharp.OpenCL.Translator

open Microsoft.FSharp.Quotations

type FSQuotationToOpenCLTranslator() =
    let translate qExpr = 
        let r = <@ fun x -> x + 1 @> 
        let q = <@ fun y -> (%r) y + 1 @>
        let x = r.Substitute(fun x -> Some (q.Raw))
        let y f = <@ fun x -> (%f) x @>
        let rec f q = 
            match q with
            | ExprShape.ShapeLambda(v,e) ->
                 printfn "expr: var: %A \nexpr:" v.Name
                 f e 
            | ExprShape.ShapeCombination (o,eLst) -> 
                printfn "expression: %A" o
                match o with
                | :? Expr as e ->
                    match e with
                    | Patterns.Call(x) 
                       -> printfn "CALL"
                    | _ -> ()
                | x -> printfn "ELSE: %A" x 
                List.iter f eLst
            | ExprShape.ShapeVar (v) -> 
                printfn "Var: name: %A type: %A" v.Name v.Type

        let rec g q = 
            match q with
            | Patterns.Call(oe,info,el) -> 
                    printfn "call: "
                    match oe with
                    | Some e -> printfn "oe: "; g e
                    | None -> ()
                    List.iter g el
            | Patterns.Lambda (v,e) -> printfn "lambda: var: %A expr: " v.Name ;g e
            | Patterns.Application(e1,e2 ) -> 
                printfn "application"
                printfn "expr1"
                g e1
                printfn "expr2"
                g e2
            | c -> printfn "ELSE: %A" c
  
        g q
        printfn "%A" x
 
    member this.Translate qExpr = "" 
