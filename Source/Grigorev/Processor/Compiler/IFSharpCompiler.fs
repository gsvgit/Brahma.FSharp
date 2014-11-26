namespace Compiler

open System.Reflection
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom

type CompilationResult =
    | Success of Assembly
    | Error of CompilerErrorCollection

type IFSharpCompiler =
    abstract member Compile : string -> CompilationResult

