using System;

namespace ApiCheck.Result.Difference
{
  internal class AddedOrRemoved : Difference
  {
    private readonly string _itemName;
    private readonly ResultContext _resultContext;

    public AddedOrRemoved(ResultContext resultContext, string itemName, Severity severity)
      : base(severity)
    {
      if (string.IsNullOrEmpty(itemName))
      {
        throw new ArgumentException("Value is null or empty", "itemName");
      }
      _itemName = itemName;
      _resultContext = resultContext;
    }

    public string ItemName
    {
      get { return _itemName; }
    }

    public ResultContext ResultContext
    {
      get { return _resultContext; }
    }

    public override string ToString()
    {
      return string.Format("{0} '{1}' has been added or removed. Severity: {2}", ResultContext, ItemName, Severity);
    }
  }
}
