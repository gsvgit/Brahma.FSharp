module UnitTests

open TTA.ASM
open NUnit.Framework

[<TestFixture>]
type TestASM() =    
    // ((3 + 6) * 4 + 8 / (3 - 1)) * 5
    let asm = [|[|
                Set((0<ln>, 0<col>), 3); Mvc((0<ln>, 0<col>), 6); Set((0<ln>, 1<col>), 3); Mvc((0<ln>, 1<col>), 1); 
                Set((0<ln>, 2<col>), 4); Mov((0<ln>, 2<col>), (0<ln>, 0<col>)); Set((0<ln>, 3<col>), 8);
                Mov((0<ln>, 3<col>), (0<ln>, 1<col>)); Set((1<ln>, 0<col>), 0); Mov((1<ln>, 0<col>), (0<ln>, 2<col>)); 
                Mov((1<ln>, 0<col>), (0<ln>, 3<col>)); Set((1<ln>, 2<col>), 5); Mov((1<ln>, 2<col>), (1<ln>, 0<col>))
              |]|]     

    [<Test>]
    member this.``result``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore        
        Assert.AreEqual(200, main.getVal 1 2)


