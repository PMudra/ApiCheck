using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace ApiCheck.Test.Builder
{
  internal class ApiBuilder
  {
    private readonly AssemblyBuilder _assemblyBuilder;
    private readonly ModuleBuilder _moduleBuilder;

    private ApiBuilder(string name, string version, string culture)
    {
      AssemblyName assemblyName = new AssemblyName
      {
        Name = name,
        Version = Version.Parse(version),
        CultureInfo = CultureInfo.CreateSpecificCulture(culture),
      };
      _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MyModule");
    }

    public static ApiBuilder CreateApi(string name = "MyAssembly", string version = "1.0.0.0", string culture = "de-DE")
    {
      return new ApiBuilder(name, version, culture);
    }

    public ApiTypeBuilder Class(string name = "MyClass", Type extends = null, Type[] interfaces = null, TypeAttributes attributes = TypeAttributes.Public)
    {
      return ApiTypeBuilder.Type(this, name, attributes, extends, interfaces);
    }

    public ApiTypeBuilder AbstractClass(string name = "MyAbstractClass", Type extends = null, Type[] interfaces = null)
    {
      return ApiTypeBuilder.Type(this, name, TypeAttributes.Public | TypeAttributes.Abstract, extends, interfaces);
    }

    public ApiTypeBuilder Interface(string name = "MyInterface", Type[] interfaces = null)
    {
      return ApiTypeBuilder.Type(this, name, TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract, null, interfaces);
    }

    public ApiBuilder Enum(string name = "MyEnum")
    {
      _moduleBuilder.DefineEnum(name, TypeAttributes.Public, typeof (int)).CreateType();
      return this;
    }

    internal ModuleBuilder ModuleBuilder
    {
      get { return _moduleBuilder; }
    }

    public Assembly Build()
    {
      return _assemblyBuilder;
    }
  }
}
