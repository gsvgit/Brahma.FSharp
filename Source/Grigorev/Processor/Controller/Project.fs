namespace Controller

open System.IO

type Project = {
    mutable Name : string
    mutable File : FileInfo
    mutable InitCode : string
    mutable SourceCode : string array array
}

