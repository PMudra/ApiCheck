﻿using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Test.Builder;
using Moq;
using NUnit.Framework;
using System;
using System.Reflection;

namespace ApiCheck.Test.Comparer
{
  class FieldComparerTest
  {
    [Test]
    public void When_comparing_field_with_different_type_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(string)).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Type", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_static_changed_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(int), FieldAttributes.Public | FieldAttributes.Static).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(int)).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedFlag("Static", true, It.IsAny<Severity>()), Times.Once);
    }

    [Test]
    public void When_comparing_equal_type_should_not_report_anything()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Class().Field("MyField", typeof(int)).Build().Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddAddedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddChangedFlag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
      sut.Verify(result => result.AddComparerResult(It.IsAny<IComparerResult>()), Times.Never);
      sut.Verify(result => result.AddRemovedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    [Test]
    public void When_comparing_enum_values_with_different_numeric_value_should_report()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", 1) }).Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", 2) }).Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddChangedProperty("Value", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    [Test]
    public void When_comparing_enum_values_with_equal_numeric_value_should_not_report_anything()
    {
        Assembly assembly1 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", 1) }).Build();
        Assembly assembly2 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", 1) }).Build();
        Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

        sut.Verify(result => result.AddAddedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
        sut.Verify(result => result.AddChangedFlag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Severity>()), Times.Never);
        sut.Verify(result => result.AddChangedProperty(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
        sut.Verify(result => result.AddComparerResult(It.IsAny<IComparerResult>()), Times.Never);
        sut.Verify(result => result.AddRemovedItem(It.IsAny<ResultContext>(), It.IsAny<string>(), It.IsAny<Severity>()), Times.Never);
    }

    [Test]
    public void When_comparing_enum_types_with_different_type_should_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", 1) }).Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Enum("MyEnum", new[] { Tuple.Create("Value1", (byte)1) }).Build();
      Mock<IComparerResult> sut = new Builder(assembly1, assembly2).ComparerResultMock;

      sut.Verify(result => result.AddChangedProperty("Value", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
      sut.Verify(result => result.AddChangedProperty("EnumBaseType", It.IsAny<string>(), It.IsAny<string>(), Severity.Error), Times.Once);
    }

    private class Builder
    {
      private readonly Mock<IComparerResult> _comparerResultMock;

      public Builder(Assembly referenceAssembly, Assembly newAssembly)
      {
        _comparerResultMock = new Mock<IComparerResult>();
        Mock<IComparerContext> comparerContextMock = new Mock<IComparerContext> { DefaultValue = DefaultValue.Mock };
        comparerContextMock.Setup(context => context.CreateComparerResult(ResultContext.Field, It.IsAny<string>())).Returns(_comparerResultMock.Object);
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
