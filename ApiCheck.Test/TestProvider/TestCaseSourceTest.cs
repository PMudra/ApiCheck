using ApiCheck.NUnit;

namespace ApiCheck.Test.TestProvider
{
  // Some of the tests must fail. Exclude ApiTest category from CI.
  [ApiTest(@"TestProject\Version1\ApiCheckTestProject.dll", @"TestProject\Version2\ApiCheckTestProject.dll", Category = "ApiTest", ComparerConfigurationPath = @"TestProvider\TestProjectConfiguration.txt", Explicit = true)]
  public class TestCaseSourceTest : ApiTest
  {
  }
}