namespace ApiCheck.Test.Project.AddedOrRemoved
{
#if VERSION1
  public class TypeElementsAddedOrRemoved
  {
    public TypeElementsAddedOrRemoved(int i) {}
    public void OldMethod() {}
    public int oldField;
    public int OldProp { get; set; }
    public delegate void Delegate();
    public event Delegate OldEvent;
  }
#else
  public class TypeElementsAddedOrRemoved
  {
    public TypeElementsAddedOrRemoved(string s) { }
    public void NewMethod() { }
    public int newField;
    public int NewProp { get; set; }
    public delegate void Delegate();
    public event Delegate NewEvent;
  }
#endif
}
