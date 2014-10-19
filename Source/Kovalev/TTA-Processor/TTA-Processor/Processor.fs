module Processor

open System.Collections.Generic

type Cell<'a> (operation: 'a -> 'a -> 'a) =
    
    let mutable value = Unchecked.defaultof<'a>
    let op = operation

    member this.Value
        with get() = value
        and set(arg) = value <- arg
    member this.ExecuteOp operand =
        value <- op value operand

type Processor<'a> (functions: array<'a -> 'a -> 'a>) =
    do
        if functions.Length = 0
        then raise (System.ArgumentException("Empty grid"))
 
    let grid = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())

    let addCellOnUserRequest key col = grid.[col].Add (key, Cell(functions.[col]))

    member this.ValueInCell row col =
        let currentCol = grid.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value