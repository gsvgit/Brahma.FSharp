
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
    | LBRACE of (string)
    | MOV of (string)
    | MVC of (string)
    | NUMBER of (string)
    | RBRACE of (string)
    | RNGLR_EOF of (string)
    | SEMI of (string)
    | SET of (string)

let genLiteral (str : string) (data : string) =
    match str.ToLower() with
    | x -> None
let tokenData = function
    | EPS x -> box x
    | LBRACE x -> box x
    | MOV x -> box x
    | MVC x -> box x
    | NUMBER x -> box x
    | RBRACE x -> box x
    | RNGLR_EOF x -> box x
    | SEMI x -> box x
    | SET x -> box x

let numToString = function
    | 0 -> "coord"
    | 1 -> "error"
    | 2 -> "expr"
    | 3 -> "line"
    | 4 -> "program"
    | 5 -> "yard_many_1"
    | 6 -> "yard_some_1"
    | 7 -> "yard_start_rule"
    | 8 -> "EPS"
    | 9 -> "LBRACE"
    | 10 -> "MOV"
    | 11 -> "MVC"
    | 12 -> "NUMBER"
    | 13 -> "RBRACE"
    | 14 -> "RNGLR_EOF"
    | 15 -> "SEMI"
    | 16 -> "SET"
    | _ -> ""

let tokenToNumber = function
    | EPS _ -> 8
    | LBRACE _ -> 9
    | MOV _ -> 10
    | MVC _ -> 11
    | NUMBER _ -> 12
    | RBRACE _ -> 13
    | RNGLR_EOF _ -> 14
    | SEMI _ -> 15
    | SET _ -> 16

let isLiteral = function
    | EPS _ -> false
    | LBRACE _ -> false
    | MOV _ -> false
    | MVC _ -> false
    | NUMBER _ -> false
    | RBRACE _ -> false
    | RNGLR_EOF _ -> false
    | SEMI _ -> false
    | SET _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|4; 7; 6; 6; 3; 5; 5; 2; 2; 2; 2; 0|]
let private rules = [|6; 4; 3; 15; 3; 15; 6; 5; 2; 5; 16; 9; 0; 12; 13; 11; 9; 0; 12; 13; 10; 9; 0; 0; 13; 8; 12; 12|]
let private rulesStart = [|0; 1; 2; 4; 7; 8; 8; 10; 15; 20; 25; 26; 28|]
let startRule = 1

let acceptEmptyInput = false

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 21; 25; 23; 26; 3; 4; 11; 16; 2; 5; 6; 9; 7; 8; 10; 12; 13; 14; 15; 17; 18; 19; 20; 22; 24|]
let private small_gotos =
        [|9; 131072; 196609; 262146; 327683; 393220; 524293; 655366; 720903; 1048584; 65542; 131072; 327689; 524293; 655366; 720903; 1048584; 262145; 589834; 327682; 11; 786444; 393218; 13; 786444; 458753; 851982; 589825; 786447; 720897; 589840; 786434; 17; 786444; 851969; 786450; 917505; 851987; 1048577; 589844; 1114114; 21; 786444; 1179649; 786454; 1245185; 851991; 1376257; 983064; 1441800; 131072; 196609; 327683; 393241; 524293; 655366; 720903; 1048584|]
let gotos = Array.zeroCreate 27
for i = 0 to 26 do
        gotos.[i] <- Array.zeroCreate 17
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
let private lists_reduces = [|[|6,1|]; [|6,2|]; [|10,1|]; [|9,5|]; [|11,2|]; [|8,5|]; [|7,5|]; [|2,2|]; [|4,1|]; [|3,3|]; [|0,1|]|]
let private small_reduces =
        [|65537; 983040; 131073; 983041; 196613; 524290; 655362; 720898; 983042; 1048578; 524293; 524291; 655363; 720899; 983043; 1048579; 655362; 786436; 851972; 983045; 524293; 655365; 720901; 983045; 1048581; 1310725; 524294; 655366; 720902; 983046; 1048582; 1441793; 917511; 1507329; 983048; 1572865; 917513; 1703937; 917514|]
let reduces = Array.zeroCreate 27
for i = 0 to 26 do
        reduces.[i] <- Array.zeroCreate 17
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
let private lists_zeroReduces = [|[|5; 4|]; [|5|]|]
let private small_zeroReduces =
        [|1; 983040; 65537; 983041; 1441793; 983040|]
let zeroReduces = Array.zeroCreate 27
for i = 0 to 26 do
        zeroReduces.[i] <- Array.zeroCreate 17
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
let private small_acc = [25]
let private accStates = Array.zeroCreate 27
for i = 0 to 26 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 14
let errorIndex = 1
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(12, new Nodes([||])), null)), null); null; new Tree<_>(null,box (new AST(new Family(4, new Nodes([|box (new AST(new Family(5, new Nodes([||])), null))|])), null)), null); null; new Tree<_>(null,box (new AST(new Family(5, new Nodes([||])), null)), null); null; null|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(12, new Nodes([||])), null)), null); null; new Tree<_>(null,box (new AST(new Family(4, new Nodes([|box (new AST(new Family(5, new Nodes([||])), null))|])), null)), null); null; new Tree<_>(null,box (new AST(new Family(5, new Nodes([||])), null)), null); null; null|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_coord * '_rnglr_type_error * '_rnglr_type_expr * '_rnglr_type_line * '_rnglr_type_program * '_rnglr_type_yard_many_1 * '_rnglr_type_yard_some_1 * '_rnglr_type_yard_start_rule>), 
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
# 183 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Parser.yrd"
               : '_rnglr_type_yard_start_rule) 
