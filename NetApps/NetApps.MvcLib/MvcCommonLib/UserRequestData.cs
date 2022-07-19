namespace Libx.Mvc;
public class RequiredParseOptions : ParseOptions
{
    public RequiredParseOptions(string label)
    {
        Label = label;
        ExceptionType = typeof(UserException);
    }
}
public class UserRequestData
{
    protected MLogger logger;
    public Dictionary<string, string> dict;
    public UserRequestData(Dictionary<string, string> nvs, MLogger logger)
    {
        this.logger = logger;
        dict = nvs;
    }
    public void SetValue(string key, string val)
    {
        dict[key] = val;
    }
    //===================================================================================================
    public string GetString_withCookieAsStorage(string paramName, MvcHttpRequest req)
    {
        string rval;
        var sv = GetStringOrDefault(paramName);
        if (sv != null)
        {
            rval = sv.ToString();
            req.Response.Cookies.Append(paramName, rval, new CookieOptions() { Expires = DateTimeOffset.Now.AddDays(300) });
        }
        else
        {
            rval = req.Request.Cookies[paramName];
        }
        return rval;
    }

    //===================================================================================================
    public int GetInt32(string key, string label = null)
    {
        var options = new RequiredParseOptions(label ?? key);
        dict.TryGetValue(key, out string sVal);
        return StringConvert.ParseInt32(sVal, options).Value;
    }

    public int GetInt32(string key, int defaultVal, string label = null)
    {
        if (dict.TryGetValue(key, out string sVal) && sVal != String.Empty)
        {
            var options = new RequiredParseOptions(label ?? key);
            return StringConvert.ParseInt32(sVal, options).Value;
        }
        else { return defaultVal; }
    }

    //===================================================================================================
    public String GetStringOrDefault(string key, string defaultVal = null)
    {
        if (dict.TryGetValue(key, out string ret))
        {
            return ret;
        }
        else { return defaultVal; }
    }

    public String GetStringOrFail(string key, string errMesg = null)
    {
        if (dict.TryGetValue(key, out string ret))
        {
            return ret;
        }
        else
        {
            errMesg = errMesg ?? "Value not found for key: " + key;
            throw new UserException(errMesg);
        }
    }

    //===================================================================================================
    public T GetOrDefault<T>(string key, T defaultVal)
    {
        string s = GetStringOrDefault(key);
        if (s == null)
        {
            return defaultVal;
        }
        return (T)StringConvert.ParseToType(s, typeof(T));
    }

    //===================================================================================================
    public T GetEntity<T>(ParseEntityOptions entOptions) where T : new()
    {
        T targetObject = new T();
        Type objType = targetObject.GetType();

        //var props = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
        //foreach (PropertyInfo prop in props)
        //{
        //    //prop.CanWrite
        //    if (nvs.TryGetValue(prop.Name, out object val))
        //    {
        //        prop.SetValue(targetObject, val, null);
        //        //SetPropertyValue(targetObject, prop, val);
        //    }
        //}
        PropColMapper mapper = new(objType, logger, new() { PrepareForWrite = true });
        foreach (var prop in mapper.AllPropCols)
        {
            if (!prop.PropInfo.CanWrite) continue; //-----------
            string propName = prop.PropertyName;

            dict.TryGetValue(propName, out string sVal);
            object val;

            ParseOptions options = new()
            {
                Label = prop.PropertyName,
                ExceptionType = typeof(UserException)
            };

            if (prop.IsString)
            {
                val = sVal;
            }
            else if (prop.IsDateTime)
            {
                val = StringConvert.ParseDateTime(sVal, options).Value;
            }
            else if (prop.IsBoolean)
            {
                val = StringConvert.ParseBoolean(sVal, options).Value;
            }
            else if (prop.IsNumeric)
            {
                val = StringConvert.ParseToType(sVal, prop.UnderlyingType, options).Value;
            }
            else
            {
                //val = null;
                throw new Exception("Unsupported data type: " + prop.toStringShort());
            }
            prop.PropInfo.SetValue(targetObject, val);
        }
        return targetObject;
    }

}