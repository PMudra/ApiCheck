namespace ApiCheck.NUnit.Writer
{
  internal interface IWriter
  {
    void StartElement(string name);
    void EndElement();
  }
}
