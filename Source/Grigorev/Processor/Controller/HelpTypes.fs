namespace Controller

open System

type ErrorListItem = {
    Message : string
    Row : int
    Col : int
}

type AlertEventArgs (str : string) =
    inherit EventArgs ()
    member this.Message = str