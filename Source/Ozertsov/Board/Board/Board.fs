module Processor

open Cell

type Matrix(rows:int, cols:int) =
    do
        if rows < 0 || cols < 0
        then
            raise (System.ArgumentException("One of the parameters is less of zero"))
    let matrix : Cell[,] = Array2D.zeroCreate(rows)(cols)

    member this.ValueInCell x y =
        matrix.[x, y].Value