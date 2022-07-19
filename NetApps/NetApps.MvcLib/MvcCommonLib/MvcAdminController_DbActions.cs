namespace Libx.Mvc;
public abstract class MvcAdminControllerDb : MvcAdminControllerBase
{
    //===================================================================================================
    public async Task<IActionResult> InsertRow()
    {
        try
        {
            Init();
            string entName = req.GetEntityNameOrFail();
            using (var db = NewDbContext())
            {
                //responseData.payload = db.ReadLookups_byName(lookupName);
            }
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> UpdateRow()
    {
        return await NoopResutlAsync();
    }
    //===================================================================================================
    public async Task<IActionResult> UpdateRawRow()
    {
        return await NoopResutlAsync();
    }
    //===================================================================================================
    public async Task<IActionResult> DeleteRow()
    {
        return await NoopResutlAsync();
    }
    //===================================================================================================
    public async Task<IActionResult> ReadEntityRaw() => await ReadEntity();
    public async Task<IActionResult> ReadEntity()
    {
        try
        {
            Init();
            var ent = CrudRegistry.GetCrudEntity(req.GetEntityNameOrFail());

            int keyID = req.GetKeyIdOrFail();
            string sql;
            if (keyID == -1)
            {
                sql = db.BuildReadLastRowSql(ent.EntityType);
            }
            else
            {
                sql = db.BuildReadRowSql(ent.EntityType, keyID);
            }
            //var sql = ent.Sql;
            if (req.Is_zzRaw())
            {
                responseData.payload = db.QueryDataTable(sql);
            }
            else
            {
                responseData.payload = db.Query(ent.EntityType, sql);
            }
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> SearchRows()
    {
        try
        {
            Init();
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> RunNamedQueryRaw() => await RunNamedQuery();
    public async Task<IActionResult> RunNamedQuery()
    {
        try
        {
            Init();

            string q = req.QueryData.GetStringOrFail("q");
            var ent = QueryRegistry.GetNamedSql(q, db);
            var sql = ent.Sql;
            if (req.Is_zzRaw())
            {
                responseData.payload = db.QueryDataTable(sql);
            }
            else
            {
                responseData.payload = db.Query(ent.EntityType, sql);
            }
            return MyActionResult();
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
}