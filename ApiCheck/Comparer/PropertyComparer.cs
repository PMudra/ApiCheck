using ApiCheck.Result;
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
        ComparerResult.AddChangedProperty("Type", ReferenceType.PropertyType.GetCompareableName(), NewType.PropertyType.GetCompareableName(), Severities.PropertyTypeChanged);
      }
      if ((ReferenceType.CanWrite && ReferenceType.SetMethod.IsPublic) != (NewType.CanWrite && NewType.SetMethod.IsPublic))
      {
          if (ReferenceType.CanWrite && ReferenceType.SetMethod.IsPublic)
          {
              ComparerResult.AddRemovedItem(ResultContext.Property, ReferenceType.Name, Severities.PropertySetterRemoved);
          }
          else
          {
              ComparerResult.AddChangedFlag("Setter", ReferenceType.CanWrite, Severities.PropertySetterChanged);
          }
      }
      if ((ReferenceType.CanRead && ReferenceType.GetMethod.IsPublic) != (NewType.CanRead && NewType.GetMethod.IsPublic))
      {
          if (ReferenceType.CanRead && ReferenceType.GetMethod.IsPublic)
          {
              ComparerResult.AddRemovedItem(ResultContext.Property, ReferenceType.Name, Severities.PropertyGetterRemoved);
          }
          else
          {
              ComparerResult.AddChangedFlag("Getter", ReferenceType.CanRead, Severities.PropertyGetterChanged);
          }
      }
      bool referenceStatic = (ReferenceType.GetMethod != null && ReferenceType.GetMethod.IsStatic) || (ReferenceType.SetMethod != null && ReferenceType.SetMethod.IsStatic);
      bool newStatic = (NewType.GetMethod != null && NewType.GetMethod.IsStatic) || (NewType.SetMethod != null && NewType.SetMethod.IsStatic);
      if (referenceStatic != newStatic)
      {
        ComparerResult.AddChangedFlag("Static", referenceStatic, Severities.StaticPropertyChanged);
      }
      return ComparerResult;
    }
  }
}
