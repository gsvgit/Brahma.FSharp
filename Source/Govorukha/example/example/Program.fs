module Example.Asm

open TTA.ASM
open main

let example =
    // 10 - 20 / (2 * 3 + 4 * 1)  - 7 
    let (asm: Program) = [|[|Set((0<ln>,2<col>),2); 
                             Mvc((0<ln>,2<col>),3); 
                             Mov((0<ln>,0<col>),(0<ln>,2<col>)); 
                             Set((1<ln>,2<col>),4);
                             Mvc((1<ln>,2<col>),1);
                             Mov((0<ln>,0<col>),(1<ln>,2<col>));
                             Set((0<ln>,3<col>),20); 
                             Mov((0<ln>,3<col>),(0<ln>,0<col>));
                             Set((0<ln>,1<col>),10);
                             Mov((0<ln>,1<col>),(0<ln>,3<col>)); 
                             Mvc((0<ln>,1<col>),7)|]|]

    TTA.Translator.asmToIL asm |> main.run |> ignore
    main.getVal 0 1
   

    

 