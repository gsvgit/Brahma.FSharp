
# 2 "Calc.yrd.fs"
module Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "Calc.yrd"

open TTA.ASM

# 13 "Calc.yrd.fs"
type Token =
    | EPS of (string)
    | MOV of (string)
    | MVC of (string)
    | NUMBER of (string)
    | RNGLR_EOF of (string)
    | SET of (string)

let genLiteral (str : string) (data : string) =
    match str.ToLower() with
    | x -> None
let tokenData = function
    | EPS x -> box x
    | MOV x -> box x
    | MVC x -> box x
    | NUMBER x -> box x
    | RNGLR_EOF x -> box x
    | SET x -> box x

let numToString = function
    | 0 -> "eps"
    | 1 -> "error"
    | 2 -> "mov"
    | 3 -> "mvc"
    | 4 -> "program"
    | 5 -> "set"
    | 6 -> "stmt"
    | 7 -> "yard_start_rule"
    | 8 -> "EPS"
    | 9 -> "MOV"
    | 10 -> "MVC"
    | 11 -> "NUMBER"
    | 12 -> "RNGLR_EOF"
    | 13 -> "SET"
    | _ -> ""

let tokenToNumber = function
    | EPS _ -> 8
    | MOV _ -> 9
    | MVC _ -> 10
    | NUMBER _ -> 11
    | RNGLR_EOF _ -> 12
    | SET _ -> 13

let isLiteral = function
    | EPS _ -> false
    | MOV _ -> false
    | MVC _ -> false
    | NUMBER _ -> false
    | RNGLR_EOF _ -> false
    | SET _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|4; 7; 6; 6; 6; 6; 0; 5; 3; 2|]
let private rules = [|6; 4; 0; 5; 3; 2; 8; 13; 11; 11; 11; 10; 11; 11; 11; 9; 11; 11; 11; 11|]
let private rulesStart = [|0; 1; 2; 3; 4; 5; 6; 7; 11; 15; 20|]
let startRule = 1

let acceptEmptyInput = false

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 2; 3; 4; 5; 6; 7; 8; 13; 17; 9; 10; 11; 12; 14; 15; 16; 18; 19; 20|]
let private small_gotos =
        [|10; 0; 131073; 196610; 262147; 327684; 393221; 524294; 589831; 655368; 851977; 524289; 720906; 589825; 720907; 655361; 720908; 720897; 720909; 851969; 720910; 917505; 720911; 983041; 720912; 1114113; 720913; 1179649; 720914; 1245185; 720915|]
let gotos = Array.zeroCreate 21
for i = 0 to 20 do
        gotos.[i] <- Array.zeroCreate 14
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
let private lists_reduces = [|[|2,1|]; [|5,1|]; [|4,1|]; [|3,1|]; [|0,1|]; [|6,1|]; [|9,5|]; [|8,4|]; [|7,4|]|]
let private small_reduces =
        [|65537; 786432; 131073; 786433; 196609; 786434; 327681; 786435; 393217; 786436; 458753; 786437; 786433; 786438; 1048577; 786439; 1310721; 786440|]
let reduces = Array.zeroCreate 21
for i = 0 to 20 do
        reduces.[i] <- Array.zeroCreate 14
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
let private lists_zeroReduces = [||]
let private small_zeroReduces =
        [||]
let zeroReduces = Array.zeroCreate 21
for i = 0 to 20 do
        zeroReduces.[i] <- Array.zeroCreate 14
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
let private small_acc = [4]
let private accStates = Array.zeroCreate 21
for i = 0 to 20 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 12
let errorIndex = 1
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(10, new Nodes([||])), null)), null); null; null; null; null; null; null|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(10, new Nodes([||])), null)), null); null; null; null; null; null; null|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_eps * '_rnglr_type_error * '_rnglr_type_mov * '_rnglr_type_mvc * '_rnglr_type_program * '_rnglr_type_set * '_rnglr_type_stmt * '_rnglr_type_yard_start_rule>), 
  [|
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_stmt) 
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
# 168 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Calc.yrd"
               : '_rnglr_type_yard_start_rule) 
# 178 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_eps) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Calc.yrd"
                      S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Calc.yrd"
               : '_rnglr_type_stmt) 
# 198 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_set) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Calc.yrd"
                            S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Calc.yrd"
               : '_rnglr_type_stmt) 
# 218 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_mvc) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Calc.yrd"
                                  S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Calc.yrd"
               : '_rnglr_type_stmt) 
# 238 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_mov) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Calc.yrd"
                                        S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Calc.yrd"
               : '_rnglr_type_stmt) 
# 258 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with EPS _rnglr_val -> [_rnglr_val] | a -> failwith "EPS expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              _rnglr_cycle_res := (
                
# 23 "Calc.yrd"
                           Eps 
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 23 "Calc.yrd"
               : '_rnglr_type_eps) 
# 278 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with SET _rnglr_val -> [_rnglr_val] | a -> failwith "SET expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
               |> List.iter (fun (i) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (j) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (v) -> 
                    _rnglr_cycle_res := (
                      
# 25 "Calc.yrd"
                                                            Set (((int i)*1<ln>, (int j)*1<col>), int v) 
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 25 "Calc.yrd"
               : '_rnglr_type_set) 
# 304 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MVC _rnglr_val -> [_rnglr_val] | a -> failwith "MVC expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
               |> List.iter (fun (i) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (j) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (v) -> 
                    _rnglr_cycle_res := (
                      
# 27 "Calc.yrd"
                                                            Mvc (((int i)*1<ln>, (int j)*1<col>), int v) 
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 27 "Calc.yrd"
               : '_rnglr_type_mvc) 
# 330 "Calc.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MOV _rnglr_val -> [_rnglr_val] | a -> failwith "MOV expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
               |> List.iter (fun (i) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (j) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (x) -> 
                    (match ((unbox _rnglr_children.[4]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                     |> List.iter (fun (y) -> 
                      _rnglr_cycle_res := (
                        
# 29 "Calc.yrd"
                                                                       Mov (((int i)*1<ln>, (int j)*1<col>), ((int x)*1<ln>, (int y)*1<col>)) 
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 29 "Calc.yrd"
               : '_rnglr_type_mov) 
# 358 "Calc.yrd.fs"
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
# 376 "Calc.yrd.fs"
      );
  |] , [|
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_eps)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_error)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_mov)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_mvc)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_program)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_set)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_stmt)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_start_rule)   ) |> List.concat));
  |] 
let translate (args : TranslateArguments<_,_>) (tree : Tree<_>) (dict : _ ) : '_rnglr_type_yard_start_rule = 
  unbox (tree.Translate _rnglr_rule_  leftSide _rnglr_concats (if args.filterEpsilons then _rnglr_filtered_epsilons else _rnglr_epsilons) args.tokenToRange args.zeroPosition args.clearAST dict) : '_rnglr_type_yard_start_rule
