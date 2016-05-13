using ApiCheck.Result.Difference;
using System.Collections.Generic;

namespace ApiCheck.Result
{
  public interface IComparerResult
  {
    int GetAllCount(Severity severity, bool ignoreChildren);
    ResultContext ResultContext { get; }
    string Name { get; }
    IEnumerable<Changed<string>> ChangedProperties { get; }
    IEnumerable<AddedOrRemoved> AddedItems { get; }
    IEnumerable<AddedOrRemoved> RemovedItems { get; }
    IEnumerable<IComparerResult> ComparerResults { get; }
    IEnumerable<Changed<bool>> ChangedFlags { get; }
    void AddChangedProperty(string propertyName, string referenceValue, string newValue, Severity severity);
    void AddRemovedItem(ResultContext resultContext, string itemName, Severity severity);
    void AddAddedItem(ResultContext resultContext, string itemName, Severity severity);
    void AddComparerResult(IComparerResult comparerResult);
    void AddChangedFlag(string flagName, bool referenceValue, Severity severity);
  }
}