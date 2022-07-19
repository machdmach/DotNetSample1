namespace Libx.Mvc;

//[CORSActionFilter]
public abstract class MvcAdminControllerBase : MvcController_DbActions
{
    protected ConfigFileBasedRoleManager roleMgr = new();
    //protected UserInfo currUser; // => req.currUser;
    protected override bool IsAdminController => true;

    //===================================================================================================
    protected override void Init()
    {
        //IsAdminController = true;
        //var username = GetCurrentUsername_woDomain();
        //currUser = new UserInfo(username, ConfigX.AppName);
        //req.IsAdminApp = true;

        base.Init();
        if (!ConfigX.IsAdminApp)
        {
            throw new Exception("IsAdminApp must set to 1 in config file for Admin Controller");
        }

        if (req.AbsoluteUrl.ToLower().Contains("zznotadmin"))
        {
            currUser.SetIsAdminUser(false);
        }
        reqData.CurrentUsername = currUser.UserName;
        //reqData.User = currUser;
        currUser.RoleList = roleMgr.GetUserRoles(currUser.UserName);
        MOutput.WriteHtmlValueOf(currUser.RoleList);
        currUser.CheckUserForAnyRole();
    }

    //===================================================================================================
    protected override void GetExtraDataSvcInfo(DataSvcInfo info)
    {
        base.GetExtraDataSvcInfo(info);
    }

    //===================================================================================================
    //protected void setIntegratedWindowsAuthenticationzz() //string accessControl_AllowOrigin = null)
    //{
    /*
<system.web>
<customErrors mode="Off"/>
<compilation debug="true" targetFramework="4.6.1"/>
<httpRuntime targetFramework="4.6.1"/>
<authorization>
<allow users="*"/>
</authorization>
</system.web>

<location path="Home">
<system.web>
<authorization>
<deny users="?"/>
</authorization>
</system.web>
</location>             
     */

    //string accessControl_AllowOrigin = Request.Headers["Origin"]; //"http://localhost:8080";
    ////--------
    //if (accessControl_AllowOrigin != null)
    //{
    //    //https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
    //    //Access-Control-Allow-Methods: POST, PUT, GET, OPTIONS
    //    //Access-Control-Allow-Headers: Origin, X-Requested-With, Content-Type, Accept, Authorization

    //    Response.Headers["Access-Control-Allow-Headers"] = "X-App-Entry-URL, X-Pass, X-Page-Size";
    //    Response.Headers["Access-Control-Allow-zzOrigin"] = accessControl_AllowOrigin;
    //    Response.Headers["Access-Control-Allow-Credentials"] = "true";
    //}
    //Response.Headers["Access-Control-Allow-Credentials"] = "true";
    //}
}
