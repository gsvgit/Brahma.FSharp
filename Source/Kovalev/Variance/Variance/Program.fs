type Base() =
    abstract member nothing : unit -> unit
    

type Derived() =
    inherit Base()

let bArr : list<Base> = List.init 10 (fun i -> new Base()) 
let bArr : list<Base> = List.init 10 (fun i -> new Derived()) 
let dArr : list<Derived> = List.init 10 (fun i -> new Derived()) 
let dArr : list<Derived> = List.init 10 (fun i -> new Base()) 