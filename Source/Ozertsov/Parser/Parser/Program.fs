open Parser.Lexer
open Parser.Parser
open TTA.ASM

open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser

type line = list<Asm*string>*string

let main (inputFile: string) = 
    use reader = new System.IO.StreamReader(inputFile)
    let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromTextReader reader
    let allTokens = 
        seq
            {
                while not lexbuf.IsPastEndOfStream do 
                    let t = token lexbuf
                    printfn "%A" t.GetType
                    yield t
            }
    let p = allTokens //|> Array.ofSeq
    //printfn "%A" p.[0].GetType
    
    let translateArgs = {
        tokenToRange = fun x -> 0UL,0UL
        zeroPosition = 0UL
        clearAST = false
        filterEpsilons = true
    }

    let tree =
        match Parser.Parser.buildAst p with
        | Success (sppf, t, d) -> 
            printfn "kolyalox:%A" //sppf.Tokens
            Parser.Parser.translate translateArgs sppf d 
        | Error (pos,errs,msg,dbg,_) -> failwithf "Error: %A    %A \n %A"  pos errs msg
    //let k =  (list<list<line>>) tree
    printfn "TREE: %A" tree

main @"..\..\input"
