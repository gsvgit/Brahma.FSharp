module Cell

type Cell(operation: int -> int -> int) =                        
    let value = ref 0
    let operation = operation
      
    member this.Value 
        with get() = !value
        and set(num) = value := num  
          
    member this.RunOp(operand) = 
        value := operation !value operand