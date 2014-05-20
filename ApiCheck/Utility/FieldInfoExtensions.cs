using System.Reflection;

namespace ApiCheck.Utility
{
  internal static class FieldInfoExtensions
  {
    public static string GetCompareableName(this FieldInfo fieldInfo)
    {
      return string.Format("{0}.{1}", fieldInfo.DeclaringType.GetCompareableName(), fieldInfo.Name);
    }
  }
}
