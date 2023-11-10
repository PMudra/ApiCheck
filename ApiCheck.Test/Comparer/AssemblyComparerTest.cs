using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Test.Builder;
using Moq;
using NUnit.Framework;
using System;
using System.Reflection;

namespace ApiCheck.Test.Comparer
{
  class AssemblyComparerTest
  {
    [Test]
    public void When_assembly_properties_changed_the_report_contains_the_changes()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("AssemblyName2", "1.0.0.2", "en-US").Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), "AssemblyName2", Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), "en-US", Severity.Warning), Times.Once);
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), "1.0.0.2", Severity.Hint), Times.Once);
    }

    [Test]
    public void When_missing_different_types_in_new_assembly_report_should_contain_them()
    {
      Assembly assembly1 = ApiBuilder.CreateApi()
        .Class("Class1").Build()
        .AbstractClass("Abstract1").Build()
        .Enum<int>("Enum1")
        .Interface("Interface1").Build()
        .Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      
      sut.Verify(result => result.AddRemovedItem(ResultContext.Class, "Class1", Severity.Error), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Enum, "Enum1", Severity.Error), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.AbstractClass, "Abstract1", Severity.Error), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Interface, "Interface1", Severity.Error), Times.Once);
    }

    [Test]
    public void When_containing_different_additional_types_in_new_assembly_report_should_contain_them()
    {
      Assembly assembly1 = ApiBuilder.CreateApi()
        .Class("Class1").Build()
        .AbstractClass("Abstract1").Build()
        .Enum<int>("Enum1")
        .Interface("Interface1").Build()
        .Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Build();
      var sut = new Builder(assembly2, assembly1).ComparerResultMock;
      
      sut.Verify(result => result.AddAddedItem(ResultContext.Class, "Class1", Severity.Warning), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Enum, "Enum1", Severity.Warning), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.AbstractClass, "Abstract1", Severity.Warning), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Interface, "Interface1", Severity.Warning), Times.Once);
    }

    [Test]
    public void When_assemblies_are_equal_no_reports_are_generated()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddAddedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddChangedFlag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddComparerResult(It.IsAny<ComparerResult>()), Times.Never);
    }

    [Test]
    public void When_comparing_types_fullName_is_used()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("Class1").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class("Sub.Class1").Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      
      sut.Verify(result => result.AddAddedItem(ResultContext.Class, "Sub.Class1", It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Class, "Class1", It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_comparing_types_with_different_generic_params_they_are_not_compared()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("MyClass`2").GenericParameter("T").GenericParameter("B").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class("MyClass`1").GenericParameter("T").Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      
      sut.Verify(result => result.AddAddedItem(ResultContext.Class, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Class, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_comparing_types_with_same_generic_parameters_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("MyClass`1").GenericParameter("T").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class("MyClass`1").GenericParameter("T").Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Class, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Class, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    [Test]
    public void When_removing_a_public_nested_type_in_an_internal_class_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class(attributes: TypeAttributes.NotPublic).NestedType().Build().Build();

      Assembly assembly2 = ApiBuilder.CreateApi().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    private class Builder
    {
      private readonly IComparerResult _comparerResult;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        MockSetup.SetupMock(comparerContextMock);
        _comparerResult = new AssemblyComparer(referenceAssembly, newAssembly, comparerContextMock.Object).Compare();
      }

      public Mock<IComparerResult> ComparerResultMock
      {
        get { return Mock.Get(_comparerResult); }
      }
    }
  }
}
