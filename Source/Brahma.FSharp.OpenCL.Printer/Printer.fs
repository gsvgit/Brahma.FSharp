module Brahma.FSharp.OpenCL.Printer.AST

open Brahma.FSharp.OpenCL.AST
open Microsoft.FSharp.Text.StructuredFormat
open Microsoft.FSharp.Text.StructuredFormat.LayoutOps
open Brahma.FSharp.OpenCL.Printer

let Print (ast:AST<'lang>) =
    let layout = 
        ast.TopDefs 
        |> List.map (fun d -> match d with | :? FunDecl<'lang> as fd -> FunDecl.Print fd | _ -> failwithf "Printer. Unsupported toplevel declaration: %A"  d)
        |> aboveListL
    StructuredFormat.Display.layout_to_string 
      {StructuredFormat.FormatOptions.Default with PrintWidth=100}
      layout
