module Cell

open System.Collections.Generic

type Cell<'a> (operation : 'a -> 'a -> 'a) =
    
    let mutable value = Unchecked.defaultof<'a>

    member this.Value
        with get() = value
        and set(currentValue) = value <- currentValue

    member this.Execute arg =
        value <- operation value arg