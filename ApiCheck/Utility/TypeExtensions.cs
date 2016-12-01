using System;
using System.Linq;
using System.Reflection;

namespace ApiCheck.Utility
{
  internal static class TypeExtensions
  {
    public static PropertyInfo[] GetApiProperties(this Type type)
    {
      return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    public static PropertyInfo GetApiProperty(this Type type, string name, Type[] types)
    {
      return type.GetApiProperties().Single(property => property.Name == name && property.GetIndexParameterTypes().SequenceEqual(types));
    }

    public static MethodInfo[] GetApiMethods(this Type type)
    {
      return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(method => !method.IsSpecialName).ToArray();
    }

    public static ConstructorInfo[] GetApiConstructors(this Type type)
    {
      return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    public static EventInfo[] GetApiEvents(this Type type)
    {
      return type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
    }

    public static FieldInfo[] GetApiFields(this Type type)
    {
      return type.GetFields();
    }

    public static ConstructorInfo GetApiConstructor(this Type type, string name, Type[] types)
    {
      ConstructorInfo constructor = type.GetConstructor(types);
      return constructor ?? type.GetApiConstructors().First(ctor => ctor.Name == name);
    }

    public static string GetCompareableName(this Type type)
    {
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
      {
        return string.Format("{0}.{1}<{2}>", type.Namespace, type.Name, string.Join(",", type.GetGenericArguments().Select(t => t.GetCompareableName())));
      }
      if (type.IsArray)
      {
        return string.Format("{0}.{1}[{2}]", type.Namespace, type.Name, GetCompareableName(type.GetElementType()));
      }
      return type.FullName;
    }
  }
}
