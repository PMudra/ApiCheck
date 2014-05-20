using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace ApiCheck.Report
{
  internal static class XmlTransformer
  {
    public static void TransformToHtml(XmlReader input, Stream output)
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      Stream styleStream = assembly.GetManifestResourceStream("ApiCheck.Report.HtmlReport.xslt");
      if (styleStream == null)
      {
        throw new Exception("Embedded xslt file could not be opened.");
      }
      using (XmlReader stylesheet = XmlReader.Create(styleStream))
      {
        XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
        xslCompiledTransform.Load(stylesheet);
        xslCompiledTransform.Transform(input, null, output);
      }
    }
  }
}
