namespace Brahma.FSharp.TTA.AST

type Operation =
    | Plus
    | Minus
    | Mult
    | Div    

type vType =
    | Op of Operation
    | Const of string

type AST<'c> (root:vType) as this =     
    inherit QuickGraph.AdjacencyGraph<vType,QuickGraph.Edge<vType>>()
    do this.AddVertex root |> ignore
    member val Root = root with get,set
