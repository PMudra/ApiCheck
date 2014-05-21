using ApiCheck.Result;
using ApiCheck.Result.Difference;
using NUnit.Framework;

namespace ApiCheck.Test.Result
{
  class ComparerResultTest
  {
    [Test]
    public void When_calling_getAllCount_mehod_should_return_correct_count()
    {
      ComparerResult sut = new Builder().Added().Added().Removed(Severity.Warning)
                                          .Result().Property(Severity.Warning).Result().Flag()._._
                                        .Build();

      Assert.AreEqual(3, sut.GetAllCount(Severity.Error));
      Assert.AreEqual(2, sut.GetAllCount(Severity.Warning));
    }

    private class Builder
    {
      private readonly Builder _parent;
      private readonly ComparerResult _comparerResult;

      public Builder(ResultContext resultContext = ResultContext.Assembly, string name = null)
        : this(null, new ComparerResult(resultContext, name)) { }

      private Builder(Builder parent, ComparerResult comparerComparerResult)
      {
        _parent = parent;
        _comparerResult = comparerComparerResult;
      }

      public Builder Added(Severity severity = Severity.Error, ResultContext resultContext = ResultContext.Class, string itemName = "Item")
      {
        _comparerResult.AddAddedItem(resultContext, itemName, severity);
        return this;
      }

      public Builder Removed(Severity severity = Severity.Error, ResultContext resultContext = ResultContext.Class, string itemName = "Item")
      {
        _comparerResult.AddRemovedItem(resultContext, itemName, severity);
        return this;
      }

      public Builder Flag(Severity severity = Severity.Error, string flagName = "Value", bool value = false)
      {
        _comparerResult.AddChangedFlag(flagName, value, severity);
        return this;
      }

      public Builder Property(Severity severity = Severity.Error, string name = "Name", string refValue = "ref", string newValue = "new")
      {
        _comparerResult.AddChangedProperty(name, refValue, newValue, severity);
        return this;
      }

      public Builder Result(ResultContext resultContext = ResultContext.Class, string name = null)
      {
        ComparerResult comparerResult = new ComparerResult(resultContext, name);
        _comparerResult.AddComparerResult(comparerResult);
        return new Builder(this, comparerResult);
      }

      public Builder _
      {
        get { return _parent; }
      }

      public ComparerResult Build()
      {
        return _comparerResult;
      }
    }
  }
}
