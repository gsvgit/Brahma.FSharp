module Cell

type Cell<'a> (operation : 'a -> 'a -> 'a) = 
  
    let mutable value = Unchecked.defaultof<'a>

    member this.Value 
        with get() = value
        and set(newValue) = value <- newValue

      member this.Execute argument =
        try
            value <- operation value argument
        with
        |ex  -> 
            raise (ex)