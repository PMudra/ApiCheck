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
      CompareMethods(type => type.GetApiMethods().Where(method => ComparerContext.IsNotIgnored(method)).Cast<MethodBase>().ToArray(), ResultContext.Method);
      CompareMethods(type => type.GetApiConstructors().Where(ctor => ComparerContext.IsNotIgnored(ctor)).Cast<MethodBase>().ToArray(), ResultContext.Constructor);
      CompareProperties();
      CompareEvents();
      CompareFields();
      return ComparerResult;
    }

    private void CompareBase()
    {
      string referenceBase = ReferenceType.BaseType != null ? ReferenceType.BaseType.GetCompareableName() : null;
      string newBase = NewType.BaseType != null ? NewType.BaseType.GetCompareableName() : null;

      if (ReferenceType.BaseType == null)
      {
        // Adding a base class is subclass warning
        AddToResultIfNotEqual("Base", referenceBase, newBase, Severities.BaseAdded);
      }
      else if (ReferenceType.BaseType != null && NewType.BaseType != null && IsSubclassOf(NewType.BaseType.BaseType, referenceBase))
      {
        // Adding a new base class that extends the old one is subclass warning
        AddToResultIfNotEqual("Base", referenceBase, newBase, Severities.BaseAdded);
      }
      else
      {
        // Everything else should be an error or the base class didn't change
        AddToResultIfNotEqual("Base", referenceBase, newBase, Severities.BaseChanged);
      }
    }

    private bool IsSubclassOf(Type subclass, string oldBase)
    {
      if (subclass == null)
      {
        return false;
      }
      if (subclass.GetCompareableName() == oldBase)
      {
        return true;
      }
      return IsSubclassOf(subclass.BaseType, oldBase);
    }

    private void CompareMethods(Func<Type, MethodBase[]> getMethods, ResultContext resultContext)
    {
      PairList<MethodItem> pairList = new PairList<MethodItem>();
      getMethods(ReferenceType).ToList().ForEach(m => pairList.AddReferenceItem(new MethodItem(m)));
      getMethods(NewType).ToList().ForEach(m => pairList.AddNewItem(new MethodItem(m)));

      foreach (MethodItem item in pairList.RemovedItems)
      {
        ComparerResult.AddRemovedItem(resultContext, item.Method.ToString(), Severities.MethodRemoved);
      }

      foreach (MethodItem item in pairList.AddedItems)
      {
        ComparerResult.AddAddedItem(resultContext, item.Method.ToString(), Severities.MethodAdded);
      }

      foreach (ItemPair<MethodItem> itemPair in pairList.EqualItems)
      {
        MethodBase referenceMethod = itemPair.ReferenceItem.Method;
        MethodBase newMethod = itemPair.NewItem.Method;
        ComparerResult.AddComparerResult(ComparerContext.CreateComparer(referenceMethod, newMethod).Compare());
      }
    }

    private void CompareFields()
    {
      IEnumerable<string> referenceFields = GetFields(ReferenceType).ToList();
      IEnumerable<string> newFields = GetFields(NewType).ToList();

      // missing fields
      foreach (string field in referenceFields.Except(newFields))
      {
        ComparerResult.AddRemovedItem(ResultContext.Field, field, Severities.FieldRemoved);
      }

      // new fields
      foreach (string field in newFields.Except(referenceFields))
      {
        ComparerResult.AddAddedItem(ResultContext.Field, field, Severities.FieldAdded);
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
      IEnumerable<string> referenceInterfaces = GetInterfaces(ReferenceType).ToList();
      IEnumerable<string> newInterfaces = GetInterfaces(NewType).ToList();

      // missing interfaces
      foreach (string @interface in referenceInterfaces.Except(newInterfaces))
      {
        ComparerResult.AddRemovedItem(ResultContext.Interface, @interface, Severities.InterfacesRemoved);
      }

      // new interfaces
      foreach (string @interface in newInterfaces.Except(referenceInterfaces))
      {
        ComparerResult.AddAddedItem(ResultContext.Interface, @interface, Severities.InterfacesAdded);
      }
    }

    private IEnumerable<string> GetInterfaces(TypeInfo typeInfo)
    {
      return typeInfo.GetInterfaces().Where(@interface => ComparerContext.IsNotIgnored(@interface.GetTypeInfo())).Select(@interface => @interface.GetCompareableName());
    }

    private void CompareEvents()
    {
      IEnumerable<string> referenceEvents = GetEvents(ReferenceType).ToList();
      IEnumerable<string> newEvents = GetEvents(NewType).ToList();

      // missing event
      foreach (string @event in referenceEvents.Except(newEvents))
      {
        ComparerResult.AddRemovedItem(ResultContext.Event, @event, Severities.EventRemoved);
      }

      // new event
      foreach (string @event in newEvents.Except(referenceEvents))
      {
        ComparerResult.AddAddedItem(ResultContext.Event, @event, Severities.EventAdded);
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
        ComparerResult.AddRemovedItem(ResultContext.Property, ReferenceType.GetApiProperty(property.Name, property.Types).ToString(), Severities.PropertyRemoved);
      }

      foreach (Item property in pairList.AddedItems)
      {
        ComparerResult.AddAddedItem(ResultContext.Property, NewType.GetApiProperty(property.Name, property.Types).ToString(), Severities.PropertyAdded);
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
        ComparerResult.AddChangedFlag("Enum", ReferenceType.IsEnum, Severities.EnumChanged);
      }
      AddToResultIfNotEqual("Static", TypeAttributes.Sealed | TypeAttributes.Abstract, Severities.StaticTypeChanged);
      if (ComparerResult.ChangedFlags.All(change => change.PropertyName != "Static"))
      {
        AddToResultIfNotEqual("Abstract", TypeAttributes.Abstract, Severities.AbstractTypeChanged);
        AddToResultIfNotEqual("Sealed", TypeAttributes.Sealed, Severities.SealedTypeChanged);
      }
      AddToResultIfNotEqual("Interface", TypeAttributes.Interface, Severities.InterfaceChanged);
      AddToResultIfNotEqual("Serializable", TypeAttributes.Serializable, Severities.SerializableChanged);
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
