module Cell

type Cell(operation: int -> int -> int) =                        

    let mutable value = 0
    let op = operation
      
    member this.Value 
        with get() = value
        and set(num) = value <- num    
    member this.ExecuteOp(operand) = 
        value <- op value operand       
        
    new() = Cell(fun x y -> y)
    