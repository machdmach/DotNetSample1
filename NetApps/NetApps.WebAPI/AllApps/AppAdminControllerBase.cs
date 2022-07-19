namespace Libx.Mvc.App;
public class AppAdminControllerBase : MvcAdminControllerDb
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
