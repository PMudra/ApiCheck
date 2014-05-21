using System;
using System.Reflection.Emit;

namespace ApiCheck.Test.Builder
{
  internal static class ApiBuilderHelper
  {
    public static OpCode GetReturnOpCodeByType(Type type)
    {
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Int32:
          return OpCodes.Ldc_I4_0;
        case TypeCode.Object:
        case TypeCode.String:
          return OpCodes.Ldnull;
      }
      return OpCodes.Ldnull;
    }
  }
}
