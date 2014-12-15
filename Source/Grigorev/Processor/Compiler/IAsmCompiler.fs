namespace Compiler

open TTA.ASM

type AsmCompilationResult<'T> =
    | Success of Program<'T>
    | Error of (string * int) array

type IAsmCompiler<'T> =
    abstract member Compile : string array -> AsmCompilationResult<'T>