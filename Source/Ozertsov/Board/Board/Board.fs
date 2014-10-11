module Processor

open Cell

type Matrix(rows:int, cols:int) =
    let matrix = Cell array = Array2D.zeroCreate(rows)(cols)
(*type Grid(rows, col, addNum, subtNum, multNum, divNum) =
    do
        if addNum + subtNum + multNum + divNum > rows * col
        then failwith "There are too many operation cells"
    
    let grid = Array2D.create (rows - 1) (col - 1)*)