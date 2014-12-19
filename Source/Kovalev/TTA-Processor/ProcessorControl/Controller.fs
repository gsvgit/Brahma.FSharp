namespace Controller

open TTA.Processor
open TTA.Compiler
open TTA.ASM

type ProcessorController() = 
    
    let processor = new Processor<int> [|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|]
    let mutable sourceCode = ""
    let mutable error = ""
    let mutable (program: Program<int>) = [| [||] |]
    let mutable debugState = false
    let mutable debugLineIndex = 0
    
    let build inputCode =
        try
            if inputCode = sourceCode
            then ()
            else
                program <- compile inputCode
                sourceCode <- inputCode
        with
        | ParserError (pos, msg) -> reraise()
        | :? System.Exception -> reraise()

    let getLineOfProgram (program: Program<int>) index =
        let (line: Program<int>) = Array.init program.Length (fun i -> Array.init 1 (fun j -> program.[i].[index]))
        line

    member this.Build inputCode =
        error <- ""
        try build inputCode
        with
        | ParserError (pos, msg) -> error <- "---BUILD FAILED--- Syntax error: " + pos.ToString() + " " + msg
        | :? System.Exception as ex -> error <- "---BUILD FAILED--- Syntax error: " + ex.Message

    member this.Run inputCode = 
        if inputCode = ""
        then ()
        else
            error <- ""
            try
                build inputCode
                processor.Clear
                processor.Run program
            with
            | ParserError (pos, msg) -> error <- "---BUILD FAILED---  Syntax error: " + pos.ToString() + " " + msg
            | IncorrectLine (i, msg) -> error <- "Error: " + msg + " in line " + i.ToString()
            | :? System.Exception as ex -> error <- ex.Message

    member this.DebugState = debugState
    member this.DebugLineIndex = debugLineIndex

    member this.StartDebugging inputCode =
        error <- ""        
        try
            processor.Clear
            build inputCode
            debugState <- true
        with
        | ParserError (pos, msg) -> 
            error <- "---BUILD FAILED--- Syntax error: " + pos.ToString() + " " + msg
            this.StopDebugging ()
        | :? System.Exception as ex -> 
            error <- "---BUILD FAILED--- Syntax error: " + ex.Message
            this.StopDebugging ()
    
    member this.StopDebugging () = 
        debugState <- false
        debugLineIndex <- 0

    member this.NextStep () =
        error <- ""
        if debugState
        then
            if debugLineIndex < program.[0].Length
            then                
                try
                    let line = getLineOfProgram program debugLineIndex
                    debugLineIndex <- debugLineIndex + 1
                    processor.Run line
                with
                | IncorrectLine (i, msg) -> 
                    error <- "Error: " + msg + " in line " + debugLineIndex.ToString()
                    this.StopDebugging ()         
            else this.StopDebugging ()
    
    member this.GetError = error
                    
    member this.AllValues = processor.AllValues
    member this.NumOfRows = processor.NumberOfRows
    member this.NumOfCols = processor.NumberOfCols    