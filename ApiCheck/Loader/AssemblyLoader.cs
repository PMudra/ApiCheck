using System;
using System.IO;
using System.Reflection;

namespace ApiCheck.Loader
{
  public sealed class AssemblyLoader : IDisposable
  {
    public AssemblyLoader()
    {
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnReflectionOnlyAssemblyResolve;
    }

    public void Dispose()
    {
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnReflectionOnlyAssemblyResolve;
    }
    
    private Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        AssemblyName assemblyName = new AssemblyName(args.Name);
        string path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), assemblyName.Name + ".dll");
        if (File.Exists(path))
          return Assembly.ReflectionOnlyLoadFrom(path);
      }
      catch (Exception)
      {
      }
      return Assembly.ReflectionOnlyLoad(args.Name);
    }

    public Assembly ReflectionOnlyLoad(string path)
    {
      return Assembly.ReflectionOnlyLoadFrom(path);
    }
  }
}
