using NUnit.Framework;

namespace ApiCheck.NUnit
{
  public abstract class ApiTest
  {
    [Test, ApiTestCaseSource]
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
  }
}
