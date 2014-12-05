namespace Compiler

open System
open System.IO
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom

type FSharpCompiler () =
    let compile (str : string) =
        let comp = new FSharpCodeProvider ()
        let par = new CompilerParameters (null, "out.dll")
        let res = comp.CompileAssemblyFromSource (par, str)
        if res.Errors.Count = 0
        then Success (res.CompiledAssembly)
        else Error (res.Errors)

    interface IFSharpCompiler with
        member this.Compile str =
            compile str

    member this.Compile (str : string) =
        compile str
