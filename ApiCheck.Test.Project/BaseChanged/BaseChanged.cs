namespace ApiCheck.Test.Project.BaseChanged
{
  public class MyBase<T> {}
  public interface IMyInterface<T> {}
#if VERSION1
  public class BaseChanged : MyBase<int> {}
  public class InterfaceChanged : IMyInterface<int> {}
#else
  public class BaseChanged : MyBase<string> {}
  public class InterfaceChanged : IMyInterface<string> {}
#endif
}
