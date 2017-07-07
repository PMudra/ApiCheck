using System.IO;
using System.Text;
using ApiCheck.Configuration;
using ApiCheck.Loader;
using ApiCheck.Result.Difference;
using NUnit.Framework;

namespace ApiCheck.Test.Loader
{
  class ConfigurationLoaderTest
  {
    [Test]
    public void When_reading_memory_stream_should_load_configuration()
    {
      ComparerConfiguration configuration = ConfigurationLoader.LoadComparerConfiguration(Builder.Stream);
      Assert.AreEqual("MyCompany.MyNamespace.MyClass.SomeMember", configuration.Ignore[0]);
      Assert.AreEqual("MyCompany.MyNamespace.MyClass2", configuration.Ignore[1]);
      Assert.AreEqual(Severity.Warning, configuration.Severities.ParameterNameChanged);
      Assert.AreEqual(Severity.Hint, configuration.Severities.AssemblyNameChanged);
    }

    private static class Builder
    {
      public static Stream Stream
      {
        get
        {
          string document = @"---
            # ApiCheck comparer configuration for MyProject

            # specifies which elements to exclude from the comparison
            # these can be types or members, listed by their fully qualified name
            ignore:   
              - MyCompany.MyNamespace.MyClass.SomeMember
              - MyCompany.MyNamespace.MyClass2 # exclude MyClass2 because it changes a lot 

            # override the default comparer severities
            # the rule names match the descriptions from the table above in camelCase
            # the severity levels can be set to error/warning/hint
            severities:
              parameterNameChanged: warning # globally set the check for changed parameter names to warning
              assemblyNameChanged: hint";

          Stream stream = new MemoryStream();

          using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 256, true) { AutoFlush = true })
          {
            streamWriter.Write(document);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
          }
        }
      }
    }
  }
}
