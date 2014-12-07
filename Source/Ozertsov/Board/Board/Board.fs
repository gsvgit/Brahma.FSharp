module Processor

open Cell
open TTA.ASM
open System.Collections.Generic
open System.Linq;

exception ParametrException of int * int

type Para = int * int

type Matrix<'a>(functions: ('a -> 'a -> 'a) array) =
    do
        if functions.Length < 1
        then raise (System.ArgumentException("null matrix"))

    //int is row of matrix, cow is length of array functions    
    let mutable matrix = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())
    //let Error = HashSet<(string * int * int)>()
    let addCellOnUserRequest row col = matrix.[col].Add (row, Cell(functions.[col]))

    //x is row, y is column
    let intASM command =
        match command with
        | Set ((x, y), arg) ->
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < inty
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " "+ inty.ToString() ))
            if matrix.[inty].ContainsKey intx
            then matrix.[inty].[intx].Value <- arg
            else 
                addCellOnUserRequest intx inty
                matrix.[inty].[intx].Value <- arg

        | Mov ((fromx, fromy), (tox, toy)) ->
            let intfromx = (int) fromx
            let intfromy = (int) fromy
            if matrix.Length < intfromy
            then raise (System.ArgumentException("can't create cell " + intfromx.ToString() + " "+ intfromy.ToString() ))
            if matrix.[intfromy].ContainsKey intfromx
            then
                let inttox = (int) tox
                let inttoy = (int) toy                
                if matrix.Length < inttoy
                then raise (System.ArgumentException("can't create cell " + inttox.ToString() + " "+ inttoy.ToString() ))
                if matrix.[inttoy].ContainsKey inttox
                then
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
                else
                    addCellOnUserRequest inttox inttoy
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
            else
                addCellOnUserRequest intfromx intfromy
                let inttox = (int) tox
                let inttoy = (int) toy                
                if matrix.Length < inttoy
                then raise (System.ArgumentException("can't create cell " + inttox.ToString() + " "+ inttoy.ToString() ))
                if matrix.[inttoy].ContainsKey inttox
                then
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
                else
                    addCellOnUserRequest inttox inttoy
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
        | Mvc ((x, y), arg) -> 
            // todo correction exceptions
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < inty
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " "+ inty.ToString() ))
            if matrix.[inty].ContainsKey intx
                then matrix.[inty].[intx].RunOp arg
            else
                addCellOnUserRequest intx inty
                matrix.[inty].[intx].RunOp arg    
        | Eps -> ()
        
    let Check (line: array<Asm<'a>>) =
        for i in 0 .. line.Length - 1 do
            let cellsForWrite = new HashSet<(int * int)>()
            let cellsForRead = new HashSet<(int * int)>()
            match line.[i] with
            | Eps -> ()
            | Set ((x, y), arg) ->
                let dot = (int x, int y)
                if matrix.Length < int y
                then raise (System.ArgumentException("can't find cell " + (int x).ToString() + " "+ (int y).ToString() ))
                if cellsForWrite.Contains(dot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(dot) |> ignore
            | Mvc ((tox, toy), arg) -> 
                let todot = (int tox, int toy)
                if matrix.Length < int toy
                then raise (System.ArgumentException("can't find cell " + (int tox).ToString() + " "+ (int toy).ToString() ))
                if cellsForWrite.Contains(todot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(int tox, int toy) |> ignore
            | Mov ((fromx, fromy), (tox, toy)) ->
                let fromdot = (int fromx, int fromy)
                if matrix.Length < int fromy
                then raise (System.ArgumentException("can't find cell " + (int fromx).ToString() + " "+ (int fromy).ToString() ))
                if cellsForWrite.Contains(fromdot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForRead.Add(fromdot) |> ignore                
                let todot = (int tox, int toy)
                if matrix.Length < int toy
                then raise (System.ArgumentException("can't find cell " + (int tox).ToString() + " "+ (int toy).ToString() ))
                if cellsForWrite.Contains(todot) || cellsForRead.Contains(todot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(todot) |> ignore

    member this.RunLine line =
        try
            Check line
        with
        | :? System.Exception -> reraise()
        Array.Parallel.iter intASM line

    member this.RunOp (program: Program<'a>) =
        try
            if program.Length = 0
            then ()
            else
                let i = program.[0].Length
                for p in program do
                    if i <> p.Length
                    then raise(System.ArgumentException("workflows aren't compatible"))
                Array.iter (fun i -> Check i ) program
        with
        | :? System.Exception -> reraise()
        Array.iter (fun i -> (Array.Parallel.iter (fun p -> intASM p) i)) program    
        
    member this.ValueInCell row col =
        let currentCol = matrix.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value
    
    member this.Dispose =
        matrix <- Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())

    member this.getMatrix =
        matrix
                
    member this.NumColls =
        matrix.Length

    member this.NumRows =
        Array.max(Array.init matrix.Length (fun i -> if (matrix.[i].Count > 0) then (matrix.[i].Keys.Max()) else 0)) + 1
    
    member this.CreateSetCells col =
        let cellsForAdd = Dictionary<int, string>()
        for kvp in matrix.[col] do
            cellsForAdd.Add(kvp.Key, kvp.Value.Value.ToString())
        cellsForAdd