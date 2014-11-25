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


[<TestFixture>]
type CheckTests () =
    let functions = [|(+)|]
    let proc = new Processor<int> (functions)

    [<Test>]
    member this.Test0 () =
        proc.Clear ()
        let res = proc.CheckCommand (Eps)
        Assert.That (None, Is.EqualTo (res))

[<TestFixture>]
type IntTests () =
    let functions = [|
            (+);
            (-);
            (*);
            (/);
            (%)
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