namespace Libx.Mvc.App;
public class AdhocText1Controller : AppAdminControllerBase
{
    protected override void Init()
    {
        base.Init();
        req.EnsureDevMachine();

        Microsoft.AspNetCore.Html.HtmlContentBuilder builder = new Microsoft.AspNetCore.Html.HtmlContentBuilder();

    }
    //===================================================================================================
    public async Task<IActionResult> Do1()
    {
        try
        {
            Init();
            string pfname = @"C:\ws\NetApps\NetApps.WebAPI\zzData.txt";
            string s = System.IO.File.ReadAllText(pfname);

        
            responseData.payload = s;
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
}

