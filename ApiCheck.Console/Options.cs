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

    [Option('i', "ignore", HelpText = "A relative or absolute path for a text file containing the names of the types to ignore.")]
    public string IgnorePath { get; set; }

    [Option('v', "verbose", HelpText = "Runs comparer with log and prints information to the console.")]
    public bool Verbose { get; set; }

    [HelpOption]
    public string GetHelp()
    {
      HelpText helpText = new HelpText
      {
        AdditionalNewLineAfterOption = true,
        AddDashesToOption = true
      };
      helpText.AddPreOptionsLine("Usage: ApiCheckConsole.exe -r <reference assembly> -n <new assembly> [-x <xml report>] [-h <html report>] [-i <ignore file>] [-v]");
      helpText.AddOptions(this);
      return helpText;
    }
  }
}
