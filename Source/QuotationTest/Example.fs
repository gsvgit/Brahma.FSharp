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
    let l = 10000
    let a = [|1..l|]
    let aBuf = new Buffer<int>(provider, Operations.ReadWrite, Memory.Device,a)
    let command = 
        <@ 
            fun (range:_1D) (buf1:Buffer<int>) -> 
                let x = range.GlobalID0                
                buf1.[x] <- buf1.[x] * buf1.[x]
        @>

    let c = command:>Expr
    let kernel = provider.Compile<_1D,_>(c)
    let cq = commandQueue.Add(kernel.Run(new _1D(l,1), aBuf)).Finish()
    let r = Array.zeroCreate(l)
    let cq2 = commandQueue.Add(aBuf.Read(0, l, r)).Finish()
    ()

do main ()