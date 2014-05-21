using System.Reflection;
using ApiCheck.Comparer;
using Moq;

namespace ApiCheck.Test.Comparer
{
  static class MockSetup
  {
    public static void SetupMock(Mock<IComparerContext> mock)
    {
      mock.Setup(context => context.IsNotIgnored(It.IsAny<TypeInfo>())).Returns(true);
      mock.Setup(context => context.IsNotIgnored(It.IsAny<MethodInfo>())).Returns(true);
      mock.Setup(context => context.IsNotIgnored(It.IsAny<ConstructorInfo>())).Returns(true);
      mock.Setup(context => context.IsNotIgnored(It.IsAny<PropertyInfo>())).Returns(true);
      mock.Setup(context => context.IsNotIgnored(It.IsAny<EventInfo>())).Returns(true);
      mock.Setup(context => context.IsNotIgnored(It.IsAny<FieldInfo>())).Returns(true);

      mock.Setup(context => context.CreateComparer(It.IsAny<ConstructorInfo>(), It.IsAny<ConstructorInfo>())).Returns((ConstructorInfo info1, ConstructorInfo info2) => new ConstructorComparer(info1, info2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<TypeInfo>(), It.IsAny<TypeInfo>())).Returns((TypeInfo type1, TypeInfo type2) => new TypeComparer(type1, type2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<EventInfo>(), It.IsAny<EventInfo>())).Returns((EventInfo object1, EventInfo object2) => new EventComparer(object1, object2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<FieldInfo>(), It.IsAny<FieldInfo>())).Returns((FieldInfo object1, FieldInfo object2) => new FieldComparer(object1, object2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<MethodInfo>(), It.IsAny<MethodInfo>())).Returns((MethodInfo object1, MethodInfo object2) => new MethodComparer(object1, object2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<ParameterInfo>(), It.IsAny<ParameterInfo>())).Returns((ParameterInfo object1, ParameterInfo object2) => new ParameterComparer(object1, object2, mock.Object));
      mock.Setup(context => context.CreateComparer(It.IsAny<PropertyInfo>(), It.IsAny<PropertyInfo>())).Returns((PropertyInfo object1, PropertyInfo object2) => new PropertyComparer(object1, object2, mock.Object));
    }
  }
}
