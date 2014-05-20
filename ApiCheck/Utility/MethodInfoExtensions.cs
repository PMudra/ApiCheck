using System.Reflection;

namespace ApiCheck.Utility
{
  internal static class MethodInfoExtensions
  {
    public static string GetCompareableName(this MethodInfo methodInfo)
    {
      return string.Format("{0}.{1}", methodInfo.ReflectedType.GetCompareableName(), methodInfo.Name);
    }
  }
}
