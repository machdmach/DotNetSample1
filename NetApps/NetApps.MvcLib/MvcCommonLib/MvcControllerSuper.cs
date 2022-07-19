using System.Text.Json;

namespace Libx.Mvc;
public abstract class MvcControllerSuper : Controller
{
    protected ResponseData responseData = new();
    protected MLogger logger = new MyLogger();
    protected MLogger log => logger;
    protected MvcHttpRequest req = new();
    protected RequestData reqData => req; //= new RequestData();
    protected UserInfo currUser; // => req.currUser;
    protected MyViewData vData; // = new(req);
    protected virtual bool IsAdminController => false;
    //protected string AppName = "Uninit";

    //===================================================================================================
    protected MyDbContext _db;
    protected string dbConnStrKey = "uninit";
    protected void UseConnStr(string s)
    {
        dbConnStrKey = s;
        vData.DbEnv = DbConnectionString.GetDbEnv(dbConnStrKey).ToString();
    }
    public virtual void DisposeDbContext()
    {
        if (_db != null)
        {
            _db.Dispose();
        }
    }

    //===================================================================================================
    protected virtual async Task<IActionResult> MyHtmlErrorResult(Exception ex) 
    {
        if (req.isRequestIgnorable)
        {
            return MyPassThroughView("Error R8920"); //--------------------------
        }
        else if (ex is UserException || ex.ToString().Contains(nameof(UserException)))
        {
            string errMesg = ex.Message;
            if (errMesg.IndexOf('<') < 0)
            {
                errMesg = "Error: " + errMesg;
                errMesg = string.Format("<h4 style='color:red'> {0} </h4>", errMesg);
            }

            if (ex is DosException)
            {
                await EmailSystemAdminErrorAsync(ex);
            }
            return MyPassThroughView(errMesg); //---------------------
        }
        //----------------------------------------------
        string debugInfo = await EmailSystemAdminErrorAsync(ex);

        if (reqData.IsDebug || IsAdminController || ConfigX.IsIISExpress || RunThis)
        {
            string err = string.Format("<p>{0}</p>", ex.Message);
            if (!req.IsDebug)
            {
                //debugInfo already generated
                err += debugInfo;
            }
            return MyPassThroughView(err); //---------------------
        }
        else
        {
            var buf = new StringBuilder();
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<h4 style='color:red'>Sorry, something went wrong, please try again later.</h4>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            buf.Append("<br style='clear:both'>");
            return MyPassThroughView(buf.ToString()); //---------------------
        }
    }

    //===================================================================================================
    protected virtual async Task<IActionResult> MyApiJsonErrorResult(Exception ex)
    {
        if (ex is UserException || ex.ToString().Contains(nameof(UserException)))
        {
            string errMesg = ex.Message;
            if (errMesg.IndexOf('<') < 0)
            {
                errMesg = "Error: " + errMesg;
                errMesg = string.Format("<h4 style='color:red'> {0} </h4>", errMesg);
            }
            responseData.payloadType = ResponseData.DataPayloadType.UserError;
            responseData.payload = errMesg;
            responseData.exception = ex.ToString();

            if (ex is DosException)
            {
                await EmailSystemAdminErrorAsync(ex);
            }
        }
        else
        {
            responseData.payloadType = ResponseData.DataPayloadType.Exception;
            responseData.payload = ex.ToString();
            responseData.exception = ex.ToString();
            await EmailSystemAdminErrorAsync(ex);
        }
        return MyJsonResult(responseData);
        //string serializedJson = JsonSerializer.Serialize(responseData);
        ////string serializedJson = JsonConvert.SerializeObject(responseData);
        //return Content(serializedJson, "application/json"); // ; charset=utf-8"); //default=charset=utf-8; //---------------------
    }
    //===================================================================================================
    protected virtual ContentResult MyJsonResult(Object jsonObj)
    {
        MOutput.Clear();
        var options = new JsonSerializerOptions() //#json
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            MaxDepth = 4,
            //PropertyNameCaseInsensitive = true, //for deserialize
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //IgnoreNullValues = false,//default=false
            //IncludeFields = false, //default=false
        };
        ///<seealso cref="JsonHValue"/>

