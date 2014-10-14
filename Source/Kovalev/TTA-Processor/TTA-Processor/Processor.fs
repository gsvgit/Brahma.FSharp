module Processor

open Cell

type Processor<'a>(functions: array<'a -> 'a -> 'a>) =
    do
        if functions.Length = 0
        then raise (System.ArgumentException("Empty grid"))
            
    let grid = Array.init functions.Length (fun i -> ResizeArray<int * Cell<'a>>())

    let addCellOnUserRequest key col =
        let currentCol = grid.[col]
        if currentCol.Count = 0
        then currentCol.Add (key, Cell(functions.[col]))
        else
            let index = currentCol.FindIndex (fun i -> key < fst i)
            if index = -1 
            then currentCol.Add (key, Cell(functions.[col]))
            else currentCol.Insert (index - 1, (key, Cell(functions.[col])))

    member this.ValueInCell row col =
        let index = grid.[col].FindIndex (fun i -> row = fst i)                
        if index <> -1
        then (snd grid.[col].[index]).Value 
        else 
            addCellOnUserRequest row col
            let indexOfNew = grid.[col].FindIndex (fun i -> row = fst i) 
            (snd grid.[col].[indexOfNew]).Value