namespace Libx;
public abstract partial class MyDbContext
{
    //===================================================================================================
    public T First<T>(string sql, QueryOptions? options = null) where T : new()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return First<T>(cmd, options);
        }
    }
    public T First<T>(DbCommand cmd, QueryOptions? options = null) where T : new()
    {
        using (var reader = ExecuteReader(cmd, options))
        {
            return First<T>(reader, options);
        }
    }
    public T First<T>(DbDataReader reader, QueryOptions? options = null) where T : new()
    {
        options = CheckOptions(options);
        options.MinRows = 1;
        options.MaxRows = 1;
        options.ThrowOnMaxRowsExceeded = false;
        var lst = Query<T>(reader, options);
        if (lst.Count() > 0)
        {
            return lst.First();
        }
        else
        {
            throw new Exception("never");
        }
    }

    //===================================================================================================
    public T? FirstOrDefault<T>(string sql, QueryOptions? options = null) where T : new()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return FirstOrDefault<T>(cmd, options);
        }
    }
    public T? FirstOrDefault<T>(DbCommand cmd, QueryOptions? options = null) where T : new()
    {
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            return FirstOrDefault<T>(reader, options);
        }
    }
    public T? FirstOrDefault<T>(DbDataReader reader, QueryOptions? options = null) where T : new()
    {
        options = CheckOptions(options);
        options.MaxRows = 1;
        var lst = this.Query<T>(reader, options);
        if (lst.Count() > 0)
        {
            return lst.First();
        }
        else
        {
            return default(T);
        }
    }

    //===================================================================================================
    public T Single<T>(string sql, QueryOptions? options = null) where T : new()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return Single<T>(cmd, options);
        }
    }
    public T Single<T>(DbCommand cmd, QueryOptions? options = null) where T : new()
    {
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            return Single<T>(reader, options);
        }
    }
    public T Single<T>(DbDataReader reader, QueryOptions? options = null) where T : new()
    {
        options = CheckOptions(options);
        options.MaxRows = 1;
        options.MinRows = 1;
        options.ThrowOnMaxRowsExceeded = true;
        var lst = Query<T>(reader, options);
        if (lst.Count() == 1)
        {
            return lst.First();
        }
        else
        {
            throw new Exception("never");
        }
    }

    //===================================================================================================
    public T SingleOrDefault<T>(string sql, QueryOptions? options = null) where T : new()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return SingleOrDefault<T>(cmd, options);
        }
    }
    public T? SingleOrDefault<T>(DbCommand cmd, QueryOptions? options = null) where T : new()
    {
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            return SingleOrDefault<T>(reader, options);
        }
    }
    public T? SingleOrDefault<T>(DbDataReader reader, QueryOptions? options = null) where T : new()
    {
        options = CheckOptions(options);
        options.MaxRows = 1;
        options.ThrowOnMaxRowsExceeded = true;
        var lst = Query<T>(reader, options);
        if (lst.Count() == 1)
        {
            return lst.First();
        }
        else
        {
            return default(T);
        }
    }

    //===================================================================================================
    public IEnumerable<T> Query<T>(string sql, QueryOptions? options = null) where T : new()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return Query<T>(cmd, options);
        }
    }
    public IEnumerable<T> Query<T>(DbCommand cmd, QueryOptions? options = null) where T : new()
    {
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            return Query<T>(reader, options);
        }
    }
    public IEnumerable<T> Query<T>(DbDataReader reader, QueryOptions? options = null) where T : new()
    {
        Type entityType = typeof(T);
        var lst = Query(entityType, reader, options);
        var ret = lst.Cast<T>();
        return ret;
    }

}
