using ApiCheck.Result.Difference;
using System.Collections.Generic;
using System.Linq;

namespace ApiCheck.Result
{
  internal class ComparerResult : IComparerResult
  {
    private readonly ResultContext _resultContext;
    private readonly IList<Changed<string>> _changedProperties;
    private readonly IList<Changed<bool>> _changedFlags;
    private readonly IList<AddedOrRemoved> _addedItems;
    private readonly IList<AddedOrRemoved> _removedItems;
    private readonly IList<IComparerResult> _comparerResults;
    private readonly string _name;

    public ComparerResult(ResultContext resultContext, string name)
    {
      _resultContext = resultContext;
      _name = name;
      _changedProperties = new List<Changed<string>>();
      _changedFlags = new List<Changed<bool>>();
      _addedItems = new List<AddedOrRemoved>();
      _removedItems = new List<AddedOrRemoved>();
      _comparerResults = new List<IComparerResult>();
    }

    public IEnumerable<Changed<string>> ChangedProperties
    {
      get { return _changedProperties; }
    }

    public IEnumerable<AddedOrRemoved> AddedItems
    {
      get { return _addedItems; }
    }

    public IEnumerable<AddedOrRemoved> RemovedItems
    {
      get { return _removedItems; }
    }

    public IEnumerable<Changed<bool>> ChangedFlags
    {
      get { return _changedFlags; }
    }

    public IEnumerable<IComparerResult> ComparerResults
    {
      get { return _comparerResults; }
    }

    public int GetAllCount(Severity severity, bool ignoreChildren)
    {
      var count = ((IEnumerable<Difference.Difference>)AddedItems).Union(RemovedItems).Union(ChangedFlags).Union(ChangedProperties).Count(addedOrRemoved => addedOrRemoved.Severity == severity);
      return count + (ignoreChildren ? 0 : ComparerResults.Sum(comparerResult => comparerResult.GetAllCount(severity, false)));
    }

    public ResultContext ResultContext
    {
      get { return _resultContext; }
    }

    public string Name
    {
      get { return _name; }
    }

    public void AddChangedProperty(string propertyName, string referenceValue, string newValue, Severity severity)
    {
      _changedProperties.Add(new Changed<string>(propertyName, referenceValue ?? string.Empty, newValue ?? string.Empty, severity));
    }

    public void AddChangedFlag(string flagName, bool referenceValue, Severity severity)
    {
      _changedFlags.Add(new Changed<bool>(flagName, referenceValue, !referenceValue, severity));
    }

    public void AddRemovedItem(ResultContext resultContext, string itemName, Severity severity)
    {
      _removedItems.Add(new AddedOrRemoved(resultContext, itemName, severity));
    }

    public void AddAddedItem(ResultContext resultContext, string itemName, Severity severity)
    {
      _addedItems.Add(new AddedOrRemoved(resultContext, itemName, severity));
    }

    public void AddComparerResult(IComparerResult comparerResult)
    {
      _comparerResults.Add(comparerResult);
    }

    public override string ToString()
    {
      return string.Format("Result of '{0}' ({1})", Name, ResultContext);
    }
  }
}
