using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ApiCheck.Loader
{
  public sealed class AssemblyLoader : IDisposable
  {
    private readonly MetadataLoadContext _metadataLoadContext;

    public AssemblyLoader(string path)
    {
      _metadataLoadContext = GetMetadataLoadContext(path);
    }

    public void Dispose()
    {
      _metadataLoadContext?.Dispose();
    }

    public Assembly GetAssembly(string path)
    {
      return _metadataLoadContext.LoadFromAssemblyPath(Path.GetFullPath(path));
    }

    private MetadataLoadContext GetMetadataLoadContext(string path)
    {
      var allAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll").ToList();

      if (PathIsDirectory(path) == true)
      {
        allAssemblies.AddRange(Directory.GetFiles(path, "*.dll"));
      }
      else if (PathIsDirectory(path) == false)
      {
        allAssemblies.Add(path);
      }

      var resolver = new PathAssemblyResolver(allAssemblies);
      var mlc = new MetadataLoadContext(resolver);

      return mlc;
    }

    public static bool? PathIsDirectory(string path)
    {
      if (File.Exists(path))
      {
        return false;
      }
      else if (Directory.Exists(path))
      {
        return true;
      }
      else
      {
        return null;
      }
    }

  }
}
