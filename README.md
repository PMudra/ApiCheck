# ApiCheck
ApiCheck is a library that compares different versions of an API using reflection to ensure compatibility with third party components. This project contains three components: the library, the console application and the NUnit integration.
##Features
* Coparing .NET assemblies.
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
```cs
using (AssemblyLoader assemblyLoader = new AssemblyLoader())
{
    Assembly refAssembly = assemblyLoader.ReflectionOnlyLoad("MyReferenceVersion.dll");
    Assembly devAssembly = assemblyLoader.ReflectionOnlyLoad("MyDevelopmentVersion.dll");
    ApiChecker.CreateInstance(refAssembly, devAssembly)
              .WithIgnoreList(new[] {"Element.To.Be.Ignored"})
              .WithDetailLogging(s => WriteLine(s))
              .WithInfoLogging(s => WriteLine(s))
              .WithHtmlReport(new FileStream("report.html", FileMode.Create))
              .WithXmlReport(new FileStream("report.xml", FileMode.Create))
              .Build()
              .CheckApi();
}
```
