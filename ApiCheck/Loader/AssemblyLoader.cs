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
        return Assembly.ReflectionOnlyLoad(args.Name);
      }
      catch (Exception)
      {
        AssemblyName assemblyName = new AssemblyName(args.Name);
        string path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), assemblyName.Name + ".dll");
        return Assembly.ReflectionOnlyLoadFrom(path);
      }
    }

    public Assembly ReflectionOnlyLoad(string path)
    {
      return Assembly.ReflectionOnlyLoadFrom(path);
    }
  }
}
