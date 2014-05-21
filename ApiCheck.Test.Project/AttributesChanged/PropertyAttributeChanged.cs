namespace ApiCheck.Test.Project.AttributesChanged
{
  public class PropertyAttributeChanged
  {
#if VERSION1
    public int TypeChanged { get; set; }
    public int SetterChanged { get { return 0; } }
    public int GetterChanged { set { } }
    public static int StaticChanged { get; set; }
#else
    public string TypeChanged { get; set; }
    public int SetterChanged { get; set; }
    public int GetterChanged { set; get; }
    public int StaticChanged { get; set; }
#endif
  }
}
