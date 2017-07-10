using CommandLine;
using CommandLine.Text;

namespace ApiCheck.Console
{
  internal class Options
  {
    [Option('r', "reference", Required = true, HelpText = "A relative or absolute path for the reference assembly file.")]
    public string ReferencePath { get; set; }

    [Option('n', "new", Required = true, HelpText = "A relative or absolute path for the new assembly file.")]
    public string NewPath { get; set; }

    [Option('h', "html", HelpText = "A relative or absolute path for a html report file. If the file exists it will be overridden.")]
    public string HtmlPath { get; set; }

    [Option('x', "xml", HelpText = "A relative or absolute path for a xml report file. If the file exists it will be overridden.")]
    public string XmlPath { get; set; }

    [Option('c', "config", HelpText = "A relative or absolute path for a comparer configuration file containing the severity levels of the changed API elements and which elements to ignore.")]
    public string ConfigPath { get; set; }

    [Option('v', "verbose", HelpText = "Runs comparer with logging and prints information to the console.")]
    public bool Verbose { get; set; }

    [HelpOption]
    public string GetHelp()
    {
      HelpText helpText = new HelpText
      {
        AdditionalNewLineAfterOption = true,
        AddDashesToOption = true
      };
      helpText.AddPreOptionsLine("Usage: ApiCheck.Console.exe -r <reference assembly> -n <new assembly> [-x <xml report>] [-h <html report>] [-c <config file>] [-v]");
      helpText.AddOptions(this);
      return helpText;
    }
  }
}
