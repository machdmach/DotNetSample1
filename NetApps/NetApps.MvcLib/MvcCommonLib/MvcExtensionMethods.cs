namespace Libx.Mvc;
public static class MvcExtensionMethods
{    
    public static Dictionary<string, string> ToStringDictionary(this IEnumerable<KeyValuePair<string, StringValues>> src)
    {
        var ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, StringValues> nv in src)
        {
            string val;
            if (nv.Value.Count == 1)
            {
                val = nv.Value;
            }
            else
            {
                val = "m/ " + nv.Value.ToString();
            }
            ret[nv.Key] = val;
        }
        return ret;
    }

}
