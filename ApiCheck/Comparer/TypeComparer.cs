using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class TypeComparer : ComparerBase<TypeInfo>
  {
    public TypeComparer(TypeInfo referenceType, TypeInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, GetItemType(referenceType), referenceType.GetCompareableName())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing {0} '{1}'", GetItemType(ReferenceType), ReferenceType.GetCompareableName()));
      CompareAttributes();
      CompareInterfaces();
      CompareBase();
      CompareMethods(type => type.GetApiMethods().Where(method => ComparerContext.IsNotIgnored(method)).Cast<MethodBase>().ToArray(), (type, name, types) => type.GetMethod(name, types), ResultContext.Method);
      CompareMethods(type => type.GetApiConstructors().Where(ctor => ComparerContext.IsNotIgnored(ctor)).Cast<MethodBase>().ToArray(), (type, name, types) => type.GetApiConstructor(name, types), ResultContext.Constructor);
      CompareProperties();
      CompareEvents();
      CompareFields();
      return ComparerResult;
    }

    private void CompareBase()
    {
      string referenceBase = ReferenceType.BaseType != null ? ReferenceType.BaseType.GetCompareableName() : null;
      string newBase = NewType.BaseType != null ? NewType.BaseType.GetCompareableName() : null;
      AddToResultIfNotEqual("Base", referenceBase, newBase, Severity.Error);
    }

    private void CompareMethods(Func<Type, MethodBase[]> getMethods, Func<Type, string, Type[], MethodBase> getMethod, ResultContext resultContext)
    {
      PairList<Item> pairList = new PairList<Item>();

      AddMethodsToPairList(pairList.AddReferenceItem, () => getMethods(ReferenceType));
      AddMethodsToPairList(pairList.AddNewItem, () => getMethods(NewType));

      foreach (Item method in pairList.RemovedItems)
      {
        ComparerResult.AddRemovedItem(resultContext, getMethod(ReferenceType, method.Name, method.Types).ToString(), Severity.Error);
      }

      foreach (Item method in pairList.AddedItems)
      {
        ComparerResult.AddAddedItem(resultContext, getMethod(NewType, method.Name, method.Types).ToString(), Severity.Warning);
      }

      foreach (ItemPair<Item> methodPair in pairList.EqualItems)
      {
        MethodBase referenceMethod = getMethod(ReferenceType, methodPair.ReferenceItem.Name, methodPair.ReferenceItem.Types);
        MethodBase newMethod = getMethod(NewType, methodPair.NewItem.Name, methodPair.NewItem.Types);
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(referenceMethod, newMethod).Compare());
      }
    }

    private static void AddMethodsToPairList(Action<Item> addToPairList, Func<MethodBase[]> getMethods)
    {
      foreach (MethodBase method in getMethods())
      {
        addToPairList(new Item(method.Name, method.GetParameters().Select(param => param.ParameterType).ToArray()));
      }
    }

    private void CompareFields()
    {
      IEnumerable<string> referenceFields = GetFields(ReferenceType).ToList();
      IEnumerable<string> newFields = GetFields(NewType).ToList();

      // missing fields
      foreach (string field in referenceFields.Except(newFields))
      {
        ComparerResult.AddRemovedItem(ResultContext.Field, field, Severity.Error);
      }

      // new fields
      foreach (string field in newFields.Except(referenceFields))
      {
        ComparerResult.AddAddedItem(ResultContext.Field, field, Severity.Warning);
      }

      // equal fields
      foreach (string field in referenceFields.Intersect(newFields))
      {
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(ReferenceType.GetField(field), NewType.GetField(field)).Compare());
      }
    }

    private IEnumerable<string> GetFields(TypeInfo typeInfo)
    {
      return typeInfo.GetApiFields().Where(field => ComparerContext.IsNotIgnored(field)).Select(field => field.Name);
    }

    private void CompareInterfaces()
    {
      IEnumerable<string> referenceInterfaces = ReferenceType.GetInterfaces().Select(@interface => @interface.GetCompareableName()).ToList();
      IEnumerable<string> newInterfaces = NewType.GetInterfaces().Select(@interface => @interface.GetCompareableName()).ToList();

      // missing interfaces
      foreach (string @interface in referenceInterfaces.Except(newInterfaces))
      {
        ComparerResult.AddRemovedItem(ResultContext.Interface, @interface, Severity.Error);
      }

      // new interfaces
      foreach (string @interface in newInterfaces.Except(referenceInterfaces))
      {
        ComparerResult.AddAddedItem(ResultContext.Interface, @interface, Severity.Warning);
      }
    }

    private void CompareEvents()
    {
      IEnumerable<string> referenceEvents = GetEvents(ReferenceType).ToList();
      IEnumerable<string> newEvents = GetEvents(NewType).ToList();

      // missing event
      foreach (string @event in referenceEvents.Except(newEvents))
      {
        ComparerResult.AddRemovedItem(ResultContext.Event, @event, Severity.Error);
      }

      // new event
      foreach (string @event in newEvents.Except(referenceEvents))
      {
        ComparerResult.AddAddedItem(ResultContext.Event, @event, Severity.Warning);
      }

      // equal events
      foreach (string @event in newEvents.Intersect(referenceEvents))
      {
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(ReferenceType.GetEvent(@event), NewType.GetEvent(@event)).Compare());
      }
    }

    private IEnumerable<string> GetEvents(TypeInfo typeInfo)
    {
      return typeInfo.GetApiEvents().Where(@event => ComparerContext.IsNotIgnored(@event)).Select(@event => @event.Name);
    }

    private void CompareProperties()
    {
      PairList<Item> pairList = new PairList<Item>();

      GetProperties(ReferenceType).ForEach(property => pairList.AddReferenceItem(new Item(property.Name, property.GetIndexParameterTypes())));
      GetProperties(NewType).ForEach(property => pairList.AddNewItem(new Item(property.Name, property.GetIndexParameterTypes())));

      foreach (Item property in pairList.RemovedItems)
      {
        ComparerResult.AddRemovedItem(ResultContext.Property, ReferenceType.GetApiProperty(property.Name, property.Types).ToString(), Severity.Error);
      }

      foreach (Item property in pairList.AddedItems)
      {
        ComparerResult.AddAddedItem(ResultContext.Property, NewType.GetApiProperty(property.Name, property.Types).ToString(), Severity.Warning);
      }

      foreach (ItemPair<Item> property in pairList.EqualItems)
      {
        PropertyInfo referenceProperty = ReferenceType.GetApiProperty(property.ReferenceItem.Name, property.ReferenceItem.Types);
        PropertyInfo newProperty = NewType.GetApiProperty(property.NewItem.Name, property.NewItem.Types);
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(referenceProperty, newProperty).Compare());
      }
    }

    private IEnumerable<PropertyInfo> GetProperties(TypeInfo typeInfo)
    {
      return typeInfo.GetApiProperties().Where(property => ComparerContext.IsNotIgnored(property));
    }

    private void CompareAttributes()
    {
      if (ReferenceType.IsEnum != NewType.IsEnum)
      {
        ComparerResult.AddChangedFlag("Enum", ReferenceType.IsEnum, Severity.Error);
      }
      AddToResultIfNotEqual("Static", TypeAttributes.Sealed | TypeAttributes.Abstract, Severity.Error);
      if (ComparerResult.ChangedFlags.All(change => change.PropertyName != "Static"))
      {
        AddToResultIfNotEqual("Abstract", TypeAttributes.Abstract, Severity.Error);
        AddToResultIfNotEqual("Sealed", TypeAttributes.Sealed, Severity.Error);
      }
      AddToResultIfNotEqual("Interface", TypeAttributes.Interface, Severity.Error);
      AddToResultIfNotEqual("Serializable", TypeAttributes.Serializable, Severity.Error);
    }

    private void AddToResultIfNotEqual(string propertyName, TypeAttributes typeAttribute, Severity severity)
    {
      bool referenceValue = ReferenceType.Attributes.HasFlag(typeAttribute);
      bool newValue = NewType.Attributes.HasFlag(typeAttribute);
      if (referenceValue != newValue)
      {
        ComparerResult.AddChangedFlag(propertyName, referenceValue, severity);
      }
    }

    private void AddToResultIfNotEqual(string propertyName, string referenceValue, string newValue, Severity severity)
    {
      if (referenceValue != newValue)
      {
        ComparerResult.AddChangedProperty(propertyName, referenceValue, newValue, severity);
      }
    }
  }
}
