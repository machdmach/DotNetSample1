using System.ComponentModel;
using System.Reflection;

public static class EnumUtils
{
    private enum E1 { A, B, C, D }

    public static T? GetDefaultValue<T>() where T : struct, Enum
    {
        //E1 e1 = null;
        return (T?)GetDefaultValue(typeof(T));
    }

    public static object? GetDefaultValue(Type enumType)
    {
        var attribute = enumType.GetCustomAttribute<DefaultValueAttribute>(inherit: false);
        if (attribute != null)
            return attribute.Value;

        var innerType = enumType.GetEnumUnderlyingType();
        var zero = Activator.CreateInstance(innerType);
        if (enumType.IsEnumDefined(zero))
            return zero;

        var values = enumType.GetEnumValues();
        return values.GetValue(0);
    }
}