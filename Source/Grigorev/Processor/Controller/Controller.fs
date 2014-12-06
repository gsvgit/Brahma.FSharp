namespace Controller

open System
open System.IO
open System.Xml.Serialization
open System.Runtime.Serialization
open System.Reflection
open Processor
open Compiler
open System.Linq
open System.Collections.Generic
open System.CodeDom.Compiler

type Controller<'T> () =
    let alert = new Event<AlertEventArgs> ()
    let mutable project = {Name = "Default"; SourceCode = [| [|"Eps"|] |]; InitCode = "namespace DefaultNamespace\r\ntype public FunctionsType () =\r\n    member this.GetArray () = [|(+); (-); (*); (/)|]"}
    let mutable fileName = null
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
            let ins = tp.GetConstructors().First().Invoke(null)
            let arr = tp.GetMethod "GetArray"
            processor <- new Processor<'T> (arr.Invoke(ins, null) :?> (('T -> 'T -> 'T) array))
            project.InitCode <- data
            null

    let openFile (file : string) =
        let reader = new FileStream (file, FileMode.Open) 
        let ser = new DataContractSerializer (typeof<Project>)
        project <- ser.ReadObject(reader) :?> Project
        reader.Close()
        fileName <- file
        init project.InitCode |> ignore

    let save (file : string) =
        let writer = new FileStream (file, FileMode.Create) 
        let ser = new DataContractSerializer (typeof<Project>)
        ser.WriteObject (writer, project)
        writer.Close()
        fileName <- file

    do
        init project.InitCode |> ignore

    interface IController<'T> with
        member this.Open file =
            try openFile file with
            | _ -> alert.Trigger (new AlertEventArgs ("Error opening file"))

        member this.Save file =
            try save file with
            | e -> alert.Trigger (new AlertEventArgs ("Error saving file"))

        member this.Save () =
            try save fileName with
            | _ -> alert.Trigger (new AlertEventArgs ("Error saving file"))
            
        member this.Init data = init data
        member this.Update code =
            project.SourceCode <- code
        member this.Compile () = ()
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
            and set (value) = Array.Resize (ref project.SourceCode, value) // тут лажа

        member this.ProjectName = project.Name
        
        [<CLIEvent>]
        member this.Alert = alert.Publish