#r @"packages\FAKE\tools\FakeLib.dll"
open Fake

RestorePackages()

// Directories
let buildDir  = @".\build\"
let testDir   = @".\test\"
let deployDir = @".\deploy\"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; deployDir]
)

Target "Compile" (fun _ ->
    !! @"ApiCheck\**\*.csproj" ++ @"ApiCheckConsole\**\*.csproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "Compile-Output: "
)

Target "CompileTest" (fun _ ->
    !! @"ApiCheckTest\**\*.csproj"
    |> MSBuildDebug testDir "Build"
    |> Log "CompileTest-Output: "
)

Target "RunTest" (fun _ ->
    !! (testDir + @"\*Test.dll")
    |> NUnit (fun p ->
        {p with
            DisableShadowCopy = true;
            OutputFile = testDir + @"TestResults.xml"
            ExcludeCategory = "ApiTest"})
)

// Dependencies
"Clean"
  ==> "Compile"
  ==> "CompileTest"
  ==> "RunTest"

RunTargetOrDefault "RunTest"
