module Processor

open Cell
open TTA.ASM
open System.Collections.Generic
open System.Linq;

exception OperationException of string
exception ArgException of string

type Para = int * int

type Matrix<'a>(functions: ('a -> 'a -> 'a) array) =
    do
        if functions.Length < 1
        then raise (System.ArgumentException("null matrix"))

    let mutable matrix = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())
    let addCellOnUserRequest row col = matrix.[col].Add (row, Cell(functions.[col]))

    //x is row, y is column
    let intASM command =
        match command with
        | Set ((x, y), arg) ->
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < inty
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " " + inty.ToString() ))
            if matrix.[inty].ContainsKey intx = false
            //matrix.[inty].[intx].Value <- arg
            then 
                addCellOnUserRequest intx inty
            matrix.[inty].[intx].Value <- arg

        | Mov ((tox, toy), (fromx, fromy)) ->
            let intfromx = (int) fromx
            let intfromy = (int) fromy
            if matrix.Length < intfromy
            then raise (System.ArgumentException("can't create cell " + intfromx.ToString() + " " + intfromy.ToString() ))
            if matrix.[intfromy].ContainsKey intfromx = false
            then
                addCellOnUserRequest intfromx intfromy
            let inttox = (int) tox
            let inttoy = (int) toy
            if matrix.Length < inttoy
            then raise (System.ArgumentException("can't create cell " + inttox.ToString() + " " + inttoy.ToString() ))
            if matrix.[inttoy].ContainsKey inttox = false
            then addCellOnUserRequest inttox inttoy
            matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
//            let inttox = (int) tox
//            let inttoy = (int) toy
//            if matrix.Length < inttoy
//            then raise (System.ArgumentException("can't create cell " + inttox.ToString() + " " + inttoy.ToString() ))
//            if matrix.[inttoy].ContainsKey inttox = false
////            then
////                matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
////            else
//            then
//                addCellOnUserRequest inttox inttoy
//            matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
        | Mvc ((x, y), arg) -> 
            // todo correction exceptions
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < inty
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " " + inty.ToString() ))
            if matrix.[inty].ContainsKey intx = false
//                then matrix.[inty].[intx].RunOp arg
//            else
            then
                addCellOnUserRequest intx inty
            matrix.[inty].[intx].RunOp arg
        | Eps -> ()
        
    let Check (line: array<Asm<'a>>) =
        let cellsForWrite = new HashSet<(int * int)>()
        let cellsForRead = new HashSet<(int * int)>()
        for i in 0 .. line.Length - 1 do
            match line.[i] with
            | Eps -> ()
            | Set ((x, y), arg) ->
                let dot = (int x, int y)
                if matrix.Length < int y || int x < 0 || int y < 0
                then raise (OperationException(" can't find cell " + (int x).ToString() + " " + (int y).ToString() ))
                if cellsForWrite.Contains(dot)
                then
                    raise (ArgException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(dot) |> ignore
            | Mvc ((tox, toy), arg) -> 
                let todot = (int tox, int toy)
                if matrix.Length < int toy || int tox < 0 || int toy < 0
                then raise (OperationException(" can't find cell " + (int tox).ToString() + " " + (int toy).ToString() ))
                if cellsForWrite.Contains(todot)
                then
                    raise (ArgException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(int tox, int toy) |> ignore
            | Mov ((fromx, fromy), (tox, toy)) ->
                let fromdot = (int fromx, int fromy)
                if matrix.Length < int fromy || int fromx < 0 || int fromy < 0
                then raise (OperationException(" can't find cell " + (int fromx).ToString() + " "+ (int fromy).ToString() ))
                if cellsForRead.Contains(fromdot)
                then
                    raise (ArgException(" can't write and read together in equal cells"))
                else
                    cellsForRead.Add(fromdot) |> ignore
                let todot = (int tox, int toy)
                if matrix.Length < int toy || int tox < 0 || int toy < 0
                then raise (OperationException(" can't find cell " + (int tox).ToString() + " " + (int toy).ToString() ))
                if cellsForWrite.Contains(todot) || cellsForRead.Contains(todot)
                then
                    raise (ArgException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(todot) |> ignore

    member this.RunLine (line: array<Asm<'a>>) =
        let nline = [|line|]
        this.RunOp nline

    member this.WorkFlows (program: Program<'a>) = 
        try
            if program.Length = 0
                then ()
                else
                    let num = program.[0].Length
                    for i in 0 .. program.Length - 1 do
                        if num <> program.[i].Length
                        then raise(System.ArgumentException("in line " + (i + 1).ToString() + " workflows aren't compatible"))
        with
        | :? System.Exception -> reraise()

    member this.RunOp (program: Program<'a>) = 
        for i in 0 .. program.Length - 1 do 
            try
                Check program.[i] 
            with
            | OperationException(str) -> raise(System.IndexOutOfRangeException("in line " + (i + 1).ToString() + " " + str))
            | ArgException(str) -> raise(System.ArgumentException("in line " + (i + 1).ToString() + " " + str))
        
        try
        //to do with usage "for" to know line of exception
            Array.iter (fun i -> (Array.Parallel.iter (fun p -> intASM p) i)) program
        with
        | :? System.Exception -> raise(System.ArgumentException("Can't do operation"))
        
    member this.ValueInCell row col =
        let currentCol = matrix.[col]
        if currentCol.ContainsKey row = false
        then
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
        for kvp in matrix.[col] do//KeyVakuePair
            cellsForAdd.Add(kvp.Key, kvp.Value.Value.ToString())
        cellsForAdd