namespace Libx.Mvc;
public abstract class MvcControllerBase : MvcControllerSuper
{
    private bool _initCalled = false;
    protected virtual void Init()
    {
        if (_initCalled) { throw new Exception("Init alread called"); }
        _initCalled = true;

        vData = new MyViewData(req);
        ViewData["myViewData"] = vData;

        MvcApp.ReloadConfigFiles();

        req.Init(Request, logger); //so if exception occurs, the req object is available for use in error handling

        logger.ShowCallerStack = req.QueryData.GetString_withCookieAsStorage("zzLogStackTrace", req) == "1";
        MOutput.Logger.ShowCallerStack = logger.ShowCallerStack;

        var username = GetCurrentUsername_woDomain();
        currUser = new UserInfo(username, ConfigX.AppName);

        if (AppSettingsX.GetString("LocalhostOnly") == "1" && !reqData.IsLocalhost) //  ConfigX.AppEnv == AppEnvEnum.LocalDev)
        {
            throw new Exception("Errorz, you might have published the web.config to non-localHost environment, please undo the file");
        }

        //string rawUrl = Request.RawUrl;
        //int sleepSecs;
        //if (Int32.TryParse(Request.QueryString["sleep"], out sleepSecs))
        //{
        //    Thread.Sleep(sleepSecs * 1000);
        //}
        //{
        //    //#CORS
        //    string accessControl_AllowOrigin = Request.Headers["Origin"]; //"http://localhost:8080";
        //    if (accessControl_AllowOrigin != null)
        //    {
        //        Response.Headers["Access-Control-Allow-Origin"] = accessControl_AllowOrigin;
        //    }
        //    Response.Headers["Vary"] = "Origin";
        //    Response.Headers["Access-Control-Allow-Headers"] = "X-App-Entry-URL, X-Pass, X-Page-Size";
        //    Response.AddHeader("X-Frame-Options", "allow-from http://localhost");
        //    Response.AddHeader("Frame-Ancesstors", "allow-from http://localhost");
        //}
        if (IsAdminController)
        {
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
        }
        if (Request.Query.ContainsKey("zzThrow"))
        {
            throw new Exception("You asked for this");
        }
    }

    //===================================================================================================
    public virtual async Task<IActionResult> GetDataSvcInfo()
    {
        try
        {
            Init();
            req.ShowDebugFullInfo = true;
            var info = new DataSvcInfo
            {
                ApiVersion = AppSettingsX.GetString("AppVersion"),
                AppEnv = ConfigX.AppEnv.ToString(),
                DbEnv = DbConnectionString.GetDbEnv(dbConnStrKey).ToString(),
                AppName = ConfigX.AppName,
                AppStartedDateTime = ConfigX.AppStartedDateTime
            };
            //var machineName = Environment.MachineName;
            //info.SrvrName = (machineName.Length > 3) ? machineName.Substring(machineName.Length - 3) : machineName;
            info.SrvrName = "sv-" + Environment.MachineName.Replace("0", "x");

            //info.UserIP = req.UserIP;
            GetExtraDataSvcInfo(info);

            responseData.payload = info;
            return MyActionResult();
        }
        catch (Exception ex)
        {
            return await MyErrorResult(ex);
        }
    }

    //===================================================================================================
    protected virtual void GetExtraDataSvcInfo(DataSvcInfo info)
    {
        var usr = currUser; 
        info.Username = usr.UserName;
        if (usr.RoleList != null) info.UserRoles = String.Join(", ", usr.RoleList);
        MOutput.WriteObject(usr, "userInfo");
    }

    //===================================================================================================
    protected virtual ActionResult MyActionResult()
    {
        DisposeDbContext();
        if (reqData.isRequestingJSON)
        {
            return MyJsonResult(responseData);
        }
        else
        {
            string jsonResUrl = req.AbsoluteUrl;
            jsonResUrl += (jsonResUrl.Contains("?") ? "&" : "?") + "zzJson";
            string s = "";
            s += responseData.ToHtml();
            s += $"<h3 style='text-align:center'> append '&ampzzjson' to the <a href='{jsonResUrl}'> addres bar URL </a> to see real json result</h3>";
            return MyPassThroughView_debugForDataApi(s);
        }
    }

    //===================================================================================================
    protected virtual Task<IActionResult> MyErrorResult(Exception ex)
    {
        DisposeDbContext();
        logger.LogError(ex);

        if (reqData.isRequestingJSON)
        {
            return MyApiJsonErrorResult(ex);
        }
        else
        {
            return MyHtmlErrorResult(ex);
        }
    }
    //===================================================================================================
    protected ActionResult ProcessCommonMethods(ResponseData res, Type entityType)
    {
        var reqMethod = "";
        //ActionResult rval = null;
        if (reqMethod.Equals("getDataFieldDefs", StringComparison.InvariantCultureIgnoreCase))
        {
            var fieldDefs = DataFieldDef.GetDataFieldDefs(entityType);
            res.payload = fieldDefs;
        }
        //else if (reqData.AbsoluteUri.Contains("zzcode"))
        //{
        //    reqData.IsDebug = true;
        //}
        else
        {
            throw new Exception("Unknown method: " + reqMethod);
        }
        return null;
    }

    //===================================================================================================
    protected virtual UserException newUserException(String format, params string[] args)
    {
        string errMesg = String.Format(format, args);
        var rval = new UserException(errMesg);
        return rval;
    }

    //===================================================================================================
    protected virtual async Task<IActionResult> MyApiResultWrapper(Func<object> callback)
    {
        try
        {
            Init();
            object val = callback();
            if (val is StringBuilder || val is HtmlBuilder)
            {
                val = val.ToString();
            }
            responseData.payload = val;
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
}
