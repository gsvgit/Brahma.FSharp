
# 2 "Compiler.yrd.fs"
module Compiler.Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "Compiler.yrd"

open TTA.ASM

# 13 "Compiler.yrd.fs"
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
let private rules = [|6; 4; 3; 15; 3; 15; 6; 5; 2; 5; 16; 0; 12; 11; 0; 12; 10; 0; 0; 8; 9; 12; 12; 13|]
let private rulesStart = [|0; 1; 2; 4; 7; 8; 8; 10; 13; 16; 19; 20; 24|]
let startRule = 1

let acceptEmptyInput = false

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 17; 21; 19; 22; 3; 4; 11; 14; 2; 5; 7; 6; 8; 9; 10; 12; 13; 15; 16; 18; 20|]
let private small_gotos =
        [|9; 131072; 196609; 262146; 327683; 393220; 524293; 655366; 720903; 1048584; 65542; 131072; 327689; 524293; 655366; 720903; 1048584; 262146; 10; 589835; 327682; 12; 589835; 458753; 786445; 524289; 786446; 589825; 851983; 720898; 16; 589835; 786433; 786449; 917506; 18; 589835; 983041; 786451; 1114113; 983060; 1179656; 131072; 196609; 327683; 393237; 524293; 655366; 720903; 1048584|]
let gotos = Array.zeroCreate 23
for i = 0 to 22 do
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
let private lists_reduces = [|[|6,1|]; [|6,2|]; [|10,1|]; [|9,3|]; [|11,4|]; [|8,3|]; [|7,3|]; [|2,2|]; [|4,1|]; [|3,3|]; [|0,1|]|]
let private small_reduces =
        [|65537; 983040; 131073; 983041; 196613; 524290; 655362; 720898; 983042; 1048578; 393221; 524291; 655363; 720899; 983043; 1048579; 655367; 524292; 589828; 655364; 720900; 786436; 983044; 1048580; 851973; 524293; 655365; 720901; 983045; 1048581; 1048581; 524294; 655366; 720902; 983046; 1048582; 1179649; 917511; 1245185; 983048; 1310721; 917513; 1441793; 917514|]
let reduces = Array.zeroCreate 23
for i = 0 to 22 do
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
        [|1; 983040; 65537; 983041; 1179649; 983040|]
let zeroReduces = Array.zeroCreate 23
for i = 0 to 22 do
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
let private small_acc = [21]
let private accStates = Array.zeroCreate 23
for i = 0 to 22 do
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
                
# 19 "Compiler.yrd"
                          S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Compiler.yrd"
               : '_rnglr_type_program) 
# 183 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Compiler.yrd"
               : '_rnglr_type_yard_start_rule) 
# 193 "Compiler.yrd.fs"
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
                    
# 19 "Compiler.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_elem) -> 
              _rnglr_cycle_res := (
                
# 19 "Compiler.yrd"
                          [yard_elem]
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Compiler.yrd"
               : '_rnglr_type_yard_some_1) 
# 224 "Compiler.yrd.fs"
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
                    
# 19 "Compiler.yrd"
                              S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_some_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 19 "Compiler.yrd"
                            yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 19 "Compiler.yrd"
               : '_rnglr_type_yard_some_1) 
# 257 "Compiler.yrd.fs"
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
                
# 21 "Compiler.yrd"
                       S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Compiler.yrd"
               : '_rnglr_type_line) 
# 277 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 21 "Compiler.yrd"
                     []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 21 "Compiler.yrd"
               : '_rnglr_type_yard_many_1) 
# 295 "Compiler.yrd.fs"
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
                  
# 21 "Compiler.yrd"
                         S1
                    )::!_rnglr_cycle_res )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_many_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 21 "Compiler.yrd"
                         yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 21 "Compiler.yrd"
               : '_rnglr_type_yard_many_1) 
# 326 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with SET _rnglr_val -> [_rnglr_val] | a -> failwith "SET expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_coord) 
               |> List.iter (fun (c) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (n) -> 
                  _rnglr_cycle_res := (
                    
# 24 "Compiler.yrd"
                                           Set (c, int n)
                      )::!_rnglr_cycle_res ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Compiler.yrd"
               : '_rnglr_type_expr) 
# 350 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MVC _rnglr_val -> [_rnglr_val] | a -> failwith "MVC expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_coord) 
               |> List.iter (fun (c) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (n) -> 
                  _rnglr_cycle_res := (
                    
# 25 "Compiler.yrd"
                                             Mvc (c, int n)
                      )::!_rnglr_cycle_res ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Compiler.yrd"
               : '_rnglr_type_expr) 
# 374 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with MOV _rnglr_val -> [_rnglr_val] | a -> failwith "MOV expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_coord) 
               |> List.iter (fun (fc) -> 
                ((unbox _rnglr_children.[2]) : '_rnglr_type_coord) 
                 |> List.iter (fun (sc) -> 
                  _rnglr_cycle_res := (
                    
# 26 "Compiler.yrd"
                                              Mov (fc, sc)
                      )::!_rnglr_cycle_res ) ) )
            !_rnglr_cycle_res
          )
            )
# 23 "Compiler.yrd"
               : '_rnglr_type_expr) 
# 398 "Compiler.yrd.fs"
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
                
# 27 "Compiler.yrd"
                        Eps
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 23 "Compiler.yrd"
               : '_rnglr_type_expr) 
# 418 "Compiler.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            (match ((unbox _rnglr_children.[0]) : Token) with LBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "LBRACE expected, but %A found" a )
             |> List.iter (fun (_rnglr_var_0) -> 
              (match ((unbox _rnglr_children.[1]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
               |> List.iter (fun (fst) -> 
                (match ((unbox _rnglr_children.[2]) : Token) with NUMBER _rnglr_val -> [_rnglr_val] | a -> failwith "NUMBER expected, but %A found" a )
                 |> List.iter (fun (snd) -> 
                  (match ((unbox _rnglr_children.[3]) : Token) with RBRACE _rnglr_val -> [_rnglr_val] | a -> failwith "RBRACE expected, but %A found" a )
                   |> List.iter (fun (_rnglr_var_1) -> 
                    _rnglr_cycle_res := (
                      
# 29 "Compiler.yrd"
                                                                  (int fst) * 1<ln>, (int snd) * 1<col>
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 29 "Compiler.yrd"
               : '_rnglr_type_coord) 
# 444 "Compiler.yrd.fs"
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
# 462 "Compiler.yrd.fs"
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
