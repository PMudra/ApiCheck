using System.Reflection;
using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheckTest.Builder;
using Moq;
using NUnit.Framework;

namespace ApiCheckTest.Comparer
{
  [TestFixture]
  class ConstructorComparerTest
  {
    [Test]
    public void When_static_flag_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Constructor(MethodAttributes.Public | MethodAttributes.Static).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Constructor().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddAddedItem(ResultContext.Constructor, It.IsAny<string>(), Severity.Warning), Times.Once);
      sut.Verify(report => report.AddRemovedItem(ResultContext.Constructor, It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_nothing_changed_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Constructor().Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Constructor().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(report => report.AddAddedItem(ResultContext.Constructor, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(report => report.AddRemovedItem(ResultContext.Constructor, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(report => report.AddComparerResult(It.IsAny<IComparerResult>()), Times.Once);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(ResultContext.Class, It.IsAny<string>())).Returns(_comparerResultMock.Object);
        
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
