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
      if (stream == null || stream.Length == 0)
      {
        return new ComparerConfiguration();
      }

      using (var reader = new StreamReader(stream))
      {
        IDeserializer deserializer = new DeserializerBuilder()
          .WithNamingConvention(CamelCaseNamingConvention.Instance)
          .Build();

        ComparerConfiguration configuration = deserializer.Deserialize<ComparerConfiguration>(reader);

        return configuration ?? new ComparerConfiguration();
      }
    }
  }
}