module Compiler

open Board
open TTA.ASM
open System.Collections.Generic
open System.Linq
open MyParser.MyParser

exception CompileException
exception RunTimeException

type Compiler() =
    let processor = new Processor<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])
    let parser = new MyParser()
    let mutable program: Program<int> = null
    let mutable Errors = new HashSet<(string*int*int)>()

    member this.Compile(str : string) =
        try
            program <- parser.Parse str
        with
        | :? System.Exception -> raise CompileException
        

    member this.Run() = 
        try
            processor.ExecuteProgram program
            program <- null
        with
        | :? SomeException ->
            Errors <- processor.GetErrors
            reraise()
    
    member this.Debug() =
        try
            processor.Check program
        with
        | :? SomeException ->
            Errors <- processor.GetErrors
            reraise()

    member this.Step(count:int) =
        processor.ExecuteLine program.[count]

    member this.Stop() =
        Errors.Clear()
        program <- null 
        processor.Dispose
    
    member this.GetGrid() =
        processor.GetGrid
    
    member this.GetStringGrid(num:int) =
        let grid = processor.GetGrid
        let cellsForAdd = Dictionary<int, string>()
        for kvp in grid.[num] do
            cellsForAdd.Add(kvp.Key, kvp.Value.Value.ToString())
        cellsForAdd
    
    member this.CountRows() =
        let grid = processor.GetGrid
        Array.max(Array.init grid.Length (fun i -> if (grid.[i].Count > 0) then (grid.[i].Keys.Max()) else 0)) + 1
    
    member this.CountCols() =
        processor.GetGrid.Length

    member this.GetErrorsList() =
        Errors