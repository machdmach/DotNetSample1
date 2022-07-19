namespace Libx.Mvc;
public class MvcHttpRequestBase : RequestData
{
    public UserRequestData FormData;
    public UserRequestData QueryData;
    public UserRequestData ParamData;

    public bool IsHomePagePath => String.IsNullOrEmpty(Request.Path) || Request.Path == "/";
    public bool ShowDebugFullInfo = false;

    protected MvcHttpRequest req;
    public HttpRequest Request;
    public RequestData reqData;

    public string controller;
    public string action;
    public string entityName;
    public string method;
    public int keyId = 0;
    public static int AppRequestCount;
    public int AppRequestCountx => AppRequestCount;
    public int SesRequestCount;

    public MLogger logger;
    protected MLogger log => logger;
    public HttpResponse Response => Request.HttpContext.Response;

    //===================================================================================================
    public bool isRequestIgnorable
    {
        get
        {
            var userAgentLower = (reqData.UserAgent + "").ToLower();
            var rawUrl = (reqData.AbsoluteUrl + "").ToLower();
            return userAgentLower.Contains("yandexbot") //Mozilla/5.0 (compatible; YandexBot/3.0; +http://yandex.com/bots) 
                || userAgentLower.Contains("googlebot")
                || userAgentLower.Contains("bingbot") //Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm) 
                || userAgentLower.Contains("semrush")
                || userAgentLower.Contains("adsbot")
                || rawUrl.EndsWith("/pubfile")
                ;
        }
    }

    //===================================================================================================
    public MyCookie NewCookie(string name)
    {
        var ret = new MyCookie(name, Request.HttpContext);
        return ret;
    }

    //===================================================================================================
    public bool Is_ToFillSampleData()
    {
        bool rval = false;
        if (Request.Method == "POST")
        {
            rval = false;
        }
        else //if (Request.HttpMethod != "POST")
        {
            if (AbsoluteUrl.Contains("zzsample"))
            {
                rval = true;
            }
            else if (reqData.IsDebug)
            {
                //rval = true;
            }
            else if (ReferrerUrl != null && ReferrerUrl.Contains("zzsample"))
            {
                rval = true;
            }
        }
        return rval;
    }

    //===================================================================================================
    public bool IsRequestingJson()
    {
        bool ret = true;
        if (MvcLib.IsAccept(Request, "text/html"))
        {
            ret = false;
            if (Request.Query.ContainsKey("zzjson"))
            {
                ret = true;
            }
        }
        return ret;
    }

    //===================================================================================================
    public bool IsMethod(string s)
    {
        s = s.Replace("-", "").ToLower();
        return method.Equals(s, StringComparison.OrdinalIgnoreCase);
    }
    public bool IsMethodPOST => IsMethod("POST");
    public bool IsMethodPUT => IsMethod("PUT");
    public bool IsMethodGET => IsMethod("GET");
    public bool IsMethodSEARCH => IsMethod("SEARCH");
    //===================================================================================================
    public Uri GetUri()
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = Request.Scheme,
            Host = Request.Host.Host,
            //Port = Request.Host.Port.GetValueOrDefault(80),
            //Path =  Request.Path.ToString(),
            Query = Request.QueryString.ToString()
        };
        if (Request.Host.Port.HasValue)
        {
            uriBuilder.Port = Request.Host.Port.Value;
        }
        //-------
        if (Request.PathBase.HasValue)
        {
            uriBuilder.Path = Request.PathBase.Value + Request.Path.Value;
        }
        else
        {
            uriBuilder.Path = Request.Path.Value; ;
        }
        return uriBuilder.Uri;
    }
    //===================================================================================================
    public bool IsFromExternal
    {
        get
        {
            var hostnameLower = Request.Host.Host.ToLower();
            return
                hostnameLower.EndsWith(".us") ||
                hostnameLower.EndsWith(".com") ||
                hostnameLower.EndsWith(".aero");
        }
    }
    //===================================================================================================
    public bool IsHttpMethodPOST
    {
        get
        {
            bool rval = Request.Method == "POST";
            return rval;
        }
    }
    //===================================================================================================
    public bool Is_zzRaw()
    {
        var text = Url.GetLeftPart(UriPartial.Path);
        bool rval = text.ToLower().Contains("raw") || (QueryData.GetStringOrDefault("zzraw", "x") == "1");
        return rval;
    }
}
