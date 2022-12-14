using System.IO;
using System.Reflection;
using System.Text;
using ApiCheck.Configuration;
using ApiCheck.Loader;
using ApiCheck.Result.Difference;
using NUnit.Framework;
using YamlDotNet.Core;

namespace ApiCheck.Test.Loader
{
  class ConfigurationLoaderTest
  {
    [Test]
    public void When_providing_null_stream_should_return_default_configuration()
    {
      ComparerConfiguration configuration = ConfigurationLoader.LoadComparerConfiguration(null);

      Assert.IsNotNull(configuration);
    }

    [Test]
    public void When_configuration_file_is_empty_should_return_default_configuration()
    {
      ComparerConfiguration configuration = ConfigurationLoader.LoadComparerConfiguration(new MemoryStream());

      Assert.IsNotNull(configuration);
    }

    [Test]
    public void When_configuration_file_only_has_comments_return_default_configuration()
    {
      string document = "# my project configuration";
      Stream stream = new Builder(document).Stream;

      ComparerConfiguration configuration = ConfigurationLoader.LoadComparerConfiguration(stream);

      Assert.IsNotNull(configuration);
    }

    [Test]
    public void When_configuration_file_has_unknown_properties_throws_exception()
    {
      string document = "someProperty: someValue";
      Stream stream = new Builder(document).Stream;

      Assert.Throws<YamlException>(() => ConfigurationLoader.LoadComparerConfiguration(stream));
    }

    [Test]
    public void When_configuration_file_has_ignore_property_not_in_camelcase_throws_exception()
    {
      string document = "Ignore: Company.NameSpace.Class";
      Stream stream = new Builder(document).Stream;

      Assert.Throws<YamlException>(() => ConfigurationLoader.LoadComparerConfiguration(stream));
    }

    [Test]
    public void When_configuration_file_has_severities_property_not_in_camelcase_throws_exception()
    {
      string document = @"Severities:
                            parameterNameChanged: warning";
      Stream stream = new Builder(document).Stream;

      Assert.Throws<YamlException>(() => ConfigurationLoader.LoadComparerConfiguration(stream));
    }

    [Test]
    public void When_configuration_file_has_unknown_severities_throws_exception()
    {
      string document = @"Severities:
                            unknownSeverity: error";
      Stream stream = new Builder(document).Stream;

      Assert.Throws<YamlException>(() => ConfigurationLoader.LoadComparerConfiguration(stream));
    }

    [Test]
    public void When_reading_example_configuration_should_load_configuration()
    {
      string document = File.ReadAllText(@"TestProvider\ExampleConfiguration.txt");
      Stream stream = new Builder(document).Stream;

      ComparerConfiguration configuration = ConfigurationLoader.LoadComparerConfiguration(stream);

      Assert.AreEqual("MyCompany.MyNamespace.MyClass.SomeMember", configuration.Ignore[0]);
      Assert.AreEqual("MyCompany.MyNamespace.MyClass2", configuration.Ignore[1]);
      Assert.AreEqual(Severity.Warning, configuration.Severities.ParameterNameChanged);
      Assert.AreEqual(Severity.Hint, configuration.Severities.AssemblyNameChanged);
    }

    private class Builder
    {
      private readonly Stream _stream;

      public Builder(string document)
      {
        _stream = new MemoryStream();

        using (StreamWriter streamWriter = new StreamWriter(_stream, Encoding.UTF8, 256, true) { AutoFlush = true })
        {
          streamWriter.Write(document);
          streamWriter.Flush();
          _stream.Seek(0, SeekOrigin.Begin);
        }
      }

      public Stream Stream
      {
        get { return _stream; }
      }
    }
  }
}
