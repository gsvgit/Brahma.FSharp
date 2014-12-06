namespace Processor

open System
open TTA.ASM

[<AllowNullLiteral>]
type Processor<'T> (functions : array<'T -> 'T -> 'T>) =
    let grid = new Grid<'T> (functions)
    
    let rotateArrayOfArrays arr =
        let l = Array.length arr
        if l = 0
        then arr
        else
            let sl = arr |> Array.fold (fun s e -> max s (Array.length e)) 0
            [| for i in 0 .. sl - 1 -> [|for j in 0 .. l - 1 -> if Array.length arr.[j] > i then arr.[j].[i] else Eps|] |]

    let checkCommand comm =
        match comm with
        | Eps -> None
        | Set (p, _) ->
            match grid.Check p with
            | CannotExist (m) -> Some (IncorrectCommand (m.ToString (), 0, 0))
            | _ -> None
        | Mvc (p, _) ->
            match grid.Check p with
            | CannotExist (m) -> Some (IncorrectCommand (m.ToString (), 0, 0))
            | _ -> None
        | Mov (p, q) ->
            match grid.Check p, grid.Check q with
            | CannotExist (m), _ -> Some (IncorrectCommand (m.ToString (), 0, 0))
            | _, CannotExist (m) -> Some (IncorrectCommand (m.ToString (), 0, 0))
            | _, _ -> None

    let checkLine line =
        let line =
            line
            |> Array.filter (fun x -> match x with | Eps -> false | _ -> true)
            |> Array.map (fun x -> match x with | Set (p, _) | Mov (p, _) | Mvc (p, _) -> p)
        let set = Set.ofArray line
        Set.count set = Array.length line
        
    let checkLineTotal line =
        let arr =
            line
            |> Array.mapi (fun i x -> match checkCommand x with | Some (IncorrectCommand (m, _, _)) -> Some (IncorrectCommand (m, 0, i)) | _ -> None)
            |> Array.filter (fun x -> x.IsSome)
            |> Array.map (fun x -> x.Value)
        if arr.Length = 0
        then if checkLine line then [||] else [|IncorrectLine ("There are 2 or more writes in single cell", 0)|]
        else arr

    let checkProgram commands =
        commands
        |> rotateArrayOfArrays
        |> Array.fold (
            fun s x -> (
                x |>
                checkLineTotal |>
                Array.map (fun y ->
                    match y with
                    | IncorrectCommand (m, _, p) -> IncorrectCommand (m, snd s, p)
                    | IncorrectLine (m, _) -> IncorrectLine (m, snd s))
                |> Array.append (fst s)), 1 + snd s) ([||], 0) |> fst

    let eval command =
        match checkCommand command with
        | Some (IncorrectCommand(m, r, c)) -> IncorrectCommandException (m, r, c) |> raise
        | _ ->
            match command with
            | Asm.Set ((r, c), v) -> grid.SetValue (r, c, v)
            | Mvc ((r, c), v) -> grid.Move (r, c, v)
            | Mov ((r, c), (vr, vc)) ->
                let v = grid.GetValue (vr, vc)
                grid.Move (r, c, v)
            | _ -> ()

    let evalRow commands =
        if checkLineTotal commands |> Array.length <> 0
        then IncorrectLineException ("Incorrect line, cannot execute", 0) |> raise
        let l = Array.length commands
        if l = 0
        then ()
        elif l = 1
        then eval commands.[0]
        else
            let commands = commands |> Array.filter (fun x -> match x with | Eps -> false | _ -> true)
            let reads = commands |> Array.map (fun x -> match x with | Set (_, v) | Mvc (_, v) -> Value v | Mov (_, p) -> Cell p)
            let readTask = seq { for x in reads -> async { return match x with | Value v -> v | Cell (r, c) -> grid.GetValue (r, c) } } |> Async.Parallel |> Async.StartAsTask
            readTask.Wait ()
            let readResult = readTask.Result
            let writeTask = seq { for i in 0 .. commands.Length - 1 -> async { match commands.[i] with | Set ((r, c), _) -> grid.SetValue (r, c, readResult.[i]) | Mov ((r, c), _) | Mvc ((r, c), _) -> grid.Move (r, c, readResult.[i]) | _ -> () } } |> Async.Parallel |> Async.StartAsTask
            writeTask.Wait ()
    
    do
        match functions with
        | null -> new ArgumentException ("Functions array cannot be null") |> raise
        | [||] -> new ArgumentException ("Functions array cannot be empty") |> raise
        | _ -> ()
       
    member this.Read (r, c) = grid.GetValue (r*1<ln>, c*1<col>)
    member this.ReadWithMeasure (r, c) = grid.GetValue (r, c)
    member this.ReadAll () = grid.GetAllValues ()

    member this.Evaluate (command : Asm<'T>) = eval command

    member this.Evaluate commands = evalRow commands

    member this.Evaluate (commands : Program<'T>) =
        let iter i e =
            try
                evalRow e
            with
                | IncorrectLineException (s, _) -> IncorrectLineException (s, i) |> raise
                | IncorrectCommandException (s, _, c) -> IncorrectCommandException (s, i, c) |> raise
            ()
        commands |> rotateArrayOfArrays |> Array.iteri iter

    member this.Clear () = grid.Clear ()

    member this.CheckCommand comm = checkCommand comm
        
    member this.CheckLine line =
        match checkLineTotal line with
        | [||] -> None
        | x -> Some (x)

    member this.CheckProgram (commands : Program<'T>) =
        match checkProgram commands with
        | [||] -> None
        | x -> Some (x)

    member this.Size with get () = grid.Size;
    member this.Width with get () = grid.Width;
    member this.Height with get () = grid.Height;