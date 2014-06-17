// Copyright (c) 2012, 2013 Semyon Grigorev <rsdpisuy@gmail.com>
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

module Brahma.FSharp.OpenCL.Translator.Type

open Brahma.FSharp.OpenCL.AST
open System.Reflection
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Quotations

let rec Translate (_type:System.Type) isKernelArg (collectedTypes:System.Collections.Generic.Dictionary<_,_>) size (context:TargetContext<_,_>) : Type<Lang> =
    let rec go (str:string) =
        let low = str.ToLowerInvariant()
        match low with
        | "int"| "int32" -> PrimitiveType<Lang>(Int) :> Type<Lang>
        | "int16" -> PrimitiveType<Lang>(Short) :> Type<Lang>
        | "uint16" -> PrimitiveType<Lang>(UShort) :> Type<Lang>
        | "uint32" -> PrimitiveType<Lang>(UInt) :> Type<Lang>
        | "float"| "float32" | "single"-> PrimitiveType<Lang>(Float) :> Type<Lang>
        | "byte" -> PrimitiveType<Lang>(UChar) :> Type<Lang>
        | "int64" -> PrimitiveType<Lang>(Long) :> Type<Lang>
        | "uint64" -> PrimitiveType<Lang>(ULong) :> Type<Lang>
        | "boolean" -> PrimitiveType<Lang>(Int) :> Type<Lang>
        | "double" -> 
            context.Flags.enableFP64 <- true
            PrimitiveType<Lang>(Double) :> Type<Lang>        
        | "unit" -> PrimitiveType<Lang>(Void) :> Type<Lang>
        | t when t.EndsWith "[]" ->
            let baseT = t.Substring(0,t.Length-2)
            if isKernelArg 
            then RefType<_>(go baseT) :> Type<Lang>
            else ArrayType<_>(go baseT, size |> Option.get) :> Type<Lang>
        | s when s.StartsWith "fsharpref" ->
            go (_type.GetGenericArguments().[0].Name)
        | f when f.StartsWith "fsharpfunc" ->
//            go (_type.GetGenericArguments().[1].Name)
            Translate (_type.GetGenericArguments().[1]) isKernelArg collectedTypes size context
        | x when collectedTypes.ContainsKey x 
            -> StructType(collectedTypes.[x]) :> Type<Lang>
        | x -> "Unsuported kernel type: " + x |> failwith 
    _type.Name
    |> go


let TransleteStructDecl collectedTypes (t:System.Type) targetContext =
    let name = t.Name
    let fields = [ for f in t.GetProperties (BindingFlags.Public ||| BindingFlags.Instance) ->
                    new StructField<_> (f.Name, Translate f.PropertyType true collectedTypes None targetContext)]
    new Struct<_>(name, fields)


open System.Collections.Generic

type Caller() =
    let letDicVars = new Dictionary<_,_>()
    let mutable lastLet = ""

    member this.NewLetDicVars nameLet = 
        letDicVars.Add(nameLet, new ResizeArray<_>())
        lastLet <- nameLet

    member this.AddVarInLastLet varName =
        let arrayNames = letDicVars.[lastLet]
        arrayNames.Add(varName)
    
    member this.AddNewLetVar nameLet varName =
        let arrayNames = letDicVars.[nameLet]
        arrayNames.Add(varName)

    member this.getListNamesForLet nameLet = 
        letDicVars.[nameLet]


//type CallRemember() =
//    let letDicVars = new Dictionary<_,_>()
//    let mutable lastLet = ""
//
//    member this.NewLetDicVars nameLet = 
//        letDicVars.Add(nameLet, new ResizeArray<_>())
//        lastLet <- nameLet
//
//    member this.AddVarInLastLet varName =
//        let arrayNames = letDicVars.[lastLet]
//        arrayNames.Add(varName)
//    
//    member this.AddNewLetVar nameLet varName =
//        let arrayNames = letDicVars.[nameLet]
//        arrayNames.Add(varName)
//
//    member this.getListNamesForLet nameLet = 
//        letDicVars.[nameLet]

type Renamer() =
    let allNames = new Dictionary<_,Stack<_>>()
    let mutable counter = 0

    let newName vName =
        if allNames.ContainsKey vName
        then 
            let name = vName + string counter
            counter <- counter + 1
            name
        else vName

    member this.addName name =
        if allNames.ContainsKey name
        then
            let nName = newName name
            let scope = allNames.[name]
            scope.Push nName
            nName
        else
            let scope = new Stack<_>(10)
            scope.Push name
            allNames.Add(name, scope)
            name

    member this.removeName name =
        let scope = allNames.[name]
        scope.Pop()

    member this.getName name =
        let scope = allNames.[name]
        scope.Peek()

