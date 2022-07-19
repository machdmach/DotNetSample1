using Microsoft.Net.Http.Headers;

namespace Libx.Mvc;
public partial class MvcHttpRequest : MvcHttpRequestBase
{
    public string DevStartupUrl = null;
    public MvcHttpRequest()
    {
        req = this;
        reqData = this; 
        AppRequestCount++;
    }
    //===================================================================================================
    static object sessionIdGenerateLock = new object();
    static int sessionIdCounter = 0;
    public void Init(HttpRequest RequestP, MLogger loggerP)
    {
        Request = RequestP;
        logger = loggerP;

        if (req.IsHttpMethodPOST)
        {
            FormData = new UserRequestData(Request.Form.ToStringDictionary(), logger);
        }
        else
        {
            FormData = new UserRequestData(new Dictionary<string, string>(), logger);
        }
        QueryData = new UserRequestData(Request.Query.ToStringDictionary(), logger);
        ParamData = new UserRequestData(FormData.dict.LeftJoin(QueryData.dict), logger);

        reqData.ApplicationPath = Request.PathBase.Value;
        if (ConfigX.WebAppVirtualPath == null)
        {
            ConfigX.WebAppVirtualPath = Request.PathBase.Value;
        }

        var mySessionIdCookie = NewCookie(nameof(MySessionId));
        var sessionReqNumCookie = NewCookie(nameof(SesRequestCount));

        MySessionId = mySessionIdCookie.GetValue();
        if (MySessionId == null)
        {
            lock (sessionIdGenerateLock)
            {
                MySessionId = DateTime.Now.ToString("s").Replace(':', '-') + "_" + (++sessionIdCounter);
                mySessionIdCookie.SetValueToSession(MySessionId);
                SesRequestCount = 1;
            }
        }
        else
        {
            SesRequestCount = sessionReqNumCookie.GetInt(0) + 1;
        }
        sessionReqNumCookie.SetValueToSession(SesRequestCount);

        IsSuperuserPasswdValidated = Request.Query["passwd"] == AppSettingsX.GetString("Superuser.passwd");

        reqData.IsLocalhost = MvcLib.IsLocalRequest(Request);
        reqData.isRequestingJSON = IsRequestingJson();

        Url = GetUri();
        AbsoluteUrl = Url.AbsoluteUri;

        reqData.AppEnv = ConfigX.AppEnv.ToString();
        //----------------------------------------------
        //Get misc
        {
            //string reg = @"(/api)?[^\/]+";
            var path = Request.Path.Value ?? "";
            //var pathLower = path.ToLower();
            if (path.ToLower().StartsWith("/api/"))
            {
                path = path.Substring(4);
            }
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            string[] pathSegments = path.Split("/");
            controller = pathSegments[0];
            int len = pathSegments.Length;
            action = (len > 1) ? pathSegments[1] : "Index";

            string keyIdStr = null;
            if (len == 3)
            {
                //LafDataApi/getLookups/LafLocation
                //LafDataApi/Do/checkStatus
                entityName = method = pathSegments[2];
            }
            else if (len == 4)
            {
                //LafDataApi/DoFoundEntity/get/2 
                //LafPubDataApi/GetChildLookups/LafSubCategory/28
                //EntityName is in the action string
                entityName = method = pathSegments[2];
                keyIdStr = pathSegments[3];
            }
            else if (len >= 5)
            {
                //LafDataApi/DoCrud/LafLocation/put/2
                entityName = pathSegments[2];
                method = pathSegments[3];
                keyIdStr = pathSegments[4];
            }

            entityName ??= "";
            method ??= "";

            if (keyIdStr != null && !Int32.TryParse(keyIdStr, out keyId))
            {
                throw new Exception("KeyId in path must be a number: " + keyIdStr);
            }
        }
        bool get_isDebug()
        {
            var ret = false;
            string zzDebugParam = req.QueryData.GetString_withCookieAsStorage("zzDebug", req);
            if (zzDebugParam == "1") ret = true; else if (zzDebugParam == "0") ret = false;
            return ret;
        }
        reqData.IsDebug = get_isDebug();
        if (reqData.IsDebug && IsFromExternal)
        {
            EnsureSuperuserPasswdValidated();
        }

        //----------------------------------------------
        ReferrerUrl = Request.Headers[HeaderNames.Referer] + "";

        //reqData.IsFromExternalNetwork = this.IsFromExternal();

        UserAgent = Request.Headers["User-Agent"].ToString();
        UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();


        //Assisting with dev
        if (ConfigX.IsDevMachine)
        {
            var lastUrlCookie = NewCookie("LastUrl");
            if (AppRequestCount == 1 || SesRequestCount == 1)
            {
                string lastUrl = lastUrlCookie.GetValue();
                if (lastUrl != AbsoluteUrl && lastUrl != null)
                {
                    //Response.Redirect(lastUrl);  //#redirect
                }
            }
            else
            {
                if (!IsHomePagePath)
                {
                    lastUrlCookie.SetValue(AbsoluteUrl);
                }
            }
        }
    }

    //===================================================================================================
    public string GetEntityNameOrFail()
    {
        string entName = req.entityName;
        if (string.IsNullOrWhiteSpace(entName))
        {
            throw new ArgumentNullException("entName is required");
        }
        return entName;
    }

    //===================================================================================================
    public int GetKeyIdOrFail()
    {
        if (keyId == 0)
        {
            throw new ArgumentNullException("KeyId is required");
        }
        return keyId;
    }

    public void EnsureLocalhost(string errMesg = null)
    {
        if (!req.IsLocalhost)
        {
            errMesg ??= "This function is available for localhost only";
            throw new UserException(errMesg);
        }
        logger.LogInformation("OK, IsLocalhost");
    }
    public void EnsureSuperuserPasswdValidated(string errMesg = null)
    {
        if (!req.IsSuperuserPasswdValidated)
        {
            errMesg ??= "This function is available for su only";
            throw new UserException(errMesg);
        }
        logger.LogInformation("OK, IsLocalhost");
    }
    public void EnsureDevMachine(string errMesg = null)
    {
        if (ConfigX.IsSampleApp)
        {
            return; //bypassing security check here
        }
        if (!ConfigX.IsDevMachine)
        {
            errMesg ??= "This function is available for dev machine only";
            throw new UserException(errMesg);
        }
        logger.LogInformation("OK, IsDevMachine");
    }
    //===================================================================================================
    public String ToHtml(ToHtmlOptions options = null)
    {
        options ??= new ToHtmlOptions();
        options.Caption = "RequestData";

        var buf = new StringBuilder();

        buf.AppendLine(HtmlValue.OfObject(this, options));
        buf.AppendLine(HtmlValue.OfList(FormData.dict, new ToHtmlOptions() { Caption = "Request.Form" }));
        buf.AppendLine(HtmlValue.OfList(QueryData.dict, new ToHtmlOptions() { Caption = "Request.Query" }));

        if (ShowDebugFullInfo)
        {
            buf.AppendLine(HtmlValue.OfList(Request.Headers.ToStringDictionary(), new ToHtmlOptions() { Caption = "Request.Headers" }));
            buf.AppendLine(HtmlValue.OfList(Request.Cookies, new ToHtmlOptions() { Caption = "Request.Cookies" }));
        }
        return buf.ToString();
    }
}
