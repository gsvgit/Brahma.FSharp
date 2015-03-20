module IR

open QuickGraph
open HelpTypes

type VSFGraph<'T>() =
    
    let graph = new AdjacencyGraph<Node<'T>, TaggedEdge<Node<'T>, EdgeType>>()