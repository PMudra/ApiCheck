# ApiCheck
ApiCheck is a library that compares different versions of an API using reflection to ensure compatibility with third party components. This project contains three components: the library, the console application and the NUnit integration.
##Features
* Comparing .NET assemblies.
* **Detection** of changed API elements such as types, methods, properties and more.
* **Reporting** results as XML or HTML files.
* **Console application** that can be used in CI environment.
* **NUnit integration** for nice analysis of the results of the comparison.

##CI Build
[![Build status](https://ci.appveyor.com/api/projects/status/b4uq1f6d2n91c8fv)](https://ci.appveyor.com/project/PMudra/apicheck)
##Installing via NuGet
Installing the library to your project (referencing ApiCheck.dll):
```
Install-Package ApiCheck
```
Installing the console application to packages/ApiCheck.Console/tools:
```
Install-Package ApiCheck.Console
```
Installing the NUnit integration to your project. This package depends on ApiCheck and NUnit which will be added automatically as NuGet packages.
```
Install-Package ApiCheck.NUnit
```
##Using the ApiCheck Library
```csharp
using (AssemblyLoader assemblyLoader = new AssemblyLoader())
{
    // using the included AssemblyLoader that automatically resolves dependencies
    Assembly refAssembly = assemblyLoader.ReflectionOnlyLoad("MyReferenceVersion.dll");
    Assembly devAssembly = assemblyLoader.ReflectionOnlyLoad("MyDevelopmentVersion.dll");
    // easy setup of the ApiChecker using the builder pattern
    ApiChecker.CreateInstance(refAssembly, devAssembly)
                // all listed elements are not compared
              .WithIgnoreList(new[] {"Element.To.Be.Ignored"})
              .WithDetailLogging(s => WriteLine(s))
              .WithInfoLogging(s => WriteLine(s))
                // write report to desired streams
              .WithHtmlReport(new FileStream("report.html", FileMode.Create))
              .WithXmlReport(new FileStream("report.xml", FileMode.Create))
              .Build()     // creating the ApiChecker
              .CheckApi(); // doing the comparison
}
```

##Using the console application
The console application provides basic parameters to run an API comparison. This application can be added to a CI build as Post-Build event.
```
Usage: ApiCheck.Console.exe -r <reference assembly> -n <new assembly> [-x <xml report>] [-h <html report>] [-i <ignore file>] [-v]
```
For more information run ```ApiCheck.Console.exe --help```
##Using the NUnit integration
Add a new class to your project like this:
```csharp
using ApiCheck.NUnit;
namespace MyNamespace
{
    [ApiTest(@"Version1\ApiCheckTestProject.dll", @"Version2\ApiCheckTestProject.dll", Category = "ApiTest", IgnoreListPath = @"ignoreList.txt")]
    [ApiTest(@"Version1\ApiCheckTestProject.Extension.dll", @"Version2\ApiCheckTestProject.Extension.dll")]
    public class ComparingApiTest : ApiTest
    {
    }
}
```
