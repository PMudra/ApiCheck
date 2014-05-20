using ApiCheckNUnit;

namespace ApiCheckTest.TestProvider
{
  // Some of the tests must fail. Exclude ApiTest category from CI.
  [ApiTest(@"TestProject\Version1\ApiCheckTestProject.dll", @"TestProject\Version2\ApiCheckTestProject.dll", Category = "ApiTest", IgnoreListPath = @"TestProvider\ignore.txt")]
  public class TestCaseSourceTest : ApiTest
  {
  }
}