# 193 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_line) 
               |> List.iter (fun (S1) -> 
                (match ((unbox _rnglr_children.[1]) : Token) with SEMI _rnglr_val -> [_rnglr_val] | a -> failwith "SEMI expected, but %A found" a )
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
# 224 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (
              let _rnglr_cycle_res = ref []
              ((unbox _rnglr_children.[0]) : '_rnglr_type_line) 
               |> List.iter (fun (S1) -> 
                (match ((unbox _rnglr_children.[1]) : Token) with SEMI _rnglr_val -> [_rnglr_val] | a -> failwith "SEMI expected, but %A found" a )
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
# 257 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_many_1) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 21 "Parser.yrd"
                       S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_line) 
# 277 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 21 "Parser.yrd"
                     []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_yard_many_1) 
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
              ((unbox _rnglr_children.[0]) : '_rnglr_type_expr) 
               |> List.iter (fun (S1) -> 
                _rnglr_cycle_res := (
                  
# 21 "Parser.yrd"
                         S1
                    )::!_rnglr_cycle_res )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_many_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 21 "Parser.yrd"
                         yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 21 "Parser.yrd"
               : '_rnglr_type_yard_many_1) 
# 326 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with SET _rnglr_val -> [_rnglr_val] | a -> failwith "SET expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with LBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "LBRACE expected, but %A found" a )
               |> List.iter (fun (_rnglr_var_1) -> 
                ((unbox _rnglr_children.[2]) : '_rnglr_type_coord) 
                 |> List.iter (fun (c) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (n) -> 
                    (match ((unbox _rnglr_children.[4]) : Token) with RBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "RBRACE expected, but %A found" a )
                     |> List.iter (fun (_rnglr_var_2) -> 
                      _rnglr_cycle_res := (
                        
# 24 "Parser.yrd"
                                                                Set (c, int n)
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_expr) 
# 354 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MVC _rnglr_val -> [_rnglr_val] | a -> failwith "MVC expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with LBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "LBRACE expected, but %A found" a )
               |> List.iter (fun (_rnglr_var_1) -> 
                ((unbox _rnglr_children.[2]) : '_rnglr_type_coord) 
                 |> List.iter (fun (c) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                   |> List.iter (fun (n) -> 
                    (match ((unbox _rnglr_children.[4]) : Token) with RBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "RBRACE expected, but %A found" a )
                     |> List.iter (fun (_rnglr_var_2) -> 
                      _rnglr_cycle_res := (
                        
# 25 "Parser.yrd"
                                                                  Mvc (c, int n)
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_expr) 
# 382 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MOV _rnglr_val -> [_rnglr_val] | a -> failwith "MOV expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with LBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "LBRACE expected, but %A found" a )
               |> List.iter (fun (_rnglr_var_1) -> 
                ((unbox _rnglr_children.[2]) : '_rnglr_type_coord) 
                 |> List.iter (fun (fc) -> 
                  ((unbox _rnglr_children.[3]) : '_rnglr_type_coord) 
                   |> List.iter (fun (sc) -> 
                    (match ((unbox _rnglr_children.[4]) : Token) with RBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "RBRACE expected, but %A found" a )
                     |> List.iter (fun (_rnglr_var_2) -> 
                      _rnglr_cycle_res := (
                        
# 26 "Parser.yrd"
                                                                   Mov (fc, sc)
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_expr) 
# 410 "Parser.yrd.fs"
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
                
# 27 "Parser.yrd"
                           Eps
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 23 "Parser.yrd"
               : '_rnglr_type_expr) 
# 430 "Parser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
             |> List.iter (fun (fst) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
               |> List.iter (fun (snd) -> 
                _rnglr_cycle_res := (
                  
# 29 "Parser.yrd"
                                                (int fst) * 1<ln>, (int snd) * 1<col>
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 29 "Parser.yrd"
               : '_rnglr_type_coord) 
# 452 "Parser.yrd.fs"
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
# 470 "Parser.yrd.fs"
      );
  |] , [|
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_coord)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_error)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_expr)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_line)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_program)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_many_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_some_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_start_rule)   ) |> List.concat));
  |] 
let translate (args : TranslateArguments<_,_>) (tree : Tree<_>) (dict : _ ) : '_rnglr_type_yard_start_rule = 
  unbox (tree.Translate _rnglr_rule_  leftSide _rnglr_concats (if args.filterEpsilons then _rnglr_filtered_epsilons else _rnglr_epsilons) args.tokenToRange args.zeroPosition args.clearAST dict) : '_rnglr_type_yard_start_rule
