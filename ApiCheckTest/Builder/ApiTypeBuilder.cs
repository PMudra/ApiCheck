using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ApiCheckTest.Builder
{
  internal class ApiTypeBuilder
  {
    private TypeBuilder _typeBuilder;
    private ApiBuilder _parent;
    private readonly IList<GenericParam> _genericParameters;

    private ApiTypeBuilder()
    {
      _genericParameters = new List<GenericParam>();
    }

    internal static ApiTypeBuilder Type(ApiBuilder parent, string name, TypeAttributes attributes, Type extends, Type[] interfaces)
    {
      ApiTypeBuilder apiTypeBuilder = new ApiTypeBuilder
      {
        _typeBuilder = parent.ModuleBuilder.DefineType(name, attributes, extends, interfaces),
        _parent = parent
      };
      return apiTypeBuilder;
    }

    public ApiTypeBuilder GenericParameter(string name, GenericParameterAttributes genericParameterAttributes = GenericParameterAttributes.None, Type baseType = null, Type[] interfaces = null)
    {
      _genericParameters.Add(new GenericParam(name, genericParameterAttributes, baseType, interfaces));
      return this;
    }

    private class GenericParam
    {
      private readonly string _name;
      private readonly GenericParameterAttributes _genericParameterAttributes;
      private readonly Type _baseType;
      private readonly Type[] _interfaces;

      public GenericParam(string name, GenericParameterAttributes genericParameterAttributes, Type baseType, Type[] interfaces)
      {
        _name = name;
        _genericParameterAttributes = genericParameterAttributes;
        _baseType = baseType;
        _interfaces = interfaces;
      }

      public string Name
      {
        get { return _name; }
      }

      public GenericParameterAttributes GenericParameterAttributes
      {
        get { return _genericParameterAttributes; }
      }

      public Type BaseType
      {
        get { return _baseType; }
      }

      public Type[] Interfaces
      {
        get { return _interfaces; }
      }
    }

    public ApiMethodBuilder Method(string name = "MyMethod", Type returnType = null, MethodAttributes attributes = MethodAttributes.Public)
    {
      return ApiMethodBuilder.Method(this, name, attributes, returnType);
    }

    public ApiMethodBuilder Constructor(MethodAttributes attributes = MethodAttributes.Public)
    {
      return ApiMethodBuilder.Method(this, attributes);
    }

    public ApiTypeBuilder Property(string name, Type propertyType, bool hasSetter = true, bool hasGetter = true, Type[] indexParameters = null, bool @static = false)
    {
      PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(name, PropertyAttributes.None, propertyType, indexParameters);

      if (hasGetter)
      {
        MethodBuilder getMethodBuilder = _typeBuilder.DefineMethod(string.Format("get_{0}", name), @static ?
          MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Static
          : MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                                   propertyType, indexParameters);

        ILGenerator getIlGenerator = getMethodBuilder.GetILGenerator();
        getIlGenerator.Emit(ApiBuilderHelper.GetReturnOpCodeByType(propertyType));
        getIlGenerator.Emit(OpCodes.Ret);
        propertyBuilder.SetGetMethod(getMethodBuilder);
      }
      if (hasSetter)
      {
        MethodBuilder setMethodBuilder = _typeBuilder.DefineMethod(string.Format("set_{0}", name), @static ?
          MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Static
          : MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
          null, new[] { propertyType });

        ILGenerator setIlGenerator = setMethodBuilder.GetILGenerator();
        setIlGenerator.Emit(OpCodes.Ret);
        propertyBuilder.SetSetMethod(setMethodBuilder);
      }
      return this;
    }

    public ApiTypeBuilder Field(string name, Type fieldType, FieldAttributes attributes = FieldAttributes.Public)
    {
      _typeBuilder.DefineField(name, fieldType, attributes);
      return this;
    }

    public ApiTypeBuilder Event(string name, Type eventType, EventAttributes attributes = EventAttributes.None, bool @static = false)
    {
      EventBuilder eventBuilder = _typeBuilder.DefineEvent(name, attributes, eventType);

      MethodBuilder addMethodBuilder = _typeBuilder.DefineMethod(string.Format("add_{0}", name),
        @static ? MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Static :
        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual
        , null, new[] { eventType });
      addMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);

      MethodBuilder removeMethodBuilder = _typeBuilder.DefineMethod(string.Format("remove_{0}", name),
        @static ? MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Static :
        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual
        , null, new[] { eventType });
      removeMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);

      eventBuilder.SetAddOnMethod(addMethodBuilder);
      eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
      return this;
    }

    internal TypeBuilder TypeBuilder
    {
      get { return _typeBuilder; }
    }

    public ApiBuilder Build()
    {
      if (_genericParameters.Any())
      {
        GenericTypeParameterBuilder[] genericTypeParameterBuilders = _typeBuilder.DefineGenericParameters(_genericParameters.Select(param => param.Name).ToArray());
        foreach (GenericTypeParameterBuilder genericTypeParameterBuilder in genericTypeParameterBuilders)
        {
          GenericParam genericParam = _genericParameters.First(param => param.Name == genericTypeParameterBuilder.Name);
          genericTypeParameterBuilder.SetBaseTypeConstraint(genericParam.BaseType);
          genericTypeParameterBuilder.SetGenericParameterAttributes(genericParam.GenericParameterAttributes);
          genericTypeParameterBuilder.SetInterfaceConstraints(genericParam.Interfaces);
        }
      }
      _typeBuilder.CreateType();
      return _parent;
    }

  }
}
