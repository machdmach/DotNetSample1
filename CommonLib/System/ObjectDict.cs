
using System.Reflection;

namespace Libx;
public class ObjectDict
{
    //============================================================================================
    public static void SetPropertyValues(object targetObject, Dictionary<string, string> nvs)
    {
        Type objType = targetObject.GetType();
        var props = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
        foreach (PropertyInfo prop in props)
        {
            //prop.CanWrite
            if (nvs.TryGetValue(prop.Name, out string str))
            {
                object val = StringConvert.ParseToType(str, prop.PropertyType);
                prop.SetValue(targetObject, val, null);
                //SetPropertyValue(targetObject, prop, val);
            }
        }
    }
    //============================================================================================
    public static void SetPropertyValues(object targetObject, Dictionary<string, object> nvs)
    {
        Type objType = targetObject.GetType();
        var props = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
        foreach (PropertyInfo prop in props)
        {
            if (nvs.TryGetValue(prop.Name, out object val))
            {
                prop.SetValue(targetObject, val, null);
            }
        }
    }
    //============================================================================================
    public static void GetPropertyValues(object targetObject, Dictionary<string, object> nvs)
    {
        Type objType = targetObject.GetType();
        var props = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        foreach (PropertyInfo prop in props)
        {
            Object val = prop.GetValue(targetObject, null);
            nvs.Add(prop.Name, val);
        }
    }
}
