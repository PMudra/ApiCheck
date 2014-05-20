namespace ApiCheckTestProject.AttributesChanged
{
  public class FieldAttributesChanged
  {
#if VERSION1
    public int TypeChanged;
    public static int StaticChanged;
#else
    public string TypeChanged;
    public int StaticChanged;
#endif
  }
}
