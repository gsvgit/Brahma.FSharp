
# 2 "Calc.yrd.fs"
module Calc.Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "Calc.yrd"

open Calc.AST

# 13 "Calc.yrd.fs"
type Token =
    | DIV of (string)
    | EQ of (string)
    | LBRACE of (string)
    | MINUS of (string)
    | MULT of (string)
    | NUMBER of (string)
    | PLUS of (string)
    | POW of (string)
    | RBRACE of (string)
    | RNGLR_EOF of (string)
    | SEMI of (string)
    | VAR of (string)

let genLiteral (str : string) (data : string) =
    match str.ToLower() with
    | x -> None
let tokenData = function
    | DIV x -> box x
    | EQ x -> box x
    | LBRACE x -> box x
    | MINUS x -> box x
    | MULT x -> box x
    | NUMBER x -> box x
    | PLUS x -> box x
    | POW x -> box x
    | RBRACE x -> box x
    | RNGLR_EOF x -> box x
    | SEMI x -> box x
    | VAR x -> box x

let numToString = function
    | 0 -> "error"
    | 1 -> "expr"
    | 2 -> "factor"
    | 3 -> "factorOp"
    | 4 -> "powExpr"
    | 5 -> "powOp"
    | 6 -> "program"
    | 7 -> "stmt"
    | 8 -> "term"
    | 9 -> "termOp"
    | 10 -> "yard_rule_binExpr_1"
    | 11 -> "yard_rule_binExpr_3"
    | 12 -> "yard_rule_binExpr_5"
    | 13 -> "yard_rule_yard_many_1_2"
    | 14 -> "yard_rule_yard_many_1_4"
    | 15 -> "yard_rule_yard_many_1_6"
    | 16 -> "yard_some_1"
    | 17 -> "yard_start_rule"
    | 18 -> "DIV"
    | 19 -> "EQ"
    | 20 -> "LBRACE"
    | 21 -> "MINUS"
    | 22 -> "MULT"
    | 23 -> "NUMBER"
    | 24 -> "PLUS"
    | 25 -> "POW"
    | 26 -> "RBRACE"
    | 27 -> "RNGLR_EOF"
    | 28 -> "SEMI"
    | 29 -> "VAR"
    | _ -> ""

let tokenToNumber = function
    | DIV _ -> 18
    | EQ _ -> 19
    | LBRACE _ -> 20
    | MINUS _ -> 21
    | MULT _ -> 22
    | NUMBER _ -> 23
    | PLUS _ -> 24
    | POW _ -> 25
    | RBRACE _ -> 26
    | RNGLR_EOF _ -> 27
    | SEMI _ -> 28
    | VAR _ -> 29

let isLiteral = function
    | DIV _ -> false
    | EQ _ -> false
    | LBRACE _ -> false
    | MINUS _ -> false
    | MULT _ -> false
    | NUMBER _ -> false
    | PLUS _ -> false
    | POW _ -> false
    | RBRACE _ -> false
    | RNGLR_EOF _ -> false
    | SEMI _ -> false
    | VAR _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|6; 17; 16; 16; 7; 1; 10; 13; 13; 9; 9; 8; 11; 14; 14; 3; 3; 2; 12; 15; 15; 5; 4; 4; 4|]
let private rules = [|16; 6; 7; 28; 7; 28; 16; 29; 19; 1; 10; 8; 13; 9; 8; 13; 24; 21; 11; 2; 14; 3; 2; 14; 22; 18; 12; 4; 15; 5; 4; 15; 25; 23; 29; 20; 1; 26|]
let private rulesStart = [|0; 1; 2; 4; 7; 10; 11; 13; 13; 16; 17; 18; 19; 21; 21; 24; 25; 26; 27; 29; 29; 32; 33; 34; 35; 38|]
let startRule = 1

let acceptEmptyInput = false

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 2; 36; 5; 3; 4; 6; 7; 8; 14; 27; 35; 33; 25; 19; 22; 23; 9; 26; 12; 13; 10; 11; 15; 24; 18; 16; 17; 20; 21; 28; 34; 31; 32; 29; 30|]
let private small_gotos =
        [|4; 393216; 458753; 1048578; 1900547; 131073; 1835012; 196611; 458753; 1048581; 1900547; 327681; 1245190; 393226; 65543; 131080; 262153; 524298; 655371; 720908; 786445; 1310734; 1507343; 1900560; 524292; 196625; 917522; 1179667; 1441812; 589830; 131093; 262153; 786445; 1310734; 1507343; 1900560; 655364; 196625; 917526; 1179667; 1441812; 917507; 327703; 983064; 1638425; 983044; 262170; 1310734; 1507343; 1900560; 1048579; 327703; 983067; 1638425; 1245194; 65564; 131080; 262153; 524298; 655371; 720908; 786445; 1310734; 1507343; 1900560; 1310721; 1703965; 1769476; 589854; 851999; 1376288; 1572897; 1835016; 131080; 262153; 524322; 720908; 786445; 1310734; 1507343; 1900560; 1900548; 589854; 852003; 1376288; 1572897|]
