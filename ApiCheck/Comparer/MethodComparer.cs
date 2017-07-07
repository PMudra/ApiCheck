using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class MethodComparer : MethodComparerBase<MethodInfo>
  {
    public MethodComparer(MethodInfo referenceType, MethodInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Method, referenceType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing method '{0}'", ReferenceType));
      CompareAttributes();
      CompareReturnType();
      CompareParameters();
      return ComparerResult;
    }

    private void CompareReturnType()
    {
      if (ReferenceType.ReturnType.GetCompareableName() != NewType.ReturnType.GetCompareableName())
      {
        ComparerResult.AddChangedProperty("Return Type", ReferenceType.ReturnType.GetCompareableName(), NewType.ReturnType.GetCompareableName(), Severities.ReturnTypeChanged);
      }
    }

    private void CompareAttributes()
    {
      AddToResultIfNotEqual("Virtual", MethodAttributes.Virtual, Severities.VirtualMethodChanged);
      AddToResultIfNotEqual("Static", MethodAttributes.Static, Severities.StaticMethodChanged);
      AddToResultIfNotEqual("Abstract", MethodAttributes.Abstract, Severities.AbstractMethodChanged);
      AddToResultIfNotEqual("Sealed", MethodAttributes.Final, Severities.SealedMethodChanged);
    }

    private void AddToResultIfNotEqual(string propertyName, MethodAttributes typeAttribute, Severity severity)
    {
      bool referenceValue = ReferenceType.Attributes.HasFlag(typeAttribute);
      bool newValue = NewType.Attributes.HasFlag(typeAttribute);
      if (referenceValue != newValue)
      {
        ComparerResult.AddChangedFlag(propertyName, referenceValue, severity);
      }
    }
  }
}
