module TTA.ASM

type Asm =
    | Set of (int*int)*int
    | Mov of (int*int)*(int*int)
    | Mvc of (int*int)*int
    | Eps

type Stmt = array<Asm>

type program = Stmt

