using ApiCheck.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;

namespace ApiCheck.Test.TestProvider
{
  // Some of the tests must fail. Exclude ApiTest category from CI.
  [ApiTest(@"TestProject\Version1\ApiCheckTestProject.dll", @"TestProject\Version2\ApiCheckTestProject.dll", Category = "ApiTest", ComparerConfigurationPath = @"TestProvider\TestProjectConfiguration.txt", Explicit = true)]
  public class TestCaseSourceTest
  {
    [Test, TestCaseSource(nameof(TestCases))]
    public void ApiElementTest(bool success, string message)
    {
      ApiTest.ApiElementTest(success, message);
    }

    public static IEnumerable<ITestCaseData> TestCases
    {
      get
      {
        var results = ApiTest.GetResults(typeof(TestCaseSourceTest));

        return ApiTest.GetTestCaseData(results);
      }
    }
  }
}
