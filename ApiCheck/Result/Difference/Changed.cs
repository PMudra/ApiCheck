using System;

namespace ApiCheck.Result.Difference
{
  public class Changed<T> : Difference
  {
    private readonly string _propertyName;
    private readonly T _referenceValue;
    private readonly T _newValue;

    public Changed(string propertyName, T referenceValue, T newValue, Severity severity)
      : base(severity)
    {
      if (propertyName == null || referenceValue == null || newValue == null)
      {
        throw new ArgumentNullException();
      }
      _propertyName = propertyName;
      _referenceValue = referenceValue;
      _newValue = newValue;
    }

    public T ReferenceValue
    {
      get { return _referenceValue; }
    }

    public T NewValue
    {
      get { return _newValue; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public override string ToString()
    {
      return string.Format("{0} has changed from '{1}' to '{2}'. Severity: {3}", PropertyName, ReferenceValue, NewValue, Severity);
    }
  }
}
