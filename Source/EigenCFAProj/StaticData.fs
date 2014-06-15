module EigenCFA.StaticData

open System.IO
open System.Diagnostics

let printfStore (store:int[]) row column = 
    for i in 0..(row - 1) do
        printf "%d. " i
        for j in 0..(column - 1) do
            printf "%d " store.[i*column + j]
        printf "\n"

let readMapInt (path:string) =
    let lines = File.ReadAllLines path
    lines.[0].Split() |> Array.map int

let readMapFloat (path:string) =
    let lines = File.ReadAllLines path
    lines.[0].Split() |> Array.map float

let nameFileFun = "Fun.txt"
let nameFileArg1 = "Arg1.txt"
let nameFileArg2 = "Arg2.txt"
let nameFileScales = "scales.txt"

let mutable pathToFun = ""
let mutable pathToArg1 = ""
let mutable pathToArg2 = ""
let mutable pathToScales = ""

//initialization all scales

//let scales = readMapInt pathToScales
let mutable scaleCall = -1//scales.[0]
let mutable scaleLam = -1//scales.[1]
let mutable scaleVar = -1//scales.[2]
let mutable scaleExp = -1//scaleLam + scaleVar
let mutable scaleM = -1//(int ((float (scaleLam)/100.0))) + 2

//printf "scaleM = %d" scaleM

let initArrayGpu (path:string) = 
    let arrayReading:int[] = path |> readMapInt
    let gpuArray = Array.init scaleCall (fun index -> arrayReading.[index])
    gpuArray

//Static matrix
//let Fun  = initArrayGpu pathToFun
//let Arg1 = initArrayGpu pathToArg1
//let Arg2 = initArrayGpu pathToArg2

//initialization Store on CPU
//let initStore = Array2D.init scaleExp scaleM (fun i j -> if i < scaleVar 
//                                                            then (if j < 1 
//                                                                  then 1.0 
//                                                                  else -1.0) 
//                                                            else (if j < 1 
//                                                                  then 2.0 
//                                                                  else (if j = 1 
//                                                                        then ((float i) - (float scaleVar) + 1.0) 
//                                                                        else -1.0)))
//copy Store on GPGPU
//let Store = initStore

let dirs path = Directory.GetDirectories(path)

let isFileContains dir pattern = Directory.GetFiles(dir, pattern).Length > 0
let isAllFilesContains subdir = (isFileContains subdir "Fun.txt" &&
                                    isFileContains subdir "Arg1.txt" &&
                                    isFileContains subdir "Arg2.txt" &&
                                    isFileContains subdir "scales.txt")

//let mutable pathTests = ""//@"F:\learn\learn\6_sem\Kursach\MyDevCode\gpu-sa\SmartGenerator\SmartGenerator\bin\Debug\Tests\SimpleTests"
let mutable pathToResFile = ""//@"F:\learn\learn\6_sem\Kursach\MyDevCode\ResTests"

let writeResTestInFile pathFolder nameFile time numTerm  numE = 
                                                                let fullPath = System.IO.Path.Combine(pathFolder, nameFile)
                                                                use wr = StreamWriter(fullPath, true)
                                                                wr.WriteLine((numTerm.ToString()) + " " + (time/1000.0).ToString().Replace(",","."))// + " " + numE.ToString())
                                                                wr.Close

                            



