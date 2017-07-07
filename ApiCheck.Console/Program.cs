using System;

namespace ApiCheck.Console
{
  internal class Program
  {
    static int Main(string[] args)
    {
      Options options = new Options();
      int returnValue = -1;
      if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
      {
        try
        {
          returnValue = new Check(options.ReferencePath, options.NewPath, options.HtmlPath, options.XmlPath, options.ConfigPath, options.Verbose).CheckAssemblies();
        }
        catch (Exception exception)
        {
          System.Console.WriteLine(exception.Message);
        }
      }
      return returnValue;
    }
  }
}
