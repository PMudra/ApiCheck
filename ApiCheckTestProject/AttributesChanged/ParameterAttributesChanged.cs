namespace ApiCheckTestProject.AttributesChanged
{
  public class ParameterAttributesChanged
  {
#if VERSION1
    public void NameChanged(int i) {}
    public void OutChanged(out int i) { i = 0; }
    public void DefaultChanged(string s = "default") {}
#else
    public void NameChanged(int j) {}
    public void OutChanged(ref int i) {}
    public void DefaultChanged(string s = "def") {}
#endif
  }
}
