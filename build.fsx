#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

RestorePackages()

let buildDir  = @".\build\"
let deployDir = @".\deploy\"

let buildVersion = if isLocalBuild then "0" else buildVersion
let version = "1.0." + buildVersion

let description = "desc"
let authors = ["authors"]
let project = "project"
let copyright = "(c)"
let releaseNotes = "releaseNotes"
let summary = "summary"
let title = "title"
let tags = "tags"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "SetVersion" (fun _ ->
    CreateCSharpAssemblyInfo "SolutionInfo.cs"
        [Attribute.FileVersion version
         Attribute.Version version]
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

Target "NuGet" (fun _ ->
    let packagingDir = @".\packaging\"
    let libDir = packagingDir @@ "lib"
    CleanDirs [packagingDir; libDir]

    Copy libDir [(buildDir @@ "ApiCheck.dll")]
    !!(buildDir + "*.txt")
    |> Copy packagingDir

    NuGet (fun p ->
        {p with
            WorkingDir = packagingDir
            OutputPath = deployDir
            Description = description
            Publish = false
            Version = version
            Authors = authors
            Project = project
            Copyright = copyright
            ReleaseNotes = releaseNotes
            Summary = summary
            Title = title
            Tags = tags
            })
            "ApiCheck.nuspec"
)

Target "Default" DoNothing

"Clean"
    ==> "SetVersion"
    ==> "Compile"
    ==> "CompileTest"
    ==> "RunTest"
    ==> "Zip"
    ==> "NuGet"
    ==> "Default"

RunTargetOrDefault "Default"
