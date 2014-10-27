module Cell
type Cell<'a>(operation: 'a -> 'a -> 'a) =                      
    let mutable value = Unchecked.defaultof<'a>

    let mutable operation = operation
    let mutable value = Unchecked.defaultof<'a>
      
    member this.Value 
        with get() = value
        and set(argument) = value <- argument
        
    member this.RunOp(operand) = 
        value <- operation value operand


        
        