namespace Libx;
public abstract class MyDbContext_Base : MyDbConnection
{
    //===================================================================================================
    public T ExecuteScalar<T>(string sql, QueryOptions? options = null)
    {
        using (var cmd = CreateCommand(options))
        {
            cmd.CommandText = sql;
            return ExecuteScalar<T>(cmd, options);
        }
    }
    public T ExecuteScalar<T>(DbCommand cmd, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        if (options.Parameters != null)
        {
            SetParameters(cmd, options.Parameters);
        }
        logger.LogSql(cmd);
        var ret = cmd.ExecuteScalar();
        return (T)ret;
    }
    //===================================================================================================
    public DbDataReader ExecuteReader(string sql, QueryOptions? options = null)
    {
        using (var cmd = CreateCommand(options))
        {
            cmd.CommandText = sql;
            return ExecuteReader(cmd, options);
        }
    }
    public DbDataReader ExecuteReader(DbCommand cmd, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        if (options.Parameters != null)
        {
            SetParameters(cmd, options.Parameters);
        }
        logger.LogSql(cmd);
        var ret = cmd.ExecuteReader();
        return ret;
    }

    //===================================================================================================
    /// <summary>
    ///  ExecuteNonQuery
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="options"></param>
    /// <returns>Affected rows</returns>
    public int ExecuteNonQuery(string sql, CrudOptions? options = null)
    {
        using (var cmd = CreateCommand(options))
        {
            cmd.CommandText = sql;
            return ExecuteNonQuery(cmd, options);
        }
    }
    public int ExecuteNonQuery(DbCommand cmd, CrudOptions? options = null)
    {
        options ??= new CrudOptions();
        if (options.Parameters != null)
        {
            SetParameters(cmd, options.Parameters);
        }
        logger.LogSql(cmd);
        var k = cmd.ExecuteNonQuery();
        if (k < options.MinAffectedRows && options.ThrowIfAffectedRowsOutOfRange)
        {
            throw new Exception($"Expected MinAffectedRows {options.MinAffectedRows}, but actual was: " + k);
        }
        else if (options.MaxAffectedRows < k && options.ThrowIfAffectedRowsOutOfRange)
        {
            throw new Exception($"Expected MaxAffectedRows {options.MaxAffectedRows}, but actual was: " + k);
        }
        logger.LogInformation($"Expected AffectedRows min/max: {options.MinAffectedRows}/{options.MaxAffectedRows}, actual: " + k);
        return k;
    }

    //===================================================================================================
    public string WrapLiteral(string s)
    {
        if (SqlReservedKeywords2.IsReserved(s))
        {
            s = string.Format("\"{0}\"", s);
        }
        return s;
    }
}

