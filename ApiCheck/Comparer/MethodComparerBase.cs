using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiCheck.Result;

namespace ApiCheck.Comparer
{
  internal abstract class MethodComparerBase<T> : ComparerBase<T> where T : MethodBase
  {
    protected MethodComparerBase(T referenceType, T newType, IComparerContext comparerContext, ResultContext resultContext, string name)
      : base(referenceType, newType, comparerContext, resultContext, name)
    {
    }

    protected void CompareParameters()
    {
      IEnumerable<Tuple<ParameterInfo, ParameterInfo>> parameterPairs = ReferenceType.GetParameters().Zip(NewType.GetParameters(), (refParam, newParam) => new Tuple<ParameterInfo, ParameterInfo>(refParam, newParam));
      foreach (Tuple<ParameterInfo, ParameterInfo> parameterPair in parameterPairs)
      {
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(parameterPair.Item1, parameterPair.Item2).Compare());
      }
    }
  }
}
