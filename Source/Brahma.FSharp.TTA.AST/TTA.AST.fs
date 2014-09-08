namespace Brahma.FSharp.TTA.AST

type Operation =
    | Plus
    | Minus
    | Mult
    | Div

type AST () = 
    inherit QuickGraph.AdjacencyGraph<Operation,QuickGraph.Edge<Operation>>()
