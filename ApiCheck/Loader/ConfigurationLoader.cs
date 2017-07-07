using System.IO;
using ApiCheck.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ApiCheck.Loader
{
  public static class ConfigurationLoader
  {
    public static ComparerConfiguration LoadComparerConfiguration(Stream stream)
    {
      if (stream == null)
      {
        return new ComparerConfiguration();
      }

      using (var reader = new StreamReader(stream))
      {
        var deserializer = new DeserializerBuilder()
          .WithNamingConvention(new CamelCaseNamingConvention())
          .Build();

        return deserializer.Deserialize<ComparerConfiguration>(reader);
      }
    }
  }
}