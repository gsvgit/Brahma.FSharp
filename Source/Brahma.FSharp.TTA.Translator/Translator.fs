namespace Brahma.FSharp.TTA.Translator


type Translator() = 
    let translate (oclAst:Brahma.FSharp.OpenCL.AST.AST<_>) =
        oclAst.TopDefs
        |> List.map (fun td -> td.Children)
    member this.X = "F#"
