module TTA.Parser
open Parser.Lexer

open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser

open TTA.ASM

exception ParserError of int * string
type line = list<Asm<int>> * string

let compile (inputString: string) =     
        
    let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromString inputString
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
    
    let (tree: #list<list<line>>) =
        match Parser.Parser.buildAst allTokens with
        | Success (sppf, t, d) -> Parser.Parser.translate translateArgs sppf d 
        | Error (pos,errs,msg,dbg,_) -> raise (ParserError(pos, msg))
    
    let tr = tree.[0]   
    let numOfCol = fst (List.maxBy (fun (ln: line) -> (fst ln).Length) tr) |> List.length

    let (program: Program<int>) = Array.init numOfCol (fun i -> Array.init tr.Length (fun j -> if i > (fst tr.[j]).Length - 1 
                                                                                               then Eps 
                                                                                               else (fst tr.[j]).[i])) 
    program