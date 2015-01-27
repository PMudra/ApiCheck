
namespace ApiCheck.Test.Project.AttributesChanged
{
#if VERSION1
  public class ClassToEnum {}
  public sealed class SealedToClass {}
  public abstract class AbstractToClass {}
  public class ClassToInterface {}
  [Serializable] public class SerializableToClass {}
#else
  public enum ClassToEnum {}
  public class SealedToClass {}
  public class AbstractToClass {}
  public interface ClassToInterface {}
  public class SerializableToClass {}
#endif
}
