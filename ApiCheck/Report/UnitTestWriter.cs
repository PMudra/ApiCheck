using System;
using System.Text;

namespace ApiCheck.Report
{
  internal class UnitTestWriter : IWriter
  {
    private int _offset;
    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public void StartElement(string name)
    {
      _stringBuilder.AppendLine(string.Format("{0}{1}", new String(' ', _offset), name));
      _offset += 4;
    }

    public void EndElement()
    {
      _offset -= 4;
    }

    public string GetTextAsString()
    {
      return _stringBuilder.ToString();
    }
  }
}
