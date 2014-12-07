namespace Controller

open TTA.Processor
open TTA.Compiler
open TTA.ASM

type ProcessorController() = 
    
    let processor = new Processor<int> [|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|]
    let mutable sourceCode = ""
    let mutable (program: Program<int>) = null
    let mutable debugState = false
    let mutable debugLineIndex = 0
    
    let build inputCode =
        if inputCode = sourceCode
        then ()
        else
            program <- compile inputCode
            sourceCode <- inputCode

    let getLineOfProgram (program: Program<int>) index =
        let (line: Program<int>) = Array.init program.Length (fun i -> Array.init 1 (fun j -> program.[i].[index]))
        line

    member this.Build inputCode = build inputCode

    member this.Run inputCode =
        build inputCode
        processor.Clear
        processor.Run program

    member this.DebugState = debugState
    member this.DebugLineIndex = debugLineIndex

    member this.StartDebugging () =
        processor.Clear
        debugState <- true
    
    member this.StopDebugging () = 
        debugState <- false
        debugLineIndex <- 0

    member this.NextStep ()=
        if debugState
        then
            if debugLineIndex < program.[0].Length
            then
                let line = getLineOfProgram program debugLineIndex
                debugLineIndex <- debugLineIndex + 1
                processor.Run line
            else this.StopDebugging ()
                
    member this.AllValues = processor.AllValues
    member this.NumOfRows = processor.NumberOfRows
    member this.NumOfCols = processor.NumberOfCols