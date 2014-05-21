using System.Linq;
using ApiCheck.Comparer;
using NUnit.Framework;

namespace ApiCheck.Test.Comparer
{
  class PairListTest
  {
    [Test]
    public void When_adding_referenceItem_should_be_added_to_list()
    {
      var sut = new PairList<string>();
      sut.AddReferenceItem("old");
      sut.AddNewItem("new");

      Assert.AreEqual(1, sut.RemovedItems.Count());
      Assert.AreEqual(1, sut.AddedItems.Count());
    }

    [Test]
    public void When_adding_newItem_should_be_added_to_list()
    {
      var sut = new PairList<string>();
      sut.AddNewItem("");

      Assert.AreEqual(1, sut.AddedItems.Count());
    }

    [Test]
    public void When_adding_reference_and_new_item_should_be_added_to_equal_list()
    {
      var sut = new PairList<string>();
      sut.AddReferenceItem("");
      sut.AddNewItem("");

      Assert.AreEqual(1, sut.EqualItems.Count());
    }

    [Test]
    public void When_adding_new_and_reference_item_should_be_added_to_equal_list()
    {
      var sut = new PairList<string>();
      sut.AddNewItem("");
      sut.AddReferenceItem("");

      Assert.AreEqual(1, sut.EqualItems.Count());
    }
  }
}
