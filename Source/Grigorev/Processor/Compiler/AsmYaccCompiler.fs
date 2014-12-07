namespace Compiler

open TTA.ASM
open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser
open Lexer

type AsmYaccCompiler<'T> () =
    let translate command =
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromString command
        let allTokens = 
            seq
                {
                    while not lexbuf.IsPastEndOfStream do yield token lexbuf
                }

        let translateArgs = {
            tokenToRange = fun x -> 0UL, 0UL
            zeroPosition = 0UL
            clearAST = false
            filterEpsilons = true
        }
    
        match Parser.buildAst allTokens with
        | Success (sppf, t, d) ->
            let tmp = Parser.translate translateArgs sppf d
            Some (List.head tmp)
        | Error (pos, errs, msg, dbg, _) -> None

    let compile code =
        code |> Array.map (fun x -> x |> Array.map (fun y -> try translate y with | _ -> None))

    interface IAsmCompiler<'T> with
        member this.Compile data =
            compile data

    member this.Compile data : Asm<'T> option array array =
        compile data