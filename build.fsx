#r @"packages\FAKE\tools\FakeLib.dll"
open Fake

RestorePackages()

// Directories
let buildDir  = @".\build\"
let deployDir = @".\deploy\"

let version = buildVersion

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Compile" (fun _ ->
    !! @"**\ApiCheck.csproj"
    ++ @"**\ApiCheckConsole.csproj"
    ++ @"**\ApiCheckNUnit.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Compile-Output: "
)

Target "CompileTest" (fun _ ->
    !! @"**\ApiCheckTest.csproj"
    |> MSBuildDebug buildDir "Build"
    |> Log "CompileTest-Output: "
)

Target "RunTest" (fun _ ->
    !! (buildDir + @"*Test.dll")
    |> NUnit (fun p ->
        {p with
            DisableShadowCopy = true;
            OutputFile = buildDir + @"TestResults.xml"
            ExcludeCategory = "ApiTest"})
)

Target "Zip" (fun _ ->
    !! (buildDir + "**\*")
    |> Zip buildDir (deployDir + "ApiCheck." + version + ".zip")
)

// Dependencies
"Clean"
    ==> "Compile"
    ==> "CompileTest"
    ==> "RunTest"
    ==> "Zip"

RunTargetOrDefault "Zip"
