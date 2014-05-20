using System.Reflection;
using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;

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
      return ComparerResult;
    }
  }
}
