module TTA.ASM

[<Measure>] type ln
[<Measure>] type col

type Asm<'a> =
    | Set of (int<ln>*int<col>)*'a
    | Mov of (int<ln>*int<col>)*(int<ln>*int<col>)
    | Mvc of (int<ln>*int<col>)*'a
    | Eps

type Program<'a> = array<array<Asm<'a>>>

