using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ApiCheck.Loader;

namespace ApiCheck.Console
{
  internal class Check
  {
    private readonly Action<string> _log;
    private readonly IList<string> _ignoreList;
    private readonly Assembly _referenceAssembly;
    private readonly Assembly _newAssembly;
    private readonly Stream _xmlStream;
    private readonly Stream _htmlStream;

    public Check(string referencePath, string newPath, string htmlPath, string xmlPath, string ignorePath, bool verbose)
    {
      using (AssemblyLoader assemblyLoader = new AssemblyLoader())
      {
        _referenceAssembly = assemblyLoader.ReflectionOnlyLoad(referencePath);
        _newAssembly = assemblyLoader.ReflectionOnlyLoad(newPath);
      }
      _htmlStream = GetWriteStream(htmlPath);
      _xmlStream = GetWriteStream(xmlPath);
      _ignoreList = IgnoreListLoader.LoadIgnoreList(GetReadStream(ignorePath));
      _log = verbose ? (Action<string>)(System.Console.WriteLine) : s => { };
    }

    public int CheckAssemblies()
    {
      return ApiChecker.CreateInstance(_referenceAssembly, _newAssembly)
        .WithIgnoreList(_ignoreList)
        .WithDetailLogging(_log)
        .WithInfoLogging(_log)
        .WithHtmlReport(_htmlStream)
        .WithXmlReport(_xmlStream)
        .Build().CheckApi();
    }

    private static Stream GetWriteStream(string path)
    {
      return string.IsNullOrEmpty(path) ? null : new FileStream(path, FileMode.Create, FileAccess.Write);
    }

    private static Stream GetReadStream(string path)
    {
      return string.IsNullOrEmpty(path) ? null : new FileStream(path, FileMode.Open, FileAccess.Read);
    }
  }
}
