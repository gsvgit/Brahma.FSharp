namespace Compiler

open TTA.ASM

type IAsmCompiler<'T> =
    abstract member Compile : string array array -> Asm<'T> option array array