namespace Controller

open System
open Processor
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom

type IController<'T> =
    abstract member Save : string -> string -> unit
    abstract member Load : string -> string
    abstract member Init : string -> CompilerErrorCollection
    abstract member Compile : string array array -> unit
    abstract member ChangeLine : int -> string array -> unit
    abstract member Run : unit -> unit
    abstract member Run : int -> unit
    abstract member Step : unit -> unit
    abstract member Step : int -> unit
    abstract member Read : int -> int -> 'T
    abstract member ReadAll : unit -> GridCell<'T> array
    abstract member Check : string array array -> ErrorListItem array
    abstract member CheckLine : string array -> ErrorListItem array
    [<CLIEvent>]
    abstract member Alert : IEvent<EventHandler, EventArgs>
