namespace ApiCheckTestProject.AttributesChanged
{
  public class EventAttributesChanged
  {
    public delegate void Delegate1();
    public delegate void Delegate2();
#if VERSION1
    public event Delegate1 ChangeType;
    public static event Delegate1 ChangeStatic;
#else
    public event Delegate2 ChangeType;
    public event Delegate1 ChangeStatic;
#endif
  }
}
