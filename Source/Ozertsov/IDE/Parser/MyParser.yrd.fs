
# 2 "MyParser.yrd.fs"
module MyParser.Parser
#nowarn "64";; // From fsyacc: turn off warnings that type variables used in production annotations are instantiated to concrete type
open Yard.Generators.RNGLR.Parser
open Yard.Generators.RNGLR
open Yard.Generators.Common.AST

# 1 "MyParser.yrd"

open TTA.ASM

# 13 "MyParser.yrd.fs"
type Token =
    | EPS of (string)
    | MOV of (string)
    | MVC of (string)
    | NUMBER of (string)
    | RNGLR_EOF of (string)
    | SEMI of (string)
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
    | SEMI x -> box x
    | SET x -> box x

let numToString = function
    | 0 -> "error"
    | 1 -> "operation"
    | 2 -> "stmt"
    | 3 -> "yard_many_1"
    | 4 -> "yard_start_rule"
    | 5 -> "EPS"
    | 6 -> "MOV"
    | 7 -> "MVC"
    | 8 -> "NUMBER"
    | 9 -> "RNGLR_EOF"
    | 10 -> "SEMI"
    | 11 -> "SET"
    | _ -> ""

let tokenToNumber = function
    | EPS _ -> 5
    | MOV _ -> 6
    | MVC _ -> 7
    | NUMBER _ -> 8
    | RNGLR_EOF _ -> 9
    | SEMI _ -> 10
    | SET _ -> 11

let isLiteral = function
    | EPS _ -> false
    | MOV _ -> false
    | MVC _ -> false
    | NUMBER _ -> false
    | RNGLR_EOF _ -> false
    | SEMI _ -> false
    | SET _ -> false

let getLiteralNames = []
let mutable private cur = 0
let leftSide = [|2; 4; 3; 3; 1; 1; 1; 1|]
let private rules = [|3; 2; 1; 10; 3; 11; 8; 8; 8; 7; 8; 8; 8; 6; 8; 8; 8; 8; 5|]
let private rulesStart = [|0; 1; 2; 2; 5; 9; 13; 18; 19|]
let startRule = 1

let acceptEmptyInput = true

let defaultAstToDot =
    (fun (tree : Yard.Generators.Common.AST.Tree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let otherAstToDot =
    (fun (tree : Yard.Generators.RNGLR.OtherSPPF.OtherTree<Token>) -> tree.AstToDot numToString tokenToNumber leftSide)

let private lists_gotos = [|1; 18; 19; 4; 5; 10; 14; 2; 3; 6; 7; 8; 9; 11; 12; 13; 15; 16; 17|]
let private small_gotos =
        [|7; 65536; 131073; 196610; 327683; 393220; 458757; 720902; 65537; 655367; 131078; 65536; 196616; 327683; 393220; 458757; 720902; 327681; 524297; 393217; 524298; 458753; 524299; 524289; 524300; 655361; 524301; 720897; 524302; 786433; 524303; 917505; 524304; 983041; 524305; 1048577; 524306|]
let gotos = Array.zeroCreate 20
for i = 0 to 19 do
        gotos.[i] <- Array.zeroCreate 12
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
let private lists_reduces = [|[|3,2|]; [|3,3|]; [|7,1|]; [|6,5|]; [|5,4|]; [|4,4|]; [|0,1|]|]
let private small_reduces =
        [|131073; 589824; 196609; 589825; 262145; 655362; 589825; 655363; 851969; 655364; 1114113; 655365; 1245185; 589830|]
let reduces = Array.zeroCreate 20
for i = 0 to 19 do
        reduces.[i] <- Array.zeroCreate 12
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
        [|1; 589824; 131073; 589825|]
let zeroReduces = Array.zeroCreate 20
for i = 0 to 19 do
        zeroReduces.[i] <- Array.zeroCreate 12
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
let private small_acc = [18; 0]
let private accStates = Array.zeroCreate 20
for i = 0 to 19 do
        accStates.[i] <- List.exists ((=) i) small_acc
let eofIndex = 9
let errorIndex = 0
let errorRulesExists = false
let private parserSource = new ParserSource<Token> (gotos, reduces, zeroReduces, accStates, rules, rulesStart, leftSide, startRule, eofIndex, tokenToNumber, acceptEmptyInput, numToString, errorIndex, errorRulesExists)
let buildAstAbstract : (seq<int*array<'TokenType*int>> -> ParseResult<Token>) =
    buildAstAbstract<Token> parserSource

let buildAst : (seq<'TokenType> -> ParseResult<Token>) =
    buildAst<Token> parserSource


let _rnglr_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(8, new Nodes([||])), null)), null); null; new Tree<_>(null,box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null)), null); new Tree<_>(null,box (new AST(new Family(2, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(1, new Nodes([|box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null))|])), null)), null)|]
let _rnglr_filtered_epsilons : Tree<Token>[] = [|new Tree<_>(null,box (new AST(new Family(8, new Nodes([||])), null)), null); null; new Tree<_>(null,box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null)), null); new Tree<_>(null,box (new AST(new Family(2, new Nodes([||])), null)), null); new Tree<_>(null,box (new AST(new Family(1, new Nodes([|box (new AST(new Family(0, new Nodes([|box (new AST(new Family(2, new Nodes([||])), null))|])), null))|])), null)), null)|]
for x in _rnglr_filtered_epsilons do if x <> null then x.ChooseSingleAst()
let _rnglr_extra_array, _rnglr_rule_, _rnglr_concats = 
  (Array.zeroCreate 0 : array<'_rnglr_type_error * '_rnglr_type_operation * '_rnglr_type_stmt * '_rnglr_type_yard_many_1 * '_rnglr_type_yard_start_rule>), 
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
                
# 20 "MyParser.yrd"
                       S1
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 20 "MyParser.yrd"
               : '_rnglr_type_stmt) 
# 170 "MyParser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          ((unbox _rnglr_children.[0]) : '_rnglr_type_stmt) 
            )
