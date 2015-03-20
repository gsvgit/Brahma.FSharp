module HelpTypes

open QuickGraph

type EdgeType = Value | State | Predicate

//type Var

type Node<'T> = 
    | Predicate of 'T
    | Op of 'T
    | State_in
    | State_out
    | Var
    | ConditionalBlock
    | NestedGraph of AdjacencyGraph<Node<'T>, TaggedEdge<Node<'T>, EdgeType>>