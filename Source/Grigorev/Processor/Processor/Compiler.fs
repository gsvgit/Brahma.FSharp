namespace Compiler

open System
open System.IO
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom

type Compiler () =
    let compile (str : string) =
        let comp = new FSharpCodeProvider ()
        let arr = "namespace TestingCompiler\r\ntype TT () =\r\n    member this.mtd (x : int) = x * x"
        let par = new CompilerParameters (null, "out.dll")
        let res = comp.CompileAssemblyFromSource (par, str)
        if res.Errors.Count = 0
        then res.CompiledAssembly
        else failwith ""

    member this.Compile (str : string) =
        compile str
