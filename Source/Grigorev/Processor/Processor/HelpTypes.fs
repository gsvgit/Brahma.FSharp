namespace Processor

open TTA.ASM

exception IncorrectCommandException of string * int * int
exception IncorrectLineException of string * int

type CodeError =
    | IncorrectLine of string * int
    | IncorrectCommand of string * int * int

type Mismatch =
    | Row
    | Column
    | Both
    override this.ToString () =
        match this with
        | Row -> "Row index is less than zero"
        | Column -> "Column index is out of range"
        | Both -> "Both row and column indexes are out of range"

type CellState =
    | Exists
    | CanExist
    | CannotExist of Mismatch

type ReadOption<'T> =
    | Cell of int<ln> * int<col>
    | Value of 'T

type GridCell<'T> = {
    Row : int
    Col : int
    Value : 'T
}