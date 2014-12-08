namespace Types

type BaseType () =
    abstract member Idle : unit -> unit
    default this.Idle () = ()

type DerivedType () =
    inherit BaseType()
    override this.Idle () = ()