module Processor

open Cell
open System.Collections.Generic

type Matrix<'a>(functions: ('a -> 'a -> 'a) array) =
    do
        if functions.Length < 1
        then raise (System.ArgumentException("null matrix"))
        
    let matrix = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())

    let addCellOnUserRequest key col = matrix.[col].Add (key, Cell(functions.[col]))

    member this.ValueInCell row col =
        let currentCol = matrix.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value