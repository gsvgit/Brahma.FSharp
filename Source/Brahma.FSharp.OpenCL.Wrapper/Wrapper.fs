module Brahma.FSharp.OpenCL.Wrapper

open Brahma.OpenCL
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL
open OpenCL.Net
open System

type CLCodeGenerator with
    static member GenerateKernel(lambda: Expr, provider: ComputeProvider, kernel:ICLKernel) =        
        let codeGenerator = new Translator.FSQuotationToOpenCLTranslator()
        kernel.Source.Append(codeGenerator.Translate(lambda))
        //kernel.SetClosures(codeGenerator.Closures);
        //kernel.SetParameters(lambda.Parameters);
        
type ComputeProvider with
    member this.CompileQuery<'T,'TRange, 'T1 
                                        when 'T :> ICLKernel
                                        and 'T1 :> Brahma.IMem 
                                        and 'TRange : struct 
                                        and 'TRange :> Brahma.OpenCL.INDRangeDimension
                                        and 'TRange :> ValueType 
                                        and 'TRange : (new: unit -> 'TRange)>(lambda:Expr) =
        let kernel = System.Activator.CreateInstance<'T>()
        let r = CLCodeGenerator.GenerateKernel(lambda, this, kernel)            
        let program, error = Cl.CreateProgramWithSource(this.Context, 1u, [|(kernel :> ICLKernel).Source.ToString()|], null)
        let _devices = Array.ofSeq  this.Devices
        let error = Cl.BuildProgram(program, _devices.Length |> uint32, _devices, this.CompileOptionsStr, null, IntPtr.Zero);
        if error <> Cl.ErrorCode.Success
        then 
            _devices |> Array.map (fun device -> Cl.GetProgramBuildInfo(program, device, Cl.ProgramBuildInfo.Log ) |> fst |> string)
            |> String.concat "\n"
            |> failwith
        let clKernel,errpr = Cl.CreateKernel(program, CLCodeGenerator.KernelName)
        (kernel :> ICLKernel).ClKernel <- clKernel
            
        kernel            
                    
    member this.Compile<'TRange,'T1 when 'T1 :> Brahma.IMem                                      
                                    and 'TRange : struct 
                                    and 'TRange :> Brahma.OpenCL.INDRangeDimension
                                    and 'TRange :> ValueType 
                                    and 'TRange : (new: unit -> 'TRange)>
            (query: Expr, ?_options:CompileOptions) =
        let options = defaultArg _options this.DefaultOptions_p
        this.SetCompileOptions(options)        
        this.CompileQuery<Kernel<'TRange, 'T1>,'TRange,'T1>(query)
    