using NUnit.Framework;
using System.IO;

namespace ApiCheck.Test
{
  [SetUpFixture]
  class ApiCheckTestFixture
  {
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
      Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
    }
  }
}