namespace Brahma.FSharp.TTA.Core

open Microsoft.FSharp.Quotations
open FSharpx.Linq.QuotationEvaluation

type ComputeProvider() =
    
    member this.CompileQuery query =
        ()

    member this.Compile (query: Expr<'TRange ->'a>) =        
        let kernel = this.CompileQuery query        
        let args = ref [||]
        let getStarterFuncton qExpr =
            let rec go expr vars =
                match expr with
                | Patterns.Lambda (v, body) ->
                    Expr.Lambda(v, go body (v::vars))
                | e ->
                    let arr =
                        let c = Expr.NewArray(typeof<obj>,vars |> List.rev 
                            |> List.map 
                                 (fun v -> Expr.Coerce (Expr.Var(v), typeof<obj>)))
                        <@@
                            let x = %%c |> List.ofArray                            
                            args := x |> Array.ofList                            
                        @@>
                    arr
            let res = <@ %%(go qExpr []):'TRange ->'a @>.Compile()()
            
            res

        kernel
        , getStarterFuncton query        
