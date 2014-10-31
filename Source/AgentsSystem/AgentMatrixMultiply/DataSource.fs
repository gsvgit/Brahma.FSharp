namespace AsyncMatrixMultiply

module DataSource =
    
    open System.Threading

    let MakeMatrix rows cols =
        Thread.Sleep(2000)
        let random = new System.Random()

        let a = Array.init (rows * cols) (fun i -> float32 (random.NextDouble()))
        let b = Array.init (rows * cols) (fun i -> float32 (random.NextDouble()))

        a, b
