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
open Microsoft.FSharp.Quotations

let Translate (_type:System.Type) isKernelArg (collectedTypes:System.Collections.Generic.Dictionary<_,_>) size (context:TargetContext<_,_>) : Type<Lang> =
    let rec go (str:string) =
        match str.ToLowerInvariant() with
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
        | t when t.EndsWith "[]" ->
            let baseT = t.Substring(0,t.Length-2)
            if isKernelArg 
            then RefType<_>(go baseT) :> Type<Lang>
            else ArrayType<_>(go baseT, size |> Option.get) :> Type<Lang>
        | s when s.StartsWith "fsharpref" ->
            go (_type.GetGenericArguments().[0].Name)
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


// Copyright (c) 2014 Klimov Ivan <ivan.klimov.92@gmail.com>
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
//    let setNames = Set.empty
    let mutable counter = 0

    let newName vName = 
//        setNames.Add(vName) |> ignore
        if allNames.ContainsKey vName
        then 
            let mutable name = vName + string counter
            counter <- counter + 1
//            while(setNames.Contains(name)) do
//                name <- vName + string counter
//                counter <- counter + 1
//            setNames.Add(name) |> ignore
            name
        else
            vName

    member this.addName name =
        if allNames.ContainsKey name
        then
            let nName = newName name
            let scope = allNames.[name]
            scope.Push(nName)
            nName
        else
            let scope = new Stack<_>(10)
            scope.Push(name)
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
    let mutable origName = orName
    let mutable newName = nName
    let mutable isV = isVar
    let mutable typeVar = typeV

    member this.GetOriginalName =
        origName
    member this.GetNewName =
        newName
    member this.IsVar =
        isV
    member this.GetVarType =
        typeVar

[<AllowNullLiteral>]
type InfoScope(orgnName, nName, originVar, isFunc:bool, nameInFunc) =
    let listVars = new ResizeArray<VarInfo>() // var current scope
    let needVars = new ResizeArray<VarInfo>()
    let mutable after:InfoScope = null//new ResizeArray<_>() // after some let
    let mutable inLet:InfoScope = null//new ResizeArray<_>() // in some lets
    let mutable originalName = orgnName
    let mutable newName = nName
    let mutable orgnVar:Var = originVar
    let mutable isFun = isFunc
    let mutable nameInFun = nameInFunc

    let FindInVars (orName:string) =
        let mutable var = null
        //listVars.ForEach( fun (x:VarInfo) -> if(x.GetOriginalName = orName) then if(var = null) then var <- x)
        for v in listVars do
            if(v.GetOriginalName = orName) then 
                if(var = null) then
                    var <- v
        if(var = null) then
            for v in needVars do
                if(v.GetOriginalName = orName) then 
                    if(var = null) then
                        var <- v
        var
    
    let FindInAfter orName =
        let mutable var = null
        let mutable curAfter:InfoScope = after
        let mutable needList = new ResizeArray<_>()
        while(curAfter <> null) do
            if(curAfter.GetOriginalName = orName) then
                let varAfter:Var = curAfter.GetOrgnVar
                var <- new VarInfo(curAfter.GetOriginalName, curAfter.GetNewName, false, varAfter.Type)
                needList.AddRange(curAfter.GetNeedVars)
                curAfter <- null
            else
                curAfter <- curAfter.GetAfter
        var, needList

    let rec GetNameForCurVar orgnName isAfter (isAboveFun:bool) (allLets:Dictionary<string, InfoScope>) (globalVars:ResizeArray<VarInfo>) =
        let nextIsAboveFun = 
            if(not isAboveFun) then
                if(originalName = nameInFun) then
                    true
                else isAboveFun
            else isAboveFun
        let nameFromMyVar =
            if(not isAfter) then
                FindInVars orgnName
            else
                null
        if(nameFromMyVar = null) then
            if(isAfter && originalName = orgnName) then
                let nameFromMe = new VarInfo(originalName, newName, false, orgnVar.Type)
                nameFromMe, new ResizeArray<_>() //так как вызов сразу после, то тоже только искомое имя и необходимые переменные не нужны
            else
                let nameFromListAfter, listNeed = FindInAfter orgnName
                if(nameFromListAfter = null) then
                    if(inLet <> null) then
                        let (nameFromInLet:VarInfo), listNeedV = inLet.GetNameForVar orgnName false nextIsAboveFun allLets globalVars
                        //возможно лучше список нужных варов возвраать
                        if((not isAfter)) then
                            let inFunction:InfoScope = allLets.[nameInFun]
                            let needVarsFunList:ResizeArray<VarInfo> = inFunction.GetNeedVars
                            let containsVars:ResizeArray<VarInfo> = inFunction.GetVars
                            if(nameFromInLet.IsVar) then
                                if((not (needVarsFunList.Contains(nameFromInLet))) &&
                                     (not (containsVars.Contains(nameFromInLet)))) then
                                    needVarsFunList.Add(nameFromInLet)
                            for need in listNeedV do
                                if((not (needVarsFunList.Contains(need))) &&
                                    (not (containsVars.Contains(need)))) then
                                    needVarsFunList.Add(need)

                        nameFromInLet, listNeedV
                    else
                        let mutable var = null
                        for v in globalVars do
                            if(v.GetOriginalName = orgnName) then 
                                if(var = null) then
                                    var <- v
                        if(nameInFun <> null) then
                            let inFunction:InfoScope = allLets.[nameInFun]
                            let needVarsFunList:ResizeArray<VarInfo> = inFunction.GetNeedVars
                            let containsVars:ResizeArray<VarInfo> = inFunction.GetVars
                            if((not (needVarsFunList.Contains(var))) &&
                                        (not (containsVars.Contains(var)))) then
                                    needVarsFunList.Add(var)
                        var, new ResizeArray<_>()
                else
                    if(not isAfter) then
                        if(not (allLets.[nameFromListAfter.GetNewName].IsFun)) then
                            let newNeedList = new ResizeArray<_>()
                            if(isFun) then
                                let inFunction:InfoScope = allLets.[nameInFun]
                                let needVarsFunList:ResizeArray<VarInfo> = inFunction.GetNeedVars
                                let containsVars:ResizeArray<VarInfo> = inFunction.GetVars
                                if((not (needVarsFunList.Contains(nameFromListAfter))) &&
                                    (not (containsVars.Contains(nameFromListAfter)))) then
                                    needVarsFunList.Add(nameFromListAfter)
                            newNeedList.Add(nameFromListAfter)
                            nameFromListAfter, newNeedList
                        else 
                            let inFunction:InfoScope = allLets.[nameInFun]
                            let needVarsFunList:ResizeArray<VarInfo> = inFunction.GetNeedVars
                            let containsVars:ResizeArray<VarInfo> = inFunction.GetVars
                            for need in listNeed do
                                if((not (needVarsFunList.Contains(need))) &&
                                    (not (containsVars.Contains(need)))) then
                                    needVarsFunList.Add(need)
                            nameFromListAfter, listNeed
                    else
                        nameFromListAfter, new ResizeArray<_>()
        else
            nameFromMyVar, new ResizeArray<_>() //возврат искомой переменно и список нужных

    member this.SetIsFun isF =
        isFun <- isF
    member this.IsFun =
        isFun

    member this.ChangeOrgnVar (newOrgnVar:Var) =
        orgnVar <- newOrgnVar
    member this.GetOrgnVar =
        orgnVar

    member this.AddVar oName nNane typeV =
        listVars.Add(new VarInfo(oName, nNane, true, typeV))
    member this.GetVars =
        listVars

    member this.AddNeedVar oName nNane typeV =
        needVars.Add(new VarInfo(oName, nNane, true, typeV))
    member this.GetNeedVars =
        needVars

    member this.AddAfter afterScope =
        after <- afterScope
    member this.GetAfter =
        after

    member this.AddInLet letInScope =
        inLet <- letInScope
    member this.GetInLet = 
        inLet

    member this.SetOriginalName orName = 
        originalName <- orName
    member this.GetOriginalName = 
        originalName

    member this.SetNewName nName =
        newName <- nName
    member this.GetNewName =
        newName

    member this.GetNameForVar orgnName isAfter isAboveFun allLets (globalVars:ResizeArray<VarInfo>) =
        GetNameForCurVar orgnName isAfter isAboveFun allLets globalVars
       
        

