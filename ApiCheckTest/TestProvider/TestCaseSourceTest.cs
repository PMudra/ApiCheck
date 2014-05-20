using ApiCheck.TestProvider;
using NUnit.Framework;

namespace ApiCheckTest.TestProvider
{
  [TestFixture]
  [ApiTest(@"TestProject\Version1\ApiCheckTestProject.dll", @"TestProject\Version2\ApiCheckTestProject.dll", Category = "ApiTest", IgnoreListPath = @"TestProvider\ignore.txt")]
  // Some of these tests must fail. Exclude ApiTest category from CI.
  public class TestCaseSourceTest : ApiTest
  {
  }
}
