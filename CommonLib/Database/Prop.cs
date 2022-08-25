using System.Reflection;

namespace Libx;
///<see cref="TypeX"/>
public class TypeInfoX
{
    public readonly bool IsDateTime;
    public readonly bool IsChar; //char

    public readonly bool IsString;
    public readonly bool IsBoolean;
    public readonly bool IsValueType;

    public readonly bool IsNumeric;
    public readonly bool IsInteger; // are natural numbers and whole numbers and negative numbers
    public readonly bool IsFraction; // are natural numbers and whole numbers and negative numbers

    public readonly bool IsByte; //signed byte
    public readonly bool IsInt16; //short
    public readonly bool IsInt32;
    public readonly bool IsInt64;
    //public readonly bool IsSByte; //sbyte; 8-bit signed integer;
    //public readonly bool IsUInt16; //unsigned short
    //public readonly bool IsUInt32;
    //public readonly bool IsUInt64;

    public readonly bool IsSingle; //float
    public readonly bool IsDouble; //double
    public readonly bool IsDecimal; //decimal, 128-bit data type suitable for financial and monetary calculations

    public readonly bool IsSimpleType;


    ///<see cref="Nullable"/>
    ///<see cref="Nullable{T}.GetValueOrDefault"/>
    public readonly bool IsNullable;
    public readonly bool IsNotNullable;
    public readonly Type UnderlyingType;
    public readonly Type OrigType;
    public readonly TypeCode UnderlyingTypeCode;

    //===================================================================================================
    public TypeInfoX(Type t)
    {
        OrigType = t;

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            IsNullable = true;
            UnderlyingType = Nullable.GetUnderlyingType(t);
            //AssertX.NotNullable(this.PropDataType);
        }
        else
        {
            if (!t.IsValueType)
            {
                IsNullable = true;
            }
            UnderlyingType = t;
        }

        //--------------------------------
        t = UnderlyingType;
        UnderlyingTypeCode = Convert.GetTypeCode(t);

        if (t == typeof(String))
        {
            IsString = true;
            IsSimpleType = true;
        }
        else if (t == typeof(Char))
        {
            IsChar = true;
            IsSimpleType = true;
        }
        else if (t == typeof(DateTime))
        {
            IsDateTime = true;
            IsSimpleType = true;
        }
        else if (t == typeof(Boolean))
        {
            IsBoolean = true;
            IsSimpleType = true;
        }
        else if (t == typeof(Byte) || t == typeof(SByte))
        {
            IsByte = true;
            IsInteger = true;
        }
        if (t == typeof(Int16) || t == typeof(UInt16))
        {
            IsInt16 = true;
            IsInteger = true;
        }
        if (t == typeof(Int32) || t == typeof(UInt32))
        {
            IsInt32 = true;
            IsInteger = true;
        }
        else if (t == typeof(Int64) || t == typeof(UInt64))
        {
            IsInt64 = true;
            IsInteger = true;
        }
        else if (t == typeof(Single)) //float
        {
            IsSingle = true;
            IsFraction = true;
        }
        else if (t == typeof(Double))
        {
            IsDouble = true;
            IsFraction = true;
        }
        else if (t == typeof(Decimal))
        {
            IsDecimal = true;
            IsFraction = true;
        }
        IsNumeric = IsInteger || IsFraction;

        IsValueType = t.IsValueType;
        IsNotNullable = !IsNullable;

        //if (IsNumeric) IsSimpleType = true;
        IsSimpleType |= IsNumeric;
    }
}


//===================================================================================================
//public class ColPropMapping
public class Prop : TypeInfoX
{
    //public string NormalizedName; //NormalizedCommonName
    public string PropertyName;
    public Type PropDataType;
    public PropertyInfo PropInfo;

    //public Prop() { }
    public Prop(PropertyInfo prop) : base(prop.PropertyType)
    {
        PropInfo = prop;
        PropertyName = prop.Name;
        PropDataType = UnderlyingType;
    }
}