type LetScope() =
    let allLet = new Dictionary<_,_>()
    let lastInLet = new Stack<_>(10)
    let lastInFunLet = new Stack<_>(10)
    let mutable isInLastLet = true
    let kernelVars = new ResizeArray<VarInfo>()

    let FindInVars (orName:string) =
        let mutable var = null
        //listVars.ForEach( fun (x:VarInfo) -> if(x.GetOriginalName = orName) then if(var = null) then var <- x)
        for v in kernelVars do
            if(v.GetOriginalName = orName) then 
                if(var = null) then
                    var <- v
        var

    member this.SetIsInLastLet isIn = 
        isInLastLet <- isIn
    member this.GetIsInLastLet = 
        isInLastLet

    member private this.AddLetInfo (infoScope:InfoScope) = 
        allLet.Add(infoScope.GetNewName, infoScope)
    member this.GetLetInfo name =
        allLet.[name]
    member this.ContainsInfo name =
        allLet.ContainsKey(name)

    member this.LetIn name newName originVar isFun nameInFun = 
        let newInfoLet = new InfoScope(name, newName, originVar, isFun, nameInFun)

        if(isInLastLet) then //если мы зашли в let после let
            if(lastInLet.Count > 0) then
                newInfoLet.AddInLet (allLet.[lastInLet.Peek()])
        else 
            if(lastInLet.Count > 0) then // если мы зашли лет после 
                let after = (allLet.[lastInLet.Peek()])
                newInfoLet.AddAfter after
                newInfoLet.AddInLet after.GetInLet

        lastInLet.Push(newName)
        if(newInfoLet.IsFun) then
            lastInFunLet.Push(newName)
        this.AddLetInfo newInfoLet

    member this.GetLastInLet =
        if(lastInLet.Count = 0) then
            null
        else
            lastInLet.Peek()
    member this.GetLastInFunLet =
        if(lastInFunLet.Count = 0) then
            null
        else
            lastInFunLet.Peek()
    member this.FunLetOut =
        lastInFunLet.Pop()

    member this.LetOut =
        lastInLet.Pop()

    member this.GetNameForVarInLet name isAfter =
        let infoLet = allLet.[this.GetLastInLet]
        //тут проверку на пустоту стека funLastLet
        //смотреть внутри это функции или нет
        let getingName, listNeed = infoLet.GetNameForVar name isAfter false allLet kernelVars
        if(getingName = null) then 
            let v = FindInVars(name)
            infoLet.AddNeedVar (v.GetOriginalName) (v.GetNewName) (v.GetVarType)
            v
        else
            getingName

    member this.AddVarInLastLet orgnName nName varType =
        let last = allLet.[this.GetLastInLet]
        last.AddVar orgnName nName varType

    member this.AddKernelVars orgnName nName varType =
        kernelVars.Add(new VarInfo(orgnName, nName, true, varType))


type Method(var:Var, expr:Expr) = 
    let mutable funVar = var
    let funExpr = expr

    member this.FunVar =
        funVar
    member this.FunExpr =
        funExpr
