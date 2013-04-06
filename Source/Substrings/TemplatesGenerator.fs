module TemplatesGenerator

open System.IO
open System.Runtime.Serialization.Formatters.Binary

type Templates = {
    mutable number : int
    mutable sizes : byte[]
    mutable content : byte[]
    }

let random = new System.Random()

let computeTemplateLengths templates maxTemplateLength minLength =
    Array.sort (Array.init templates (fun _ -> (byte) (random.Next((int) maxTemplateLength - (int) minLength) + (int) minLength)))

let computeTemplatesSum templates (templateLengths:array<byte>) =
    let mutable l = 0
    for i in 0..(templates - 1) do
        l <- l + (int) templateLengths.[i]
    l 

let generateTemplates templatesSum = Array.init templatesSum (fun _ -> (byte) (random.Next(255)))

let path = "../../templates.txt"

let Main =
    let mutable generated = 0L
    let mutable i = 0

    let maxTemplateLength = 32uy
    let minTemplateLength = 1uy
    let templatesNumber = 512

    let lengths = computeTemplateLengths templatesNumber maxTemplateLength minTemplateLength
    let sum = computeTemplatesSum templatesNumber lengths
    let templateBytes = generateTemplates sum

    let templates = { number = templatesNumber; sizes = lengths; content = templateBytes;}

    let writer = new FileStream(path, FileMode.Create)
    let formatter = new BinaryFormatter()

    formatter.Serialize(writer, templates)

    writer.Close

//do Main()