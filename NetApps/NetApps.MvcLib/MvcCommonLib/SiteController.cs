namespace NetApps;
public class SiteController : MvcControllerBase
{
    //===================================================================================================
    public IActionResult Error1()
    {
        return MyPassThroughView("An error has occured, please try again later.");
    }
    
    //===================================================================================================
    public async Task<IActionResult> Test4()
    {
        try
        {
            Init();
            return MyPassThroughView("444444444444444");
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }

}
