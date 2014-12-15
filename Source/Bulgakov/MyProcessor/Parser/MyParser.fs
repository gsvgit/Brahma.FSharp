module MyParser.MyParser
open MyParser.Lexer

open Yard.Generators.Common.AST
open Yard.Generators.RNGLR.Parser

open TTA.ASM

type MyParser() =
    let lex (input: string) = 
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromString input
        let allTokens = 
            seq
                {
                    while not lexbuf.IsPastEndOfStream do 
                        yield token lexbuf
                }
        //printfn "%A" allTokens
    
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
    
        //printfn "TREE: %A" tree
        tree

    //type line = list<list<list<Asm<int>*string>*string>*list<Asm<int>*string>>

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
            
//    member this.Parse (input: string) = 
//        let result = seq { 
//            use reader = new System.IO.StreamReader(input)
//            while (not reader.EndOfStream ) do
//                let s = reader.ReadLine();
//                yield compileArr(lex s)
//        }
//        Array.ofSeq result

//printfn "%A" (matrix @"..\..\input")
//let pars = new MyParser()
//printfn "%A" (pars.Parse @"Eps;\nEps;")