module NonGpuTest
//Нужно проводить тесты от mandelbrot (cx:array<_>) 0.5 size -1.5 -1.0 400 400
//Где cx это подаваемый на вход массив. А size меняется. С julia тоже самое только (julia cx 0.5 size -1.5 -1.0 0.4 0.24 400 400)
//Для GPU тестов используй Mandelbrot ()  и Julia () от тех же параметров
let mandelbrot (cx:array<_>) scaling size mx my boxwidth boxheight =                                   
    for x in 0..boxwidth-1 do
        for y in 0..boxheight-1 do
            let fx = float x / size * scaling + mx
            let fy = float y / size * scaling + my
            let cr = fx
            let ci = fy
            let iter = 4000
            let mutable flag = true
            let mutable zr1 = 0.0
            let mutable zi1 = 0.0
            let mutable count = 0;
            while flag && count < iter do
                if (zr1 * zr1 + zi1 * zi1 ) <= 4.0
                then                        
                    let t = zr1
                    zr1 <- (zr1 * zr1 - zi1 * zi1 + cr)
                    zi1 <- (2.0 * zi1 * t + ci)
                    count <- count + 1
                else
                    flag <- false
            if count = iter
            then
                cx.[x * boxwidth + y] <- 0
            else
                cx.[x * boxwidth + y] <- count
    cx

let julia (cx: array<_>) scaling size mx my cr ci boxwidth boxheight =
    for x in 0..boxwidth-1 do
        for y in 0..boxheight-1 do
            let fx = float x / size * scaling + mx
            let fy = float y / size * scaling + my 
            let iter = 4000
            let mutable fl =  true
            let mutable zr1 = fx
            let mutable zi1 = fy
            let mutable count =  0
            let mutable t = 0.0
            while fl && (count < iter) do
                if zr1 * zr1 + zi1 * zi1 <= 4.0
                then 
                    t <- zr1
                    zr1 <- zr1 * zr1 - zi1 * zi1 + cr
                    zi1 <- 2.0 * zi1 * t + ci
                    count <- count + 1
                else
                    fl <- false
            if count = iter
            then
                cx.[x * boxwidth + y] <- 0
            else
                cx.[x * boxwidth + y] <- count
    cx
//let cx = Array.zeroCreate 160000
//printfn "%A" (mandelbrot cx 0.5 100.0 -1.5 -1.0 400 400)
//printfn "%A" (julia cx 0.5 100.0 -1.5 -1.0 0.4 0.24 400 400)