module MyParser.AST

type Var = string


[<Measure>] type ln
[<Measure>] type col

type Op = 
    | Set of (int<ln>*int<col>)
    | Mov of (int<ln>*int<col>)*(int<ln>*int<col>)
    | Mvc of (int<ln>*int<col>)
    | Eps

type Expr =
    | Num of float
    | EVar of Var
    | BinOp of Op*Expr*Expr

type Stmt = Var*Expr

type program = List<Stmt>