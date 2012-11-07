open Brahma.Types
open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Wrapper

open Microsoft.FSharp.Quotations

let deviceType = Cl.DeviceType.Default
let platformName = "*"

let provider =
    try  ComputeProvider.Create(platformName, deviceType)
    with 
    | ex -> failwith ex.Message
    
printfn "Using %A" provider

let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head );

let main () = 
    let command = 
        <@ 
            fun (range:NDRange<_2D>) (buf1:Buffer<float32>) -> 
                let r = [for r in range -> r]
                ()
        @>

    let c = command:>Expr
    let kwernel = provider.Compile<_2D,_>(c)
    ()

do main ()