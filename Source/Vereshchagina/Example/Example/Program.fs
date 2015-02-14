open NUnit.Framework
open TTA.ASM

//board:
//  | 0 | 1 | 2 | 3
//-----------------
//0 | + | - | * | /
//-----------------
//1 | + | - | * | /
//----------------- 
//2 | + | - | * | /
//-----------------
//3 | + | - | * | /
//-----------------

[<TestFixture>]
type Test() =    
    
    //calculation of 1 - (2 + 3) * 4 * (5 + 6) - 7    
   
    let asm = [|[|Set((0<ln>, 1<col>), 1);  
                  Set((0<ln>, 0<col>), 2); Mvc((0<ln>, 0<col>), 3); 
                  Set((1<ln>, 0<col>), 5); Mvc((1<ln>, 0<col>), 6);
                  Set((0<ln>, 2<col>), 4); Mov((0<ln>, 2<col>), (0<ln>, 0<col>)); 
                  Mov((0<ln>, 2<col>), (1<ln>, 0<col>));
                  Mov((0<ln>, 1<col>), (0<ln>, 2<col>));  Mvc((0<ln>, 1<col>), 7)|]|]     

    [<Test>]
    member this.``intermediate result``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore
        Assert.That(220, Is.EqualTo (main.getVal 0 2))

    [<Test>]
    member this.``result``() =        
        main.run (TTA.Translator.asmToIL asm) |> ignore
        Assert.That(-226, Is.EqualTo (main.getVal 0 1))