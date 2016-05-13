using ApiCheck.Comparer;
using ApiCheck.Report;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace ApiCheck
{
  /// <summary>
  /// An .NET API comparer. The comparer can be used to proof compatibility of two different versions of an API.
  /// </summary>
  public class ApiComparer
  {
    private readonly Assembly _referenceVersion;
    private readonly Assembly _newVersion;
    private readonly Stream _htmlOutput;
    private readonly Stream _xmlOutput;
    private readonly IComparerContext _comparerContext;

    private ApiComparer(Assembly referenceVersion, Assembly newVersion, Action<string> logInfo, Action<string> logDetail, Stream htmlOutput, Stream xmlOutput, IList<string> ignoredElements)
    {
      _referenceVersion = referenceVersion;
      _newVersion = newVersion;
      _htmlOutput = htmlOutput;
      _xmlOutput = xmlOutput;
      _comparerContext = new ComparerContext(logInfo, logDetail, ignoredElements);
    }

    /// <summary> Compares the versions of the API and generates the desired reports. </summary>
    /// <returns> The amount of errors and warnings found. </returns>
    public int CheckApi()
    {
      // Comparing
      _comparerContext.LogInfo("Comparing assemblies.");
      ComparerResult = _comparerContext.CreateComparer(_referenceVersion, _newVersion).Compare();

      // Reporting
      _comparerContext.LogInfo("Generating xml result.");
      XElement element = XmlGenerator.GenerateXml(ComparerResult);
      if (_xmlOutput != null)
      {
        _comparerContext.LogInfo("Exporting xml report.");
        element.Save(_xmlOutput);
      }
      if (_htmlOutput != null)
      {
        _comparerContext.LogInfo("Exporting html report.");
        XmlTransformer.TransformToHtml(element.CreateReader(), _htmlOutput);
      }
      return ComparerResult.GetAllCount(Severity.Error, false) + ComparerResult.GetAllCount(Severity.Warning, false);
    }

    /// <summary>
    /// The result of the comparison.
    /// </summary>
    public IComparerResult ComparerResult { get; private set; }

    /// <summary> Creates a new instance of <see cref="ApiComparerBuilder"/>. </summary>
    ///
    /// <param name="referenceVersion"> The reference version. </param>
    /// <param name="newVersion">       The new version. </param>
    ///
    /// <returns> The instance of the <see cref="ApiComparerBuilder"/>. </returns>
    public static ApiComparerBuilder CreateInstance(Assembly referenceVersion, Assembly newVersion)
    {
      if (referenceVersion == null || newVersion == null)
      {
        throw new ArgumentNullException();
      }
      return new ApiComparerBuilder(referenceVersion, newVersion);
    }

    /// <summary>
    /// A builder of an API comparer.
    /// </summary>
    public class ApiComparerBuilder
    {
      private readonly Assembly _referenceVersion;
      private readonly Assembly _newVersion;
      private Action<string> _logInfo;
      private Action<string> _logDetail;
      private Stream _htmlReport;
      private Stream _xmlReport;
      private IList<string> _ignoredElements;

      internal ApiComparerBuilder(Assembly referenceVersion, Assembly newVersion)
      {
        _referenceVersion = referenceVersion;
        _newVersion = newVersion;
      }

      /// <summary> Enables information logging of the <see cref="ApiComparer"/>. </summary>
      ///
      /// <param name="logInfo">  Action that is called when a new line is logged. </param>
      ///
      /// <returns> The instance of the builder. </returns>
      public ApiComparerBuilder WithInfoLogging(Action<string> logInfo)
      {
        _logInfo = logInfo;
        return this;
      }

      /// <summary> Enables detailed logging of the <see cref="ApiComparer"/>. </summary>
      ///
      /// <param name="logDetail">  Action that is called when a new line is logged. </param>
      ///
      /// <returns> The instance of the builder. </returns>
      public ApiComparerBuilder WithDetailLogging(Action<string> logDetail)
      {
        _logDetail = logDetail;
        return this;
      }

      /// <summary> Enables HTML reporting. </summary>
      ///
      /// <param name="htmlOutput"> The HTML output stream. </param>
      ///
      /// <returns> The instance of the builder. </returns>
      public ApiComparerBuilder WithHtmlReport(Stream htmlOutput)
      {
        _htmlReport = htmlOutput;
        return this;
      }

      /// <summary> Enables XML reporting. </summary>
      ///
      /// <param name="xmlOutput">  The XML output stream. </param>
      ///
      /// <returns> The instance of the builder. </returns>
      public ApiComparerBuilder WithXmlReport(Stream xmlOutput)
      {
        _xmlReport = xmlOutput;
        return this;
      }

      /// <summary> Sets a list of ignored types, methods, constructors, properties, events and fields. </summary>
      ///
      /// <param name="ignoredElements">  The list containing the full name of the elements to be ignored. </param>
      ///
      /// <returns> The instance of the builder. </returns>
      public ApiComparerBuilder WithIgnoreList(IList<string> ignoredElements)
      {
        _ignoredElements = ignoredElements;
        return this;
      }

      /// <summary> Builds the <see cref="ApiComparer"/>. </summary>
      ///
      /// <returns> The instance of the <see cref="ApiComparer"/>. </returns>
      public ApiComparer Build()
      {
        return new ApiComparer(_referenceVersion, _newVersion, _logInfo ?? (s => { }), _logDetail ?? (s => { }), _htmlReport, _xmlReport, _ignoredElements ?? new List<string>());
      }
    }
  }
}
