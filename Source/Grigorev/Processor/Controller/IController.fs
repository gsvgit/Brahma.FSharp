namespace Controller

open System
open System.IO
open Processor
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom
open System.Linq

type IController<'T> =
    abstract member Save : string -> unit
    abstract member Save : unit -> unit
    abstract member Open : string -> unit
    abstract member Init : string -> ErrorListItem array
    abstract member Update : string array array -> unit
    abstract member Compile : unit -> unit
    abstract member ChangeLine : int -> string array -> unit
    abstract member Run : unit -> unit
    abstract member Run : int -> unit
    abstract member Step : unit -> unit
    abstract member Step : int -> unit
    abstract member Read : int -> int -> 'T
    abstract member ReadAll : unit -> GridCell<'T> array
    abstract member Check : string array array -> ErrorListItem array
    abstract member CheckLine : string array -> ErrorListItem array
    abstract member Source : string array array
    abstract member InitCode : string
    abstract member ThreadNumber : int with get, set
    abstract member ProjectName : string
    [<CLIEvent>]
    abstract member Alert : IEvent<AlertEventArgs>
