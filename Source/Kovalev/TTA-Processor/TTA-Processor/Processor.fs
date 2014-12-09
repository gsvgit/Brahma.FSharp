module TTA.Processor

open ASM
open System.Collections.Generic

exception DoubleWriteIntoCell of int * int
exception CellOutOfRange of int * int
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
    do
        if functions.Length = 0
        then raise (System.ArgumentException("Empty grid"))

    let grid = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())
    let mutable gridHeight = 0

    let addCellOnUserRequest key col = 
        grid.[col].Add (key, Cell(functions.[col]))
        if key > gridHeight
        then gridHeight <- key

    let interpretASM command =
        match command with
        | Set ((fst, snd), arg) ->
            let row, col = int fst, int snd
            let currentCol = grid.[col]
            
            if currentCol.ContainsKey row
            then currentCol.[row].Value <- arg
            else 
                addCellOnUserRequest row col
                currentCol.[row].Value <- arg
        
        | Mov ((fstTo, sndTo), (fstFrom, sndFrom)) ->
            let rowTo, colTo, rowFrom, colFrom = int fstTo, int sndTo, int fstFrom, int sndFrom
            let currentColTo = grid.[colTo]
            let currentColFrom = grid.[colFrom]
            
            if not (currentColFrom.ContainsKey rowFrom)
            then addCellOnUserRequest rowFrom colFrom
            if not (currentColTo.ContainsKey rowTo)
            then addCellOnUserRequest rowTo colTo
            
            currentColTo.[rowTo].ExecuteOp currentColFrom.[rowFrom].Value

        | Mvc ((fst, snd), arg) ->
            let row, col = int fst, int snd
            let currentCol = grid.[col]

            if currentCol.ContainsKey row
            then currentCol.[row].ExecuteOp arg
            else 
                addCellOnUserRequest row col
                currentCol.[row].ExecuteOp arg

        | Eps -> ()

    let executeLine (line: array<Asm<'a>>) =
        let cellsForWrite = new HashSet<(int * int)>()
        let priorityCommands = new List<int>()
        let commandsForSecondCheck = new List<int>()
                    
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
                else
                    if cellsForWrite.Contains (int fstFrom, int sndFrom)
                    then priorityCommands.Add i
                    else commandsForSecondCheck.Add i
            
            | Eps -> ()
        
        for i in commandsForSecondCheck do
            match line.[i] with
            | Mov ((fstTo, sndTo), (fstFrom, sndFrom)) ->
                if cellsForWrite.Contains (int fstFrom, int sndFrom)
                then priorityCommands.Add i
                else ()
            | _ -> ()
         
        let arrayForParallel = Array.mapi (fun i x -> if not (priorityCommands.Contains i) then x else Eps) line
        for index in priorityCommands do
            interpretASM line.[index]
        Array.Parallel.iter interpretASM arrayForParallel

    member this.ValueInCell row col =
        let currentCol = grid.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value
    
    member this.AllValues =
        let values = Array.zeroCreate (Array.sumBy (fun (dict: Dictionary<int, Cell<'a>>) -> dict.Count) grid)
        let mutable index = 0
        for i in 0..grid.Length - 1 do
            for cell in grid.[i] do
                values.[index] <- (cell.Key, i, (cell.Value).Value)
                index <- index + 1
        values

    member this.NumberOfRows = gridHeight

    member this.NumberOfCols = grid.Length

    member this.NumberOfCells =
        Array.sumBy (fun (x: Dictionary<int, Cell<'a>>) -> x.Count) grid

    member this.Clear = for col in grid do col.Clear()

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