module IR

open QuickGraph
open HelpTypes

type VSFGraph() =
    
    let graph = new AdjacencyGraph<Node, TaggedEdge<Node, EdgeType>>
