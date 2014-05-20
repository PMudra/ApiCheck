using System;

namespace ApiCheckNUnit
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class ApiTestAttribute : Attribute
  {
    private readonly string _referenceVersionPath;
    private readonly string _newVersionPath;

    public ApiTestAttribute(string referenceVersionPath, string newVersionPath)
    {
      _referenceVersionPath = referenceVersionPath;
      _newVersionPath = newVersionPath;
    }

    public string ReferenceVersionPath
    {
      get { return _referenceVersionPath; }
    }

    public string NewVersionPath
    {
      get { return _newVersionPath; }
    }

    public string Category { get; set; }

    public string IgnoreListPath { get; set; }
  }
}
