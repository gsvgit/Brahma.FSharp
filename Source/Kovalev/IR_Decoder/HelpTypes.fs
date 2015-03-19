module HelpTypes

type EdgeType = Value | State | Predicate

type Node = 
    | Predicate   //of
    | Op          //of
    | Block