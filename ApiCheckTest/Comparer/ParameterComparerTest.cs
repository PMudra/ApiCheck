using System.Reflection;
using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheckTest.Builder;
using Moq;
using NUnit.Framework;

namespace ApiCheckTest.Comparer
{
  class ParameterComparerTest
  {
    [Test]
    public void When_out_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int).MakeByRefType(), attributes: ParameterAttributes.Out).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int).MakeByRefType()).Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedFlag("Out", true, Severity.Error), Times.Once);
    }

    [Test]
    public void When_default_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().DefaultParameter(typeof(int), 1).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int)).Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Default Value", "1", "", Severity.Error), Times.Once);
    }

    [Test]
    public void When_name_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int), "myParam1").Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int), "myParam2").Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Name", "myParam1", "myParam2", Severity.Error), Times.Once);
    }

    [Test]
    public void When_default_value_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().DefaultParameter(typeof(string), "default1").Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().DefaultParameter(typeof(string), "default2").Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Default Value", "default1", "default2", Severity.Error), Times.Once);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(ResultContext.Parameter, It.IsAny<string>())).Returns(_comparerResultMock.Object);
        MockSetup.SetupMock(comparerContextMock);
        new AssemblyComparer(referenceAssembly, newAssembly, comparerContextMock.Object).Compare();
      }

      public Mock<IComparerResult> ComparerResultMock
      {
        get { return _comparerResultMock; }
      }
    }
  }
}
