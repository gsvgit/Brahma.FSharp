module TTA.Translator

let asmToIL (prog:TTA.ASM.Program<'a>) =
    if prog |> Array.length > 1
    then failwith "Please, use sequential asm. Parallel asm is not supported."
    elif prog.Length = 0
    then [|Seq.empty|]
    else
        let x = 
            prog.[0]
            |> Array.map
                (function
                 | TTA.ASM.Set ((l, c), v)          -> NA.IL.Set (NA.IL.Const v, NA.IL.OpBlock (int c, int l))
                 | TTA.ASM.Eps                      -> NA.IL.Eps
                 | TTA.ASM.Mov ((l1, c1), (l2, c2)) -> NA.IL.Move (NA.IL.OpBlock (int c2, int l2), NA.IL.OpBlock (int c1, int l1))
                 | TTA.ASM.Mvc ((l1, c1), v)        -> NA.IL.Move (NA.IL.Const v, NA.IL.OpBlock (int c1, int l1)))

        [|
            x |> Seq.cast<NA.IL.IInstruction>
            Seq.init x.Length (fun _ -> NA.IL.Eps :> NA.IL.Instruction<int> :> NA.IL.IInstruction)
            Seq.init x.Length (fun _ -> NA.IL.Eps :> NA.IL.Instruction<int> :> NA.IL.IInstruction)
            Seq.init x.Length (fun _ -> NA.IL.Eps :> NA.IL.Instruction<int> :> NA.IL.IInstruction)|]