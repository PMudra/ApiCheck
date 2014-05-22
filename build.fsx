#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

RestorePackages()

let buildDir  = @".\build\"
let deployDir = @".\deploy\"
let packagingDir = @".\packaging\"

let buildVersion = if isLocalBuild then "0" else buildVersion
let version = "1.0." + buildVersion

let authors = ["ise Individuelle Software-Entwicklung GmbH"]
let releaseNotes = "Initial Release."
let tags = "TODO"

let packages =
    ["ApiCheck", "desc", "summary", []
     "ApiCheck.Console", "desc", "summary", []
     "ApiCheck.NUnit", "desc", "summary", ["ApiCheck", version
                                           "NUnit", "2.6.3"]]

Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir; packagingDir]
)

Target "SetVersion" (fun _ ->
    CreateCSharpAssemblyInfo "SolutionInfo.cs"
        [Attribute.FileVersion version
         Attribute.Version version]
)

Target "Compile" (fun _ ->
    !! @"ApiCheck.sln"
    |> MSBuildRelease buildDir "Build"
    |> Log "Compile-Output: "
)

Target "CompileTest" (fun _ ->
    !! @"**\ApiCheck.Test.csproj"
    |> MSBuildDebug buildDir "Build"
    |> Log "CompileTest-Output: "
)

Target "RunTest" (fun _ ->
    !! (buildDir @@ @"*Test.dll")
    |> NUnit (fun p ->
        {p with
            DisableShadowCopy = true;
            OutputFile = buildDir @@ @"TestResults.xml"
            ExcludeCategory = "ApiTest"})
)

Target "Zip" (fun _ ->
    !! (buildDir @@ "**\*")
    |> Zip buildDir (deployDir @@ "ApiCheck." + version + ".zip")
)

Target "NuGet" (fun _ ->
    for package, description, summary, dependencies in packages do
        let libDir = packagingDir @@ "lib"
        let toolDir = packagingDir @@ "tools"
        CleanDirs [libDir; toolDir]
        !! (buildDir @@ "*.txt") |> CopyFiles packagingDir
        match package with
        | "ApiCheck" ->
            CopyFile libDir (buildDir @@ "ApiCheck.dll")
            CopyFile libDir (buildDir @@ "ApiCheck.XML")
        | "ApiCheck.NUnit" ->
            CopyFile libDir (buildDir @@ "ApiCheck.NUnit.dll")
            CopyFile libDir (buildDir @@ "ApiCheck.NUnit.XML")
        | "ApiCheck.Console" ->
            CopyFile toolDir (buildDir @@ "ApiCheck.dll")
            CopyFile toolDir (buildDir @@ "ApiCheck.Console.exe")
            CopyFile toolDir (buildDir @@ "ApiCheck.Console.exe.config")
            CopyFile toolDir (buildDir @@ "CommandLine.dll")
        | _ -> ()
        NuGet (fun p ->
            {p with
                WorkingDir = packagingDir
                OutputPath = deployDir
                Publish = false
                Description = description
                Version = version
                Authors = authors
                Project = package
                ReleaseNotes = releaseNotes
                Summary = summary
                Tags = tags
                Dependencies = dependencies
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
