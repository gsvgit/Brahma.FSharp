
# 2 "Asm.yrd.fs"
module Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "Asm.yrd"

open TTA.ASM

# 13 "Asm.yrd.fs"
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
    | 7 -> "yard_many_1"
    | 8 -> "yard_start_rule"
    | 9 -> "EPS"
    | 10 -> "MOV"
    | 11 -> "MVC"
    | 12 -> "NUMBER"
    | 13 -> "RNGLR_EOF"
    | 14 -> "SET"
    | _ -> ""

let tokenToNumber = function
    | EPS _ -> 9
    | MOV _ -> 10
    | MVC _ -> 11
    | NUMBER _ -> 12
    | RNGLR_EOF _ -> 13
    | SET _ -> 14

let isLiteral = function
    | EPS _ -> false
    | MOV _ -> false
    | MVC _ -> false
    | NUMBER _ -> false
    | RNGLR_EOF _ -> false
    | SET _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|4; 8; 7; 7; 6; 6; 6; 6; 0; 5; 3; 2|]
let private rules = [|7; 4; 6; 7; 0; 5; 3; 2; 9; 14; 12; 12; 12; 11; 12; 12; 12; 10; 12; 12; 12; 12|]
let private rulesStart = [|0; 1; 2; 2; 4; 5; 6; 7; 8; 9; 13; 17; 22|]
let startRule = 1

let acceptEmptyInput = true

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 2; 3; 4; 5; 6; 22; 8; 9; 14; 18; 7; 10; 11; 12; 13; 15; 16; 17; 19; 20; 21|]
let private small_gotos =
        [|11; 0; 131073; 196610; 262147; 327684; 393221; 458758; 589831; 655368; 720905; 917514; 393226; 0; 131073; 196610; 327684; 393221; 458763; 589831; 655368; 720905; 917514; 589825; 786444; 655361; 786445; 720897; 786446; 786433; 786447; 917505; 786448; 983041; 786449; 1048577; 786450; 1179649; 786451; 1245185; 786452; 1310721; 786453|]
let gotos = Array.zeroCreate 23
for i = 0 to 22 do
        gotos.[i] <- Array.zeroCreate 15
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
let private lists_reduces = [|[|4,1|]; [|7,1|]; [|6,1|]; [|5,1|]; [|3,1|]; [|3,2|]; [|8,1|]; [|11,5|]; [|10,4|]; [|9,4|]; [|0,1|]|]
let private small_reduces =
        [|65541; 589824; 655360; 720896; 851968; 917504; 131077; 589825; 655361; 720897; 851969; 917505; 196613; 589826; 655362; 720898; 851970; 917506; 327685; 589827; 655363; 720899; 851971; 917507; 393217; 851972; 458753; 851973; 524293; 589830; 655366; 720902; 851974; 917510; 851973; 589831; 655367; 720903; 851975; 917511; 1114117; 589832; 655368; 720904; 851976; 917512; 1376261; 589833; 655369; 720905; 851977; 917513; 1441793; 851978|]
let reduces = Array.zeroCreate 23
for i = 0 to 22 do
        reduces.[i] <- Array.zeroCreate 15
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
let private lists_zeroReduces = [|[|2; 1; 0|]; [|2|]|]
let private small_zeroReduces =
        [|1; 851968; 393217; 851969|]
let zeroReduces = Array.zeroCreate 23
for i = 0 to 22 do
        zeroReduces.[i] <- Array.zeroCreate 15
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
let private small_acc = [4; 0]
let private accStates = Array.zeroCreate 23
for i = 0 to 22 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 13
let errorIndex = 1
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(12, new Nodes([||])), null)), null); null; null; new Tree<_>(null,box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null)), null); null; null; new Tree<_>(null,box (new AST(new Family(2, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(1, new Nodes([|box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null))|])), null)), null)|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|null; new Tree<_>(null,box (new AST(new Family(12, new Nodes([||])), null)), null); null; null; new Tree<_>(null,box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null)), null); null; null; new Tree<_>(null,box (new AST(new Family(2, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(1, new Nodes([|box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null))|])), null)), null)|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_eps * '_rnglr_type_error * '_rnglr_type_mov * '_rnglr_type_mvc * '_rnglr_type_program * '_rnglr_type_set * '_rnglr_type_stmt * '_rnglr_type_yard_many_1 * '_rnglr_type_yard_start_rule>), 
  [|
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_yard_many_1) 
             |> List.iter (fun (S1) -> 
              _rnglr_cycle_res := (
                
# 19 "Asm.yrd"
                         S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 19 "Asm.yrd"
               : '_rnglr_type_program) 
# 169 "Asm.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_program) 
            )
# 19 "Asm.yrd"
               : '_rnglr_type_yard_start_rule) 
# 179 "Asm.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 19 "Asm.yrd"
                       []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 19 "Asm.yrd"
               : '_rnglr_type_yard_many_1) 
# 197 "Asm.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            ((unbox _rnglr_children.[0]) : '_rnglr_type_stmt) 
             |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[1]) : '_rnglr_type_yard_many_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 19 "Asm.yrd"
                           yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 19 "Asm.yrd"
               : '_rnglr_type_yard_many_1) 
# 219 "Asm.yrd.fs"
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
                
# 21 "Asm.yrd"
                      S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Asm.yrd"
               : '_rnglr_type_stmt) 
# 239 "Asm.yrd.fs"
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
                
# 21 "Asm.yrd"
                            S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Asm.yrd"
               : '_rnglr_type_stmt) 
# 259 "Asm.yrd.fs"
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
                
# 21 "Asm.yrd"
                                  S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Asm.yrd"
               : '_rnglr_type_stmt) 
# 279 "Asm.yrd.fs"
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
                
# 21 "Asm.yrd"
                                        S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 21 "Asm.yrd"
               : '_rnglr_type_stmt) 
# 299 "Asm.yrd.fs"
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
                
# 23 "Asm.yrd"
                           Eps 
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 23 "Asm.yrd"
               : '_rnglr_type_eps) 
# 319 "Asm.yrd.fs"
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
                      
# 25 "Asm.yrd"
                                                            Set (((int i)*1<ln>, (int j)*1<col>), int v) 
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 25 "Asm.yrd"
               : '_rnglr_type_set) 
# 345 "Asm.yrd.fs"
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
                      
# 27 "Asm.yrd"
                                                            Mvc (((int i)*1<ln>, (int j)*1<col>), int v) 
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 27 "Asm.yrd"
               : '_rnglr_type_mvc) 
# 371 "Asm.yrd.fs"
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
                        
# 29 "Asm.yrd"
                                                                       Mov (((int i)*1<ln>, (int j)*1<col>), ((int x)*1<ln>, (int y)*1<col>)) 
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 29 "Asm.yrd"
               : '_rnglr_type_mov) 
# 399 "Asm.yrd.fs"
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
# 417 "Asm.yrd.fs"
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
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_many_1)   ) |> List.concat));
    (fun (_rnglr_list : list<_>) -> 
      box ( 
        _rnglr_list |> List.map (fun _rnglr_item -> ((unbox _rnglr_item) : '_rnglr_type_yard_start_rule)   ) |> List.concat));
  |] 
let translate (args : TranslateArguments<_,_>) (tree : Tree<_>) (dict : _ ) : '_rnglr_type_yard_start_rule = 
  unbox (tree.Translate _rnglr_rule_  leftSide _rnglr_concats (if args.filterEpsilons then _rnglr_filtered_epsilons else _rnglr_epsilons) args.tokenToRange args.zeroPosition args.clearAST dict) : '_rnglr_type_yard_start_rule
