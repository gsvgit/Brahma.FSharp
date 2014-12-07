module TTA.ASM

type Asm<'a> =
    | Set of (int*int)*'a
    | Mov of (int*int)*(int*int)
    | Mvc of (int*int)*'a
    | Eps

type Program<'a> = array<array<Asm<'a>>>