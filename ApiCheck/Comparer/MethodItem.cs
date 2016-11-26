using ApiCheck.Utility;
using System;
using System.Linq;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class MethodItem : Item
  {
    private readonly string[] _genericStrings;
    private readonly MethodBase _method;

    public MethodItem(MethodBase method)
      : base(method.Name, method.GetParameters().Select(param => param.ParameterType).ToArray())
    {
      _method = method;

      Type[] genericTypes = method.IsGenericMethod ? method.GetGenericArguments() : new Type[0];

      _genericStrings = genericTypes.Select(genericType => genericType.GetCompareableName()).ToArray();
    }

    public MethodBase Method
    {
      get { return _method; }
    }

    private bool Equals(MethodItem other)
    {
      return _genericStrings.SequenceEqual(other._genericStrings);
    }

    public override bool Equals(object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }
      if (!(obj is MethodItem))
      {
        return false;
      }
      return Equals((MethodItem)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashCode = base.GetHashCode();
        foreach (string genericString in _genericStrings)
        {
          hashCode = (hashCode * 231) ^ genericString.GetHashCode();
        }
        return hashCode;
      }
    }
  }
}