[<AllowNullLiteral>]
type VarInfo(orName:string, nName:string, isVar, typeV:System.Type) =
    let origName = orName
    let newName = nName
    let isV = isVar
    let typeVar = typeV

    member this.GetOriginalName =
        origName
    member this.GetNewName =
        newName
    member this.IsVar =
        isV
    member this.GetVarType =
        typeVar

[<AllowNullLiteral>]
type InfoScope(orgnName, nName, originVar, isFunc, nameInFunc:Option<string>) as this =
    let listVars = new ResizeArray<VarInfo>() // var current scope
    let needVars = new ResizeArray<VarInfo>()    
    let mutable nameInFun = nameInFunc

    let FindInVars orName =        
        match listVars |> ResizeArray.tryFind (fun v -> v.GetOriginalName = orName) with 
        | Some x -> Some x
        | None -> needVars |> ResizeArray.tryFind (fun v -> v.GetOriginalName = orName)
        
    let FindInAfter orName =
        let mutable var = None
        let mutable curAfter = this.After
        let mutable needList = new ResizeArray<_>()
        while curAfter.IsSome do
            let ca = curAfter.Value
            if ca.OriginalName = orName
            then
                let varAfter = ca.OriginalVar
                var <- Some <| new VarInfo(ca.OriginalName, ca.NewName, false, varAfter.Type)
                needList.AddRange ca.GetNeedVars
                curAfter <- None
            else curAfter <- ca.After
        var, needList

    let GetNameForCurVar orgnName isAfter isAboveFun (allLets:Dictionary<_, InfoScope>) (globalVars:ResizeArray<VarInfo>) =
        let nextIsAboveFun = ((not isAboveFun) && nameInFun.IsSome && this.OriginalName = nameInFun.Value) || isAboveFun

        let nameFromMyVar =
            if not isAfter
            then FindInVars orgnName
            else None

        if nameFromMyVar.IsNone then
            if isAfter && this.OriginalName = orgnName then
                let nameFromMe = new VarInfo(this.OriginalName, this.NewName, false, this.OriginalVar.Type)
                Some nameFromMe, new ResizeArray<_>() //так как вызов сразу после, то тоже только искомое имя и необходимые переменные не нужны
            else
                let nameFromListAfter, listNeed = FindInAfter orgnName
                if nameFromListAfter.IsNone
                then                   
                    let nameFromInLet, listNeedV = 
                        match this.InLet with 
                        | Some inLet -> inLet.GetNameForVar orgnName false nextIsAboveFun allLets globalVars
                        | None -> 
                            globalVars |> ResizeArray.tryFind (fun v -> v.GetOriginalName = orgnName)
                            , new ResizeArray<_>()
                    //возможно лучше список нужных варов возвраать
                    if (not isAfter)  && nameInFun.IsSome
                    then
                        let inFunction:InfoScope = allLets.[nameInFun.Value]
                        let needVarsFunList:ResizeArray<VarInfo> = inFunction.GetNeedVars
                        let containsVars:ResizeArray<VarInfo> = inFunction.GetVars
                        match nameFromInLet with
                        | Some x 
                            when x.IsVar 
                                 && (not (needVarsFunList.Contains x))
                                 && not (containsVars.Contains x)
                            -> needVarsFunList.Add x
                        | _ -> ()

                        listNeedV
                        |> ResizeArray.filter (fun need -> (not (needVarsFunList.Contains need)) && not (containsVars.Contains need))
                        |> needVarsFunList.AddRange

                    nameFromInLet, listNeedV
                    
                elif not isAfter
                then
                    let x = nameFromListAfter.Value
                    if not (allLets.[x.GetNewName].IsFun) then
                        let newNeedList = new ResizeArray<_>()
                        if this.IsFun
                        then
                            let inFunction = allLets.[nameInFun.Value]
                            let needVarsFunList = inFunction.GetNeedVars
                            let containsVars = inFunction.GetVars
                            if (not (needVarsFunList.Contains x))
                               && not (containsVars.Contains x)
                            then needVarsFunList.Add x
                        newNeedList.Add x
                        nameFromListAfter, newNeedList
                    else 
                        if nameInFun.IsSome
                        then
                            let inFunction = allLets.[nameInFun.Value]
                            let needVarsFunList = inFunction.GetNeedVars
                            let containsVars = inFunction.GetVars
                            listNeed 
                            |> ResizeArray.filter (fun need -> (not (needVarsFunList.Contains need)) && not (containsVars.Contains need))
                            |> needVarsFunList.AddRange
                        nameFromListAfter, listNeed
                  else nameFromListAfter, new ResizeArray<_>()
        else nameFromMyVar, new ResizeArray<_>() //возврат искомой переменно и список нужных

    member this.AddVar oName nNane typeV =
        listVars.Add(new VarInfo(oName, nNane, true, typeV))
    
    member this.AddNeedVar oName nNane typeV =
        match nameInFun with
        | Some s when s = oName -> needVars.Add(new VarInfo(oName, nNane, true, typeV))
        | _ -> ()

    member this.GetVars = listVars
    member this.GetNeedVars = needVars

    member val OriginalVar:Var = originVar with get, set
    member val IsFun = isFunc with get, set
    member val After:Option<InfoScope> = None with get, set
    member val InLet:Option<InfoScope> = None with get, set
    member val OriginalName = orgnName with get, set
    member val NewName = nName with get, set

    member this.GetNameForVar orgnName isAfter isAboveFun allLets (globalVars:ResizeArray<VarInfo>) =
        GetNameForCurVar orgnName isAfter isAboveFun allLets globalVars

