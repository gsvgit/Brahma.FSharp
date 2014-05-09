module test

open System.Windows.Forms.DataVisualization.Charting
open System.Drawing
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System
open System.Windows.Forms

open allfractals

let time f =
    let start = System.DateTime.Now
    f() |> ignore
    (System.DateTime.Now - start).TotalMilliseconds

let test1 scaling size mx my cr ci = allfractals.Juliacpu scaling size mx my cr ci  
let test2 scaling size mx my cr ci = allfractals.Juliagpu scaling size mx my cr ci   
let test3 scaling size mx my = allfractals.Mandelbrotcpu scaling size mx my 
let test4 scaling size mx my = allfractals.Mandelbrotgpu scaling size mx my 


let m = ref -1.0
let arr = [|for i in -10 .. 10 -> m := !m + 0.08; !m|]
let p1  = Array.zeroCreate arr.Length
let p2  = Array.zeroCreate arr.Length
let p3  = Array.zeroCreate 2
let p4  = Array.zeroCreate 2

let fill () = 
    for i in 0 .. arr.Length - 1 do
            let f1 () = test1 0.7 100.0 -1.5 -1.5 arr.[i] arr.[i] 
            let f2 () = test2 0.7 100.0 -1.5 -1.5 arr.[i] arr.[i] //0.75 100.0 -1.5 -1.0 -0.75 0.17 
            p1.[i] <-  (i, time f1)
            p2.[i] <-  (i, time f2)


let fill2 () = 
    
    for i in 0 .. 1  do
            
            let f3 () = test1 0.7 0.5 100.0 -1.5 -1.0 
            let f4 () = test1 0.7 0.5 100.0 -1.5 -1.0
            p3.[i] <- (i, time f3)
            p4.[i] <- (i, time f4)
            
//fill()
fill2 ()

[<STAThread; EntryPoint>]
let main args =
    let c1 = p1 |> Chart.Line |> Chart.WithYAxis(Title = "cpu") 
    let c2 = p2 |> Chart.Line |> Chart.WithYAxis(Title = "gpu") 
    let c3 = p3 |> Chart.Line |> Chart.WithYAxis(Title = "cpu")  
    let c4 = p4 |> Chart.Line |> Chart.WithYAxis(Title = "gpu")
    let myChart = [c1; c2] |> Chart.Combine  
    let myChart2 = [c3; c4] |> Chart.Combine         
    //let myChartControl = new ChartControl(myChart, Dock=DockStyle.Fill) 
    let myChartControl = new ChartControl(myChart2, Dock=DockStyle.Fill)
    let lbl = new Label(Text = "test")
    let form = new Form(Visible = true, TopMost = true, Width = 1000, Height = 800)
    form.Controls.Add lbl
    form.Controls.Add(myChartControl)
    do Application.Run(form) |> ignore
    0