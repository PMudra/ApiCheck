namespace ApiCheckTestProject.AttributesChanged
{
  public class Base
  {
    public virtual void Derived() { }
  }

  public abstract class MethodAttributeChanged : Base
  {
#if VERSION1
    public virtual void VirtualToNonVirtual() {}
    public static void StaticToInstance() {}
    public abstract void AbstractToNonAbstract();
    public int IntToString() { return 0; }
    public override void Derived() {}
#else
    public void VirtualToNonVirtual() {}
    public void StaticToInstance() {}
    public void AbstractToNonAbstract() {}
    public string IntToString() { return null; }
    public override sealed void Derived() {}
#endif
  }
}
