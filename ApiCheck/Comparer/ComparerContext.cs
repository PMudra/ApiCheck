using ApiCheck.Result;
using ApiCheck.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using ApiCheck.Configuration;

namespace ApiCheck.Comparer
{
  internal class ComparerContext : IComparerContext
  {
    private readonly Action<string> _logInfo;
    private readonly Action<string> _logDetail;
    private readonly IList<string> _ignoredElements;
    private readonly Severities _severities;

    public ComparerContext(Action<string> logInfo, Action<string> logDetail, ComparerConfiguration configuration)
    {
      _logInfo = logInfo;
      _logDetail = logDetail;
      _ignoredElements = configuration.Ignore;
      _severities = configuration.Severities;
    }

    public Action<string> LogInfo
    {
      get { return _logInfo; }
    }

    public Action<string> LogDetail
    {
      get { return _logDetail; }
    }

    public Severities Severities
    {
      get { return _severities; }
    }

    public IComparerResult CreateComparerResult(ResultContext resultContext, string name)
    {
      return new ComparerResult(resultContext, name);
    }

    public IComparer CreateComparer(object referenceObject, object newObject)
    {
      if (referenceObject.GetType() != newObject.GetType())
      {
        throw new ArgumentException("Arguments must be of same type.");
      }
      
      if (referenceObject is Assembly)
      {
        return new AssemblyComparer((Assembly)referenceObject, (Assembly)newObject, this);
      }
      if (referenceObject is MethodInfo)
      {
        return new MethodComparer((MethodInfo)referenceObject, (MethodInfo)newObject, this);
      }
      if (referenceObject is ConstructorInfo)
      {
        return new ConstructorComparer((ConstructorInfo)referenceObject, (ConstructorInfo)newObject, this);
      }
      if (referenceObject is PropertyInfo)
      {
        return new PropertyComparer((PropertyInfo)referenceObject, (PropertyInfo)newObject, this);
      }
      if (referenceObject is TypeInfo)
      {
        return new TypeComparer((TypeInfo)referenceObject, (TypeInfo)newObject, this);
      }
      if (referenceObject is FieldInfo)
      {
        return new FieldComparer((FieldInfo)referenceObject, (FieldInfo)newObject, this);
      }
      if (referenceObject is EventInfo)
      {
        return new EventComparer((EventInfo)referenceObject, (EventInfo)newObject, this);
      }
      if (referenceObject is ParameterInfo)
      {
        return new ParameterComparer((ParameterInfo)referenceObject, (ParameterInfo)newObject, this);
      }
      throw new ArgumentException(string.Format("Type '{0}' not supported.", referenceObject.GetType()));
    }

    public bool IsNotIgnored(TypeInfo typeInfo)
    {
      return !_ignoredElements.Contains(typeInfo.GetCompareableName());
    }

    public bool IsNotIgnored(MethodInfo methodInfo)
    {
      return !_ignoredElements.Contains(methodInfo.GetCompareableName());
    }

    public bool IsNotIgnored(PropertyInfo propertyInfo)
    {
      return !_ignoredElements.Contains(propertyInfo.GetCompareableName());
    }

    public bool IsNotIgnored(EventInfo eventInfo)
    {
      return !_ignoredElements.Contains(eventInfo.GetCompareableName());
    }

    public bool IsNotIgnored(FieldInfo fieldInfo)
    {
      return !_ignoredElements.Contains(fieldInfo.GetCompareableName());
    }

    public bool IsNotIgnored(ConstructorInfo constructorInfo)
    {
      return !_ignoredElements.Contains(constructorInfo.GetCompareableName());
    }
  }
}
