using ApiCheck.Loader;
using ApiCheck.NUnit.Writer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApiCheck.NUnit
{
  public abstract class ApiTest
  {
    [Test, TestCaseSource("TestCases")]
    public void ApiElementTest(bool success, string message)
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

    public IEnumerable<ITestCaseData> TestCases
    {
      get
      {
        return GetTestCaseData(GetResults());
      }
    }

    private IEnumerable<ApiTestData> GetResults()
    {
      using (AssemblyLoader assemblyLoader = new AssemblyLoader())
      {
        object[] customAttributes = GetType().GetCustomAttributes(typeof(ApiTestAttribute), true);
        if (customAttributes.Length == 0)
        {
          throw new Exception(string.Format("The test class should define the {0} at least once", typeof(ApiTestAttribute)));
        }
        foreach (object customAttribute in customAttributes)
        {
          ApiTestAttribute apiTestAttribute = (ApiTestAttribute)customAttribute;
          ApiComparer apiComparer = ApiComparer.CreateInstance(assemblyLoader.ReflectionOnlyLoad(apiTestAttribute.ReferenceVersionPath),
                                                            assemblyLoader.ReflectionOnlyLoad(apiTestAttribute.NewVersionPath))
                                                            .Build();
          apiComparer.CheckApi();
          IList<string> ignoreList = IgnoreListLoader.LoadIgnoreList(GetReadStream(apiTestAttribute.IgnoreListPath));
          yield return new ApiTestData(apiComparer.ComparerResult, apiTestAttribute.Category, ignoreList, apiTestAttribute.Explicit, apiTestAttribute.HandleWarningsAsErrors);
        }
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
      testCaseData.AddRange(apiTestData.SelectMany(result => result.ComparerResult.ComparerResults.Select(r => GenerateTestCase(new ApiTestData(r, result.Category, result.IgnoreList, result.Explicit, result.HandleWarningsAsErrors)))));
      return testCaseData;
    }

    private static ITestCaseData GenerateTestCase(ApiTestData apiTestData)
    {
      IComparerResult comparerResult = apiTestData.ComparerResult;
      bool fail = comparerResult.GetAllCount(Severity.Error) > 0 || apiTestData.HandleWarningsAsErrors && comparerResult.GetAllCount(Severity.Warning) > 0;
      TestCaseData testCaseData = new TestCaseData(!fail, GetFailMessage(comparerResult)).SetName(comparerResult.Name).SetCategory(apiTestData.Category);
      if (apiTestData.IgnoreList.Contains(comparerResult.Name))
      {
        testCaseData.Ignore("Ignored by ignore list");
      }
      if (apiTestData.Explicit)
      {
        testCaseData.MakeExplicit("Set explicit by ApiTestAttribute");
      }
      return testCaseData;
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
      element.Add("Added Elements:", comparerResult.AddedItems.Select(added => string.Format("{0} -- {1}", added.ItemName, added.ResultContext)));
      element.Add("Removed Elements:", comparerResult.RemovedItems.Select(removed => string.Format("{0} -- {1}", removed.ItemName, removed.ResultContext)));
      element.Add("Changed Flags:", comparerResult.ChangedFlags.Select(changed => string.Format("{0} from {1} to {2}", changed.PropertyName, changed.ReferenceValue, changed.NewValue)));
      element.Add("Changed Attributes:", comparerResult.ChangedProperties.Select(changed => string.Format("{0} from {1} to {2}", changed.PropertyName, changed.ReferenceValue, changed.NewValue)));
      element.Add("Changed Children:", comparerResult.ComparerResults.Select(GetFailMessageElements));
      return element.HasElements ? element : null;
    }

    private class ApiTestData
    {
      public ApiTestData(IComparerResult comparerResult, string category, IList<string> ignoreList, bool @explicit, bool handleWarningsAsErrors)
      {
        ComparerResult = comparerResult;
        Category = category;
        IgnoreList = ignoreList;
        Explicit = @explicit;
        HandleWarningsAsErrors = handleWarningsAsErrors;
      }

      public bool HandleWarningsAsErrors { get; private set; }

      public IComparerResult ComparerResult { get; private set; }

      public string Category { get; private set; }

      public bool Explicit { get; private set; }

      public IList<string> IgnoreList { get; private set; }
    }
  }
}
