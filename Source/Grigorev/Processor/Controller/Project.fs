namespace Controller

open System.IO
open System.Runtime.Serialization

type Project = {
    [<field : DataMember>]
    mutable Name : string
    [<field : DataMember>]
    mutable InitCode : string
    [<field : DataMember>]
    mutable SourceCode : string array array
}
