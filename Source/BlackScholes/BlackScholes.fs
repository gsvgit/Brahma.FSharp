// Copyright (c) 2013 Semyon Grigorev <rsdpisuy@gmail.com>
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

module BlackScholes

open Brahma.Samples
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eps = 0.001f;
        
let riskFreeInterestRate = <@0.02f@>
let volatility = <@0.30f@>

let RiskFreeInterestRate = riskFreeInterestRate.Compile()()
let Volatility = volatility.Compile()()


let A1 = <@0.31938153f@>
let A2 = <@ -0.356563782f@>
let A3 = <@1.781477937f@>
let A4 = <@ -1.821255978f@>
let A5 = <@1.330274429f@>

let cumulativeNormalDistribution = 
    <@ fun x ->
        let l = abs x
        let k = 1.0f / (1.0f + 0.2316419f * l)
        let cnd =
            1.0f - 1.0f / sqrt(2.0f * float32 System.Math.PI)
            * exp (-l * l / 2.0f) * (%A1 * k + %A2 * k * k + %A3 * (pown k 3)
            + %A4 * (pown k 4) + %A5 * (pown k 5))
        if x < 0.0f then 1.0f - cnd else cnd
    @>
    
let d1 =
    <@
        fun stockPrice strikePrice timeToExpirationYears riskFreeInterestRate volatility ->
            log(stockPrice / strikePrice) + (riskFreeInterestRate + volatility * volatility / 2.0f) * timeToExpirationYears
              / (volatility * sqrt timeToExpirationYears)
    @>

let D1 = d1.Compile()()

let d2 =
    <@
        fun d1 timeToExpirationYears volatility ->
            d1 - volatility * sqrt timeToExpirationYears
    @>

let D2 = d2.Compile()()

let blackScholesCallOption =
    <@
        fun d1 d2 stockPrice strikePrice timeToExpirationYears riskFreeInterestRate volatility ->
            stockPrice * (%cumulativeNormalDistribution) d1 -
            strikePrice * exp(-riskFreeInterestRate * timeToExpirationYears) * (%cumulativeNormalDistribution) d2
    @>

let BlackScholesCallOption = blackScholesCallOption.Compile()()

let blackScholesPutOption = 
    <@
        fun d1 d2 stockPrice strikePrice timeToExpirationYears riskFreeInterestRate volatility ->
            strikePrice * exp(-riskFreeInterestRate * timeToExpirationYears) * (%cumulativeNormalDistribution)(-d2) - stockPrice * (%cumulativeNormalDistribution)(-d1)        
    @>

let BlackScholesPutOption = blackScholesPutOption.Compile()()
    
let Main() =
    let platformName = "*"
    let iterations = 16
    let optionCount = 4000000
    let localWorkSize = 32
    let deviceType = Cl.DeviceType.Default

    
    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    printfn "Using %A" provider

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)

    let random = new System.Random(2009)

    let putCpu = Array.init  optionCount (fun _ -> -1.0f)
    let callCpu = Array.init  optionCount (fun _ -> -1.0f)
            
    let putParallel = Array.copy putCpu
    let callParallel = Array.copy callCpu

    let stockPrices = Array.init optionCount (fun _ -> random.Random(5.0f, 30.0f))
    let strikePrices = Array.init optionCount (fun _ -> random.Random(1.0f, 100.0f))
    let timesToExpiration = Array.init optionCount (fun _ -> random.Random(0.25f, 10.0f))
    
    printfn "Running %A iterations on %A options using .NET..." iterations optionCount

//    for iteration in 0..iterations do
//        Timer<string>.Global.Start()
//        for i in 0..optionCount do
//            let d1 = D1 stockPrices.[i] strikePrices.[i] timesToExpiration.[i] RiskFreeInterestRate Volatility
//            let d2 = D2 d1 RiskFreeInterestRate Volatility
//            putCpu.[i] <- BlackScholesPutOption d1 d2 stockPrices.[i] strikePrices.[i] timesToExpiration.[i]
//                                                RiskFreeInterestRate Volatility
//            callCpu.[i] <- BlackScholesCallOption d1 d2 stockPrices.[i] strikePrices.[i] timesToExpiration.[i]
//                                                RiskFreeInterestRate Volatility
//
//        Timer<string>.Global.Lap(".NET", true)    
            
    printfn "done"

    printfn "Compiling kernel(s)..."

    let blackScholes = 
        <@ fun (rng:_1D) (stocks:array<_>) (strikes:array<_>) (times:array<_>) (call:array<_>) (put:array<_>) ->            
            let index = rng.GlobalID0            

            let d1 = (%d1) stockPrices.[index] strikePrices.[index] timesToExpiration.[index] times.[index] %volatility
            let d2 = (%d2) d1 times.[index] %volatility
            
            call.[index] <- (%blackScholesCallOption) d1 d2 stocks.[index] strikes.[index] times.[index] %riskFreeInterestRate %volatility
            put.[index] <- (%blackScholesPutOption) d1 d2 stocks.[index] strikes.[index] times.[index] %riskFreeInterestRate %volatility
         @>

    printfn "done."

    printfn "Running %A iterations on %A options using OpenCL..." iterations optionCount

    let c =  string blackScholes

    let kernel, kernelPrepare, kernelRun = provider.Compile blackScholes

    Timer<string>.Global.Start(); // We're going to time the OpenCL version a little differently
    kernelPrepare (new _1D(optionCount, localWorkSize)) stockPrices strikePrices timesToExpiration callParallel putParallel
    for iteration in 0..iterations do    
        commandQueue.Add(kernelRun()).Finish() |> ignore

    commandQueue.Finish() |> ignore
    Timer<string>.Global.Lap("OpenCL", true);

    printfn "done"

    printfn "Verifying results..."
    commandQueue
        .Add(putParallel.ToHost provider)
        .Add(callParallel.ToHost provider)
        .Finish()
        |> ignore

    let mutable isSuccess1 = true
    let mutable isSuccess2 = true

    for i in 0..optionCount do
        if isSuccess1 && abs (putCpu.[i] - putParallel.[i]) > Eps
        then
            isSuccess1 <- false
            printfn "Expected: %A Actual: %A Error = %A" putCpu.[i] putParallel.[i] (abs putCpu.[i] - putParallel.[i])
        if isSuccess2 && abs (callCpu.[i] - callParallel.[i]) > Eps
            then
                isSuccess1 <- false
                printfn "Expected: %A Actual: %A Error = %A" callCpu.[i] callParallel.[i] (abs callCpu.[i] - callParallel.[i])
                            
    printfn "done."
    
    Timer<string>.Global.Average(".NET") |> printfn "Avg. time, C#: %A"
    Timer<string>.Global.Total("OpenCL") / float iterations |> printfn "Avg. time, OpenCL: %A"

do Main()