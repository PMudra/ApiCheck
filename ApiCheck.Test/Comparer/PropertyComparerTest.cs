using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Test.Builder;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace ApiCheck.Test.Comparer
{
  class PropertyComparerTest
  {
    [Test]
    public void When_property_has_different_type_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(string)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int)).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Type", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_property_has_no_setter_and_getter_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), false).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasGetter: false).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedFlag("Setter", false, Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedFlag("Getter", true, Severity.Error), Times.Once);
    }

    [Test]
    public void When_static_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof (int), hasGetter: false, @static: true).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasGetter: false).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedFlag("Static", true, It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_properties_are_equal_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int)).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddChangedFlag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddComparerResult(It.IsAny<IComparerResult>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(ResultContext.Property, It.IsAny<string>())).Returns(_comparerResultMock.Object);
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
