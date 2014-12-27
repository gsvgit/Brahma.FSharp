module TTA.Processor
 
open System.Collections.Generic
open TTA.ASM
 

exception DoubleWriteIntoCell of string
 
type Cell<'a> (op: 'a -> 'a -> 'a) =
    let mutable value = Unchecked.defaultof<'a>
    member this.Value
        with get() = value
        and set(arg) = value <- arg
    member this.ExecuteOp operand =
        value <- op value operand
 
type Processor<'a> (functions: array<'a -> 'a -> 'a>) =
    do  
        if functions.Length < 1
        then raise <| new System.ArgumentException ("incorrect input")
    let grid = Dictionary<(int * int), Cell<'a>>()
    let mutable gHeight = 0
    let addCell ln col =
        grid.Add ((ln, col), Cell(functions.[col]))
        if ln + 1 > gHeight
        then gHeight <- ln + 1
 
    let intASM command =
        match command with
        | Set ((ln, col), arg) -> ((int ln, int col), arg)            
     
        | Mov ((ln1, col1), (ln2, col2)) ->
                        let mutable arg1 = Unchecked.defaultof<'a>
                        let mutable arg2 = Unchecked.defaultof<'a> 
                        if (grid.ContainsKey (int ln2, int col2))
                        then arg1 <- grid.[int ln2, int col2].Value
                        if (grid.ContainsKey (int ln1, int col1))
                        then arg2 <- grid.[int ln1, int col1].Value 
                        ((int ln1, int col1), functions.[int col1] arg2 arg1)
 
        | Mvc ((ln, col), arg) ->
                        let mutable arg1 = Unchecked.defaultof<'a> 
                        if grid.ContainsKey (int ln, int col)
                        then arg1 <- grid.[int ln, int col].Value 
                        ((int ln, int col), functions.[int col] arg1 arg)
 
        | Eps -> failwith "Just for VS"
    
    let checkLine (line: array<Asm<'a>>) =
        let cellsForWrite = new HashSet<(int * int)>()
        for i in 0..line.Length - 1 do
            match line.[i] with
            | Set ((ln, col), arg) | Mvc ((ln, col), arg) ->
                             if int col > functions.Length - 1
                             then raise <| System.ArgumentException ("incorrect input data")
                             if not <| cellsForWrite.Add (int ln, int col) 
                             then raise <| DoubleWriteIntoCell ("DoubleWriteIntoCell: " + ln.ToString() + " " + col.ToString())
           
            | Mov ((ln1, col1), (ln2, col2)) ->
                            if (int col1 > functions.Length - 1) || (int col2 > functions.Length - 1)
                            then raise <| System.ArgumentException ("incorrect input data")                            
                            if not <| cellsForWrite.Add (int ln1, int col1) 
                            then raise <| DoubleWriteIntoCell ("DoubleWriteIntoCell" + ln1.ToString() + " " + col1.ToString())
           
            | Eps -> ()

    let executeLine (line: array<Asm<'a>>) =
        let res = Array.Parallel.map intASM (Array.filter (fun x -> match x with | Eps -> false | _ -> true) line)
        for i in res do
            if grid.ContainsKey (fst i)
            then grid.[fst i].Value <- snd i
            else
                addCell (fst (fst i)) (snd (fst i))
                grid.[fst i].Value <- snd i
 
    member this.ValueInCell ln col =
        if grid.ContainsKey (ln, col)
        then grid.[ln, col].Value
        else
            addCell ln col
            grid.[ln, col].Value
   
    member this.AllValues =
        let vls = Array.zeroCreate grid.Count
        let mutable index = 0        
        for cell in grid do
            vls.[index] <- (fst cell.Key, snd cell.Key, (cell.Value).Value)
            index <- index + 1
        vls
 
    member this.NumberOfRows = gHeight
    member this.NumberOfCols = functions.Length
    member this.NumberOfCells = grid.Count
 
    member this.Clear =
                grid.Clear()
                gHeight <- 0
                
    member this.Run (program: Program<'a>) =
                if program.Length = 0
                then ()   
                let maxLength = Array.maxBy (fun (x: array<Asm<'a>>) -> x.Length) program |> Array.length
                if maxLength = 0
                then ()
                else
                    try
                        for i in 0..maxLength - 1 do
                                let line = Array.map (fun (x: array<Asm<'a>>) -> if i >= x.Length then Eps else x.[i]) program
                                checkLine line
                                executeLine line
                    with
                    | DoubleWriteIntoCell (msg) -> reraise()
                    | :? System.ArgumentException -> reraise()