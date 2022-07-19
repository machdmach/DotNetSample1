using System.Runtime.Serialization;

namespace System;

[DataContract]
public enum AppEnvEnum
{
    [EnumMember]
    Unknown = 0,
    [EnumMember]
    Dev,
    [EnumMember]
    Test,
    [EnumMember]
    Prod
}

public class AppSettingsX
{
    public static Dictionary<string, string> AppSettings = new Dictionary<string, string>();
    public static bool Contains(String key)
    {
        var val = GetString(key, false);
        return (val != null);
    }
    //===============================================================================
    public static String GetString(String key, string defaultValue)
    {
        var ret = GetString(key, false);
        if (ret == null)
        {
            ret = defaultValue;
        }
        return ret;
    }
    //===============================================================================
    public static String GetString(String key, bool throwsOnNotFound = true)
    {
        if (AppSettings.TryGetValue(key, out string ret))
        {
            ret = ret.Trim();
        }
        else
        {
            if (throwsOnNotFound)
            {
                throw new Exception("key not found: " + key);
            }
        }
        return ret;
    }
    //========================================================================
    public static List<Int32> GetIntList(String key)
    {
        string val = GetString(key);
        if (val == null)
        {
            throw new Exception("No settings found for key: " + key);
        }
        List<Int32> rval = new List<Int32>();

        string[] vals = val.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in vals)
        {
            rval.Add(Int32.Parse(s));
        }
        return rval;
    }

    //========================================================================
    public static int GetInt(String key)
    {
        string s = GetString(key);
        return Int32.Parse(s);
    }
}
//            XDocument xdoc = XDocument.Load(pfname);
//            //var xe = xdoc.Elements();
//            //XElement xe = XElement.Parse(settingsText);
//#if docs
//<appSettings>
//  <add key="SmtpHost" value="1.1.2.3" />
//#endif
//            //foreach (XNode xnode in xdoc.DescendantNodes())
//            //{
//            //}
//            var nvs = new NameValueCollection();
//            foreach (XElement elt in xdoc.Descendants())
//            {
//                if (elt.Name == "add")
//                {
//                    string key = elt.Attribute("key").Value;
//                    string value = elt.Attribute("value").Value;
//                    nvs[key] = value;
//                }
//            }

