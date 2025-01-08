#if FAKE
#r "paket:
nuget FSharp.Core 6.0.0.0
nuget MSBuild.StructuredLogger
nuget Microsoft.Build.Framework 17.3
nuget Fake.DotNet
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Testing.NUnit
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip
nuget Fake.Core.Target //"
#endif
#load ".fake/build.fsx/intellisense.fsx"

open Fake
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators //enables !! and globbing
open Fake.Core
open Fake.DotNet.Testing

let buildDir  = __SOURCE_DIRECTORY__ @@ @".\build\"
let testDir  = __SOURCE_DIRECTORY__ @@ @".\test\"
let deployDir = __SOURCE_DIRECTORY__ @@ @".\deploy\"
let packagingDir = __SOURCE_DIRECTORY__ @@ @".\packaging\"

let buildVersion = if BuildServer.isLocalBuild then "0" else BuildServer.buildVersion
let version = "2.0." + buildVersion

let authors = ["ise Individuelle Software-Entwicklung GmbH"]
let releaseNotes = "Initial Release."
let tags = "ApiCheck Assembly Comparer NUnit Different Version Build Integration Compatibility Api Test Sdk Console"

let globalDescription = "Library comparing different versions of an api using reflection to ensure compatibility with third party components."

let packages =
    [ "ApiCheck", globalDescription, ["YamlDotNet", "16.3.0"]
      "ApiCheck.Console", globalDescription + " Console application.", []
      "ApiCheck.NUnit", globalDescription + " NUnit integration.", ["ApiCheck", version; "NUnit", "3.13.3"] ]

Target.create "Clean" (fun _ ->
    Shell.cleanDirs [buildDir; deployDir; packagingDir]
)

Target.create "SetVersion" (fun _ ->
    AssemblyInfoFile.createCSharp @".\SolutionInfo.cs"
        [AssemblyInfo.FileVersion version
         AssemblyInfo.Version version]
)

Target.create "RestorePackages" (fun _ ->
    "./ApiCheck.sln"
    |> NuGet.Restore.RestoreMSSolutionPackages id
)

Target.create "Compile" (fun _ ->
    !! @"ApiCheck.sln"
    |> MSBuild.runRelease 
        (fun p ->
            { p with Properties = [ "BaseOutputPath", buildDir ] } )
        "" "Build"
    |> Trace.logItems "Compile-Output: "
)

Target.create "CompileTest" (fun _ ->
    !! @"**\ApiCheck.Test.csproj"
    |> MSBuild.runDebug
        (fun p ->
            { p with Properties = [ "BaseOutputPath", testDir ] } )
        "" "Build"
    |> Trace.logItems "CompileTest-Output: "
)

Target.create "RunTest" (fun _ ->
    !! (testDir @@ @"**\*Test.dll")
    |> NUnit3.run (fun p ->
        {p with
            ShadowCopy = false
            OutputDir = buildDir @@ @"TestResults.xml"})
)

Target.create "Zip" (fun _ ->
    !! (buildDir @@ "**\*")
    |> Zip.zip buildDir (deployDir @@ "ApiCheck." + version + ".zip")
)

Target.create "NuGet" (fun _ ->
    for package, description, dependencies in packages do
        let buildDirNet6 = buildDir @@ "Release" @@ "net6.0"
        let libDir = packagingDir @@ "lib" @@ "netstandard2.0" 
        let toolDirNet6 = packagingDir @@ "tools" @@ "net6.0"
        Shell.cleanDirs [libDir; toolDirNet6]
        !! (buildDirNet6 @@ "*.txt") |> Shell.copyFiles packagingDir
        match package with
        | "ApiCheck" ->
            Shell.copyFile libDir (buildDirNet6 @@ "ApiCheck.dll")
            Shell.copyFile libDir (buildDirNet6 @@ "ApiCheck.XML")
        | "ApiCheck.NUnit" ->
            Shell.copyFile libDir (buildDirNet6 @@ "ApiCheck.NUnit.dll")
            Shell.copyFile libDir (buildDirNet6 @@ "ApiCheck.NUnit.XML")
        | "ApiCheck.Console" ->
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "ApiCheck.dll")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "ApiCheck.Console.exe")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "ApiCheck.Console.dll")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "ApiCheck.Console.deps.json")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "ApiCheck.Console.runtimeconfig.json")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "CommandLine.dll")
            Shell.copyFile toolDirNet6 (buildDirNet6 @@ "YamlDotNet.dll")
        | _ -> ()
        NuGet.NuGet.NuGet (fun p ->
            {p with
                WorkingDir = packagingDir
                OutputPath = deployDir
                Publish = false
                Description = description
                Version = version
                Authors = authors
                Project = package
                ReleaseNotes = releaseNotes
                Tags = tags
                Dependencies = dependencies
                })
                "ApiCheck.nuspec"
)

open Fake.Core.TargetOperators

Target.create "Default" (fun _ ->
    Trace.trace "Starting Default target..."
)

"Clean"
    ==> "SetVersion"
    ==> "RestorePackages"
    ==> "Compile"
    ==> "CompileTest"
    ==> "RunTest"
    ==> "Zip"
    ==> "NuGet"
    ==> "Default"

Target.runOrDefault "Default"
