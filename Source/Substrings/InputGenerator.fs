module InputGenerator

open System.IO


let length = 11000000L
let path = "../../random.txt"
let random = new System.Random()

let Main =
    let mutable generated = 0L
    let mutable i = 0
    let chunk = 3000000
    let buffer = Array.zeroCreate chunk

    let writer = new FileStream(path, FileMode.Create)


    while generated < length do
        buffer.[i] <- (byte) (random.Next(255))
        generated <- generated + 1L
        i <- i + 1
        
        if i = chunk then
            writer.Write(buffer, 0, chunk)
            i <- 0

    if i > 0 then
        writer.Write(buffer, 0, i)

    writer.Close

do Main()