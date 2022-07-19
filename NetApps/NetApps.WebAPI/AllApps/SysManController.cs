namespace Libx.Mvc.App;
public class SysManController : SysManControllerBase
{
    public async Task<IActionResult> Index()
    {
        Init();
        await NoopAsync();
        return MyPassThroughView("SysMan Home");
    }
    //===================================================================================================
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> GenCode()
    {
        Init();
        await NoopAsync();

        req.EnsureLocalhost();

        var entityType = typeof(SampleEntity);

        var g = new CodeGenerator(entityType);
        g.Generate_TypeScript_Code();
        g.GenCodeAll();
        //if (False)
        //{
        //   new CodeGenerator(typeof(DataFieldDef)).GetTypeScriptInterfaceDef();
        //}
        responseData.payload = "done";
        //rval = PassThroughView("done");
        return MyActionResult();

    }
}
