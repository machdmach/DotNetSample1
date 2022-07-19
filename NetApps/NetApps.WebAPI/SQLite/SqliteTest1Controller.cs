using Microsoft.AspNetCore.Html;
using Microsoft.Data.Sqlite;

namespace Libx.Mvc.App;

/// <summary>
/// MVC Controller to test functionalities and features of the In-Memory SQLite DBMS
/// </summary>
public class SQLiteTest1Controller : AppAdminControllerBase
{
    protected override void Init()
    {
        base.Init();

        //Ensure this controller to be called on Dev Machine only (or when running as a sample app)
        req.EnsureDevMachine();
    }

    protected override Func<MyDbContext> NewDbContext => () =>
    {
        //dbConnStrKey must already set by calling method UseConnStr()
        //logger has been initialized by the base Init() method
        var ret = new SQLiteDbContext(dbConnStrKey, logger);
        return ret;
    };

    //===================================================================================================
    //https://localhost:44321/api/AdhocDb1/SQLiteSample1
    public async Task<IActionResult> SQLiteSample1()
    {
        try
        {
            //Initialize various variables and components (such as logger)
            Init();

            //Read SQL script from a text file
            string sqlFilePath = "Sample/script1.sql";
            string sql = DbManLib.ReadSqlFile(sqlFilePath);

            UseConnStr("Data Source=InMemorySample;Mode=Memory;Cache=Shared");

            //db is an on-demand database connection
            var scalarResult = db.ExecuteScalar<Object>("select 1");

            //responseData.payload = db.ExecuteNonQuery(sql);
            responseData.payload = db.ExecuteScalar<string>(sql);

            //Rollback transaction if there is one and not already explicitly committed
            //Show the result in HTML if requested for text/html, otherwise in JSON format
            return MyActionResult();
        }
        catch (Exception ex)
        {
            //Rollback transaction if there is one.
            //Log Send email alert about the error
            //Show debug info in the output to help with identify and fixing the exception
            return await MyErrorResult(ex);
        }
    }
}
