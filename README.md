# ApiCheck

[![Join the chat at https://gitter.im/PMudra/ApiCheck](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/PMudra/ApiCheck?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

ApiCheck is a library that compares different versions of an API using reflection to ensure compatibility with third party components. This project contains three components: the library, the console application and the NUnit integration.

## Features
* Comparing .NET assemblies.
* **Detection** of changed API elements such as types, methods, properties and more.
* **Reporting** results as XML or HTML files.
* **Console application** that can be used in CI environment.
* **NUnit integration** for nice analysis of the results of the comparison.

## CI Build
[![Build status](https://ci.appveyor.com/api/projects/status/b4uq1f6d2n91c8fv)](https://ci.appveyor.com/project/PMudra/apicheck)
[![NuGet status](http://img.shields.io/nuget/v/ApiCheck.svg)](http://www.nuget.org/packages/ApiCheck/)
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
## Using the ApiCheck Library
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

## Using the console application
The console application provides basic parameters to run an API comparison. This application can be added to a CI build as Post-Build event.
```
Usage: ApiCheck.Console.exe -r <reference assembly> -n <new assembly> [-x <xml report>] [-h <html report>] [-i <ignore file>] [-v]
```
For more information run ```ApiCheck.Console.exe --help```
## Using the NUnit integration
Add a new class to your project like this:
```csharp
using ApiCheck.NUnit;
namespace MyNamespace
{
    [ApiTest(@"Version1\ApiCheckTestProject.dll", @"Version2\ApiCheckTestProject.dll", Category = "ApiTest", IgnoreListPath = @"ignoreList.txt")]
    [ApiTest(@"Version1\ApiCheckTestProject.Extension.dll", @"Version2\ApiCheckTestProject.Extension.dll", Explicit = true)]
    public class ComparingApiTest : ApiTest
    {
    }
}
```
## Detected changes
These are the changes in an api that are detected by the comparer:

#### Assemblies
Description | Before | After | Severity
----------- | ------ | ----- | --------
assembly name | Company.MyAssembly | Company.YourAssembly | Error
assembly version | 1.0.0.0 | 2.0.0.0 | Hint
assembly public key token | B03F5F7F11D50A3A | 0A3AB03F5F7F11D5 | Error
assembly culture | de-DE | en-US | Warning
type removed | public class B { } | | Error
type added | | public class B { } | Warning
nested type removed | public class B { public class A { } } | public class B { } | Error
nested type added | public class B { } | public class B { public class A { } } | Warning

#### Types
Description | Before | After | Severity
----------- | ------ | ----- | --------
enum changed | public enum E { } | public class E { } | Error
abstract changes | public abstract class A { } | public class A { } | Error
sealed changed | public sealed class A { } | public class A { } | Error
interface changed | public interface I { } | public class I { } | Error
serializable changed | [Serializable]public class S { } | public class S { } | Error
interfaces removed | public class A : Interface { } | public class A { } | Error
interfaces added | public class A { } | public class A : Interface { } | Warning
base changed | public class A : AbstractClass { } | public class A { } | Error
method added | | public int A() { } | Warning
method removed | public int A() { } | | Error
constructor added | | public A() { } | Warning
constructor removed | public A() { } | | Error
property added | | public int P {get; set;} | Warning
property removed | public int P {get; set;} | | Error
event added | | public event DelegateType MyEvent | Warning
event removed | public event DelegateType MyEvent | | Error
field added | | public int i; | Warning
field removed | public int i; | | Error

#### Methods
Description | Before | After | Severity
----------- | ------ | ----- | --------
virtual changed | public virtual int A() | public int A() { } | Error
static changed | public static int A() | public int A() { } | Error
abstract changed | public abstract int A() | public int A() { } | Error
sealed changed | public override sealed int A() | public int A() { } | Error
return value changed | public int A() { } | public string A() { } | Error
parameter name changed | public int A(int i) { } | public int A(int j) { } | Error
default value changed | public int A(int i = 0) { } | public int A(int i = 1) { } | Error
out changed | public int A(out int i) { } | public int A(ref int i) { } | Error

#### Properties, Events & Fields
Description | Before | After | Severity
----------- | ------ | ----- | --------
property type changed | public int A {get; set;} | public string A {get;set;} | Error
property setter changed | public int A {get; set;} | public int A {get;} | Error
property getter changed | public int A {get; set;} | public int A {set;} | Error
static changed | public static int A {get; set;} | public int A {ret; set;} | Error
event type changed | public event DelegateType MyEvent | public event NewDelegateType MyEvent | Error
static changed | public static event DelegateType MyEvent | public event DelegateType MyEvent | Error
field type changed | public int i | public string i | Error
static changed | public static int i | public string i | Error
