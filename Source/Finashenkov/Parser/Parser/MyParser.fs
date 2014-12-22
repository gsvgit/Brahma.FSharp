module MyParser.MyParser
open TTA.ASM
open MyParser.Lexer

open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser

type MyParser() =
    let lex (input: string) = 
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromString input
        let allTokens = 
            seq
                {
                    while not lexbuf.IsPastEndOfStream do 
                        yield token lexbuf
                }
    
        let translateArgs = {
            tokenToRange = fun x -> 0UL,0UL
            zeroPosition = 0UL
            clearAST = false
            filterEpsilons = true
        }
    
        let tree =
            match MyParser.Parser.buildAst allTokens with
            | Success (sppf, t, d) -> MyParser.Parser.translate translateArgs sppf d
            | Error (pos,errs,msg,dbg,_) -> failwithf "Error: %A    %A \n %A"  pos errs msg
    
        tree

    let compileArr tree = 
        let (list: list<list<Asm<int>*string>>) = tree
        let (clearList:list<Asm<int>*string>) = list.Head
        let arr = Array.init clearList.Length (fun i -> match clearList.[i] with |(a, b) -> a)
        arr

    member this.Parse (input: string) = 
        let result = seq {
            let strarr = input.Replace("\r","").Split('\n')
            for s in strarr do

                yield compileArr(lex s)
        }
        Array.ofSeq result

let func =
    let parser = new MyParser()
    let p = parser.Parse("Eps; Set 0 0 1; \n Eps;")
    p
printfn "%A" func