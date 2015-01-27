using ApiCheck.Result;
using ApiCheck.Result.Difference;
using ApiCheck.Utility;
using System.Reflection;

namespace ApiCheck.Comparer
{
  internal class EventComparer : ComparerBase<EventInfo>
  {
    public EventComparer(EventInfo referenceType, EventInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Event, referenceType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing event '{0}'", ReferenceType));
      if (ReferenceType.EventHandlerType.GetCompareableName() != NewType.EventHandlerType.GetCompareableName())
      {
        ComparerResult.AddChangedProperty("Type", ReferenceType.EventHandlerType.GetCompareableName(), NewType.EventHandlerType.GetCompareableName(), Severity.Error);
      }
      bool referenceStatic = (ReferenceType.AddMethod != null && ReferenceType.AddMethod.IsStatic) || (ReferenceType.RaiseMethod != null && ReferenceType.RaiseMethod.IsStatic);
      bool newStatic = (NewType.AddMethod != null && NewType.AddMethod.IsStatic) || (NewType.RaiseMethod != null && NewType.RaiseMethod.IsStatic);
      if (referenceStatic != newStatic)
      {
        ComparerResult.AddChangedFlag("Static", referenceStatic, Severity.Error);
      }
      return ComparerResult;
    }
  }
}