        string serializedJson = JsonSerializer.Serialize(jsonObj, options);
        //string serializedJson = JsonConvert.SerializeObject(responseData);
        return Content(serializedJson, "application/json"); // ; charset=utf-8"); //default=charset=utf-8;
        //return JsonResult()
    }
    //===================================================================================================
    protected virtual void PreReturnActionResult()
    {
    }

    //===================================================================================================
    protected async Task<ViewResult> MyPassThroughViewAsync(object content)
    {
        if (NotRunThis) await Task.Yield();
        return MyPassThroughView(content);
    }

    //===================================================================================================
    protected async Task<IActionResult> NoopResutlAsync()
    {
        if (NotRunThis) await Task.Yield();
        //if (NotRunThis) await NoopAsync();
        return null;
    }

    //===================================================================================================
    protected virtual ViewResult MyPassThroughView_debugForDataApi(object contentObj)
    {
        return MyPassThroughView(contentObj);
    }
    //===================================================================================================
    protected virtual ActionResult MyPassThroughView_orApiResult(object contentObj)
    {
        if (reqData.isRequestingJSON)
        {
            responseData.payloadType = "text/html";
            responseData.payload = contentObj.ToString();
            return MyJsonResult(responseData);
        }
        else
        {
            return MyPassThroughView(contentObj);
        }
    }

    //===================================================================================================
    protected virtual ViewResult MyPassThroughView(object contentObj)
    {
        DisposeDbContext();
        PreReturnActionResult();
        if (req.IsDebug)
        {
            vData.FooterDebugInfoBuf.Append(GetDebugInfoHtml());
        }

        string PassThroughViewName = "PassThroughView";
        //string PassThroughViewName = "_Layout";
        string s = null;
        if (contentObj == null)
        {
            s = "null";
        }
        else if (contentObj is StringBuilder sb)
        {
            s = sb.ToString();
        }
        else if (contentObj is HtmlBuilder hm)
        {
            string html = hm.ToString();
            s += html;
            if (req.IsDebug)
            {
                s += HtmlText.HtmlEncodeAndPre(html);
            }
        }
        else
        {
            s = contentObj.ToString();
            if (s.IndexOf('<') < 0)
            {
                s = "<p>" + s + "</p>";
            }
        }
        ViewResult ret = View(PassThroughViewName, s);
        return ret;
    }

    //===================================================================================================
    public async Task<IActionResult> ViewNotExist()
    {
        if (NotRunThis) await Task.Yield();
        return View("ViewNotExist");
    }

    //===================================================================================================
    protected async Task<string> EmailSystemAdminErrorAsync(Exception ex)
    {
        string debugInfo = "";
        debugInfo += GetDebugInfoHtml(ex);

        if (ConfigX.IsAppEnvDev || ConfigX.IsSampleApp)
        {
            //#email
            debugInfo += MOutput.WriteLine("isDev, not sending sysadmin err email!!!");
            //await AppErrorHandler.EmailSystemAdminErrorAsync(ex, debugInfo);
        }
        else
        {
            debugInfo += MOutput.WriteLine("Try sending sysadmin err email!!!");
            await AppErrorHandler.TryEmailSystemAdminErrorAsync(ex, debugInfo);
        }
        return debugInfo;
    }

    //===================================================================================================
    public String GetCurrentUsername_woDomain()
    {
        //HttpSessionStateBase seshion = Request.RequestContext.HttpContext.Session;
        ////if (AppSettingsX.AppEnv != AppEnvEnum.Prod)

        //document.cookie="keyofcookie=valueofcookie"
        //document.cookie = "username=John Doe; expires=Thu, 18 Dec 2013 12:00:00 UTC; path=/";
        //document.cookie = "username=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        //2025-01-01T05:31:50.664Z
        //document.cookie = "username=John Doe; expires=2025-01-01T05:31:50.664Z; path=/";

        string username = null;
        string unameKey = "zzuname";
        //throw new Exception("username=" + username);
        //if (ck != null) { username = ck.Value; }

        //if (string.IsNullOrEmpty(username))
        //{
        //    username = Request.Query[unameKey];
        //    //if (username == "1") username = null;
        //}
        if (string.IsNullOrEmpty(username))
        {
            username = req.QueryData.GetString_withCookieAsStorage(unameKey, req);
            //username = Request.Cookies[unameKey];
            //if (!string.IsNullOrEmpty(username)) MOutput.WriteLine("Cookie zzuname=" + username);
            //if (username == "1") username = null;
        }

        if (string.IsNullOrEmpty(username))
        {
            username = User.Identity.Name;
            //username = Request.RequestContext.HttpContext.User.Identity.Name;
        }
        if (string.IsNullOrEmpty(username))
        {
            username = Environment.UserName;
        }
        if (username != null)
        {
            int i_backslash = username.IndexOf('\\');
            if (i_backslash >= 0)
            {
                username = username.Substring(i_backslash + 1);
            }
        }
        username = username.ToLower();
        return username;
    }

    //===================================================================================================
    protected virtual String GetDebugInfoHtml(Exception ex = null)
    {
        string s = "";
        if (ex != null)
        {
            s += "<pre  style='overflow:visible'>" + System.Net.WebUtility.HtmlEncode(ex.ToString()) + "</pre>";
        }
        if (req.IsDebug)
        {
            s = "LastSql: " + MLogger.LastSqlHtmlPre + s;

            s += "<hr>";
            s += MOutput.ToHtmlAndClear();
            s += logger.ToHtml();
            s += req.ToHtml();
        }
        else
        {
            //MOutput.Clear();
        }
        return s;
    }
}
