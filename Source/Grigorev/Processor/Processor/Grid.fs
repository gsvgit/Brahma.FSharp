namespace Processor

open System.Collections.Generic
open TTA.ASM

type Grid<'T> (functions : array<'T -> 'T -> 'T>) =
    let grid = new Dictionary<int * int, 'T> ()
    let getCellCoordinates (r : int<ln>) (c : int<col>) = int r, int c
        
    member this.Check (r, c) =
        let r, c = getCellCoordinates r c
        let rm = r < 0
        let cm = c >= functions.Length || c < 0
        match rm, cm with
        | true, true -> CannotExist (Both)
        | true, false -> CannotExist (Row)
        | false, true -> CannotExist (Column)
        | _ when grid.ContainsKey ((r, c)) -> Exists
        |_ -> CanExist

    member this.GetValue (r, c) = 
        let r, c = getCellCoordinates r c
        if grid.ContainsKey (r, c)
        then grid.Item (r, c)
        else Unchecked.defaultof<'T>

    member this.SetValue (r, c, v) = grid.Item (getCellCoordinates r c) <- v

    member this.Move (r, c, v) =
        let rr, cc = getCellCoordinates r c
        grid.Item ((rr, cc)) <- functions.[cc] <| this.GetValue (r, c) <| v

    member this.Clear () = grid.Clear ()

    member this.Size = functions.Length