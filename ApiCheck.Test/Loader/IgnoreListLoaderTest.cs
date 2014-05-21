using System.Collections.Generic;
using System.IO;
using System.Text;
using ApiCheck.Loader;
using NUnit.Framework;

namespace ApiCheck.Test.Loader
{
  class IgnoreListLoaderTest
  {
    [Test]
    public void When_reading_memory_stream_should_load_list()
    {
      IList<string> ignoreList = IgnoreListLoader.LoadIgnoreList(Builder.Stream);
      Assert.AreEqual("Line1", ignoreList[0]);
      Assert.AreEqual("Line2", ignoreList[1]);
    }

    private static class Builder
    {
      public static Stream Stream
      {
        get
        {
          Stream stream = new MemoryStream();
          using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 256, true) { AutoFlush = true })
          {
            streamWriter.WriteLine("Line1");
            streamWriter.WriteLine("Line2");
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
          }
        }
      }
    }
  }
}
