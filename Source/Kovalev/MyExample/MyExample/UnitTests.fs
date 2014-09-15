module UnitTests

open TTA.ASM
open NUnit.Framework

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

[<TestFixture>]
type TestASM() =    
    // (1 + 2) * (3 + 4) * (9 - 5) * 10
    let asm = [|[|Set((0<ln>, 0<col>), 1); Mvc((0<ln>, 0<col>), 2); 
                  Set((1<ln>, 0<col>), 3); Mvc((1<ln>, 0<col>), 4); 
                  Set((0<ln>, 1<col>), 9); Mvc((0<ln>, 1<col>), 5);
                  Set((0<ln>, 2<col>), 10); Mov((0<ln>, 2<col>), (0<ln>, 0<col>));
                  Mov((0<ln>, 2<col>), (1<ln>, 0<col>)); Mov((0<ln>, 2<col>), (0<ln>, 1<col>))|]|]     

    [<Test>]
    member this.``result``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore
        Assert.AreEqual(840, main.getVal 0 2)
        
    [<Test>]
    member this.``valueInCell``() =
        Assert.AreEqual(3, main.getVal 0 0)
        
