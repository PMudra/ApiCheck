using System;
using System.Collections.Generic;

namespace ApiCheck.Utility
{
  public static class EnumerableExtensions
  {
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
      foreach (TSource element in source)
      {
        action(element);
      }
    }
  }
}
