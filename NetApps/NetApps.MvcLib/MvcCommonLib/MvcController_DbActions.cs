namespace Libx.Mvc;
public class MvcController_DbActions : MvcControllerBase
{
    protected virtual Func<MyDbContext> NewDbContext => () =>
    {
        var ret = new AutoDbContext(dbConnStrKey, logger);
        return ret;
    };
    protected virtual MyDbContext db
    {
        get
        {
            if (_db == null) { _db = NewDbContext(); }
            return _db;
        }
    }
    //===================================================================================================
    protected override void PreReturnActionResult()
    {
        DisposeDbContext();
        base.PreReturnActionResult();
    }
    //===================================================================================================
    protected override void GetExtraDataSvcInfo(DataSvcInfo info)
    {
        base.GetExtraDataSvcInfo(info);
        //using (var dbc = NewDbContext())
        {
            db.GetDatabaseInfo(info);
        }
    }


    //===================================================================================================
    public async Task<IActionResult> ReadActiveLookups()
    {
        try
        {
            Init();
            var lookupName = req.method;
            //var addlKeyId = req.QueryData.GetStringOrDefault("addlKeyId");
            using (var db = NewDbContext())
            {
                responseData.payload = db.ReadLookups_byName(lookupName);
            }
            return MyActionResult();
        }
        catch (Exception ex)
        {
            return await MyErrorResult(ex);
        }
    }
    //===================================================================================================
    public async Task<IActionResult> ReadActiveChildLookups()
    {
        try
        {
            Init();
            string lookupName = req.method;
            int parentKeyId = req.keyId;
            if (parentKeyId == 0)
            {
                throw new Exception("Param KeyId required");
            }
            using (var db = NewDbContext())
            {
                responseData.payload = db.ReadLookups_byName(lookupName, parentKeyId);
            }
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
}