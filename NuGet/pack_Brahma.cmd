rem Debug to Release

rem set corePathDbg=..\Source\Brahma.FSharp.OpenCL.Core\bin\Debug
rem set corePathRel=..\Source\Brahma.FSharp.OpenCL.Core\bin\Release

rem xcopy /s /i /Y %corePathDbg%\* %corePathRel%

rem nuget.exe pack "..\Source\Brahma.FSharp.OpenCL.Core\Brahma.FSharp.OpenCL.Core.fsproj" -IncludeReferencedProjects -Prop Configuration=Release
nuget.exe pack "..\Source\Brahma.FSharp.OpenCL.Core\Brahma.FSharp.OpenCL.Core.nuspec"
rem nuget push SamplePackage.1.0.0.nupkg f6ba9139-9d42-4cf1-acaf-344f963ff807 -Source https://www.myget.org/F/brahma_fsharp/api/v2/package