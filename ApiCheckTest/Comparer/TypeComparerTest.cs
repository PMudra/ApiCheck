using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheckTest.Builder;
using Moq;
using NUnit.Framework;

namespace ApiCheckTest.Comparer
{
  class TypeComparerTest
  {
    [Test]
    public void When_class_is_changed_to_enum_changed_attributes_are_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("Class").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Enum("Class").Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      sut.Verify(result => result.AddChangedFlag("Enum", false, Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedFlag("Sealed", false, Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedFlag(It.IsNotIn(new[] { "Enum", "Sealed" }), It.IsAny<bool>(), It.IsAny<Severity>()), Times.Never);
    }

    [Test]
    public void When_serializable_class_is_changed_to_abstract_class_changed_attributes_are_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("Class", attributes: TypeAttributes.Public | TypeAttributes.Serializable).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().AbstractClass("Class").Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      sut.Verify(result => result.AddChangedFlag("Serializable", true, Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedFlag("Abstract", false, Severity.Error), Times.Once);
    }

    [Test]
    public void When_class_is_changed_to_interface_changed_attributes_are_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class("Class").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Interface("Class").Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      sut.Verify(result => result.AddChangedFlag("Interface", false, Severity.Error), Times.Once);
    }

    [Test]
    public void When_interfaces_changed_this_is_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class(interfaces: new[] { typeof(IEnumerable) })
        .Method("GetEnumerator", typeof(IEnumerator), MethodAttributes.Virtual | MethodAttributes.Public).Build()
        .Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class(interfaces: new[] { typeof(IDisposable) })
        .Method("Dispose", attributes: MethodAttributes.Virtual | MethodAttributes.Public).Build()
        .Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;
      sut.Verify(result => result.AddRemovedItem(ResultContext.Interface, typeof(IEnumerable).FullName, Severity.Error), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Interface, typeof(IDisposable).FullName, Severity.Warning), Times.Once);
    }

    [Test]
    public void When_base_class_changed_it_is_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class(extends: typeof(List<object>)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class(extends: typeof(List<int>)).Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Base", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_property_name_has_changed_it_is_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("Prop1", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("Prop2", typeof(int)).Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Property, It.IsAny<string>(), Severity.Warning), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Property, It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_field_name_has_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Field("Field1", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Field("Field2", typeof(int)).Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Field, "Field2", It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Field, "Field1", It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_event_name_changed_should_be_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Event("Event1", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Event("Event2", typeof(int)).Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddRemovedItem(ResultContext.Event, "Event1", It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Event, "Event2", It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_parameter_was_removed_from_method_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().Parameter(typeof(int)).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddRemovedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_method_hasnt_changed_nothing_is_reported()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddRemovedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddAddedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    [Test]
    public void When_parameter_was_removed_from_constructor_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Constructor().Parameter(typeof(int)).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Constructor().Build().Build().Build();
      var sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddRemovedItem(ResultContext.Constructor, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
      sut.Verify(result => result.AddAddedItem(ResultContext.Constructor, It.IsAny<string>(), It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_defining_two_properties_with_index_parameter_should_compare_without_exception()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Property("Prop", typeof(int), indexParameters: new[] { typeof(int) }).Property("Prop", typeof(int), indexParameters: new[] { typeof(string) }).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Property("Prop", typeof(int), indexParameters: new[] { typeof(int) }).Property("Prop", typeof(int), indexParameters: new[] { typeof(string) }).Build().Build();

      Mock<IComparerResult> sut;
      Assert.DoesNotThrow(() => sut = new Builder(assembly1, assembly2).ComparerResultMock);
    }

    [Test]
    public void When_method_did_not_change_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Method().Build().Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(ResultContext.Method, It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddComparerResult(It.IsAny<IComparerResult>()), Times.AtLeastOnce);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(It.IsIn(ResultContext.Class, ResultContext.Enum, ResultContext.Interface, ResultContext.AbstractClass), It.IsAny<string>())).Returns(_comparerResultMock.Object);
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
