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
    let matrix = Array.init functions.Length (fun i -> Dictionary<int, Cell<'a>>())

    let addCellOnUserRequest row col = matrix.[col].Add (row, Cell(functions.[col]))

    //x is row, y is column
    let intASM command =
        match command with
        | Set ((x, y), arg) ->
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < intx
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " "+ inty.ToString() ))
            if matrix.[inty].ContainsKey intx
            then matrix.[inty].[intx].Value <- arg
            else 
                addCellOnUserRequest intx inty
                matrix.[inty].[intx].Value <- arg

        | Mov ((fromx, fromy), (tox, toy)) ->
            let intfromx = (int) fromx
            let intfromy = (int) fromy
            if matrix.Length < intfromx
            then raise (System.ArgumentException("can't create cell " + intfromx.ToString() + " "+ intfromy.ToString() ))
            if matrix.[intfromy].ContainsKey intfromx
            then
                let inttox = (int) tox
                let inttoy = (int) toy                
                if matrix.Length < inttox
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
                if matrix.Length < inttox
                then raise (System.ArgumentException("can't create cell " + inttox.ToString() + " "+ inttoy.ToString() ))
                if matrix.[inttoy].ContainsKey inttox
                then
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
                else
                    addCellOnUserRequest inttox inttoy
                    matrix.[inttoy].[inttox].RunOp (matrix.[intfromy].[intfromx].Value)
        | Mvc ((x, y), arg) -> 
            let intx = (int) x
            let inty = (int) y
            if matrix.Length < intx
            then raise (System.ArgumentException("can't create cell " + intx.ToString() + " "+ inty.ToString() ))
            matrix.[inty].[intx].RunOp arg
        | Eps -> ()
    let Check (line: array<Asm<'a>>) =
        let cellsForWrite = new HashSet<(int * int)>()
        let cellsForRead = new HashSet<(int * int)>()
        for i in 0 .. line.Length - 1 do
            match line.[i] with
            | Set ((tox, toy), arg) ->
                let todot = (int tox, int toy)
                if cellsForWrite.Contains(todot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(int tox, int toy) |> ignore
            | Mvc ((tox, toy), arg) -> 
                let todot = (int tox, int toy)
                if cellsForWrite.Contains(todot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(int tox, int toy) |> ignore
            | Mov ((fromx, fromy), (tox, toy)) ->
                let todot = (int tox, int toy)
                if cellsForWrite.Contains(todot) || cellsForRead.Contains(todot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForWrite.Add(int tox, int toy) |> ignore
                let fromdot = (int fromx, int fromy)
                if cellsForWrite.Contains(fromdot)
                then
                    raise (System.ArgumentException("can't write and read together in equal cells"))
                else
                    cellsForRead.Add(int tox, int toy) |> ignore
            | Eps -> ()
    member this.RunOp (program: Program<'a>) =
        if program.Length = 0
        then ()
        else
            let i = program.[0].Length
            for p in program do
                if i <> p.Length
                then raise(System.ArgumentException("workflows aren't compatible"))
            Array.Parallel.iter (fun i -> Check i ) program
            Array.iter (fun i -> (Array.Parallel.iter (fun p -> intASM p) i)) program
        
    member this.ValueInCell row col =
        let currentCol = matrix.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value
            
    member this.NumColls =
        matrix.Length
    member this.NumRows =
        Array.max(Array.init matrix.Length (fun i -> if (matrix.[i].Count > 0) then (matrix.[i].Keys.Max()) else 0)) + 1
        
//let public static matrix = new Matrix<int>([|(fun x y -> x + y); (fun x y -> x - y); (fun x y -> x * y); (fun x y -> x / y)|])