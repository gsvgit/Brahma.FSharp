namespace Compiler

open System
open System.Text.RegularExpressions
open TTA.ASM

type RegexType =
    | Eps of Regex
    | Set of Regex
    | Mvc of Regex
    | Mov of Regex
    member this.Value =
        match this with
        | Eps r | Set r | Mvc r | Mov r -> r

type AsmRegexCompiler<'T> () =
    let regexs =
        [|
            Eps (new Regex ("^[ \t]*eps[ \t]*$", RegexOptions.IgnoreCase));
            Set (new Regex ("^[ \t]*set[ \t]+(0|[1-9][0-9]*)[ \t]+(0|[1-9][0-9]*)[ \t]+(.+)$", RegexOptions.IgnoreCase));
            Mvc (new Regex ("^[ \t]*mvc[ \t]+(0|[1-9][0-9]*)[ \t]+(0|[1-9][0-9]*)[ \t]+(.+)$", RegexOptions.IgnoreCase));
            Mov (new Regex ("^[ \t]*mov[ \t]+(0|[1-9][0-9]*)[ \t]+(0|[1-9][0-9]*)[ \t]+(0|[1-9][0-9]*)[ \t]+(0|[1-9][0-9]*)[ \t]*$", RegexOptions.IgnoreCase))
        |]

    let parseString (str : string) =
        if str.StartsWith "\"" && str.EndsWith "\""
        then Some (str.Substring (1, str.Length - 2) :> obj)
        else None

    let parseBool (str : string) =
        match str.ToLower () with
        | "true" -> Some (true :> obj)
        | "false" -> Some (false :> obj)
        | _ -> None

    let parseInt32 str =
        let res = ref 0
        if Int32.TryParse (str, res)
        then Some (!res :> obj)
        else None

    let parseLiteral (str : string) =
        let str = str.TrimEnd ([|' '; '\t'|])
        match typeof<'T> with
        | t when t = typeof<int> -> parseInt32 str
        | t when t = typeof<string> -> parseString str
        | t when t = typeof<bool> -> parseString str
        | _ -> failwith "Incorrect implementation" // Should not be raised if code is correct

    let matchRegex (r : RegexType) s =
        let m = r.Value.Match s
        if not m.Success
        then None
        else
            match r with
            | Eps _ -> Some Asm.Eps
            | Set _ ->
                let lit = parseLiteral m.Groups.[3].Value
                if lit.IsNone
                then None
                else 
                    let lit = lit.Value :?> 'T
                    let r = Int32.Parse m.Groups.[1].Value
                    let c = Int32.Parse m.Groups.[2].Value
                    Some (Asm.Set ((r*1<ln>, c*1<col>), lit))
            | Mvc _ ->
                let lit = parseLiteral m.Groups.[3].Value
                if lit.IsNone
                then None
                else 
                    let lit = lit.Value :?> 'T
                    let r = Int32.Parse m.Groups.[1].Value
                    let c = Int32.Parse m.Groups.[2].Value
                    Some (Asm.Mvc ((r*1<ln>, c*1<col>), lit))
            | Mov _ ->
                let r1 = Int32.Parse m.Groups.[1].Value
                let c1 = Int32.Parse m.Groups.[2].Value
                let r2 = Int32.Parse m.Groups.[3].Value
                let c2 = Int32.Parse m.Groups.[4].Value
                Some (Asm.Mov ((r1*1<ln>, c1*1<col>), (r2*1<ln>, c2*1<col>)))

    let parseCommand str =
        let ms = regexs |> Array.map (fun x -> matchRegex x str) |> Array.filter (fun x -> x.IsSome)
        if ms.Length = 0
        then None
        else ms.[0]
        
    let compile data =
        data |> Array.map (fun e -> e |> Array.map (fun el -> parseCommand el))

//    interface IAsmCompiler<'T> with
//        member this.Compile data =
//            compile data

    member this.Compile data =
        compile data

