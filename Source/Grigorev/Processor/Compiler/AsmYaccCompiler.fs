namespace Compiler

open TTA.ASM
open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser
open Lexer

type private Res<'T> =
    | S of Asm<'T> array
    | E of string
    member this.GetAsm =
        match this with
        | S a -> a
        | _ -> failwith "error"
    member this.GetErr =
        match this with
        | E e -> e
        | _ -> failwith "error"

type AsmYaccCompiler<'T> () =
    let translate command : Res<'T> =
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
            let (h : #list<list<Asm<'T>>>) = tmp
            S ( h.Head |> List.toArray )
        | Error (pos, errs, msg, dbg, _) -> E (msg)

    let compile code =
        let res = code |> Array.map (fun x -> try translate x with | e -> E (e.Message))
        let err, suc = Array.partition (fun x -> match x with | E _ -> true | S _ -> false) res
        if err.Length = 0
        then AsmCompilationResult.Success (suc |> Array.map (fun x -> x.GetAsm))
        else AsmCompilationResult.Error (err |> Array.map (fun x -> x.GetErr))

    interface IAsmCompiler<'T> with
        member this.Compile data =
            compile data

    member this.Compile data : AsmCompilationResult<'T> =
        compile data