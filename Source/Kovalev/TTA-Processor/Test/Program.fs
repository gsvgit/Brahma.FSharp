open Cell

let matrix : Cell[,] = Array2D.create 4 4 (Cell('*'))

matrix.[0, 3].Value |> printfn "%A"
System.Console.ReadKey() |> ignore

matrix.[0, 1].Value <- 5
matrix.[0, 1].ExecuteOp 5 
matrix.[0, 1].Value |> printfn "%A"
System.Console.ReadKey() |> ignore

let cell = Cell()
cell.ExecuteOp 5
cell.Value |> printfn "%A"
System.Console.ReadKey() |> ignore

cell.ExecuteOp 6
cell.Value |> printfn "%A"
System.Console.ReadKey() |> ignore

let badCell = Cell('k')