module Processor

open Cell
open TTA.ASM
open System.Collections.Generic

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
        //must create exception, if have not y
            let intx = (int) x
            let inty = (int) y
            if matrix.[inty].ContainsKey intx
            then matrix.[inty].[intx].Value <- arg
            else 
                addCellOnUserRequest intx inty
                matrix.[inty].[intx].Value <- arg

        | Mov ((fromx, fromy), (tox, toy)) ->
        //must create exception, if haven't fromy or toy
            let intfromx = (int) fromx
            let intfromy = (int) fromy
            if matrix.[intfromy].ContainsKey intfromx
            then
                let inttox = (int) tox
                let inttoy = (int) toy
                if matrix.[inttoy].ContainsKey inttox
                then
                    matrix.[inttoy].[inttox].Value <- matrix.[intfromy].[intfromx].Value//Matrix.ValueInCell intfromx intfromy
                else
                    addCellOnUserRequest inttox inttoy
                    matrix.[inttoy].[inttox].Value <- matrix.[intfromy].[intfromx].Value
            //else
                //create exception????
        | Mvc ((x, y), arg) -> 
            //must create Exception if haven't y
            let intx = (int) x
            let inty = (int) y
            matrix.[inty].[intx].RunOp arg
        | Eps -> ()
        //must create Exception if haven't y
            

    member this.ValueInCell row col =
        let currentCol = matrix.[col]
        if currentCol.ContainsKey row
        then currentCol.[row].Value
        else
            addCellOnUserRequest row col
            currentCol.[row].Value


