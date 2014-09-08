open TTA.ASM

(*
board:
  | 0 | 1 | 2 | 3
-----------------
0 | + | - | * | /
-----------------
1 | + | - | * | /
-----------------
2 | + | - | * | /
-----------------
3 | + | - | * | /
-----------------
*)
do
    //asm for (1 + 2) * 3
    let asm = [|[|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 2); Set((0<ln>, 2<col>),3); Mov((0<ln>, 2<col>),(0<ln>, 0<col>));|]|]
    main.run (TTA.Translator.asmToIL asm) |> ignore
    main.getVal 0 2 |> (printfn "result: %A")