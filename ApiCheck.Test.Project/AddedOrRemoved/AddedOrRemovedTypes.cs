namespace ApiCheck.Test.Project.AddedOrRemoved
{
#if VERSION1
  public class OldClass { }
  public enum OldEnum { }
  public abstract class OldAbstract<T> { }
  public interface OldInterface<T> { }
  public class OldGeneric<T> { }
  public struct OldStruct { }
#else
  public class NewClass {}
  public enum NewEnum {}
  public abstract class NewAbstract<T> {}
  public interface NewInterface<T> {}
  public class NewGeneric<T> {}
  public struct NewStruct {}
#endif
  public class ParentClass
  {
#if VERSION1
    public class OldNested<T> {}
    public static class Nested {}
#else
    public class NewNested<T> {}
    public class Nested {}
#endif
  }
}
