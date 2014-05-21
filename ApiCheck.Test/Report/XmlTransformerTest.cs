using System.IO;
using System.Xml.Linq;
using ApiCheck.Report;
using NUnit.Framework;

namespace ApiCheck.Test.Report
{
  class XmlTransformerTest
  {
    [Test]
    public void When_transforming_xml_to_html_output_is_generated()
    {
      XElement element = new XElement("ApiCheckResult");
      MemoryStream sut = new MemoryStream();
      Assert.DoesNotThrow(() => XmlTransformer.TransformToHtml(element.CreateReader(), sut));
      Assert.Greater(sut.Length, 0);
    }
  }
}
