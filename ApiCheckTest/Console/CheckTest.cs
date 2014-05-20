using System;
using System.IO;
using ApiCheckConsole;
using NUnit.Framework;

namespace ApiCheckTest.Console
{
  class CheckTest
  {
    [Test]
    public void When_passing_null_as_assembly_name_should_throw()
    {
      Assert.Throws<ArgumentNullException>(() => Builder.Create(referencePath: null));
      Assert.Throws<ArgumentNullException>(() => Builder.Create(newPath: null));
    }

    [Test]
    public void When_passing_null_for_all_other_values_should_not_throw()
    {
      Assert.DoesNotThrow(() => Builder.Create());
    }

    [Test]
    public void When_invalid_file_is_passed_should_throw()
    {
      Assert.Throws<BadImageFormatException>(() => Builder.Create(referencePath: @"TestProject\invalidDll.txt"));
      Assert.Throws<BadImageFormatException>(() => Builder.Create(newPath: @"TestProject\invalidDll.txt"));
      Assert.Throws<FileNotFoundException>(() => Builder.Create(referencePath: @"__Not__Found__"));
      Assert.Throws<FileNotFoundException>(() => Builder.Create(newPath: @"__Not__Found__"));
      Assert.Throws<FileNotFoundException>(() => Builder.Create(ignorePath: @"__Not__Found__"));
    }

    private static class Builder
    {
      public static Check Create(string referencePath = @"TestProject\Version1\ApiCheckTestProject.dll", string newPath = @"TestProject\Version2\ApiCheckTestProject.dll",
                                string htmlPath = null, string xmlPath = null, string ignorePath = null, bool verbose = false)
      {
        return new Check(referencePath, newPath, htmlPath, xmlPath, ignorePath, verbose);
      }
    }
  }
}
