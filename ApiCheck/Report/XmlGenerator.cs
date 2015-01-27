using ApiCheck.Result;
using ApiCheck.Result.Difference;
using System;
using System.Linq;
using System.Xml.Linq;

namespace ApiCheck.Report
{
  internal static class XmlGenerator
  {
    private const string Root = "ApiCheckResult";
    private const string Name = "Name";
    private const string AddedElement = "AddedElement";
    private const string RemovedElement = "RemovedElement";
    private const string ChangedAttribute = "ChangedAttribute";
    private const string Severity = "Severity";
    private const string NewValue = "NewValue";
    private const string ReferenceValue = "ReferenceValue";
    private const string Context = "Context";
    private const string ChangedElement = "ChangedElement";

    public static XElement GenerateXml(IComparerResult comparerResult)
    {
      if (comparerResult == null)
      {
        throw new ArgumentNullException("comparerResult");
      }
      return new XElement(Root, GenerateXElement(comparerResult));
    }

    private static XElement GenerateXElement(IComparerResult comparerResult)
    {
      XElement element = new XElement(ChangedElement);
      element.Add(new XAttribute(Name, comparerResult.Name));
      element.Add(new XAttribute(Context, comparerResult.ResultContext.ToString()));
      element.Add(comparerResult.ChangedFlags.Select(changed => new XElement(ChangedAttribute, GenerateXAttribute(changed))));
      element.Add(comparerResult.ChangedProperties.Select(changed => new XElement(ChangedAttribute, GenerateXAttribute(changed))));
      element.Add(comparerResult.AddedItems.Select(added => new XElement(AddedElement, GenerateXAttribute(added))));
      element.Add(comparerResult.RemovedItems.Select(removed => new XElement(RemovedElement, GenerateXAttribute(removed))));
      element.Add(comparerResult.ComparerResults.Select(GenerateXElement));
      return element.HasElements ? element : null;
    }

    private static object[] GenerateXAttribute<T>(Changed<T> changed)
    {
      return new object[]
        {
          new XAttribute(Name, changed.PropertyName),
          new XAttribute(ReferenceValue, changed.ReferenceValue),
          new XAttribute(NewValue, changed.NewValue),
          new XAttribute(Severity, changed.Severity)
        };
    }

    private static object[] GenerateXAttribute(AddedOrRemoved addedOrRemoved)
    {
      return new object[]
        {
          new XAttribute(Name, addedOrRemoved.ItemName),
          new XAttribute(Context, addedOrRemoved.ResultContext),
          new XAttribute(Severity, addedOrRemoved.Severity)
        };
    }
  }
}
