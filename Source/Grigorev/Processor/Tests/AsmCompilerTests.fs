namespace AsmCompilerTests

open NUnit.Framework
open TTA.ASM
open Compiler

[<TestFixture>]
type public RegexTests () =
    [<Test>]
    member this.Test0 () =
        let code = 
            [|[|
                " ePs   ";
                "Set 5 3 7 ";
                "mvc 0 8    -34   ";
                "\tmov 0 9 7 6   ";
                "adf";
                "";
                "set 6 -7 3";
                "mov 4 5 6 o";
                "epsilon";
                "SET 0 0 0"
                "asdf eps";
                "mov  0 8 66 9";
                "set 0 0 df";
                "set 0 0 ";
                "mvc 8 6     56-7"
            |]|]
        let exRes =
            [|[|
                Some (Asm.Eps);
                Some (Asm.Set ((5<ln>, 3<col>), 7));
                Some (Asm.Mvc ((0<ln>, 8<col>), -34));
                Some (Asm.Mov ((0<ln>, 9<col>), (7<ln>, 6<col>)));
                None;
                None;
                None;
                None;
                None;
                Some (Asm.Set ((0<ln>, 0<col>), 0));
                None;
                Some (Asm.Mov ((0<ln>, 8<col>), (66<ln>, 9<col>)));
                None;
                None;
                None
            |]|]
        let comp = new AsmRegexCompiler<int> ()
        Assert.That (comp.Compile code, Is.EqualTo (exRes))

[<TestFixture>]
type public YaccTests () =
    [<Test>]
    member this.Test0 () =
        let thr = 
            [|
                "ePs";
                "Set 5 3 7 ";
                "mvc 0 8    -34   ";
                "\tmov 0 9 7 6   ";
                "adf";
                "";
                "set 6 -7 3";
                "mov 4 5 6 o";
                "epsilon";
                "SET 0 0 0"
                "asdf eps";
                "mov  0 8 66 9";
                "set 0 0 df";
                "set 0 0 ";
                "mvc 8 6     56-7"
            |] |> String.concat "\r\n"
        let code = [| thr |]
        let exRes =
            [|[|
                Some (Asm.Eps);
                Some (Asm.Set ((5<ln>, 3<col>), 7));
                Some (Asm.Mvc ((0<ln>, 8<col>), -34));
                Some (Asm.Mov ((0<ln>, 9<col>), (7<ln>, 6<col>)));
                None;
                None;
                Some (Asm.Set ((6<ln>, -7<col>), 3));
                None;
                None;
                Some (Asm.Set ((0<ln>, 0<col>), 0));
                None;
                Some (Asm.Mov ((0<ln>, 8<col>), (66<ln>, 9<col>)));
                None;
                None;
                None
            |]|]
        let comp = new AsmYaccCompiler<int> ()
        Assert.That (comp.Compile code, Is.EqualTo (exRes))