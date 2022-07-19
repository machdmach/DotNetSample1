namespace Libx.Mvc;
public abstract class SysManControllerBase : MvcControllerBase
{
    protected override void Init()
    {
        base.Init();
        //reqData.IsDebug = true;
        req.ShowDebugFullInfo = true;
    }
    //===================================================================================================
    public async Task<IActionResult> RefreshCache()
    {
        try
        {
            Init();
            MvcApp.ReloadConfigFiles(true);
            return MyPassThroughView("RefreshCache Done");
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    static TimedCounter dosTest1 = new("DosTest", 3, TimeSpan.FromMinutes(3));
    public async Task<IActionResult> DosTest()
    {
        try
        {
            Init();
            dosTest1.CheckDoS__postIncrement();
            return MyPassThroughView("DosTest Done");
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> TestCookies()
    {
        try
        {
            Init();
            Response.Cookies.Append("sessionCookie1", Environment.TickCount + "");
            Response.Cookies.Append("timedCookie1", Environment.TickCount + "", new CookieOptions() { Expires = DateTimeOffset.Now.AddDays(300) });
            Response.Cookies.Append("cookie1", Environment.TickCount + "", new CookieOptions() { Expires = DateTimeOffset.Now.AddDays(-300) });

            string s = "";
            s += HtmlValue.OfList(Request.Cookies);
            s += HtmlValue.OfObject(Response.Cookies);

            return MyPassThroughView(s + "TestCookies Done");
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> Main()
    {
        try
        {
            Init();
            if (req.IsMethod("xRefreshCache"))
            {
                //MvcApp.ReloadConfigFiles();
                //return MyPassThroughView("RefreshCache Done");
            }
            else if (req.IsMethod("setAppEnv"))
            {
                return MyPassThroughView("xx");
            }
            else if (req.IsMethod("ThrowError"))
            {
                throw new Exception("Error, you asked for this");
            }

            return MyPassThroughView("SysMan Main");
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================

}