let gotos = Array.zeroCreate 37
for i = 0 to 36 do
        gotos.[i] <- Array.zeroCreate 30
cur <- 0
while cur < small_gotos.Length do
    let i = small_gotos.[cur] >>> 16
    let length = small_gotos.[cur] &&& 65535
    cur <- cur + 1
    for k = 0 to length-1 do
        let j = small_gotos.[cur + k] >>> 16
        let x = small_gotos.[cur + k] &&& 65535
        gotos.[i].[j] <- lists_gotos.[x]
    cur <- cur + length
let private lists_reduces = [|[|2,2|]; [|3,3|]; [|4,3|]; [|12,1|]; [|14,2|]; [|14,3|]; [|16,1|]; [|15,1|]; [|18,1|]; [|20,2|]; [|20,3|]; [|21,1|]; [|24,3|]; [|22,1|]; [|23,1|]; [|18,2|]; [|17,1|]; [|12,2|]; [|6,1|]; [|8,2|]; [|8,3|]; [|10,1|]; [|9,1|]; [|11,1|]; [|6,2|]; [|5,1|]; [|0,1|]|]
let private small_reduces =
        [|196609; 1769472; 262145; 1769473; 458753; 1835010; 524292; 1376259; 1572867; 1703939; 1835011; 655364; 1376260; 1572868; 1703940; 1835012; 720900; 1376261; 1572869; 1703941; 1835013; 786435; 1310726; 1507334; 1900550; 851971; 1310727; 1507335; 1900551; 917510; 1179656; 1376264; 1441800; 1572872; 1703944; 1835016; 1048582; 1179657; 1376265; 1441801; 1572873; 1703945; 1835017; 1114118; 1179658; 1376266; 1441802; 1572874; 1703946; 1835018; 1179651; 1310731; 1507339; 1900555; 1376263; 1179660; 1376268; 1441804; 1572876; 1638412; 1703948; 1835020; 1441799; 1179661; 1376269; 1441805; 1572877; 1638413; 1703949; 1835021; 1507335; 1179662; 1376270; 1441806; 1572878; 1638414; 1703950; 1835022; 1572870; 1179663; 1376271; 1441807; 1572879; 1703951; 1835023; 1638406; 1179664; 1376272; 1441808; 1572880; 1703952; 1835024; 1703940; 1376273; 1572881; 1703953; 1835025; 1769474; 1703954; 1835026; 1900546; 1703955; 1835027; 1966082; 1703956; 1835028; 2031619; 1310741; 1507349; 1900565; 2097155; 1310742; 1507350; 1900566; 2162692; 1376279; 1572887; 1703959; 1835031; 2228226; 1703960; 1835032; 2293762; 1703961; 1835033; 2359297; 1769498|]
let reduces = Array.zeroCreate 37
for i = 0 to 36 do
        reduces.[i] <- Array.zeroCreate 30
cur <- 0
while cur < small_reduces.Length do
    let i = small_reduces.[cur] >>> 16
    let length = small_reduces.[cur] &&& 65535
    cur <- cur + 1
    for k = 0 to length-1 do
        let j = small_reduces.[cur + k] >>> 16
        let x = small_reduces.[cur + k] &&& 65535
        reduces.[i].[j] <- lists_reduces.[x]
    cur <- cur + length
let private lists_zeroReduces = [|[|13|]; [|19|]; [|7|]|]
let private small_zeroReduces =
        [|524292; 1376256; 1572864; 1703936; 1835008; 655364; 1376256; 1572864; 1703936; 1835008; 917510; 1179649; 1376257; 1441793; 1572865; 1703937; 1835009; 1048582; 1179649; 1376257; 1441793; 1572865; 1703937; 1835009; 1769474; 1703938; 1835010; 1900546; 1703938; 1835010|]
let zeroReduces = Array.zeroCreate 37
for i = 0 to 36 do
        zeroReduces.[i] <- Array.zeroCreate 30
