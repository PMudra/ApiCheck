using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class AssemblyComparer : ComparerBase<Assembly>
  {
    public AssemblyComparer(Assembly referenceType, Assembly newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Assembly, referenceType.GetName().Name)
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogInfo(string.Format("Comparing assemblies '{0}' and '{1}'", ReferenceType.FullName, NewType.FullName));
      CompareAssemblyInformation();
      CompareTypes();
      return ComparerResult;
    }

    private void CompareAssemblyInformation()
    {
      AssemblyName referenceName = new AssemblyName(ReferenceType.FullName);
      AssemblyName newName = new AssemblyName(NewType.FullName);
      AddToResultIfNotEqual("Name", referenceName.Name, newName.Name, Severity.Error);
      AddToResultIfNotEqual("Version", referenceName.Version.ToString(), newName.Version.ToString(), Severity.Hint);
      AddToResultIfNotEqual("Culture", referenceName.CultureInfo.Name, newName.CultureInfo.Name, Severity.Warning);
      AddToResultIfNotEqual("Public Key Token", ConvertToHexString(referenceName.GetPublicKeyToken()), ConvertToHexString(newName.GetPublicKeyToken()), Severity.Error);
    }

    private void CompareTypes()
    {
      IEnumerable<string> referenceTypes = GetTypeNames(() => ReferenceType.DefinedTypes).ToList();
      IEnumerable<string> newTypes = GetTypeNames(() => NewType.DefinedTypes).ToList();

      // Missing types
      foreach (string type in referenceTypes.Except(newTypes))
      {
        ComparerResult.AddRemovedItem(GetItemType(ReferenceType.GetType(type)), type, Severity.Error);
      }

      // New types
      foreach (string type in newTypes.Except(referenceTypes))
      {
        ComparerResult.AddAddedItem(GetItemType(NewType.GetType(type)), type, Severity.Warning);
      }

      // Equal types
      foreach (string type in referenceTypes.Intersect(newTypes))
      {
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(ReferenceType.GetType(type), NewType.GetType(type)).Compare());
      }
    }

    private void AddToResultIfNotEqual(string propertyName, string referenceValue, string newValue, Severity severity)
    {
      if (referenceValue != newValue)
      {
        ComparerResult.AddChangedProperty(propertyName, referenceValue, newValue, severity);
      }
    }

    private static string ConvertToHexString(byte[] bytes)
    {
      return String.Concat(Array.ConvertAll(bytes, b => b.ToString("x2")));
    }

    private IEnumerable<string> GetTypeNames(Func<IEnumerable<TypeInfo>> getTypes)
    {
      return getTypes().Where(type => type.IsVisible && ComparerContext.IsNotIgnored(type)).Select(type => type.GetCompareableName());
    }
  }

}
