module Compilator

open Processor
open TTA.ASM
open System.Collections.Generic
open MyParser.MyParser
exception CompileException

type Compiler() =
    let processor = new Matrix<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])
    let parser = new MyParser()
    let mutable program: Program<int> = null
    
    member this.Compile(str : string) =
        program <- parser.Parse str

    member this.GetProgram() =
        program
    
    member this.Run() =
            processor.RunOp program
            program <- null
            processor.Dispose
        
    member this.Debug = 
        ignore

    member this.Step(count:int) =
        processor.RunLine program.[count]

    member this.Stop() = 
        program <- null
        processor.Dispose
    
    member this.getGrid col =
        processor.CreateSetCells col
    
    member this.NumRows() = 
        processor.NumRows

    member this.NumCols() = 
        processor.NumColls