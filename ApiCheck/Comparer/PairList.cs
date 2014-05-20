using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiCheck.Comparer
{
  internal class PairList<T>
    where T : class
  {
    private readonly IList<ItemPair<T>> _pairs = new List<ItemPair<T>>();

    public void AddReferenceItem(T referenceItem)
    {
      ItemPair<T> pair = _pairs.SingleOrDefault(tuple => tuple.NewItem != null && tuple.NewItem.Equals(referenceItem));
      if (pair != null)
      {
        _pairs.Remove(pair);
        _pairs.Add(new ItemPair<T>(referenceItem, pair.NewItem));
      }
      else
      {
        _pairs.Add(new ItemPair<T>(referenceItem, null));
      }
    }

    public void AddNewItem(T newItem)
    {
      ItemPair<T> pair = _pairs.SingleOrDefault(tuple => tuple.ReferenceItem != null && tuple.ReferenceItem.Equals(newItem));
      if (pair != null)
      {
        _pairs.Remove(pair);
        _pairs.Add(new ItemPair<T>(pair.ReferenceItem, newItem));
      }
      else
      {
        _pairs.Add(new ItemPair<T>(null, newItem));
      }
    }

    public IEnumerable<ItemPair<T>> EqualItems
    {
      get { return _pairs.Where(tuple => tuple.ReferenceItem != null && tuple.NewItem != null); }
    }

    public IEnumerable<T> AddedItems
    {
      get { return _pairs.Where(tuple => tuple.ReferenceItem == null).Select(tuple => tuple.NewItem); }
    }

    public IEnumerable<T> RemovedItems
    {
      get { return _pairs.Where(tuple => tuple.NewItem == null).Select(tuple => tuple.ReferenceItem); }
    }
  }

  internal class ItemPair<TItem>
  {
    private readonly Tuple<TItem, TItem> _tuple;

    public ItemPair(TItem referenceItem, TItem newItem)
    {
      _tuple = new Tuple<TItem, TItem>(referenceItem, newItem);
    }

    public TItem ReferenceItem
    {
      get { return _tuple.Item1; }
    }

    public TItem NewItem
    {
      get { return _tuple.Item2; }
    }
  }
}
