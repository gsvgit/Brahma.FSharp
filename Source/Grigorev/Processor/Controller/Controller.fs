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
open Compiler
open TTA.ASM

type Controller<'T> () =
    let alert = new Event<AlertEventArgs> ()
    let defProject = {Name = "Default"; SourceCode = [| [|"Eps"|] |]; InitCode = "namespace DefaultNamespace\r\n\r\ntype public FunctionsType () =\r\n    let f (x : 'T) (y : 'T) = Unchecked.defaultof<'T>\r\n    member this.GetArray () = [| f |]"}
    let mutable project = defProject
    let mutable fileName = null
    let mutable processor = null
    let mutable clearOnRun = true
    let mutable errors = [||]
    let mutable binary = None
    let comp = new AsmYaccCompiler<'T>() :> IAsmCompiler<'T>
    let mutable inDebug = false
    let mutable debugPosition = 0

    let init (data : string) =
        processor <- new Processor<'T> ([| (fun x y -> x); (fun x y -> y) |])
        null
//        processor <- null
//        let data = data.Replace("'T", typeof<'T>.ToString())
//        project.InitCode <- data
//        let comp = new FSharpCompiler ()
//        let res = comp.Compile data
//        match res with
//        | Error (c) ->
//            let arr = Array.zeroCreate c.Count
//            for i in 0 .. c.Count - 1 do
//                let x = c.[i]
//                arr.[i] <- {Row = x.Line; Col = x.Column; Message = x.ErrorText}
//            arr
//        | Success (a) ->
//            let tp = a.GetType "DefaultNamespace.FunctionsType"
//            let ins = tp.GetConstructors().First().Invoke(null)
//            let arr = tp.GetMethod "GetArray"
//            let arr =
//                if arr.IsGenericMethod
//                then arr.MakeGenericMethod(typeof<'T>)
//                else arr
//            processor <- new Processor<'T> (arr.Invoke(ins, null) :?> (('T -> 'T -> 'T) array))
//            null

    let openFile (file : string) =
        let reader = new FileStream (file, FileMode.Open) 
        let ser = new DataContractSerializer (typeof<Project>)
        project <- ser.ReadObject(reader) :?> Project
        reader.Close()
        fileName <- file
        init project.InitCode |> ignore

    let save (file : string) =
        project.Name <- file.Substring(file.LastIndexOf('\\') + 1)
        let writer = new FileStream (file, FileMode.Create) 
        let ser = new DataContractSerializer (typeof<Project>)
        ser.WriteObject (writer, project)
        writer.Close()
        fileName <- file

    let compile () =
        binary <- None
        let result = project.SourceCode |> comp.Compile
        let arr = new List<ErrorListItem>()
        let help (x : option<Asm<'T>>) i j =
            if x.IsSome
            then x.Value
            else
                arr.Add({Row = i; Col = j; Message = "Parsing error"})
                Asm.Eps
        let result = result |> Array.mapi (fun j x -> x |> Array.mapi (fun i y -> help y i j))
        let res = result |> processor.CheckProgram
        let help2 x =
            match x with
            | IncorrectLine (s, i) -> {Row = i; Col = -1; Message = s}
            | IncorrectCommand (s, i, j) -> {Row = i; Col = j; Message = s}
        if res.IsSome
        then arr.AddRange(res.Value |> Array.map (fun x -> help2 x))
        errors <- arr.ToArray()
        if errors.Length = 0
        then binary <- Some (result)

    do
        try init project.InitCode |> ignore with | _ -> ()

    interface IController<'T> with
        member this.New () = project <- defProject

        member this.Open file =
            try openFile file with
            | _ -> alert.Trigger (new AlertEventArgs ("Error opening file"))

        member this.Save file =
            try save file with
            | e -> alert.Trigger (new AlertEventArgs ("Error saving file"))

        member this.Save () =
            try save fileName with
            | _ -> alert.Trigger (new AlertEventArgs ("Error saving file"))
            
        member this.Init data =
            try init data with
            | e ->
                //alert.Trigger ((new AlertEventArgs ("Error initializing processor")))
                [| {Row = 0; Col = 0; Message = e.Message} |]
            
        member this.Update code =
            project.SourceCode <- code

        member this.Compile () =
            compile ()
            
        member this.ChangeLine lineNumber data = ()

        member this.Run () = 
            if clearOnRun
            then processor.Clear()
            if errors.Length = 0 && binary.IsSome
            then processor.Evaluate binary.Value

        member this.Run tillLine = ()

        member this.StartDebug () =
            if clearOnRun
            then processor.Clear()
            if errors.Length = 0 && binary.IsSome
            then
                inDebug <- true
                debugPosition <- 0
        member this.StopDebug () = inDebug <- false
        member this.InDebug with get () = inDebug

        member this.Step () =
            if not inDebug
            then ()
            let f = ref true
            let mapper x =
                if debugPosition < Array.length x
                then
                    f := false
                    x.[debugPosition]
                else Eps
            let line = binary.Value |> Array.map (fun x -> mapper x)
            if !f
            then
                inDebug <- false
                ()
            processor.Evaluate line
            debugPosition <- debugPosition + 1
            

        member this.Step count = ()

        member this.Read row col = processor.Read (row, col)
        member this.ReadAll () = if processor = null then [||] else processor.ReadAll ()

        member this.Clear () = if processor <> null then processor.Clear()
        member this.ClearOnRun with get () = clearOnRun and set (v) = clearOnRun <- v

        member this.CompilationErrors with get () = errors
            
        member this.FunctionsCount with
            get () = if processor = null then 0 else processor.Size

        member this.GridWidth with
            get () = if processor = null then 0 else processor.Width

        member this.GridHeight with
            get () = if processor = null then 0 else processor.Height

        member this.Source = project.SourceCode
        member this.InitCode = project.InitCode

        member this.ThreadNumber with
            get () = Array.length project.SourceCode
            and set (value) =
                let n = project.SourceCode.Length
                project.SourceCode <- Array.init value (fun i -> if i < n then project.SourceCode.[i] else [||])

        member this.ProjectName = project.Name
        
        [<CLIEvent>]
        member this.Alert = alert.Publish