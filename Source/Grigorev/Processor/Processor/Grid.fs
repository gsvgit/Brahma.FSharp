namespace Processor

open System.Collections.Generic
open TTA.ASM

type Grid<'T> (functions : array<'T -> 'T -> 'T>) =
    let grid = new Dictionary<int * int, 'T> ()
    let mutable width = 0
    let mutable height = 0
    
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
        height <- max height (rr + 1)
        width <- max width (cc + 1)
        grid.Item ((rr, cc)) <- functions.[cc] <| this.GetValue (r, c) <| v

    member this.GetAllValues () =
        let l = grid.Count
        let arr = Array.zeroCreate l
        let mutable i = 0
        for x in grid do
            arr.[i] <- x
            i <- i + 1
        arr |> Array.map (fun x -> {Row = fst x.Key; Col = snd x.Key; Value = x.Value})

    member this.Clear () =
        width <- 0
        height <- 0
        grid.Clear ()

    member this.Size = functions.Length
    member this.Width = width
    member this.Height = height