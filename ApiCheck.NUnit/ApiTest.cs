using ApiCheck.Loader;
using ApiCheck.NUnit.Writer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiCheck.Configuration;
using NUnit.Framework.Interfaces;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace ApiCheck.NUnit
{
  public static class ApiTest
  {
    public static void ApiElementTest(bool success, string message)
    {
      if (success)
      {
        Assert.Pass();
      }
      else
      {
        Assert.Fail(message);
      }
    }

    public static IEnumerable<ApiTestData> GetResults(Type type)
    {
      using (AssemblyLoader assemblyLoader = new AssemblyLoader())
      {
        object[] customAttributes = type.GetCustomAttributes(typeof(ApiTestAttribute), true);
        if (customAttributes.Length == 0)
        {
          throw new Exception(string.Format("The test class should define the {0} at least once", typeof(ApiTestAttribute)));
        }
        foreach (object customAttribute in customAttributes)
        {
          ApiTestAttribute apiTestAttribute = (ApiTestAttribute)customAttribute;

          Stream comparerConfigurationStream = GetReadStream(Path.GetFullPath(apiTestAttribute.ComparerConfigurationPath, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
          ComparerConfiguration comparerConfiguration = ConfigurationLoader.LoadComparerConfiguration(comparerConfigurationStream);

          ApiComparer apiComparer = ApiComparer.CreateInstance(assemblyLoader.ReflectionOnlyLoad(Path.GetFullPath(apiTestAttribute.ReferenceVersionPath, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))),
                                                               assemblyLoader.ReflectionOnlyLoad(Path.GetFullPath(apiTestAttribute.NewVersionPath, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))))
                                                               .WithComparerConfiguration(comparerConfiguration)
                                                               .Build();
          apiComparer.CheckApi();
          yield return new ApiTestData(apiComparer.ComparerResult, apiTestAttribute.Category, apiTestAttribute.Explicit, apiTestAttribute.HandleWarningsAsErrors);
        }
      }
    }

    private static Stream GetReadStream(string path)
    {
      return string.IsNullOrEmpty(path) ? null : new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    public static IEnumerable<ITestCaseData> GetTestCaseData(IEnumerable<ApiTestData> results)
    {
      List<ITestCaseData> testCaseData = new List<ITestCaseData>();
      IEnumerable<ApiTestData> apiTestData = results.ToList();
      testCaseData.AddRange(apiTestData.Select(GenerateTestCase));
      testCaseData.AddRange(apiTestData.SelectMany(result => result.ComparerResult.ComparerResults.Select(r => GenerateTestCase(new ApiTestData(r, result.Category, result.Explicit, result.HandleWarningsAsErrors)))));
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
      return comparerResult.GetAllCount(Severity.Error, ignoreChildren) > 0 || apiTestData.HandleWarningsAsErrors && comparerResult.GetAllCount(Severity.Warning, ignoreChildren) > 0;
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
      element.Add("Added Elements:", comparerResult.AddedItems.Select(added => string.Format("{0} -- {1} ({2})", added.ItemName, added.ResultContext, added.Severity)));
      element.Add("Removed Elements:", comparerResult.RemovedItems.Select(removed => string.Format("{0} -- {1} ({2})", removed.ItemName, removed.ResultContext, removed.Severity)));
      element.Add("Changed Flags:", comparerResult.ChangedFlags.Select(changed => string.Format("{0} from {1} to {2} ({3})", changed.PropertyName, changed.ReferenceValue, changed.NewValue, changed.Severity)));
      element.Add("Changed Attributes:", comparerResult.ChangedProperties.Select(changed => string.Format("{0} from {1} to {2} ({3})", changed.PropertyName, changed.ReferenceValue, changed.NewValue, changed.Severity)));

      // Do not list child elements for assemblies because all of the children will be placed in a separate test
      if (comparerResult.ResultContext != ResultContext.Assembly)
      {
        element.Add("Changed Children:", comparerResult.ComparerResults.Select(GetFailMessageElements));
      }
      return element.HasElements ? element : null;
    }

    public class ApiTestData
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
