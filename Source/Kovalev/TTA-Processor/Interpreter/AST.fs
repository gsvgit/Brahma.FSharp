module Interpreter.AST

type Var = string

type Op = 
    | Pow
    | Plus
    | Mult
    | Div
    | Minus

type Expr =
    | Num of float
    | EVar of Var
    | BinOp of Op*Expr*Expr

type Stmt = Var*Expr

type program = List<Stmt>