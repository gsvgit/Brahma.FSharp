namespace Controller

open System
open System.Reflection
open Processor
open Compiler

type Controller<'T> () =
    let alert = new Event<EventHandler, EventArgs> ()
    
    interface IController<'T> with
        member this.Load file = file
        member this.Save data file = ()
        member this.Init data = null
        member this.Compile code = ()
        member this.ChangeLine lineNumber data = ()
        member this.Run () = ()
        member this.Run tillLine = ()
        member this.Step () = alert.Trigger (null, EventArgs.Empty)
        member this.Step count = ()
        member this.Read row col = Unchecked.defaultof<'T>
        member this.ReadAll () = [||]
        member this.Check code = [||]
        member this.CheckLine code = [||]
        [<CLIEvent>]
        member this.Alert = alert.Publish