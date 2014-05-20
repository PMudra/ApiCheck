using System;
using System.Linq;
using System.Reflection;

namespace ApiCheck.Utility
{
  internal static class PropertyInfoExtensions
  {
    public static Type[] GetIndexParameterTypes(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetIndexParameters().Select(param => param.ParameterType).ToArray();
    }

    public static string GetCompareableName(this PropertyInfo propertyInfo)
    {
      return string.Format("{0}.{1}", propertyInfo.DeclaringType.GetCompareableName(), propertyInfo.Name);
    }
  }
}
