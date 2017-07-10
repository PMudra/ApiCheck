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
      AddToResultIfNotEqual("Name", referenceName.Name, newName.Name, Severities.AssemblyNameChanged);
      AddToResultIfNotEqual("Version", referenceName.Version.ToString(), newName.Version.ToString(), Severities.AssemblyVersionChanged);
      AddToResultIfNotEqual("Culture", referenceName.CultureInfo.Name, newName.CultureInfo.Name, Severities.AssemblyCultureChanged);
      AddToResultIfNotEqual("Public Key Token", ConvertToHexString(referenceName.GetPublicKeyToken()), ConvertToHexString(newName.GetPublicKeyToken()), Severities.AssemblyPublicKeyTokenChanged);
    }

    private void CompareTypes()
    {
      IEnumerable<string> referenceTypes = GetTypeNames(() => ReferenceType.DefinedTypes).ToList();
      IEnumerable<string> newTypes = GetTypeNames(() => NewType.DefinedTypes).ToList();

      // Missing types
      foreach (string type in referenceTypes.Except(newTypes))
      {
        ComparerResult.AddRemovedItem(GetItemType(ReferenceType.GetType(type)), type, Severities.TypeRemoved);
      }

      // New types
      foreach (string type in newTypes.Except(referenceTypes))
      {
        ComparerResult.AddAddedItem(GetItemType(NewType.GetType(type)), type, Severities.TypeAdded);
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
