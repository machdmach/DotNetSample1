
namespace Libx;
public static class Dictionary_Extensions
{
    public static int GetInt32(this Dictionary<string, string> dict, string key, int defaultVal = 0)
    {
        if (dict.TryGetValue(key, out string sVal))
        {
            return Int32.Parse(sVal);
        }
        else { return defaultVal; }
    }
    //===================================================================================================
    public static string Getstring(this Dictionary<string, string> dict, string key, string defaultVal = null)
    {
        if (dict.TryGetValue(key, out string ret))
        {
            return ret;
        }
        else { return defaultVal; }
    }
    ////===================================================================================================
    //public static string GetstringOrFail(this Dictionary<string, string> dict, string key, string errMesg = null)
    //{
    //    if (dict.TryGetValue(key, out string ret))
    //    {
    //        return ret;
    //    }
    //    else
    //    {
    //        errMesg = errMesg ?? "Value not found for key: " + key;
    //        throw new Exception(errMesg);
    //    }
    //}
    ////===================================================================================================
    //public static string GetstringOrUserError(this Dictionary<string, string> dict, string key, string userErrMesg = null)
    //{
    //    if (dict.TryGetValue(key, out string ret))
    //    {
    //        return ret;
    //    }
    //    else
    //    {
    //        userErrMesg = userErrMesg ?? "Value not found for key: " + key;
    //        throw new UserException(userErrMesg);
    //    }
    //}
    //===================================================================================================
    public static string? TryGetValueCI(this Dictionary<string, string> dict, string key)
    {
        if (dict.TryGetValue(key, out string ret))
        {
            return ret;
        }
        string keyLower = key.ToLower();
        //nvs.GetEnumerator();

        return null;
    }
    //===================================================================================================
    public static void LeftJoinIn(this Dictionary<string, string> dict, Dictionary<string, string> dict2)
    {
        foreach (KeyValuePair<string, string> kv in dict2)
        {
            if (!dict.ContainsKey(kv.Key))
            {
                dict[kv.Key] = kv.Value;
            }
        }
    }
    //===================================================================================================
    public static Dictionary<string, string> LeftJoin(this Dictionary<string, string> dict, Dictionary<string, string> dict2)
    {
        Dictionary<string, string> ret = new(dict, StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> kv in dict2)
        {
            if (!dict.ContainsKey(kv.Key))
            {
                //ret.[kv.Key] = kv.Value;
                ret.Add(kv.Key, kv.Value);
            }
        }
        return ret;
    }

}
