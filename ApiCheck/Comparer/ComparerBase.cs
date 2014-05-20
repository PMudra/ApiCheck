using System;
using ApiCheck.Result;

namespace ApiCheck.Comparer
{
  internal abstract class ComparerBase<T> : IComparer where T : class
  {
    private readonly T _referenceType;
    private readonly T _newType;
    private readonly IComparerContext _comparerContext;
    private readonly IComparerResult _comparerResult;

    protected ComparerBase(T referenceType, T newType, IComparerContext comparerContext, ResultContext resultContext, string name)
    {
      _referenceType = referenceType;
      _newType = newType;
      _comparerContext = comparerContext;
      _comparerResult = _comparerContext.CreateComparerResult(resultContext, name);
    }

    protected T ReferenceType
    {
      get { return _referenceType; }
    }

    protected T NewType
    {
      get { return _newType; }
    }

    protected IComparerContext ComparerContext
    {
      get { return _comparerContext; }
    }

    protected IComparerResult ComparerResult
    {
      get { return _comparerResult; }
    }

    public abstract IComparerResult Compare();

    public static ResultContext GetItemType(Type type)
    {
      if (type.IsClass)
      {
        return type.IsAbstract ? ResultContext.AbstractClass : ResultContext.Class;
      }
      if (type.IsEnum)
      {
        return ResultContext.Enum;
      }
      if (type.IsInterface)
      {
        return ResultContext.Interface;
      }
      if (type.IsValueType)
      {
        return ResultContext.Struct;
      }
      throw new ArgumentException(string.Format("Type '{0}' not supported", type), "type");
    }

    public override string ToString()
    {
      return string.Format("Comparing {0} '{1}'", ComparerResult.ResultContext, ComparerResult.Name);
    }
  }
}
