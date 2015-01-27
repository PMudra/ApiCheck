using ApiCheck.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ApiCheck.Test.Utility
{
  class TypeExtensionsTest
  {
    [Test]
    public void When_getting_compareable_name_from_type_with_generic_parameters_should_return_correct_string()
    {
      Type type = typeof(Tuple<IEnumerable<int>, double>);
      StringAssert.AreEqualIgnoringCase("System.Tuple`2<System.Collections.Generic.IEnumerable`1<System.Int32>,System.Double>", type.GetCompareableName());
    }
  }
}
