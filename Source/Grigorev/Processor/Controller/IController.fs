namespace Controller

open System
open System.IO
open Processor
open System.CodeDom.Compiler
open FSharp.Compiler.CodeDom
open System.Linq

type IController<'T> =
    abstract member New : unit -> unit
    abstract member Save : string -> unit
    abstract member Save : unit -> unit
    abstract member Open : string -> unit
    abstract member Init : string -> ErrorListItem array
    abstract member Update : string array -> unit
    abstract member Compile : unit -> unit
    abstract member CompilationErrors : ErrorListItem array with get
    abstract member ChangeLine : int -> string array -> unit
    abstract member Run : unit -> unit
    abstract member Run : int -> unit
    abstract member StartDebug : unit -> unit
    abstract member StopDebug : unit -> unit
    abstract member InDebug : bool with get
    abstract member Step : unit -> unit
    abstract member Step : int -> unit
    abstract member Read : int -> int -> 'T
    abstract member ReadAll : unit -> GridCell<'T> array
    abstract member Clear : unit -> unit
    abstract member ClearOnRun : bool with get, set
    abstract member Source : string array
    abstract member InitCode : string
    abstract member ThreadNumber : int with get, set
    abstract member ProjectName : string
    abstract member FunctionsCount : int with get
    abstract member GridWidth : int with get
    abstract member GridHeight : int with get
    abstract member IsSaved : bool with get
    abstract member HasFilename : bool with get
    [<CLIEvent>]
    abstract member Alert : IEvent<AlertEventArgs>
