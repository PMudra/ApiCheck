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

      sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Once);
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

    [Test]
    public void When_adding_internal_setter_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), false).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), true, setterInternal: true).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Never);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Never);
    }

    [Test]
    public void When_adding_internal_getter_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasGetter: false).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasGetter: true, getterInternal: true).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Never);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Never);
    }

    [Test]
    public void When_adding_public_setter_should_report_warning()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: false, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Once);
    }

    [Test]
    public void When_adding_public_getter_should_report_warning()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: false).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Once);
    }

    [Test]
    public void When_removing_public_setter_should_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: false, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Once);
    }


    [Test]
    public void When_removing_public_getter_should_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter:true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter:true, hasGetter: false).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Once);
    }

    [Test]
    public void When_removing_private_setter_should_not_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, setterInternal: true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: false, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Never);
    }

    [Test]
    public void When_removing_private_getter_should_not_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true, getterInternal: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: false).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Never);
    }

    [Test]
    public void When_making_public_setter_internal_should_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, setterInternal:true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Once);
    }

    [Test]
    public void When_making_public_getter_internal_should_report_error()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, setterInternal: true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddRemovedItem(ResultContext.Property, "MyProp", Severity.Error), Times.Once);
    }

    [Test]
    public void When_making_internal_setter_public_should_report_warning()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, setterInternal: true, hasGetter: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Once);
    }

    [Test]
    public void When_making_internal_getter_public_should_report_warning()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true, getterInternal: true).Build().Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("MyProp", typeof(int), hasSetter: true, hasGetter: true).Build().Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddAddedItem(ResultContext.Property, "MyProp", Severity.Warning), Times.Once);
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
