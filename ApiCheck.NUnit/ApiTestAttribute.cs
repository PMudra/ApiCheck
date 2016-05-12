using NUnit.Framework;
using System;

namespace ApiCheck.NUnit
{
  /// <summary>
  /// Provides parameters for the comparison of an API. The class that this attribute is applied to must inherit from <see cref="ApiTest"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class ApiTestAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTestAttribute"/> class with the path to the reference version of an assembly and the development version of an assembly.
    /// </summary>
    /// <param name="referenceVersionPath">The absolute or relative path to the reference version of the assembly.</param>
    /// <param name="newVersionPath">The absolute or relative path to the development version of the assembly.</param>
    public ApiTestAttribute(string referenceVersionPath, string newVersionPath)
    {
      ReferenceVersionPath = referenceVersionPath;
      NewVersionPath = newVersionPath;
    }

    /// <summary>
    /// Gets the path for the reference version of the assembly.
    /// </summary>
    public string ReferenceVersionPath { get; private set; }

    /// <summary>
    /// Gets the path for the development version of the assembly.
    /// </summary>
    public string NewVersionPath { get; private set; }

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

    /// <summary>
    /// Gets or sets whether api check warnings should be handled as errors or not. If an error occurs the unit test will fail. This means setting this property to true will lead to failing unit tests if api check warnings occur.
    /// </summary>
    public bool HandleWarningsAsErrors { get; set; }
  }
}
