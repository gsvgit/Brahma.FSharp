namespace ProcessorTests

open NUnit.Framework
open TTA.ASM
open Processor

type PseudoAsm<'T> =
    | PSet of int * int * 'T
    | PMov of int * int * int * int
    | PMvc of int * int * 'T
    | Nop
    member this.ToAsm () =
        match this with
        | Nop -> Eps
        | PSet (r, c, v) -> Set ((r*1<ln>, c*1<col>), v)
        | PMvc (r, c, v) -> Mvc ((r*1<ln>, c*1<col>), v)
        | PMov (r, c, R, C) -> Mov ((r*1<ln>, c*1<col>), (R*1<ln>, C*1<col>))

type Help () =
    static member ArrToAsm (comm : array<array<PseudoAsm<_>>>) = comm |> Array.map (fun x -> x |> Array.map (fun y -> y.ToAsm ()))

[<TestFixture>]
type CheckTests () =
    let functions = [|(+)|]
    let proc = new Processor<int> (functions)

    [<Test>]
    member this.Test0 () =
        proc.Clear ()
        let res = proc.CheckCommand (Eps)
        Assert.That (None, Is.EqualTo (res))

    [<Test>]
    member this.Test1 () =
        proc.Clear ()
        let comm = 
            [|
                [|PSet (0, 0, 1)|];
                [|PSet (0, 0, 1)|]
            |]
        Assert.Throws<IncorrectLineException> (fun () -> proc.Evaluate (Help.ArrToAsm (comm))) |> ignore

    [<Test>]
    member this.Test2 () =
        proc.Clear ()
        let comm = 
            [|
                [|PMov (0, 1, 1, 1)|];
                [|PSet (0, 0, 1)|]
            |]
        Assert.Throws<IncorrectLineException> (fun () -> proc.Evaluate (Help.ArrToAsm (comm))) |> ignore

    [<Test>]
    member this.Test3 () =
        proc.Clear ()
        let comm = 
            [|
                [|Nop; PSet (1, 0, 1)|];
                [|Nop; PMov (0, 0, 1, 1)|]
            |]
        let res = proc.CheckProgram (Help.ArrToAsm comm)
        Assert.That (res, Is.EqualTo (Some ([|IncorrectCommand ("Column index is out of range", 1, 1)|])))

    [<Test>]
    member this.Test4 () =
        proc.Clear ()
        let comm = 
            [|
                [|Nop; PMov (0, 0, 1, 0)|];
                [|Nop; PSet (0, 0, 1)|]
            |]
        let res = proc.CheckProgram (Help.ArrToAsm comm)
        Assert.That (res, Is.EqualTo (Some ([|IncorrectLine ("There are 2 or more writes in single cell", 1)|])))


[<TestFixture>]
type IntTests () =
    let functions = [|
            (+);
            (-);
            (*);
            (/);
            fun x y -> -y
        |]
    let proc = new Processor<int> (functions)
    
    [<Test>]
    member this.Test0 () =
        proc.Clear ()
        let comm =
            [|
                [| PSet (0, 0, 1); PMvc (0, 0, 4); PMvc (0, 0, 4) |];
                [| PSet (0, 1, 1); PMvc (0, 1, 4); PMvc (0, 1, 4) |];
                [| PSet (1, 0, 0); PMov (1, 0, 0, 1); PMov (1, 0, 0, 1); PMov (1, 0, 0, 1) |]
            |]
            |> Array.map (fun x -> x |> Array.map (fun y -> y.ToAsm ()))
        proc.Evaluate (comm)
        Assert.That (proc.Read (1, 0), Is.EqualTo (-9))

    [<Test>]
    member this.Test1 () = // 17 + 4*9 - 9/3 - 2 + 11*5 = 115
        proc.Clear ()
        let comm =
            [|
                [| PSet (0, 1, 17); PMvc (0, 1, 2); PMov (0, 0, 0, 1); PMov (0, 0, 0, 2); PMov (0, 0, 1, 0) |];
                [| PSet (1, 2, 11); PMvc (1, 2, 5); PMov (1, 0, 1, 2); PMov (1, 0, 0, 3) |];
                [| PSet (0, 2, 4); PMvc (0, 2, 12) |];
                [| PSet (0, 3, -9); PMvc (0, 3, 3) |]
            |]
            |> Array.map (fun x -> x |> Array.map (fun y -> y.ToAsm ()))
        proc.Evaluate (comm)
        Assert.That (proc.Read (0, 0), Is.EqualTo (115))

    [<Test>]
    member this.Test2 () =
        proc.Clear ()
        let comm =
            [|
                [| PSet (0, 1, 7); PMov (0, 1, 1, 0) |];
                [| PSet (1, 0, 100); PMov (1, 0, 0, 1) |]
            |]
            |> Array.map (fun x -> x |> Array.map (fun y -> y.ToAsm ()))
        proc.Evaluate (comm)
        Assert.That (proc.Read (1, 0), Is.EqualTo (107))
        Assert.That (proc.Read (0, 1), Is.EqualTo (-93))