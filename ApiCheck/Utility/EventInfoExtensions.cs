using System.Reflection;

namespace ApiCheck.Utility
{
  internal static class EventInfoExtensions
  {
    public static string GetCompareableName(this EventInfo eventInfo)
    {
      return string.Format("{0}.{1}", eventInfo.DeclaringType.GetCompareableName(), eventInfo.Name);
    }
  }
}
