using System.Collections.Generic;
using System.IO;

namespace ApiCheck.Loader
{
  public static class IgnoreListLoader
  {
    public static IList<string> LoadIgnoreList(Stream stream)
    {
      if (stream == null)
      {
        return new List<string>(0);
      }
      using (StreamReader streamReader = new StreamReader(stream))
      {
        IList<string> ignoreList = new List<string>();
        while (!streamReader.EndOfStream)
        {
          ignoreList.Add(streamReader.ReadLine());
        }
        return ignoreList;
      }
    }
  }
}
