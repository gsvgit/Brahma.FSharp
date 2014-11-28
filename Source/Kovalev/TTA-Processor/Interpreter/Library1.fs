module Interpreter.test
open Interpreter.Lexer

open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser

let main (inputFile: string) = 
    use reader = new System.IO.StreamReader(inputFile)
    let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromTextReader reader
    let allTokens = 
        seq
            {
                while not lexbuf.IsPastEndOfStream do yield token lexbuf
            }

    let translateArgs = {
        tokenToRange = fun x -> 0UL,0UL
        zeroPosition = 0UL
        clearAST = false
        filterEpsilons = true
    }
    
    let tree =
        match Interpreter.Parser.buildAst allTokens with
        | Success (sppf, t, d) -> Interpreter.Parser.translate translateArgs sppf d 
        | Error (pos,errs,msg,dbg,_) -> failwithf "Error: %A    %A \n %A"  pos errs msg

    printfn "TREE: %A" tree