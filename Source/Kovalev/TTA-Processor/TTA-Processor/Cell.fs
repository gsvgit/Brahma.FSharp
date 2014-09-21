module Cell

type Cell(opSymbol: char) =                        
    do        
        if Array.tryFind (fun elem -> elem = opSymbol) [|'+'; '-'; '*'; '/'; ' '|] = None
        then failwith "It's not an operation"
    
    let opDeterm ch =
        match ch with
        |'+' -> (fun (a: int) (b: int) -> a + b)
        |'-' -> (fun a b -> a - b)
        |'*' -> (fun a b -> a * b)
        |'/' -> (fun a b -> a / b)
        |' ' -> (fun a b -> b)
        |_ -> failwith "Impossible, just for VS"       

    let mutable op = opSymbol    
    let mutable opFunc = opDeterm opSymbol
    let mutable value = 0        
        
    member this.Op
        with get() = op
        and set(newOpSymbol: char) =  
            op <- newOpSymbol
            opFunc <- opDeterm newOpSymbol
    member this.Value
       with get() = value
       and set(num: int) = value <- num
    
    member this.ExecuteOp(operand: int) = value <- opFunc value operand 
    
    new() = Cell(' ')
    