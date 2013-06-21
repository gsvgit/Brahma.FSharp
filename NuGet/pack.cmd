rem Debug to Release

set corePathDbg=..\Source\Brahma.FSharp.OpenCL.Core\bin\Debug
set corePathRel=..\Source\Brahma.FSharp.OpenCL.Core\bin\Release

if  exist %corePathRel% (
    rd /s /q %corePathRel% )

mkdir %corePathRel%

xcopy /s /i /Y %corePathDbg%\* %corePathRel%


nuget.exe pack "..\Source\Brahma.FSharp.OpenCL.Core\Brahma.FSharp.OpenCL.Core.fsproj" -IncludeReferencedProjects -Prop Configuration=Release

nuget.exe pack "..\Source\Substrings\Brahman.Substrings.fsproj" -IncludeReferencedProjects -Prop Configuration=Release