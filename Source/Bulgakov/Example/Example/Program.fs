module UnitTests

open TTA.ASM
open NUnit.Framework

[<TestFixture>]
type TestASM() =    
    // 1 + 2 * 3 + (10 - 6) * 5
    let asm = [|[|Set((0<ln>, 2<col>), 2); Mvc((0<ln>, 2<col>), 3); 
                  Set((0<ln>, 0<col>), 1); Mov((0<ln>, 0<col>), (0<ln>, 2<col>)); 
                  Set((0<ln>, 1<col>), 10); Mvc((0<ln>, 1<col>), 6);
                  Set((1<ln>, 2<col>), 5); Mov((1<ln>, 2<col>), (0<ln>, 1<col>));
                  Mov((0<ln>, 0<col>), (1<ln>, 2<col>))|]|]     

    [<Test>]
    member this.``Test1``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore
        Assert.AreEqual(27, main.getVal 0 0)