cur <- 0
while cur < small_zeroReduces.Length do
    let i = small_zeroReduces.[cur] >>> 16
    let length = small_zeroReduces.[cur] &&& 65535
    cur <- cur + 1
    for k = 0 to length-1 do
        let j = small_zeroReduces.[cur + k] >>> 16
        let x = small_zeroReduces.[cur + k] &&& 65535
        zeroReduces.[i].[j] <- lists_zeroReduces.[x]
    cur <- cur + length
let private small_acc = [1]
let private accStates = Array.zeroCreate 37
for i = 0 to 36 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 27
let errorIndex = 0
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(25, new Nodes([||])), null)), null); null; null; null; null; null; null; null; null; null; null; null; null; new Tree<_>(null,box (new AST(new Family(7, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(13, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(19, new Nodes([||])), null)), null); null; null|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(25, new Nodes([||])), null)), null); null; null; null; null; null; null; null; null; null; null; null; null; new Tree<_>(null,box (new AST(new Family(7, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(13, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(19, new Nodes([||])), null)), null); null; null|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_error * '_rnglr_type_expr * '_rnglr_type_factor * '_rnglr_type_factorOp * '_rnglr_type_powExpr * '_rnglr_type_powOp * '_rnglr_type_program * '_rnglr_type_stmt * '_rnglr_type_term * '_rnglr_type_termOp * '_rnglr_type_yard_rule_binExpr_1 * '_rnglr_type_yard_rule_binExpr_3 * '_rnglr_type_yard_rule_binExpr_5 * '_rnglr_type_yard_rule_yard_many_1_2 * '_rnglr_type_yard_rule_yard_many_1_4 * '_rnglr_type_yard_rule_yard_many_1_6 * '_rnglr_type_yard_some_1 * '_rnglr_type_yard_start_rule>), 
  [|
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_some_1) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 19 "Calc.yrd"
                          S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Calc.yrd"
               : '_rnglr_type_program) 
# 208 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Calc.yrd"
               : '_rnglr_type_yard_start_rule) 
# 218 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_stmt) 
               |> List.iter (fun (S1) -> 
                (match ((unbox _rnglr_children.[1]) : Token) with SEMI _rnglr_val -> [_rnglr_val] | a -> failwith "SEMI expected, but %A found" a )
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 19 "Calc.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_elem) -> 
              _rnglr_cycle_res := (
                
# 19 "Calc.yrd"
                          [yard_elem]
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Calc.yrd"
               : '_rnglr_type_yard_some_1) 
# 249 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_stmt) 
               |> List.iter (fun (S1) -> 
                (match ((unbox _rnglr_children.[1]) : Token) with SEMI _rnglr_val -> [_rnglr_val] | a -> failwith "SEMI expected, but %A found" a )
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 19 "Calc.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_some_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 19 "Calc.yrd"
                            yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 19 "Calc.yrd"
               : '_rnglr_type_yard_some_1) 
# 282 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with VAR _rnglr_val -> [_rnglr_val] | a -> failwith "VAR expected, but %A found" a )
             |> List.iter (fun (v) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with EQ _rnglr_val -> [_rnglr_val] | a -> failwith "EQ expected, but %A found" a )
               |> List.iter (fun (_rnglr_var_0) -> 
                ((unbox _rnglr_children.[2]) : '_rnglr_type_expr) 
                 |> List.iter (fun (e) -> 
                  _rnglr_cycle_res := (
                    
# 21 "Calc.yrd"
                                           v, e
                      )::!_rnglr_cycle_res ) ) )
            !_rnglr_cycle_res
          )
            )
# 21 "Calc.yrd"
               : '_rnglr_type_stmt) 
# 306 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_rule_binExpr_1) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 25 "Calc.yrd"
                      S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 25 "Calc.yrd"
               : '_rnglr_type_expr) 
# 326 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_term) 
             |> List.iter (fun (h) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_rule_yard_many_1_2) h
               |> List.iter (fun (tl) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                                         List.fold (fun l (op,r) -> BinOp(op,l,r)) h tl
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_binExpr_1) 
# 348 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 23 "Calc.yrd"
                                                    []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_2) 
# 366 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_termOp) 
               |> List.iter (fun (S1) -> 
                ((unbox _rnglr_children.[1]) : '_rnglr_type_term) 
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 25 "Calc.yrd"
                                       S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_rule_yard_many_1_2) h
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                        yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_2) 
# 399 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with PLUS _rnglr_val -> [_rnglr_val] | a -> failwith "PLUS expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 27 "Calc.yrd"
                              Plus
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 27 "Calc.yrd"
               : '_rnglr_type_termOp) 
# 419 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MINUS _rnglr_val -> [_rnglr_val] | a -> failwith "MINUS expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 27 "Calc.yrd"
                                             Minus
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 27 "Calc.yrd"
               : '_rnglr_type_termOp) 
# 439 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_rule_binExpr_3) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 29 "Calc.yrd"
                      S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 29 "Calc.yrd"
               : '_rnglr_type_term) 
# 459 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_factor) 
             |> List.iter (fun (h) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_rule_yard_many_1_4) h
               |> List.iter (fun (tl) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                                         List.fold (fun l (op,r) -> BinOp(op,l,r)) h tl
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_binExpr_3) 
# 481 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 23 "Calc.yrd"
                                                    []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_4) 
# 499 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_factorOp) 
               |> List.iter (fun (S1) -> 
                ((unbox _rnglr_children.[1]) : '_rnglr_type_factor) 
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 29 "Calc.yrd"
                                         S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_rule_yard_many_1_4) h
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                        yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_4) 
# 532 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MULT _rnglr_val -> [_rnglr_val] | a -> failwith "MULT expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 31 "Calc.yrd"
                                Mult
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 31 "Calc.yrd"
               : '_rnglr_type_factorOp) 
# 552 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with DIV _rnglr_val -> [_rnglr_val] | a -> failwith "DIV expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 31 "Calc.yrd"
                                             Div
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 31 "Calc.yrd"
               : '_rnglr_type_factorOp) 
# 572 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_rule_binExpr_5) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 33 "Calc.yrd"
                        S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 33 "Calc.yrd"
               : '_rnglr_type_factor) 
# 592 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_powExpr) 
             |> List.iter (fun (h) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_rule_yard_many_1_6) h
               |> List.iter (fun (tl) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                                         List.fold (fun l (op,r) -> BinOp(op,l,r)) h tl
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_binExpr_5) 
# 614 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 23 "Calc.yrd"
                                                    []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_6) 
