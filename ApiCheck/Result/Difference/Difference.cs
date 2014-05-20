namespace ApiCheck.Result.Difference
{
  public abstract class Difference
  {
    private readonly Severity _severity;

    protected Difference(Severity severity)
    {
      _severity = severity;
    }

    public Severity Severity
    {
      get { return _severity; }
    }

    public override string ToString()
    {
      return string.Format("Severity: {0}", Severity.ToString());
    }
  }
}
