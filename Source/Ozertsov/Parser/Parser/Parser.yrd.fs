
# 2 "Parser.yrd.fs"
module Parser.Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "Parser.yrd"

open TTA.ASM

# 13 "Parser.yrd.fs"
type Token =
    | EPS of (string)
    | MOV of (string)
    | MVC of (string)
    | NL of (string)
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
    | NL x -> box x
    | NUMBER x -> box x
    | RNGLR_EOF x -> box x
    | SET x -> box x

let numToString = function
    | 0 -> "error"
    | 1 -> "operation"
    | 2 -> "program"
    | 3 -> "stmt"
    | 4 -> "yard_some_1"
    | 5 -> "yard_some_2"
    | 6 -> "yard_start_rule"
    | 7 -> "EPS"
    | 8 -> "MOV"
    | 9 -> "MVC"
    | 10 -> "NL"
    | 11 -> "NUMBER"
    | 12 -> "RNGLR_EOF"
    | 13 -> "SET"
    | _ -> ""

let tokenToNumber = function
    | EPS _ -> 7
    | MOV _ -> 8
    | MVC _ -> 9
    | NL _ -> 10
    | NUMBER _ -> 11
    | RNGLR_EOF _ -> 12
    | SET _ -> 13

let isLiteral = function
    | EPS _ -> false
    | MOV _ -> false
    | MVC _ -> false
    | NL _ -> false
    | NUMBER _ -> false
    | RNGLR_EOF _ -> false
    | SET _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|2; 6; 4; 4; 3; 5; 5; 1; 1; 1; 1|]
let private rules = [|4; 2; 3; 10; 3; 10; 4; 5; 1; 1; 5; 7; 13; 11; 11; 11; 9; 11; 11; 11; 8; 11; 11; 11; 11|]
let private rulesStart = [|0; 1; 2; 4; 7; 8; 9; 11; 12; 16; 20; 25|]
let startRule = 1

let acceptEmptyInput = false

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 17; 18; 22; 21; 3; 4; 9; 13; 2; 5; 6; 7; 8; 10; 11; 12; 14; 15; 16; 19; 20|]
let private small_gotos =
        [|9; 65536; 131073; 196610; 262147; 327684; 458757; 524294; 589831; 851976; 65542; 65536; 327689; 458757; 524294; 589831; 851976; 262145; 720906; 327681; 720907; 393217; 720908; 458753; 720909; 589825; 720910; 655361; 720911; 720897; 720912; 851969; 720913; 917505; 720914; 983041; 720915; 1179649; 655380; 1245192; 65536; 196610; 262165; 327684; 458757; 524294; 589831; 851976|]
let gotos = Array.zeroCreate 23
for i = 0 to 22 do
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
let private lists_reduces = [|[|5,1|]; [|6,2|]; [|7,1|]; [|10,5|]; [|9,4|]; [|8,4|]; [|2,2|]; [|3,3|]; [|4,1|]; [|0,1|]|]
let private small_reduces =
        [|65537; 655360; 131073; 655361; 196613; 458754; 524290; 589826; 655362; 851970; 524293; 458755; 524291; 589827; 655363; 851971; 786437; 458756; 524292; 589828; 655364; 851972; 1048581; 458757; 524293; 589829; 655365; 851973; 1245185; 786438; 1310721; 786439; 1376257; 655368; 1441793; 786441|]
let reduces = Array.zeroCreate 23
for i = 0 to 22 do
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
let zeroReduces = Array.zeroCreate 23
for i = 0 to 22 do
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
let private small_acc = [17]
let private accStates = Array.zeroCreate 23
for i = 0 to 22 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 12
let errorIndex = 0
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(11, new Nodes([||])), null)), null); null; null; null; null; null; null|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(11, new Nodes([||])), null)), null); null; null; null; null; null; null|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_error * '_rnglr_type_operation * '_rnglr_type_program * '_rnglr_type_stmt * '_rnglr_type_yard_some_1 * '_rnglr_type_yard_some_2 * '_rnglr_type_yard_start_rule>), 
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
                
# 19 "Parser.yrd"
                          S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Parser.yrd"
               : '_rnglr_type_program) 
# 172 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Parser.yrd"
               : '_rnglr_type_yard_start_rule) 
# 182 "Parser.yrd.fs"
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
                (match ((unbox _rnglr_children.[1]) : Token) with NL _rnglr_val -> [_rnglr_val] | a -> failwith "NL expected, but %A found" a )
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 19 "Parser.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_elem) -> 
              _rnglr_cycle_res := (
                
# 19 "Parser.yrd"
                          [yard_elem]
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Parser.yrd"
               : '_rnglr_type_yard_some_1) 
# 213 "Parser.yrd.fs"
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
                (match ((unbox _rnglr_children.[1]) : Token) with NL _rnglr_val -> [_rnglr_val] | a -> failwith "NL expected, but %A found" a )
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 19 "Parser.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_some_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 19 "Parser.yrd"
                            yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 19 "Parser.yrd"
               : '_rnglr_type_yard_some_1) 
# 246 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_some_2) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Parser.yrd"
                         S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_stmt) 
# 266 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_operation) 
               |> List.iter (fun (S1) -> 
                _rnglr_cycle_res := (
                  
# 21 "Parser.yrd"
                           S1
                    )::!_rnglr_cycle_res )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_elem) -> 
              _rnglr_cycle_res := (
                
# 21 "Parser.yrd"
                         [yard_elem]
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_yard_some_2) 
# 295 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_operation) 
               |> List.iter (fun (S1) -> 
                _rnglr_cycle_res := (
                  
# 21 "Parser.yrd"
                           S1
                    )::!_rnglr_cycle_res )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_some_2) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 21 "Parser.yrd"
                           yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_yard_some_2) 
# 326 "Parser.yrd.fs"
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
                
# 23 "Parser.yrd"
                                Eps
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_operation) 
# 346 "Parser.yrd.fs"
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
               |> List.iter (fun (ln) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (col) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (arg) -> 
                    _rnglr_cycle_res := (
                      
# 23 "Parser.yrd"
                                                                                  Set((int ln, int col), int arg)
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_operation) 
# 372 "Parser.yrd.fs"
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
               |> List.iter (fun (ln) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (col) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (arg) -> 
                    _rnglr_cycle_res := (
                      
# 23 "Parser.yrd"
                                                                                                                                                          Mvc((int ln, int col), int arg)
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_operation) 
# 398 "Parser.yrd.fs"
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
               |> List.iter (fun (ln1) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (col1) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (ln2) -> 
                    (match ((unbox _rnglr_children.[4]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                     |> List.iter (fun (col2) -> 
                      _rnglr_cycle_res := (
                        
# 23 "Parser.yrd"
                                                                                                                                                                                                                                                  Mov((int ln1, int col1), (int ln2, int col2))
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_operation) 
# 426 "Parser.yrd.fs"
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
# 444 "Parser.yrd.fs"
      );
  |] , [|
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_error)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_operation)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_program)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_stmt)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_some_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_some_2)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_start_rule)   ) |> List.concat));
  |] 
let translate (args : TranslateArguments<_,_>) (tree : Tree<_>) (dict : _ ) : '_rnglr_type_yard_start_rule = 
  unbox (tree.Translate _rnglr_rule_  leftSide _rnglr_concats (if args.filterEpsilons then _rnglr_filtered_epsilons else _rnglr_epsilons) args.tokenToRange args.zeroPosition args.clearAST dict) : '_rnglr_type_yard_start_rule
