using ApiCheck.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ApiCheck.NUnit.Writer
{
  internal class Element
  {
    private readonly string _name;
    private readonly ISet<Element> _elements;

    public Element(string name)
    {
      _name = name;
      _elements = new HashSet<Element>();
    }

    private Element(string name, IEnumerable<Element> elements)
    {
      _name = name;
      _elements = new HashSet<Element>(elements.Where(element => element != null));
    }

    private void Add(Element element)
    {
      _elements.Add(element);
    }

    public void Add(string name, IEnumerable<string> children)
    {
      IEnumerable<string> elements = children.ToList();
      if (elements.Any())
      {
        Add(new Element(name, elements.Select(child => new Element(child))));
      }
    }

    public void Add(string name, IEnumerable<Element> children)
    {
      IEnumerable<Element> elements = children.Where(element => element != null).ToList();
      if (elements.Any())
      {
        Add(new Element(name, elements));
      }
    }

    public bool HasElements
    {
      get { return _elements.Count > 0; }
    }

    public void Write(IWriter writer)
    {
      writer.StartElement(_name);
      _elements.ForEach(element => element.Write(writer));
      writer.EndElement();
    }
  }
}
