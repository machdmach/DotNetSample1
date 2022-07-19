using System.Runtime.Serialization;

namespace Libx;
[DataContract]
public class RequestData
{
    //[DataMember]
    //public UserInfo User { get; set; } //= new UserInfo();

    [DataMember]
    public string ApplicationPath { get; set; } //HostingEnvironment.ApplicationVirtualPath: /

    string _appName;
    public string AppName
    {
        get => _appName ?? ConfigX.AppName;
        set => _appName = value;
    } // = "uninit";
    //public string AppName => ConfigX.AppName;

    public string AppEnv { get; set; }
    //public bool IsAdminApp { get; set; }

    [DataMember]
    public string CurrentUsername { get; set; }

    [DataMember]
    public bool IsDebug { get; set; }

    [DataMember]
    public bool IsLocalhost { get; set; }

    //[DataMember]
    //public bool IsFromExternalNetwork { get; set; }

    //[DataMember]
    //public bool IsPostBack { get; set; }

    [DataMember]
    public string MySessionId { get; set; }

    public Uri Url { get; set; }

    [DataMember]
    public string AbsoluteUrl { get; set; } //http://localhost:20166/Home/Covid19Info?zzprod&zzdb

    [DataMember]
    public string ReferrerUrl { get; set; }

    [DataMember]
    public string AppEntryURL { get; set; }

    //[DataMember]
    //public string UserHostAddress { get; set; }

    [DataMember]
    public string UserAgent { get; set; }

    public string UserIP { get; set; }

    //[DataMember]
    //public string UIHostName { get; set; }

    //[DataMember]
    //public string UrlReferer { get; set; }

    public bool isRequestingJSON { get; set; }
    public bool IsSuperuserPasswdValidated { get; set; }

    //A common use for the Tag property is to store data that is closely associated with the control z
    [DataMember]
    public Object Tag { get; set; }
    public string SortBy { get; set; }
    public string SortDir { get; set; }



    //===================================================================================================
    public static bool Is_zzThrow(string text)
    {
        text = text + "";
        bool rval = text.ToLower().Contains("zzthrow");
        return rval;
    }
    //===================================================================================================
    public static bool Is_zzThrowUser(string text)
    {
        text = text + "";
        bool rval = text.ToLower().Contains("zzthrowuser");
        return rval;
    }
    //===================================================================================================
    public static bool Is_zzTest(string text)
    {
        text = text + "";
        bool rval = text.ToLower().Contains("zztest");
        return rval;
    }
    //===================================================================================================
    public bool Is_zzProd(string text)
    {
        text = text + "";
        bool rval = text.ToLower().Contains("zztest");
        return rval;
    }
    //===================================================================================================
    public static bool Is_zzRaw(string text)
    {
        text = text + "";
        bool rval = text.ToLower().Contains("zzraw");
        return rval;
    }


    //===================================================================================================
    public string GetFQUrl(string relPath)
    {
        return GetAbsolutePath(relPath);
    }

    public string GetAbsolutePath(String relUrl = null)
    {
        if (ApplicationPath == null)
        {
            throw new Exception("ApplicationPath required..");
        }
        string fqurl;
        if (string.IsNullOrEmpty(relUrl))
        {
            fqurl = ApplicationPath; // + "/";
        }
        else if (relUrl.StartsWith("?"))
        {
            fqurl = relUrl;
        }
        else if (relUrl.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase)
            || relUrl.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase))
        {
            fqurl = relUrl;
        }
        else
        {
            if (relUrl.StartsWith("~"))
            {
                relUrl = relUrl.Substring(1);
            }
            if (relUrl.StartsWith("/"))
            {
                relUrl = relUrl.Substring(1);
            }
            fqurl = ApplicationPath + relUrl;
        }
        return fqurl;
    }
}

