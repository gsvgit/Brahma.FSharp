module Main

open Types

[<EntryPoint>]
let main argv = 
    
    let bArr : list<BaseType> = List.init 10 (fun i -> new BaseType()) // Everything is OK
    let bArr : list<BaseType> = List.init 10 (fun i -> new DerivedType()) // Covarience is not supported by lists
    let dArr : list<DerivedType> = List.init 10 (fun i -> new DerivedType()) // Everything is OK
    let dArr : list<DerivedType> = List.init 10 (fun i -> new BaseType()) // Contravarience is not supported by lists

    let mutable baseArr = [| new BaseType() |]
    let mutable derArr = [| new DerivedType() |]

    let b : BaseType = new DerivedType() // We cannot store uncasted instances of derived class in base class vatiable
    
    baseArr <- [| new BaseType(); new DerivedType() |] // But array can contain both its own type instances and derived type instances. Anyway, arrays type is the same with variable type
    let toPrint = baseArr.GetType() // BaseType
    
    derArr <- [| new BaseType() |]

    // To conclude, collections in F# are invariant, but can contain instances of derived class
    
    toPrint |> printfn "%A"

    0 // return an integer exit code
