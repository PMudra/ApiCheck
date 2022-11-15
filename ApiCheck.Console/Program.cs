using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace ApiCheck.Console
{
  internal class Program
  {
    static int Main(string[] args)
    {
      var parserResult = new Parser(c => c.HelpWriter = null).ParseArguments<Options>(args);

      return parserResult.MapResult(Run, errs => DisplayHelp(parserResult));
    }

    private static int DisplayHelp<T>(ParserResult<T> parserResult)
    {
      var helpText = HelpText.AutoBuild(parserResult, h =>
      {
        h.AdditionalNewLineAfterOption = true;
        h.AddDashesToOption = true;
        h.AddPreOptionsLine("Usage: ApiCheck.Console.exe -r <reference assembly> -n <new assembly> [-x <xml report>] [-h <html report>] [-c <config file>] [-v]");
        return h;
      });

      System.Console.WriteLine(helpText);
      return -1;
    }

    private static int Run(Options options)
    {
      int returnValue = -1;

      try
      {
        returnValue = new Check(options.ReferencePath, options.NewPath, options.HtmlPath, options.XmlPath, options.ConfigPath, options.Verbose).CheckAssemblies();
      }
      catch (Exception exception)
      {
        System.Console.WriteLine(exception.Message);
      }
      return returnValue;
    }
  }
}
