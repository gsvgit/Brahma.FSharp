module TTA.Processor
 
open System.Collections.Generic
open TTA.ASM
 
exception CellOutOfRange of int * int
exception DoubleWriteIntoCell of int * int
exception IncorrectLine of int * string
 
type Cell<'a> (operation: 'a -> 'a -> 'a) =
   
    let mutable value = Unchecked.defaultof<'a>
    let op = operation
 
    member this.Value
        with get() = value
        and set(arg) = value <- arg
    member this.ExecuteOp operand =
        value <- op value operand
 
type Processor<'a> (functions: array<'a -> 'a -> 'a>) =
 
    let grid = Dictionary<(int * int), Cell<'a>>()
    let mutable gridHeight = 0
 
    let addCell row col =
        grid.Add ((row, col), Cell(functions.[col]))
        if row + 1 > gridHeight
        then gridHeight <- row + 1
 
    let interpretASM command =
        match command with
        | Set ((fst, snd), arg) -> ((int fst, int snd), arg)            
     
        | Mov ((rowTo, colTo), (rowFrom, colFrom)) ->
            let rowTo, colTo, rowFrom, colFrom = int rowTo, int colTo, int rowFrom, int colFrom
            let mutable arg1 = Unchecked.defaultof<'a>
            let mutable arg2 = Unchecked.defaultof<'a>
 
            if (grid.ContainsKey (rowFrom, colFrom))
            then arg1 <- grid.[rowFrom, colFrom].Value
            if (grid.ContainsKey (rowTo, colTo))
            then arg2 <- grid.[rowTo, colTo].Value
 
            ((rowTo, colTo), functions.[colTo] arg2 arg1)
 
        | Mvc ((row, col), arg) ->
            let row, col = int row, int col
            let mutable arg1 = Unchecked.defaultof<'a>
 
            if grid.ContainsKey (row, col)
            then arg1 <- grid.[row, col].Value
 
            ((row, col), functions.[col] arg1 arg)
 
        | Eps -> failwith "Just for VS"
 
    let executeLine (line: array<Asm<'a>>) =
        let cellsForWrite = new HashSet<(int * int)>()
                   
        for i in 0..line.Length - 1 do
            match line.[i] with
            | Set ((fst, snd), arg)
            | Mvc ((fst, snd), arg) ->
                if int snd > functions.Length - 1
                then raise (CellOutOfRange (int fst, int snd))
               
                if cellsForWrite.Add (int fst, int snd) = false
                then raise (DoubleWriteIntoCell (int fst, int snd))
           
            | Mov ((fstTo, sndTo), (fstFrom, sndFrom)) ->
                if int sndTo > functions.Length - 1
                then raise (CellOutOfRange (int fstTo, int sndTo))
                elif int sndFrom > functions.Length - 1
                then raise (CellOutOfRange (int fstFrom, int sndFrom))
               
                if cellsForWrite.Add (int fstTo, int sndTo) = false
                then raise ((DoubleWriteIntoCell (int fstTo, int sndTo)))
           
            | Eps -> ()
               
        let results = Array.Parallel.map interpretASM (Array.filter (fun x -> match x with | Eps -> false | _ -> true) line)
        for i in results do
            if grid.ContainsKey (fst i)
            then grid.[fst i].Value <- snd i
            else
                addCell (fst (fst i)) (snd (fst i))
                grid.[fst i].Value <- snd i
 
    member this.ValueInCell row col =
        if grid.ContainsKey (row, col)
        then grid.[row, col].Value
        else
            addCell row col
            grid.[row, col].Value
   
    member this.AllValues =
        let values = Array.zeroCreate grid.Count
        let mutable index = 0        
        for cell in grid do
            values.[index] <- (fst cell.Key, snd cell.Key, (cell.Value).Value)
            index <- index + 1
        values
 
    member this.NumberOfRows = gridHeight
    member this.NumberOfCols = functions.Length
    member this.NumberOfCells = grid.Count
 
    member this.Clear =
        grid.Clear()
        gridHeight <- 0
 
    member this.Run (program: Program<'a>) =
        try
            if program.Length = 0
            then ()
            else
                let maxLength = Array.maxBy (fun (x: array<Asm<'a>>) -> x.Length) program |> Array.length
                if maxLength = 0
                then ()
                else
                    for i in 0..maxLength - 1 do
                        try
                            let line = Array.map (fun (x: array<Asm<'a>>) -> if i >= x.Length then Eps else x.[i]) program
                            executeLine line
                        with
                        | DoubleWriteIntoCell (fst, snd) -> raise (IncorrectLine (i, "Double write into cell" + " " +
                                                                                  fst.ToString() + " " + snd.ToString()))
                        | CellOutOfRange (fst, snd) -> raise (IncorrectLine (i, "Can't create cell" +  " " +
                                                                             fst.ToString() + " " + snd.ToString()))
        with
        | IncorrectLine (i, msg) -> reraise ()