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
    // (2 / 1) + (3 + 4) * (9 - 5) * 10
    let asm = [|[|
                Set((0<ln>, 3<col>), 2); Mvc((0<ln>, 3<col>), 1); 
                Set((0<ln>, 0<col>), 3); Mvc((0<ln>, 0<col>), 4); 
                Set((0<ln>, 1<col>), 9); Mvc((0<ln>, 1<col>), 5);
                Set((0<ln>, 2<col>), 10); 
                Mov((0<ln>, 2<col>), (0<ln>, 1<col>));
                Mov((0<ln>, 2<col>), (0<ln>, 0<col>));
                Set((0<ln>, 0<col>), 0);
                Mov((0<ln>, 0<col>), (0<ln>, 3<col>));
                Mov((0<ln>, 0<col>), (0<ln>, 2<col>))
              |]|]     

    [<Test>]
    member this.``result``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore
        Assert.AreEqual(282, main.getVal 0 0)
        
    [<Test>]
    member this.``valueInCell``() =
        Assert.AreEqual(2, main.getVal 0 3)
        
