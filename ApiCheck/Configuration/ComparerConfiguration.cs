using System.Collections.Generic;

namespace ApiCheck.Configuration
{
  public class ComparerConfiguration
  {
    public ComparerConfiguration()
    {
      Ignore = new List<string>();
      Severities = new Severities();
    }

    public IList<string> Ignore { get; private set; }
    public Severities Severities { get; private set; }
  }
}