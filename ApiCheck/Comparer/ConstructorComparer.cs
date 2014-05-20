using System.Reflection;
using ApiCheck.Result;

namespace ApiCheck.Comparer
{
  internal class ConstructorComparer : MethodComparerBase<ConstructorInfo>
  {
    public ConstructorComparer(ConstructorInfo referenceType, ConstructorInfo newType, IComparerContext comparerContext)
      : base(referenceType, newType, comparerContext, ResultContext.Constructor, referenceType.ToString())
    {
    }

    public override IComparerResult Compare()
    {
      ComparerContext.LogDetail(string.Format("Comparing constructor '{0}'", ReferenceType));
      CompareParameters();
      return ComparerResult;
    }
  }
}