type LetScope() =
    let allLet = new Dictionary<_,_>()
    let lastInLet = new Stack<_>(10)
    let lastInFunLet = new Stack<_>(10)
    let mutable isInLastLet = true
    let kernelVars = new ResizeArray<VarInfo>()

    let FindInVars orName =
        kernelVars |> ResizeArray.tryFind (fun v -> v.GetOriginalName = orName)

    member this.SetIsInLastLet isIn = 
        isInLastLet <- isIn
    member this.GetIsInLastLet = isInLastLet

    member private this.AddLetInfo (infoScope:InfoScope) = 
        allLet.Add(infoScope.NewName, infoScope)
    member this.GetLetInfo name =
        allLet.[name]
    member this.ContainsInfo name =
        allLet.ContainsKey name

    member this.LetIn name newName originVar isFun nameInFun = 
        let newInfoLet = new InfoScope(name, newName, originVar, isFun, nameInFun)

        if isInLastLet
        then //если мы зашли в let после let
            if lastInLet.Count > 0
            then newInfoLet.InLet <- allLet.[lastInLet.Peek()] |> Some
        elif lastInLet.Count > 0
        then // если мы зашли лет после 
            let after = allLet.[lastInLet.Peek()]
            newInfoLet.After <- Some after
            newInfoLet.InLet <- after.InLet

        lastInLet.Push newName
        if newInfoLet.IsFun
        then lastInFunLet.Push newName
        this.AddLetInfo newInfoLet

    member this.GetLastInLet () =
        if lastInLet.Count = 0
        then None
        else lastInLet.Peek() |> Some

    member this.GetLastInFunLet () =
        if lastInFunLet.Count = 0
        then None
        else lastInFunLet.Peek() |> Some

    member this.FunLetOut = lastInFunLet.Pop
    member this.LetOut = lastInLet.Pop

    member this.GetNameForVarInLet name isAfter =
        if allLet.Count > 0
        then
            let infoLet = allLet.[(this.GetLastInLet()).Value]
            let getingName, listNeed = infoLet.GetNameForVar name isAfter false allLet kernelVars
            match getingName with
            | None -> 
                let v = (FindInVars name).Value
                infoLet.AddNeedVar v.GetOriginalName v.GetNewName v.GetVarType
                v
            | Some x -> x
        else kernelVars |> ResizeArray.find (fun v -> v.GetOriginalName = name)

    member this.AddVarInLastLet orgnName nName varType =
        let last = allLet.[(this.GetLastInLet()).Value]
        last.AddVar orgnName nName varType

    member this.AddKernelVars orgnName nName varType =
        kernelVars.Add(new VarInfo(orgnName, nName, true, varType))

//    member this.AddForVars (var:VarInfo) = 
//        forVars.Push(var)
//    member this.RemoveForVar =
//        forVars.Pop()
//    member this.FindVarInForVars name =
//        let mutable needVars = null
//        for elem in forVars do
//            if(elem.GetOriginalName = name) then
//                needVars <- elem
//        needVars


type Method(var:Var, expr:Expr) = 
    let funVar = var
    let funExpr = expr

    member this.FunVar:Var =
        funVar
    member this.FunExpr =
        funExpr
