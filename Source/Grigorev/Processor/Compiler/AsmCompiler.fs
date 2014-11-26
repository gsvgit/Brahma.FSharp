namespace Compiler

open System.Text.RegularExpressions
open TTA.ASM

type RegexType =
    | Eps of Regex
    | Set of Regex
    | Mvc of Regex
    | Mov of Regex
    member this.Value =
        match this with
        | Eps r | Set r | Mvc r | Mov r -> r

type AsmRegexCompiler<'T> () =
    let regexs =
        [|
            Eps (new Regex ("eps"));
            Set (new Regex ("set (0 | [1-9][0-9]*) (0 | [1-9][0-9]*) (.*)"));
            Mvc (new Regex ("mvc (0 | [1-9][0-9]*) (0 | [1-9][0-9]*) (.*)"));
            Mov (new Regex ("mov (0 | [1-9][0-9]*) (0 | [1-9][0-9]*) (0 | [1-9][0-9]*) (0 | [1-9][0-9]*)"))
        |]

    let compile data =
        data |> Array.mapi (fun j e -> e |> Array.mapi (fun i el -> None))
    interface IAsmCompiler<'T> with
        member this.Compile data =
            compile data

