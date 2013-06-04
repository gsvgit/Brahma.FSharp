module OneMatchSearch

open System.IO
open System.Runtime.Serialization.Formatters.Binary

open TemplatesGenerator

let path = "../../templates.txt"

let writeFile position = 
    let mutable generated = 0L
    let mutable i = 0
    let chunk = 3000000
    let buffer = Array.zeroCreate chunk

    let writer = new FileStream("../../" + position.ToString() + ".txt", FileMode.Create)

    while generated < 100000000L do
        if (generated) >= (int64) position && (generated) < ((int64) position + 4L) then
            buffer.[i] <- 1uy
        else
            buffer.[i] <- 0uy

        generated <- generated + 1L
        i <- i + 1
        
        if i = chunk then
            writer.Write(buffer, 0, chunk)
            i <- 0

    if i > 0 then
        writer.Write(buffer, 0, i)

    writer.Close()

let Main () =
    let readyTemplates = { number = 1; sizes = [|4uy|]; content = [|1uy; 1uy; 1uy; 1uy|];}

    let writer = new FileStream(path, FileMode.Create)
    let formatter = new BinaryFormatter()

    formatter.Serialize(writer, readyTemplates)

    writer.Close()

    let first = [88604636..88604640]
    let second = [88604668..88604672]
    let positions = List.append first second

    for i in positions do
        writeFile i

    