using ApiCheck.Result;
using ApiCheck.Result.Difference;
using System;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class ParameterComparer : ComparerBase<ParameterInfo>
  {
    public ParameterComparer(ParameterInfo referenceType, ParameterInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Parameter, referenceType.ParameterType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing parameter '{0}'", ReferenceType.ParameterType));
      CompareAttributes();
      CompareName();
      CompareDefaultValue();
      return ComparerResult;
    }

    private void CompareDefaultValue()
    {
      if (!Equals(ReferenceType.RawDefaultValue, NewType.RawDefaultValue))
      {
        ComparerResult.AddChangedProperty("Default Value", (ReferenceType.RawDefaultValue ?? "null").ToString(), (NewType.RawDefaultValue ?? "null").ToString(), Severities.DefaultValueChanged);
      }
    }

    private void CompareName()
    {
      if (ReferenceType.Name != NewType.Name)
      {
        ComparerResult.AddChangedProperty("Name", ReferenceType.Name, NewType.Name, Severities.ParameterNameChanged);
      }
    }

    private void CompareAttributes()
    {
      AddToResultIfNotEqual("Out", param => param.IsOut, Severities.OutChanged);
    }

    private void AddToResultIfNotEqual(string propertyName, Func<ParameterInfo, bool> getFlag, Severity severity)
    {
      bool referenceValue = getFlag(ReferenceType);
      bool newValue = getFlag(NewType);
      if (referenceValue != newValue)
      {
        ComparerResult.AddChangedFlag(propertyName, referenceValue, severity);
      }
    }
  }
}
