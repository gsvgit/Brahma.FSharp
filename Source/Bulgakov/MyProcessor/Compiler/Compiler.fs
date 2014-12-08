module Compiler

open Processor
open TTA.ASM
open System.Collections.Generic
open MyParser.MyParser
open System.Linq
exception CompileException


type Compiler() =
    let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])
    let parser = new MyParser()
    let mutable program: Program<int> = null
    let Errors = new HashSet<(string*int*int)>()

//Late
    member this.getErrorsList  = 
        let CellsSet = new HashSet<(int*int)>()
        let grid = processor.getGrid
        for i in 0..program.Length-1 do
            for j in 0..program.[i].Length-1 do
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
                |Eps -> () |> ignore
            CellsSet.Clear() 
        Errors

    member this.Compile(str : string) =
        try
            program <- parser.Parse str
        with
        | :? System.Exception -> raise CompileException

    member this.Run() = 
        processor.executeProgram program
        program <- null

    member this.Step(count:int) =
        processor.executeLine program.[count]

    member this.Stop() =
        Errors.Clear()
        program <- null 
        processor.Dispose
    
    member this.getGrid() =
        processor.getGrid
    
    member this.getStringGrid(num:int) =
        let grid = processor.getGrid
        let cellsForAdd = Dictionary<int, string>()
        for kvp in grid.[num] do
            cellsForAdd.Add(kvp.Key, kvp.Value.Value.ToString())
        cellsForAdd
    
    member this.CountRows() =
        let grid = processor.getGrid
        Array.max(Array.init grid.Length (fun i -> if (grid.[i].Count > 0) then (grid.[i].Keys.Max()) else 0)) + 1
    
    member this.CountCols() =
        processor.getGrid.Length

