open NUnit.Framework
open IndexOf
open Brahma.OpenCL
open OpenCL.Net
open System.IO
open System
open System.Text

type Options = {
    Substring : string
    Position : int64
    FileName : string
}

[<TestFixture>]
[<Ignore>]
type TestsForSerial () =
    let testOptions =
        let sr = new StreamReader ("options.txt")
        let rec iter lst =
            if sr.EndOfStream
            then lst
            else
                let s = sr.ReadLine ()
                let arr = s.Split (' ')
                {Substring = arr.[1]; Position = Int64.Parse (arr.[3]); FileName = arr.[4]} :: lst |> iter
        iter [] |> List.rev

    let func = serialIndexOf

    [<Test>]
    member this.Test0 () = 
        let testNumber = 0
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    [<Ignore>]
    member this.Test1 () = 
        let testNumber = 1
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test2 () = 
        let testNumber = 2
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test3 () = 
        let testNumber = 3
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test4 () = 
        let testNumber = 4
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test5 () = 
        let testNumber = 5
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test6 () = 
        let testNumber = 6
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()


[<TestFixture>]
[<Ignore>]
type TestsForQuasiParallel () =
    let testOptions =
        let sr = new StreamReader ("options.txt")
        let rec iter lst =
            if sr.EndOfStream
            then lst
            else
                let s = sr.ReadLine ()
                let arr = s.Split (' ')
                {Substring = arr.[1]; Position = Int64.Parse (arr.[3]); FileName = arr.[4]} :: lst |> iter
        iter [] |> List.rev

    let func = quasiParallelIndexOf

    [<Test>]
    member this.Test0 () = 
        let testNumber = 0
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    [<Ignore>]
    member this.Test1 () = 
        let testNumber = 1
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test2 () = 
        let testNumber = 2
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test3 () = 
        let testNumber = 3
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test4 () = 
        let testNumber = 4
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test5 () = 
        let testNumber = 5
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test6 () = 
        let testNumber = 6
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

[<TestFixture>]
//[<Ignore>]
type TestsForPseudoParallel () =
    let testOptions =
        let sr = new StreamReader ("options.txt")
        let rec iter lst =
            if sr.EndOfStream
            then lst
            else
                let s = sr.ReadLine ()
                let arr = s.Split (' ')
                {Substring = arr.[1]; Position = Int64.Parse (arr.[3]); FileName = arr.[4]} :: lst |> iter
        iter [] |> List.rev

    let func = pseudoParallelIndexOf

    [<Test>]
    [<Ignore>]   
    member this.Test0 () = 
        let testNumber = 0
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    [<Ignore>]
    member this.Test1 () = 
        let testNumber = 1
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test2 () = 
        let testNumber = 2
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test3 () = 
        let testNumber = 3
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test4 () = 
        let testNumber = 4
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test5 () = 
        let testNumber = 5
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test6 () = 
        let testNumber = 6
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

[<TestFixture>]
type TestsForPseudoParallelHandmade () =
    let func = pseudoParallelIndexOf

    [<Test>]
    member this.Test0 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "ri"
        let pos = 2L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test1 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "i"
        let pos = 3L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test2 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "aaa"
        let pos = -1L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test3 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "ri"
        let pos = 2L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test4 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ssssssssssssstring"))
        let substr = "sstri"
        let pos = 11L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test5 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ababcababcababababababa"))
        let substr = "ababa"
        let pos = 10L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test6 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ababababababababababa"))
        let substr = "ababababababababababa"
        let pos = 0L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    

[<TestFixture>]
//[<Ignore>]
type TestsForGpu () =
    let testOptions =
        let sr = new StreamReader ("options.txt")
        let rec iter lst =
            if sr.EndOfStream
            then lst
            else
                let s = sr.ReadLine ()
                let arr = s.Split (' ')
                {Substring = arr.[1]; Position = Int64.Parse (arr.[3]); FileName = arr.[4]} :: lst |> iter
        iter [] |> List.rev

    let func = gpuIndexOf

    [<Test>]
    [<Ignore>]
    member this.Test0 () = 
        let testNumber = 0
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    [<Ignore>]
    member this.Test1 () = 
        let testNumber = 1
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test2 () = 
        let testNumber = 2
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test3 () = 
        let testNumber = 3
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test4 () = 
        let testNumber = 4
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test5 () = 
        let testNumber = 5
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

    [<Test>]
    member this.Test6 () = 
        let testNumber = 6
        let stream = new FileStream (testOptions.[testNumber].FileName, FileMode.Open)
        let substr = testOptions.[testNumber].Substring
        let pos = testOptions.[testNumber].Position
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))
        stream.Close ()

[<TestFixture>]
type TestsForGpuHandmade () =
    let func = gpuIndexOf

    [<Test>]
    member this.Test0 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "ri"
        let pos = 2L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test1 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "i"
        let pos = 3L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test2 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "aaa"
        let pos = -1L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test3 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("string"))
        let substr = "ri"
        let pos = 2L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test4 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ssssssssssssstring"))
        let substr = "sstri"
        let pos = 11L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test5 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ababcababcababababababa"))
        let substr = "ababa"
        let pos = 10L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))

    [<Test>]
    member this.Test6 () = 
        let stream = new MemoryStream (Encoding.Default.GetBytes ("ababababababababababa"))
        let substr = "ababababababababababa"
        let pos = 0L
        let res = func stream substr
        stream.Close ()
        Assert.That (res, Is.EqualTo (pos))



[<TestFixture>]
[<Ignore>]
type TestsForPrefixFunction () =
    [<Test>]
    member this.Test0 () =
        let str = "abcde"
        let arr = [|0; 0; 0; 0; 0|]
        Assert.That (prefixFunction str, Is.EqualTo (arr))

    [<Test>]
    member this.Test1 () =
        let str = "abcabc"
        let arr = [|0; 0; 0; 1; 2; 3|]
        let pref = prefixFunction str
        Assert.That (pref, Is.EqualTo (arr))

    [<Test>]
    member this.Test2 () =
        let str = "abcdabcabcdabcdab"
        let arr = [|0; 0; 0; 0; 1; 2; 3; 1; 2; 3; 4; 5; 6; 7; 4; 5; 6|]
        Assert.That (prefixFunction str, Is.EqualTo (arr))

    [<Test>]
    member this.Test3 () =
        let str = "abcdabscabcdabia"
        let arr = [|0; 0; 0; 0; 1; 2; 0; 0; 1; 2; 3; 4; 5; 6; 0 ;1|]
        Assert.That (prefixFunction str, Is.EqualTo (arr))