# 632 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( fun h ->
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_powOp) 
               |> List.iter (fun (S1) -> 
                ((unbox _rnglr_children.[1]) : '_rnglr_type_powExpr) 
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 33 "Calc.yrd"
                                            S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_rule_yard_many_1_6) h
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 23 "Calc.yrd"
                                                        yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_yard_rule_yard_many_1_6) 
# 665 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with POW _rnglr_val -> [_rnglr_val] | a -> failwith "POW expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 35 "Calc.yrd"
                            Pow
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 35 "Calc.yrd"
               : '_rnglr_type_powOp) 
# 685 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
             |> List.iter (fun (n) -> 
              _rnglr_cycle_res := (
                
# 38 "Calc.yrd"
                             Num (float n)
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 37 "Calc.yrd"
               : '_rnglr_type_powExpr) 
# 705 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with VAR _rnglr_val -> [_rnglr_val] | a -> failwith "VAR expected, but %A found" a )
             |> List.iter (fun (v) -> 
              _rnglr_cycle_res := (
                
# 39 "Calc.yrd"
                            EVar(v)
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 37 "Calc.yrd"
               : '_rnglr_type_powExpr) 
# 725 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with LBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "LBRACE expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_expr) 
               |> List.iter (fun (e) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with RBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "RBRACE expected, but %A found" a )
                 |> List.iter (fun (_rnglr_var_1) -> 
                  _rnglr_cycle_res := (
                    
# 40 "Calc.yrd"
                                               e
                      )::!_rnglr_cycle_res ) ) )
            !_rnglr_cycle_res
          )
            )
# 37 "Calc.yrd"
               : '_rnglr_type_powExpr) 
# 749 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              

              parserRange
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )

               : '_rnglr_type_error) 
# 767 "Calc.yrd.fs"
      );
  |] , [|
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_error)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_expr)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_factor)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_factorOp)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_powExpr)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_powOp)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_program)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_stmt)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_term)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_termOp)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_binExpr_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_binExpr_3)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_binExpr_5)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( fun h ->
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_yard_many_1_2)  h ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( fun h ->
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_yard_many_1_4)  h ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( fun h ->
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_rule_yard_many_1_6)  h ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_some_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_start_rule)   ) |> List.concat));
  |] 
let translate (args : TranslateArguments<_,_>) (tree : Tree<_>) (dict : _ ) : '_rnglr_type_yard_start_rule = 
  unbox (tree.Translate _rnglr_rule_  leftSide _rnglr_concats (if args.filterEpsilons then _rnglr_filtered_epsilons else _rnglr_epsilons) args.tokenToRange args.zeroPosition args.clearAST dict) : '_rnglr_type_yard_start_rule
