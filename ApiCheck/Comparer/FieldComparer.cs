using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class FieldComparer : ComparerBase<FieldInfo>
  {
    public FieldComparer(FieldInfo referenceType, FieldInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Field, referenceType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing field '{0}'", ReferenceType));
      if (ReferenceType.FieldType.GetCompareableName() != NewType.FieldType.GetCompareableName())
      {
        ComparerResult.AddChangedProperty("Type", ReferenceType.FieldType.GetCompareableName(), NewType.FieldType.GetCompareableName(), Severity.Error);
      }
      if (ReferenceType.IsStatic != NewType.IsStatic)
      {
        ComparerResult.AddChangedFlag("Static", ReferenceType.IsStatic, Severity.Error);
      }
      if (ReferenceType.IsStatic && NewType.IsStatic && ReferenceType.FieldType.IsEnum)
      {
        // compare numeric enum values
        object referenceValue = ReferenceType.GetRawConstantValue();
        object newValue = NewType.GetRawConstantValue();
        if (Convert.ToInt32(referenceValue) != Convert.ToInt32(newValue))
        {
          ComparerResult.AddChangedProperty("Value", referenceValue.ToString(), newValue.ToString(), Severity.Error);
        }
      }
      return ComparerResult;
    }
  }
}
