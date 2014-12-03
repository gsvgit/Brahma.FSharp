module Cell

type Cell(OpSymbol: char) =                        
    do        
        if Array.tryFind (fun elem -> elem = OpSymbol) [|'+'; '-'; '*'; '/'; ' '|] = None
        then failwith "It's not an operation"
    
    let opDeterm ch =
        match ch with
        |'+' -> (fun (a: int) (b: int) -> a + b)
        |'-' -> (fun (a: int) (b: int) -> a - b)
        |'*' -> (fun (a: int) (b: int) -> a * b)
        |'/' -> (fun (a: int) (b: int) -> 
                                            if (b <>0)
                                            then 
                                                a / b
                                            else
                                                failwith "Null")
        |' ' -> (fun (a: int) (b: int) -> 0)
        |_ -> failwith "Not found your operation"       

    let mutable op = OpSymbol    
    let mutable opFunc = opDeterm OpSymbol
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
    