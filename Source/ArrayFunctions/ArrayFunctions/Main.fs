module Main

open Filter

printfn "%A" (arrayFilter (fun x -> x.Length > 5) [|"mrmrm"; "qwerty"; "ad"|])