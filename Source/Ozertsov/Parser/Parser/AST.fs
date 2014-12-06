module Parser.AST

type Var = string

type Op = 
    | Set of (int*int)*int
    | Mov of (int*int)*(int*int)
    | Mvc of (int*int)*int
    | Eps

type Expr =
    | Num of float
    | EVar of Var
    | BinOp of Op*Expr*Expr

type Stmt = Op

type program = Stmt
