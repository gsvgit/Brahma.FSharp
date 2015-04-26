module HelpTypes

open QuickGraph

type EdgeType = Value | State | Predicate

type Var = 
    | Int of int
    | Bool of bool

type Node<'T> =
    | Predicate of 'T
    | Op of 'T
    | State_in
    | State_out
    | Var
    | Multiplexer
    | NestedGraph of AdjacencyGraph<Node<'T>, TaggedEdge<Node<'T>, EdgeType>>