# 20 "MyParser.yrd"
               : '_rnglr_type_yard_start_rule) 
# 180 "MyParser.yrd.fs"
      );
  (
    fun (_rnglr_children : array<_>) (parserRange : (uint64 * uint64)) -> 
      box (
        ( 
          (
            let _rnglr_cycle_res = ref []
            _rnglr_cycle_res := (
              
# 20 "MyParser.yrd"
                     []
                )::!_rnglr_cycle_res
            !_rnglr_cycle_res
          )
            )
# 20 "MyParser.yrd"
               : '_rnglr_type_yard_many_1) 
# 198 "MyParser.yrd.fs"
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
                (match ((unbox _rnglr_children.[1]) : Token) with SEMI _rnglr_val -> [_rnglr_val] | a -> failwith "SEMI expected, but %A found" a )
                 |> List.iter (fun (S2) -> 
                  _rnglr_cycle_res := (
                    
# 20 "MyParser.yrd"
                           S1, S2
                      )::!_rnglr_cycle_res ) )
              !_rnglr_cycle_res
            ) |> List.iter (fun (yard_head) -> 
              ((unbox _rnglr_children.[2]) : '_rnglr_type_yard_many_1) 
               |> List.iter (fun (yard_tail) -> 
                _rnglr_cycle_res := (
                  
# 20 "MyParser.yrd"
                         yard_head::yard_tail
                    )::!_rnglr_cycle_res ) )
            !_rnglr_cycle_res
          )
            )
# 20 "MyParser.yrd"
               : '_rnglr_type_yard_many_1) 
# 231 "MyParser.yrd.fs"
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
                      
# 23 "MyParser.yrd"
                                                            Set(((int ln)*1<ln>, (int col)*1<col>), int arg)
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 22 "MyParser.yrd"
               : '_rnglr_type_operation) 
# 257 "MyParser.yrd.fs"
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
                      
# 24 "MyParser.yrd"
                                                              Mvc(((int ln)*1<ln>, (int col)*1<col>), int arg)
                        )::!_rnglr_cycle_res ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 22 "MyParser.yrd"
               : '_rnglr_type_operation) 
# 283 "MyParser.yrd.fs"
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
                        
# 25 "MyParser.yrd"
                                                                              Mov(((int ln1)*1<ln>, (int col1)*1<col>), ((int ln2)*1<ln>, (int col2)*1<col>))
                          )::!_rnglr_cycle_res ) ) ) ) )
            !_rnglr_cycle_res
          )
            )
# 22 "MyParser.yrd"
               : '_rnglr_type_operation) 
# 311 "MyParser.yrd.fs"
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
                
# 26 "MyParser.yrd"
                        Eps
                  )::!_rnglr_cycle_res )
            !_rnglr_cycle_res
          )
            )
# 22 "MyParser.yrd"
               : '_rnglr_type_operation) 
# 331 "MyParser.yrd.fs"
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
# 349 "MyParser.yrd.fs"
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
