using ApiCheck.Report;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using NUnit.Framework;
using System;
using System.Linq;
using System.Xml.Linq;

namespace ApiCheck.Test.Report
{
  class XmlGeneratorTest
  {
    private const string ChangedElement = "ChangedElement";
    private const string AddedElement = "AddedElement";
    private const string RemovedElement = "RemovedElement";
    private const string ChangedAttribute = "ChangedAttribute";

    [Test]
    public void When_passing_null_to_generator_should_throw_exception()
    {
      Assert.Throws<ArgumentNullException>(() => XmlGenerator.GenerateXml(null));
    }

    [Test]
    public void When_passing_result_with_no_content_it_is_not_exported()
    {
      IComparerResult result = new ComparerResult(ResultContext.Assembly, "Assembly");
      XElement sut = XmlGenerator.GenerateXml(result);

      Assert.AreEqual(0, sut.Elements(ChangedElement).Count());
    }

    [Test]
    public void When_passing_two_level_report_xml_should_contain_it()
    {
      IComparerResult result1 = new ComparerResult(ResultContext.Assembly, "Assembly");
      IComparerResult result2 = new ComparerResult(ResultContext.Class, "Class");
      result2.AddChangedFlag("dummy", true, Severity.Error);
      result1.AddComparerResult(result2);
      XElement sut = XmlGenerator.GenerateXml(result1);

      Assert.NotNull(sut.Element(ChangedElement).Element(ChangedElement));
    }

    [Test]
    public void When_passing_changed_flag_xml_should_contain_it()
    {
      IComparerResult result = new ComparerResult(ResultContext.Assembly, "Assembly");
      result.AddChangedFlag("Flag", true, Severity.Hint);
      XElement xml = XmlGenerator.GenerateXml(result);
      XElement sut = xml.Element("ChangedElement").Element("ChangedAttribute");

      Assert.AreEqual("Flag", sut.Attribute("Name").Value);
      StringAssert.AreEqualIgnoringCase(Severity.Hint.ToString(), sut.Attribute("Severity").Value);
      StringAssert.AreEqualIgnoringCase(true.ToString(), sut.Attribute("ReferenceValue").Value);
      StringAssert.AreEqualIgnoringCase(false.ToString(), sut.Attribute("NewValue").Value);
    }

    [Test]
    public void When_passing_changed_property_xml_should_contain_it()
    {
      IComparerResult result = new ComparerResult(ResultContext.Assembly, "Assembly");
      result.AddChangedProperty("PropName", "old", "new", Severity.Error);
      XElement xml = XmlGenerator.GenerateXml(result);
      XElement sut = xml.Element(ChangedElement).Element(ChangedAttribute);

      Assert.AreEqual("PropName", sut.Attribute("Name").Value);
      StringAssert.AreEqualIgnoringCase(Severity.Error.ToString(), sut.Attribute("Severity").Value);
      Assert.AreEqual("old", sut.Attribute("ReferenceValue").Value);
      Assert.AreEqual("new", sut.Attribute("NewValue").Value);
    }

    [Test]
    public void When_passing_new_item_xml_should_contain_it()
    {
      IComparerResult result = new ComparerResult(ResultContext.Assembly, "Assembly");
      result.AddAddedItem(ResultContext.Constructor, ".ctor", Severity.Warning);
      XElement xml = XmlGenerator.GenerateXml(result);
      XElement sut = xml.Element(ChangedElement).Element(AddedElement);

      Assert.AreEqual(".ctor", sut.Attribute("Name").Value);
      StringAssert.AreEqualIgnoringCase(Severity.Warning.ToString(), sut.Attribute("Severity").Value);
      StringAssert.AreEqualIgnoringCase(ResultContext.Constructor.ToString(), sut.Attribute("Context").Value);
    }

    [Test]
    public void When_passing_removed_item_xml_should_contain_it()
    {
      IComparerResult result = new ComparerResult(ResultContext.Assembly, "Assembly");
      result.AddRemovedItem(ResultContext.Constructor, ".ctor", Severity.Warning);
      XElement xml = XmlGenerator.GenerateXml(result);
      XElement sut = xml.Element(ChangedElement).Element(RemovedElement);

      Assert.AreEqual(".ctor", sut.Attribute("Name").Value);
      StringAssert.AreEqualIgnoringCase(Severity.Warning.ToString(), sut.Attribute("Severity").Value);
      StringAssert.AreEqualIgnoringCase(ResultContext.Constructor.ToString(), sut.Attribute("Context").Value);
    }
  }
}
