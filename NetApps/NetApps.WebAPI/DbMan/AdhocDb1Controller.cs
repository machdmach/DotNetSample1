using Microsoft.AspNetCore.Html;

namespace Ccdoa.Mvc.App;
public class AdhocDb1Controller : AppAdminControllerBase
{
    protected override void Init()
    {
        base.Init();
        req.EnsureDevMachine();
    }

    //===================================================================================================
    //https://localhost:44321/api/AdhocDb1/GetPageLinks
    public async Task<IActionResult> GetPageLinks()
    {
        try
        {
            Init();
            string sqlFilePath = "AppSec/Page_Links.sql";
            string sql = DbManLib.ReadSqlFile(sqlFilePath);

            //UseConnStr("DoingBusiness.dev");
            UseConnStr("AppSec.prod");
            //db.StartTransaction();

            var resultTab = db.QueryDataTable(sql);
            foreach (DataRow row in resultTab.Rows)
            {
                row[2] = string.Format("<a href='{0}' target='win2'> {0} </a>", row[2]);
            }
            responseData.payload = resultTab;
            //db.CommitTransaction();

            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
        //catch (Exception ex) when (NotRunThis) { return await MyErrorResult(ex); } //do not handle exception
    }
}
