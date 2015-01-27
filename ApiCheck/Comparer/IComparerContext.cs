using ApiCheck.Result;
using System;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal interface IComparerContext
  {
    Action<string> LogInfo { get; }
    Action<string> LogDetail { get; }
    IComparerResult CreateComparerResult(ResultContext resultContext, string name);
    IComparer CreateComparer(object referenceObject, object newObject);
    bool IsNotIgnored(TypeInfo typeInfo);
    bool IsNotIgnored(MethodInfo methodInfo);
    bool IsNotIgnored(PropertyInfo propertyInfo);
    bool IsNotIgnored(EventInfo eventInfo);
    bool IsNotIgnored(FieldInfo fieldInfo);
    bool IsNotIgnored(ConstructorInfo constructorInfo);
  }
}
