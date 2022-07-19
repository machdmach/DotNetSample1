namespace Libx.Mvc.App;
public class AppControllerBase : MvcController_DbActions
{
    protected override void Init()
    {
        base.Init();
        //todo something more here
        if (req.IsDebug)
        {
        }
    }
}
