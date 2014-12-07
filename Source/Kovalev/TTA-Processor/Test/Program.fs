module Program
open TTA.Compiler
open TTA.Processor

let x = compile "Set (0 0) 1 Set (0 1) 2;
                 Mvc (0 0) 3;
                 Eps         Eps         Eps;"

let proc = Processor<int> [|(fun x y -> x + y); (fun x y -> x * y)|]
proc.Run x

printfn "%A" x
printfn "%A" (proc.ValueInCell 0 0)

System.Console.ReadKey() |> ignore