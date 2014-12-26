module TTA.Processor
 
open System.Collections.Generic
open TTA.ASM
 

exception DoubleWriteIntoCell of int * int * string
 
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
    let addCell row col =
        grid.Add ((row, col), Cell(functions.[col]))
        if row + 1 > gHeight
        then gHeight <- row + 1
 
    let intASM command =
        let CellsForWrite = new HashSet<(int * int)>()
        match command with
        | Set ((ln, col), arg) -> 
                            let l, c = int ln, int col
                            if c > functions.Length - 1 
                            then raise <| System.ArgumentException ("incorrect cell" + (l).ToString() + " " + (c).ToString())
                            if not <| CellsForWrite.Add (l, c)
                            then raise <| DoubleWriteIntoCell (l, c, "incorrect input: double write into cell")
                            ((l, c), arg)             
     
        | Mov ((ln1, col1), (ln2, col2)) ->
                            let l1, c1, l2, c2 = int ln1, int col1, int ln2, int col2
                            if (c1 > functions.Length - 1) || (c2 > functions.Length - 1)
                            then raise <| System.ArgumentException ("check input data" + (l1).ToString() + "," + (c1).ToString() +  " " + (l2).ToString() + "," + (c2).ToString())
                            let mutable arg1 = Unchecked.defaultof<'a>
                            let mutable arg2 = Unchecked.defaultof<'a>
                            if grid.ContainsKey (l2, c2)
                            then arg1 <- grid.[l2, c2].Value
                            if grid.ContainsKey (l1, c1)
                            then arg2 <- grid.[l1, c1].Value                        
                            ((l1, c1), functions.[c1] arg2 arg1)
 
        | Mvc ((ln, col), arg) ->
                            let l, c = int ln, int col
                            if c > functions.Length - 1 
                            then raise <| System.ArgumentException ("incorrect cell" + (l).ToString() + " " + (c).ToString())
                            if not <| CellsForWrite.Add (l, c)
                            then raise <| DoubleWriteIntoCell (l, c, "incorrect input: double write into cell")
                            let mutable arg1 = Unchecked.defaultof<'a>
                            if grid.ContainsKey (l, c)  
                            then arg1 <- grid.[l, c].Value
                            ((l, c), functions.[c] arg1 arg)  
 
        | Eps -> failwith "Just for VS"
 
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
                    for i in 0..maxLength - 1 do
                            let line = Array.map (fun (x: array<Asm<'a>>) -> if i >= x.Length then Eps else x.[i]) program
                            executeLine line