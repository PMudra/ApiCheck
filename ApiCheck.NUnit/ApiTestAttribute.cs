using System;
using NUnit.Framework;

namespace ApiCheck.NUnit
{
  /// <summary>
  /// Provides parameters for the comparison of an API. The class that this attribute is applied to must inherit from <see cref="ApiTest"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class ApiTestAttribute : Attribute
  {
    private readonly string _referenceVersionPath;
    private readonly string _newVersionPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTestAttribute"/> class with the path to the reference version of an assembly and the development version of an assembly.
    /// </summary>
    /// <param name="referenceVersionPath">The absolute or relative path to the reference version of the assembly.</param>
    /// <param name="newVersionPath">The absolute or relative path to the development version of the assembly.</param>
    public ApiTestAttribute(string referenceVersionPath, string newVersionPath)
    {
      _referenceVersionPath = referenceVersionPath;
      _newVersionPath = newVersionPath;
    }

    /// <summary>
    /// Gets the path for the reference version of the assembly.
    /// </summary>
    public string ReferenceVersionPath
    {
      get { return _referenceVersionPath; }
    }

    /// <summary>
    /// Gets the path for the development version of the assembly.
    /// </summary>
    public string NewVersionPath
    {
      get { return _newVersionPath; }
    }

    /// <summary>
    /// Gets or sets the test category that will include all generated tests. For more information refer to the <see cref="CategoryAttribute"/> class.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether the generated tests should be explicit. For more information refer to the <see cref="ExplicitAttribute"/> class.
    /// </summary>
    public bool Explicit { get; set; }

    /// <summary>
    /// Gets or sets the absolute or relative path for the ignored types. The file must contain elements separated by line breaks.
    /// </summary>
    public string IgnoreListPath { get; set; }
  }
}
