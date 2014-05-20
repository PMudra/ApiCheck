using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ApiCheckTest.Builder
{
  internal class ApiMethodBuilder
  {
    private ApiTypeBuilder _parent;
    private string _name;
    private MethodAttributes _methodAttributes;
    private Type _returnType;
    private readonly IList<ApiParameter> _parameters = new List<ApiParameter>();
    private bool _ctor;

    private ApiMethodBuilder()
    {
    }

    internal static ApiMethodBuilder Method(ApiTypeBuilder parent, string name, MethodAttributes attributes, Type returnType)
    {
      return new ApiMethodBuilder
      {
        _parent = parent,
        _name = name,
        _methodAttributes = attributes,
        _returnType = returnType
      };
    }

    internal static ApiMethodBuilder Method(ApiTypeBuilder parent, MethodAttributes attributes)
    {
      return new ApiMethodBuilder
      {
        _parent = parent,
        _methodAttributes = attributes,
        _ctor = true
      };
    }

    public ApiMethodBuilder Parameter(Type type, string name = "param", ParameterAttributes attributes = ParameterAttributes.None)
    {
      _parameters.Add(new ApiParameter
        {
          Type = type,
          Name = name,
          Attributes = attributes
        });
      return this;
    }

    public ApiMethodBuilder DefaultParameter(Type type, object defaultValue, string name = "param", ParameterAttributes attributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault)
    {
      _parameters.Add(new ApiParameter
      {
        Type = type,
        Name = name,
        DefaultValue = defaultValue,
        Attributes = attributes
      });
      return this;
    }

    public ApiTypeBuilder Build()
    {
      if (_ctor)
      {
        ConstructorBuilder constructorBuilder = _parent.TypeBuilder.DefineConstructor(_methodAttributes, CallingConventions.Standard, _parameters.Select(parameter => parameter.Type).ToArray());
        DefineParameters(constructorBuilder.DefineParameter);
        ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ret);
      }
      else
      {
        MethodBuilder methodBuilder = _parent.TypeBuilder.DefineMethod(_name, _methodAttributes, _returnType, _parameters.Select(parameter => parameter.Type).ToArray());
        DefineParameters(methodBuilder.DefineParameter);
        ILGenerator ilGenerator = methodBuilder.GetILGenerator();
        ilGenerator.Emit(ApiBuilderHelper.GetReturnOpCodeByType(_returnType));
        ilGenerator.Emit(OpCodes.Ret);
      }
      return _parent;
    }

    private void DefineParameters(Func<int, ParameterAttributes, string, ParameterBuilder> defineParameter)
    {
      foreach (ApiParameter parameter in _parameters)
      {
        ParameterBuilder parameterBuilder = defineParameter(_parameters.IndexOf(parameter) + 1, parameter.Attributes, parameter.Name);
        if (parameterBuilder.IsOptional)
        {
          parameterBuilder.SetConstant(parameter.DefaultValue);
        }
      }
    }

    private class ApiParameter
    {
      public string Name { get; set; }
      public Type Type { get; set; }
      public object DefaultValue { get; set; }
      public ParameterAttributes Attributes { get; set; }
    }
  }
}
