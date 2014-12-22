module Compilator

open Processor
open TTA.ASM
open System.Collections.Generic
open MyParser.MyParser
exception CompileException

type Compiler() =
    let processor = new Matrix<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x / y); (fun x y -> x * y)|])
    let parser = new MyParser()
    let mutable program: Program<int> = null
    
    member this.Compile(str : string) =
        try
            program <- parser.Parse str
        with
        | :? System.ArgumentException -> reraise()

    member this.GetProgram() =
        program
    
    member this.Run() =
        try
            processor.WorkFlows program
            processor.RunOp program
            program <- null
        with
        | :? System.ArgumentException -> reraise()
        | :? System.IndexOutOfRangeException -> reraise()
        | :? System.Exception -> reraise()

    member this.Debug = 
        ignore

    member this.Step(count:int) =
        if count = 0
        then
            try 
                processor.WorkFlows program
            with
            | :? System.ArgumentException -> reraise()
        try
            processor.RunLine program.[count]
        with
        | :? System.IndexOutOfRangeException -> reraise()
        | :? System.ArgumentException -> reraise()

    member this.Stop() = 
        program <- null
        processor.Dispose
    
    member this.getGrid col =
        processor.CreateSetCells col
    
    member this.NumRows() = 
        processor.NumRows

    member this.NumCols() = 
        processor.NumColls