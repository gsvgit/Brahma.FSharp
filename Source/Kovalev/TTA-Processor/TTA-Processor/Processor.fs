module Processor

open Cell

type GridConfig(rows, col, functions: ((int -> int -> int) * int) array) =   //I don't know where the best place for handling these exceptions.
    do                                                                       //Maybe "failwith" should be there or parameters must be checked  
        if rows = 0 || col = 0                                               //after enter in GUI immediately
        then raise (System.ArgumentException("One of the parameters is equal to zero"))  
        if functions.Length > 20
        then raise (System.ArgumentException("Maximum number of operations - 20"))
        if functions.Length > rows * col
        then raise (System.ArgumentException("The number of operation types is more than the total number of cells"))        
        if Array.sumBy (fun i -> snd i) functions > rows * col
        then raise (System.ArgumentException("The amount of requested cells is more than the total number of cells"))

    member val Rows = rows with get
    member val Col = col with get
    member val Operations = functions with get
 
 type Processor(config: GridConfig) =        
    
    let grid : Cell[,] = Array2D.zeroCreate config.Rows config.Col 
    
    do  
        let mutable currentRow, currentCol = 0, 0        
        for op in config.Operations do 
            for j = 1 to snd op do
                grid.[currentRow, currentCol] <- Cell(fst op)
                currentRow <- currentRow + 1
                if currentRow = config.Rows
                then 
                    currentRow <- 0
                    currentCol <- currentCol + 1
        while currentCol < config.Col do
            grid.[currentRow, currentCol] <- Cell()  
            currentRow <- currentRow + 1
            if currentRow = config.Rows
                then 
                    currentRow <- 0
                    currentCol <- currentCol + 1
                                                           
    member this.ValueInCell x y =
        grid.[x, y].Value
    

    //some methods...