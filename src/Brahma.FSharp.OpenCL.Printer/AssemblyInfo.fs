namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Brahma.FSharp.OpenCL.Printer")>]
[<assembly: AssemblyProductAttribute("Brahma.FSharp")>]
[<assembly: AssemblyDescriptionAttribute("F# quotation to OpenCL translator.")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
    let [<Literal>] InformationalVersion = "1.0"
