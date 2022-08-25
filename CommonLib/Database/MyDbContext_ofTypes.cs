namespace Libx;
public abstract partial class MyDbContext
{
    //===================================================================================================
    public Object SingleOrDefault(Type entityType, string sql, QueryOptions? options = null)
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return SingleOrDefault(entityType, cmd, options);
        }
    }
    public Object SingleOrDefault(Type entityType, DbCommand cmd, QueryOptions? options = null)
    {
        using (var reader = ExecuteReader(cmd, options))
        {
            return SingleOrDefault(entityType, reader, options);
        }
    }
    public Object? SingleOrDefault(Type entityType, DbDataReader reader, QueryOptions? options = null) 
    {
        options = CheckOptions(options);
        options.MaxRows = 1;
        options.ThrowOnMaxRowsExceeded = true;
        var lst = Query(entityType, reader, options);
        if (lst.Count() == 1)
        {
            return lst.First();
        }
        else
        {
            return Activator.CreateInstance(entityType);
        }
    }

    //===================================================================================================
    public IEnumerable<Object> Query(Type entityType, string sql, QueryOptions? options = null)
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return Query(entityType, cmd, options);
        }
    }
    //===================================================================================================
    public IEnumerable<Object> Query(Type entityType, DbCommand cmd, QueryOptions? options = null)
    {
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            return Query(entityType, reader, options);
        }
    }
    public IEnumerable<Object> Query(Type entityType, DbDataReader reader, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        var typeInfo = new TypeInfoX(entityType);
        if (typeInfo.IsSimpleType)
        {
            return QueryOfSimpleType(entityType, reader, options); //------------------------------------
        }

        var mapper = new PropColMapper(entityType, logger, new() { PrepareForWrite = true });
        mapper.AddPropColMappings(options.propColNameMaps);
        mapper.MapDbColumns(reader);

        if (options.ThrowIfAnyColumnsUnMapped && mapper.UnMappedColumnNames.Count() > 0)
        {
            throw new Exception("unmapped columns: " + String.Join(",", mapper.UnMappedColumnNames));
        }

        var ret = new List<Object>();
        var maps = mapper.MappedPropCols.OrderBy(e => e.ColumnOrdinal).ToArray();
        int fieldCount = maps.Count();
        int k = 0;
        while (reader.Read())
        {
            if (++k > options.MaxRows)
            {
                if (options.ThrowOnMaxRowsExceeded)
                {
                    throw new Exception($"The number of rows returning has exceeeded MaxRows: {options.MaxRows}");
                }
                break;
            }

            //var ent = new T();
            Object ent = Activator.CreateInstance(entityType);
            for (var i = 0; i < fieldCount; i++)
            {
                var m = maps[i];
                m.ReadDbAndSetValue(ent, reader);
            }
            ret.Add(ent);
        }
        logger.LogInfo(k + " rows found,.");

        int rowsFound = ret.Count;
        if (rowsFound < options.MinRows)
        {
            throw new Exception($"MinRows expected is {options.MinRows}, but actual is {rowsFound}");
        }
        return ret;
    }
    //===================================================================================================
    public IEnumerable<Object> QueryOfSimpleType(Type entityType, DbDataReader reader, QueryOptions? options = null)
    {
        options = CheckOptions(options);

        if (reader.FieldCount != 1)
        {
            throw new Exception("reader.FieldCount must be 1, actual=" + reader.FieldCount);
        }
        var ret = new List<Object>();
        int k = 0;
        while (reader.Read())
        {
            if (++k > options.MaxRows)
            {
                if (options.ThrowOnMaxRowsExceeded)
                {
                    throw new Exception($"The number of rows returning has exceeeded MaxRows: {options.MaxRows}");
                }
                break;
            }
            object val = reader.GetValue(0);
            if (val != DBNull.Value)
            {
                ret.Add(val);
            }
        }
        logger.LogInfo(k + " rows found,.");

        int rowsFound = ret.Count;
        if (rowsFound < options.MinRows)
        {
            throw new Exception($"MinRows expected is {options.MinRows}, but actual is {rowsFound}");
        }
        return ret;
    }

}
