using System;
using System.Linq;
using ApiCheck.Utility;

namespace ApiCheck.Comparer
{
  internal class Item
  {
    private readonly string _name;
    private readonly string[] _paramStrings;
    private readonly Type[] _types;

    public Item(string name, Type[] types)
    {
      _name = name;
      _types = types;
      _paramStrings = _types.Select(paramType => paramType.GetCompareableName()).ToArray();
    }

    public string Name
    {
      get { return _name; }
    }

    public Type[] Types
    {
      get { return _types; }
    }

    private bool Equals(Item other)
    {
      return String.Equals(_name, other._name) && _paramStrings.SequenceEqual(other._paramStrings);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != typeof(Item))
      {
        return false;
      }
      return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashCode = (_name != null ? _name.GetHashCode() : 0);
        foreach (string paramString in _paramStrings)
        {
          hashCode = (hashCode * 397) ^ paramString.GetHashCode();
        }
        return hashCode;
      }
    }
  }
}
