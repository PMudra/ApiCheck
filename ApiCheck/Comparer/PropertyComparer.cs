using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class PropertyComparer : ComparerBase<PropertyInfo>
  {
    public PropertyComparer(PropertyInfo referenceType, PropertyInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Property, referenceType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing property '{0}'", ReferenceType));
      if (ReferenceType.PropertyType.GetCompareableName() != NewType.PropertyType.GetCompareableName())
      {
        ComparerResult.AddChangedProperty("Type", ReferenceType.PropertyType.GetCompareableName(), NewType.PropertyType.GetCompareableName(), Severity.Error);
      }
      if ((ReferenceType.CanWrite && ReferenceType.SetMethod.IsPublic) != (NewType.CanWrite && NewType.SetMethod.IsPublic))
      {
        ComparerResult.AddChangedFlag("Setter", ReferenceType.CanWrite, Severity.Error);
      }
      if ((ReferenceType.CanRead && ReferenceType.GetMethod.IsPublic) != (NewType.CanRead && NewType.GetMethod.IsPublic))
      {
        ComparerResult.AddChangedFlag("Getter", ReferenceType.CanRead, Severity.Error);
      }
      bool referenceStatic = (ReferenceType.GetMethod != null && ReferenceType.GetMethod.IsStatic) || (ReferenceType.SetMethod != null && ReferenceType.SetMethod.IsStatic);
      bool newStatic = (NewType.GetMethod != null && NewType.GetMethod.IsStatic) || (NewType.SetMethod != null && NewType.SetMethod.IsStatic);
      if (referenceStatic != newStatic)
      {
        ComparerResult.AddChangedFlag("Static", referenceStatic, Severity.Error);
      }
      return ComparerResult;
    }
  }
}
