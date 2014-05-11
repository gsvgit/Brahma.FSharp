module test

open System.Windows.Forms.DataVisualization.Charting
open System.Drawing
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System
open System.Windows.Forms

open fr

let time f =
    let start = System.DateTime.Now
    f() |> ignore
    (System.DateTime.Now - start).TotalMilliseconds

let test1 (cx: array<_>) scaling size mx my cr ci boxwidth boxheight = fr.juliacpu cx scaling size mx my cr ci boxwidth boxheight  
let test2 (cx: array<_>) scaling size mx my cr ci boxwidth boxheight = fr.Juliagpu cx scaling size mx my cr ci boxwidth boxheight  
let test3 (cx:array<_>) scaling size mx my boxwidth boxheight = fr.mandelbrotcpu cx scaling size mx my boxwidth boxheight
let test4 (cx:array<_>) scaling size mx my boxwidth boxheight = fr.Mandelbrotgpu cx scaling size mx my boxwidth boxheight


let p1 = Array.zeroCreate 21
let p2 = Array.zeroCreate 21
let p3 = Array.zeroCreate 21
let p4 = Array.zeroCreate 21

let m = ref 0.0
let n = ref -1.0
let arr = [|for i in -10 .. 10 -> n := !n + 0.08; !n|]
let cx = Array.zeroCreate 160000

let fill () = 
    for i in 0 .. 20 do
        let f1 () = test1 cx 0.7 100.0 -1.5 -1.5 arr.[i] arr.[i] 400 400
        let f2 () = test2 cx 0.7 100.0 -1.5 -1.5 arr.[i] arr.[i] 400 400
        p1.[i] <-  (i, time f1)
        p2.[i] <-  (i, time f2)



let fill2 () = 
    for i in 0 .. 20  do
        m := float (i + 50) 
        let f3 () = test3 cx 0.5 !m -1.5 -1.0 400 400
        let f4 () = test4 cx 0.5 !m -1.5 -1.0 400 400 
        p3.[i] <-  (i, time f3)
        p4.[i] <-  (i, time f4)
        
            
//fill()
fill2()
printfn "%A" arr

[<STAThread; EntryPoint>]
let main args =
    let c1 = p1 |> Chart.Line |> Chart.WithLegend(Title = "1 - cpu, 2 - gpu") 
    let c2 = p2 |> Chart.Line |> Chart.WithLegend(Title = "1 - cpu, 2 - gpu") 
    let c3 = p3 |> Chart.Line |> Chart.WithLegend(Title = "1 - cpu, 2 - gpu") 
    let c4 = p4 |> Chart.Line |> Chart.WithLegend(Title = "1 - cpu, 2 - gpu")
    let myChart = [c1; c2] |> Chart.Combine  
    let myChart2 = [c3; c4] |> Chart.Combine         
    //let myChartControl = new ChartControl(myChart, Dock=DockStyle.Fill) 
    let myChartControl = new ChartControl(myChart2, Dock=DockStyle.Fill)
    let lbl = new Label(Text = "JULIA")
    let lbl2 = new Label(Text = "MANDELBROT")
    let form = new Form(Visible = true, TopMost = true, Width = 1000, Height = 800)
    //form.Controls.Add lbl
    form.Controls.Add lbl2
    form.Controls.Add(myChartControl)
    do Application.Run(form) |> ignore
    0