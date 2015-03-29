using ApiCheck.Test.Builder;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ApiCheck.Test
{
  class ApiCheckTest
  {
    [Test]
    public void When_checking_with_logging_should_write_to_stream()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Build().Build();

      using (MemoryStream detailStream = new MemoryStream())
      using (MemoryStream infoStream = new MemoryStream())
      {
        StreamWriter infoLog = new StreamWriter(infoStream) { AutoFlush = true };
        StreamWriter detailLog = new StreamWriter(detailStream) { AutoFlush = true };

        ApiComparer.CreateInstance(assembly1, assembly2).WithInfoLogging(infoLog.WriteLine).WithDetailLogging(detailLog.WriteLine).Build().CheckApi();

        Assert.Greater(infoStream.Length, 0);
        Assert.Greater(detailStream.Length, 0);
      }
    }

    [Test]
    public void When_comparing_two_assemblies_reports_are_generated()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Build().Build();

      using (MemoryStream xmlStream = new MemoryStream())
      using (MemoryStream htmlStream = new MemoryStream())
      {
        ApiComparer.CreateInstance(assembly1, assembly2).WithHtmlReport(htmlStream).WithXmlReport(xmlStream).Build().CheckApi();

        Assert.Greater(htmlStream.Length, 0);
        Assert.Greater(xmlStream.Length, 0);
      }
    }

    [Test]
    public void When_passing_invalid_values_should_throw_exception()
    {
      Assert.Throws<ArgumentNullException>(() => ApiComparer.CreateInstance(null, null));
    }

    [Test]
    public void When_passing_null_should_not_throw()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Build();
      Assert.DoesNotThrow(
        () => ApiComparer.CreateInstance(assembly1, assembly2).WithDetailLogging(null).WithHtmlReport(null).WithIgnoreList(null).WithInfoLogging(null).WithXmlReport(null).Build().CheckApi());
    }

    [Test]
    [Category("SystemTest")]
    public void When_comparing_testProject_reporting_works()
    {
      Stream htmlReport = new MemoryStream();
      Stream xmlReport = new MemoryStream();
      Assembly newAssembly = Assembly.ReflectionOnlyLoadFrom(@"TestProject\Version2\ApiCheckTestProject.dll");
      Assembly referenceAssembly = Assembly.ReflectionOnlyLoadFrom(@"TestProject\Version1\ApiCheckTestProject.dll");
      int logCount = 0;

      int returnValue = -1;
      Assert.DoesNotThrow(
        () =>
        returnValue = ApiComparer.CreateInstance(referenceAssembly, newAssembly)
                  .WithDetailLogging(s => logCount++)
                  .WithInfoLogging(s => logCount++)
                  .WithHtmlReport(htmlReport)
                  .WithXmlReport(xmlReport)
                  .Build()
                  .CheckApi());

      Assert.Greater(htmlReport.Length, 0);
      Assert.Greater(xmlReport.Length, 0);
      Assert.Greater(logCount, 0);
      Assert.AreEqual(62, returnValue);
    }
  }
}
