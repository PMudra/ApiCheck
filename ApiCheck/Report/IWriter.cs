namespace ApiCheck.Report
{
  internal interface IWriter
  {
    void StartElement(string name);
    void EndElement();
  }
}
