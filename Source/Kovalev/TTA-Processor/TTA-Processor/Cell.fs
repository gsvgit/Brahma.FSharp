module Cell

type Cell<'a>(operation: 'a -> 'a -> 'a) =

    let mutable value = Unchecked.defaultof<'a>
    let op = operation
      
    member this.Value
        with get() = value
        and set(arg) = value <- arg
    member this.ExecuteOp(operand) =
        value <- op value operand