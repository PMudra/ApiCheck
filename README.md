# ApiCheck
ApiCheck is a library that compares different versions of an api using reflection to ensure compatibility with third party components. This project contains three components: the library, the console application and the NUnit integration.
###CI Build
[![Build status](https://ci.appveyor.com/api/projects/status/b4uq1f6d2n91c8fv)](https://ci.appveyor.com/project/PMudra/apicheck)
###Installing via NuGet
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
