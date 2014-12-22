module Board

open Cell
open TTA.ASM
open System.Collections.Generic
open System.Linq

exception ParallelException of int*int 
exception OutOfBounds of int*int
exception SomeException

type Processor<'a> (operations : array<'a -> 'a -> 'a>) =
    do
        if operations.Length = 0
        then raise(System.ArgumentNullException("Empty grid"))

    let mutable grid = Array.init operations.Length (fun i -> Dictionary<int, Cell<'a>>())

    let addCell row column = 
        if  column < grid.Length || row >= 0 || column  >= 0  
        then grid.[column].Add(row, Cell(operations.[column]))
        else raise(OutOfBounds(row, column))
    
    let Errors = new HashSet<string*int*int>()

    let interpret command = 
        match command with
        |Set ((row1, col1), arg) ->
            let row, col = int row1, int col1
            if not(grid.[col].ContainsKey row)
            then addCell row col              
            grid.[col].[row].Value <- arg                
        |Mov ((row1, col1), (row2, col2)) ->
            let toRow, toCol, fromRow, fromCol = int row1, int col1, int row2, int col2
            if not(grid.[toCol].ContainsKey toRow)
            then addCell toRow toCol |> ignore                
            if not(grid.[fromCol].ContainsKey fromRow)
            then addCell fromRow fromCol |> ignore               
            grid.[toCol].[toRow].Execute grid.[fromCol].[fromRow].Value
        |Mvc((row1, col1), arg) ->
            let row, col = int row1, int col1
            if not(grid.[col].ContainsKey row)
            then addCell row col                
            grid.[col].[row].Execute arg
        |Eps -> ()

    member this.Check (program: Program<'a>) = 
        Errors.Clear()
        let CellsSet = new HashSet<(int*int)>()
        for i in 0..program.Length - 1 do
            for j in 0..program.[i].Length - 1 do
                let operation = program.[i].[j]
                match operation with
                |Set((row1, col1), arg)
                |Mvc((row1, col1), arg) ->
                    let row, col = int row1, int col1
                    if not (col < grid.Length && row >= 0 && col >= 0)
                    then Errors.Add("Index out of bound", i, j) |> ignore
                    if CellsSet.Add (int row, int col) = false
                    then Errors.Add ("Parrallel Exception",i,j) |> ignore
                |Mov((toRow, toCol), (fromRow, fromCol)) ->
                    let row2, col2, row1, col1 = int toRow, int toCol, int fromRow, int fromCol
                    if not (col1 < grid.Length  && row1 >= 0 && col1 >= 0 && row2>=0 && col2 >= 0)
                    then Errors.Add("Index out of bound", i, j) |> ignore
                    if not (col2 < grid.Length)
                    then Errors.Add("Index out of bound", i, j) |> ignore
                    if CellsSet.Add (int row1, int col1) = false
                    then Errors.Add ("Parrallel Exception",i,j) |> ignore
                    if CellsSet.Add (int row2, int col2) = false
                    then Errors.Add ("Parrallel Exception",i,j) |> ignore
                |Eps -> ()
            CellsSet.Clear()
        if Errors.Count > 0
        then raise(SomeException)
        else ()

    member this.ValueInCell row col =
        if not(grid.[col].ContainsKey row)
        then addCell row col
        grid.[col].[row].Value      

    member this.ExecuteLine line =
        Array.Parallel.iter interpret line

    member this.ExecuteProgram(arr: Program<'a>) =
        try
            this.Check arr
            for i in arr do
                Array.Parallel.iter interpret i
        with
        | :? SomeException -> reraise()

    member this.CreateSetCells col =
        let cellsForAdd = Dictionary<int, string>()
        for kvp in grid.[col] do
            cellsForAdd.Add(kvp.Key, kvp.Value.Value.ToString())
        cellsForAdd
    
    member this.GetErrors =
        Errors

    member this.NumberOfColls =
        grid.Length

    member this.NumberOfRows =
        Array.max(Array.init grid.Length (fun i -> if (grid.[i].Count > 0) then (grid.[i].Keys.Max()) else 0)) + 1

    member this.GetGrid = 
        grid

    member this.Dispose = 
        grid <- Array.init operations.Length (fun i -> Dictionary<int, Cell<'a>>())






