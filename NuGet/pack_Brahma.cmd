rem Debug to Release

set corePathDbg=..\Source\Brahma.FSharp.OpenCL.Core\bin\Debug
set corePathRel=..\Source\Brahma.FSharp.OpenCL.Core\bin\Release

xcopy /s /i /Y %corePathDbg%\* %corePathRel%

nuget.exe pack "..\Source\Brahma.FSharp.OpenCL.Core\Brahma.FSharp.OpenCL.Core.fsproj" -IncludeReferencedProjects -Prop Configuration=Release