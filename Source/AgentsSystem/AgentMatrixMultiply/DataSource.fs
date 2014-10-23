namespace AsyncMatrixMultiply

module DataSource =
    
//    type IDataSource = interface
//        end

    let random = new System.Random()

    let MakeMatrix rows cols =
        Array.init (rows * cols) (fun i -> float32 (random.NextDouble())), Array.init (rows * cols) (fun i -> float32 (random.NextDouble()))

    let rows = 600
    let columns = 600    

    let aValues = MakeMatrix rows columns
    let bValues = MakeMatrix rows columns
        
    let GenerateMatrices pairsCount = Array.init pairsCount (fun _ -> (MakeMatrix rows columns, MakeMatrix rows columns))