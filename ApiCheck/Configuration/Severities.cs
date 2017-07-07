  using ApiCheck.Result.Difference;

namespace ApiCheck.Configuration
{
  public class Severities
  {
    public Severities()
    {
      AssemblyNameChanged = Severity.Error;
      AssemblyVersionChanged = Severity.Hint;
      AssemblyCultureChanged = Severity.Warning;
      AssemblyPublicKeyTokenChanged = Severity.Error;
      TypeRemoved = Severity.Error;
      TypeAdded = Severity.Warning;
      NestedTypeRemoved = Severity.Error;
      NestedTypeAdded = Severity.Warning;

      EnumChanged = Severity.Error;
      StaticTypeChanged = Severity.Error;
      AbstractTypeChanged = Severity.Error;
      SealedTypeChanged = Severity.Error;
      InterfaceChanged = Severity.Error;
      SerializableChanged = Severity.Error;
      InterfacesRemoved = Severity.Error;
      InterfacesAdded = Severity.Warning;
      BaseAdded = Severity.Warning;
      BaseChanged = Severity.Error;
      MethodAdded = Severity.Warning;
      MethodRemoved = Severity.Error;
      ConstructorAdded = Severity.Warning;
      ConstructorRemoved = Severity.Error;
      PropertyAdded = Severity.Warning;
      PropertyRemoved = Severity.Error;
      EventAdded = Severity.Warning;
      EventRemoved = Severity.Error;
      FieldAdded = Severity.Warning;
      FieldRemoved = Severity.Error;

      VirtualMethodChanged = Severity.Error;
      StaticMethodChanged = Severity.Error;
      AbstractMethodChanged = Severity.Error;
      SealedMethodChanged = Severity.Error;
      ReturnTypeChanged = Severity.Error;
      ParameterNameChanged = Severity.Error;
      DefaultValueChanged = Severity.Error;
      OutChanged = Severity.Error;

      PropertyTypeChanged = Severity.Error;
      PropertySetterChanged = Severity.Error;
      PropertyGetterChanged = Severity.Error;
      StaticPropertyChanged = Severity.Error;

      FieldTypeChanged = Severity.Error;
      StaticFieldChanged = Severity.Error;
      ConstEnumValueChanged = Severity.Error;

      EventTypeChanged = Severity.Error;
      StaticEventChanged = Severity.Error;
    }

    public Severity AssemblyNameChanged { get; set; }
    public Severity AssemblyVersionChanged { get; set; }
    public Severity AssemblyCultureChanged { get; set; }
    public Severity AssemblyPublicKeyTokenChanged { get; set; }
    public Severity TypeRemoved { get; set; }
    public Severity TypeAdded { get; set; }
    public Severity NestedTypeRemoved { get; set; }
    public Severity NestedTypeAdded { get; set; }

    public Severity EnumChanged { get; set; }
    public Severity StaticTypeChanged { get; set; }
    public Severity AbstractTypeChanged { get; set; }
    public Severity SealedTypeChanged { get; set; }
    public Severity InterfaceChanged { get; set; }
    public Severity SerializableChanged { get; set; }
    public Severity InterfacesRemoved { get; set; }
    public Severity InterfacesAdded { get; set; }
    public Severity BaseAdded { get; set; }
    public Severity BaseChanged { get; set; }
    public Severity MethodAdded { get; set; }
    public Severity MethodRemoved { get; set; }
    public Severity ConstructorAdded { get; set; }
    public Severity ConstructorRemoved { get; set; }
    public Severity PropertyAdded { get; set; }
    public Severity PropertyRemoved { get; set; }
    public Severity EventAdded { get; set; }
    public Severity EventRemoved { get; set; }
    public Severity FieldAdded { get; set; }
    public Severity FieldRemoved { get; set; }

    public Severity VirtualMethodChanged { get; set; }
    public Severity StaticMethodChanged { get; set; }
    public Severity AbstractMethodChanged { get; set; }
    public Severity SealedMethodChanged { get; set; }
    public Severity ReturnTypeChanged { get; set; }
    public Severity ParameterNameChanged { get; set; }
    public Severity DefaultValueChanged { get; set; }
    public Severity OutChanged { get; set; }

    public Severity PropertyTypeChanged { get; set; }
    public Severity PropertySetterChanged { get; set; }
    public Severity PropertyGetterChanged { get; set; }
    public Severity StaticPropertyChanged { get; set; }

    public Severity FieldTypeChanged { get; set; }
    public Severity StaticFieldChanged { get; set; }
    public Severity ConstEnumValueChanged { get; set; }

    public Severity EventTypeChanged { get; set; }
    public Severity StaticEventChanged { get; set; }
  }
}