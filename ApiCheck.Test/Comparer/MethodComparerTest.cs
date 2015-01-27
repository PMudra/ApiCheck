using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Test.Builder;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace ApiCheck.Test.Comparer
{
  class MethodComparerTest
  {
    [Test]
    public void When_return_type_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method(returnType: typeof(int)).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method(returnType: typeof(double)).Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddChangedProperty("Return Type", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_static_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method(attributes: MethodAttributes.Public | MethodAttributes.Static).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddChangedFlag("Static", true, Severity.Error), Times.Once);
    }

    [Test]
    public void When_virtual_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method(attributes: MethodAttributes.Public | MethodAttributes.Virtual).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddChangedFlag("Virtual", true, Severity.Error), Times.Once);
    }

    [Test]
    public void When_abstract_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().AbstractClass().Method(attributes: MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().AbstractClass().Method().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddChangedFlag("Abstract", true, Severity.Error), Times.Once);
    }

    [Test]
    public void When_sealed_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method(attributes: MethodAttributes.Public | MethodAttributes.Final).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddChangedFlag("Sealed", true, Severity.Error), Times.Once);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(ResultContext.Method, It.IsAny<string>())).Returns(_comparerResultMock.Object);
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
