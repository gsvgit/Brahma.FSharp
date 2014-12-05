namespace Controller

open System
open System.Reflection
open Processor
open Compiler
open System.Linq
open System.Collections.Generic
open System.CodeDom.Compiler

type Controller<'T> () =
    let alert = new Event<AlertEventArgs> ()
    let project = {Name = "Default"; File = null; SourceCode = [| [|"Eps"|] |]; InitCode = "namespace DefaultNamespace\ntype public FunctionsType () =\n    static member GetArray () = [|(+); (-); (*); (/)|]"}
    let mutable processor = null

    let init data =
        let comp = new FSharpCompiler ()
        let res = comp.Compile data
        match res with
        | Error (c) ->
            let arr = Array.zeroCreate c.Count
            for i in 0 .. c.Count - 1 do
                let x = c.[i]
                arr.[i] <- {Row = x.Line; Col = x.Column; Message = x.ErrorText}
            arr
        | Success (a) ->
            let tp = a.GetType "DefaultNamespace.FunctionsType"
            let arr = tp.GetMethod "GetArray"
            processor <- new Processor<'T> (arr.Invoke(null, null) :?> (('T -> 'T -> 'T) array))
            null
    do
        init project.InitCode |> ignore

    interface IController<'T> with
        member this.Open file = ()
        member this.Save data file = ()
        member this.Init data = null
        member this.Compile code = ()
        member this.ChangeLine lineNumber data = ()
        member this.Run () = ()
        member this.Run tillLine = ()
        member this.Step () = ()
        member this.Step count = ()
        member this.Read row col = Unchecked.defaultof<'T>
        member this.ReadAll () = [||]
        member this.Check code = [||]
        member this.CheckLine code = [||]
        member this.Source = project.SourceCode
        member this.InitCode = project.InitCode

        member this.ThreadNumber with
            get () = Array.length project.SourceCode
            and set (value) = Array.Resize (ref project.SourceCode, value)

        member this.ProjectName = project.Name
        
        [<CLIEvent>]
        member this.Alert = alert.Publish