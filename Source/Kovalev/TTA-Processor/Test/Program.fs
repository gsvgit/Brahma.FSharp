module Program

open Processor
open TTA.ASM

let proc = new Processor<int>([|(fun x y -> x * y); (fun x y -> x + y)|])

let prog: Program<int> = [| [|Set((0<ln>, 0<col>), 6); Set((1<ln>, 1<col>), 3); 
                              Mvc((1<ln>, 1<col>), 4); Mov((0<ln>, 0<col>), (1<ln>, 1<col>))|] |]

proc.Run prog

printfn "%A" (proc.ValueInCell 0 0)

System.Console.ReadKey() |> ignore