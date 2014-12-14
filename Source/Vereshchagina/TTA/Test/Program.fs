open TTA.Processor
open TTA.ASM
open TTA.Parser

let processor = new Processor<_>([|(fun x y -> x + y); (fun x y -> x + y)|])
for i in 0..100 do
    processor.Run [| [|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 2)|]; 
                     [|Set((0<ln>, 1<col>), 3); Mvc((0<ln>, 1<col>), 4)|];
                     [|Eps|]|]
         
    processor.ValueInCell 0 0 |> printfn "%A"
    
let x  = parse "Set (0 0 3) Mov (0 0 1 1);
                Set (0 0 4) Eps;
                Eps         Eps           Eps        Mvc (0 0 3);"

printfn "%A" x