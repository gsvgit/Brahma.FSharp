module TTA.ASM

[<Measure>] type ln
[<Measure>] type col

type Asm =
    | Set of (int<ln>*int<col>)*int
    | Mov of (int<ln>*int<col>)*(int<ln>*int<col>)
    | Mvc of (int<ln>*int<col>)*int
    | Eps

type Program = array<array<Asm>>

