module NA.IL

type aType<'a> =
    | Connector of int
    | OpBlock of int*int
    | Const of 'a

type Instruction<'a> =
     | Set of aType<'a>*aType<'a>
     | Move of aType<'a>*aType<'a>
     | Eps
