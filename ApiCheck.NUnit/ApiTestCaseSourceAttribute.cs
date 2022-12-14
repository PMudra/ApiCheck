using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCheck.Configuration;
using ApiCheck.Loader;
using ApiCheck.NUnit.Writer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace ApiCheck.NUnit
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  internal class ApiTestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
  {
    private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
    {
      foreach (TestCaseParameters parms in GetTestCasesFor(method))
      {
        yield return _builder.BuildTestMethod(method, suite, parms);
      }
    }

    private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
    {
      return GetTestCaseData(GetResults(method));
    }

    private IEnumerable<ApiTestData> GetResults(IMethodInfo method)
    {
      ApiTestAttribute[] customAttributes = method.TypeInfo.GetCustomAttributes<ApiTestAttribute>(true);
      if (customAttributes.Length == 0)
      {
        throw new Exception(string.Format("The test class should define the {0} at least once", typeof(ApiTestAttribute)));
      }

      foreach (ApiTestAttribute apiTestAttribute in customAttributes)
      {
        Stream comparerConfigurationStream = GetReadStream(apiTestAttribute.ComparerConfigurationPath);
        ComparerConfiguration comparerConfiguration = ConfigurationLoader.LoadComparerConfiguration(comparerConfigurationStream);

        using var referenceLoader = new AssemblyLoader(apiTestAttribute.ReferenceVersionPath);
        using var newLoader = new AssemblyLoader(apiTestAttribute.NewVersionPath);

        ApiComparer apiComparer = ApiComparer
          .CreateInstance(
            referenceLoader.GetAssembly(apiTestAttribute.ReferenceVersionPath),
            newLoader.GetAssembly(apiTestAttribute.NewVersionPath))
          .WithComparerConfiguration(comparerConfiguration)
          .Build();

        apiComparer.CheckApi();

        yield return new ApiTestData(apiComparer.ComparerResult, apiTestAttribute.Category, apiTestAttribute.Explicit,
          apiTestAttribute.HandleWarningsAsErrors);
      }
    }

    private static Stream GetReadStream(string path)
    {
      return string.IsNullOrEmpty(path) ? null : new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    private static IEnumerable<ITestCaseData> GetTestCaseData(IEnumerable<ApiTestData> results)
    {
      List<ITestCaseData> testCaseData = new List<ITestCaseData>();
      IEnumerable<ApiTestData> apiTestData = results.ToList();
      testCaseData.AddRange(apiTestData.Select(GenerateTestCase));
      testCaseData.AddRange(apiTestData.SelectMany(result =>
        result.ComparerResult.ComparerResults.Select(r =>
          GenerateTestCase(new ApiTestData(r, result.Category, result.Explicit, result.HandleWarningsAsErrors)))));
      return testCaseData;
    }

    private static ITestCaseData GenerateTestCase(ApiTestData apiTestData)
    {
      IComparerResult comparerResult = apiTestData.ComparerResult;
      bool fail = Fail(apiTestData, comparerResult);
      TestCaseData testCaseData = new TestCaseData(!fail, GetFailMessage(comparerResult)).SetName(comparerResult.Name).SetCategory(apiTestData.Category);
      if (apiTestData.Explicit)
      {
        testCaseData.Explicit("Set explicit by ApiTestAttribute");
      }

      return testCaseData;
    }

    private static bool Fail(ApiTestData apiTestData, IComparerResult comparerResult)
    {
      bool ignoreChildren = apiTestData.ComparerResult.ResultContext == ResultContext.Assembly;
      return comparerResult.GetAllCount(Severity.Error, ignoreChildren) > 0 ||
             apiTestData.HandleWarningsAsErrors && comparerResult.GetAllCount(Severity.Warning, ignoreChildren) > 0;
    }

    private static string GetFailMessage(IComparerResult comparerResult)
    {
      UnitTestWriter unitTestWriter = new UnitTestWriter();
      Element failMessageElements = GetFailMessageElements(comparerResult);
      if (failMessageElements != null)
      {
        failMessageElements.Write(unitTestWriter);
      }

      return unitTestWriter.GetTextAsString();
    }

    private static Element GetFailMessageElements(IComparerResult comparerResult)
    {
      Element element = new Element(string.Format("{0} {1}", comparerResult.ResultContext, comparerResult.Name));
      element.Add("Added Elements:",
        comparerResult.AddedItems.Select(added => string.Format("{0} -- {1} ({2})", added.ItemName, added.ResultContext, added.Severity)));
      element.Add("Removed Elements:",
        comparerResult.RemovedItems.Select(removed => string.Format("{0} -- {1} ({2})", removed.ItemName, removed.ResultContext, removed.Severity)));
      element.Add("Changed Flags:",
        comparerResult.ChangedFlags.Select(changed =>
          string.Format("{0} from {1} to {2} ({3})", changed.PropertyName, changed.ReferenceValue, changed.NewValue, changed.Severity)));
      element.Add("Changed Attributes:",
        comparerResult.ChangedProperties.Select(changed =>
          string.Format("{0} from {1} to {2} ({3})", changed.PropertyName, changed.ReferenceValue, changed.NewValue, changed.Severity)));

      // Do not list child elements for assemblies because all of the children will be placed in a separate test
      if (comparerResult.ResultContext != ResultContext.Assembly)
      {
        element.Add("Changed Children:", comparerResult.ComparerResults.Select(GetFailMessageElements));
      }

      return element.HasElements ? element : null;
    }

    private class ApiTestData
    {
      public ApiTestData(IComparerResult comparerResult, string category, bool @explicit, bool handleWarningsAsErrors)
      {
        ComparerResult = comparerResult;
        Category = category;
        Explicit = @explicit;
        HandleWarningsAsErrors = handleWarningsAsErrors;
      }

      public bool HandleWarningsAsErrors { get; private set; }

      public IComparerResult ComparerResult { get; private set; }

      public string Category { get; private set; }

      public bool Explicit { get; private set; }
    }
  }
}
