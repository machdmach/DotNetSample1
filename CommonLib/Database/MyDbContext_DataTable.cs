using System.Collections.ObjectModel;

namespace Libx;
public abstract partial class MyDbContext
{
    //===================================================================================================
    public DataTable QueryDataTable(string sql, QueryOptions? options = null)
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return QueryDataTable(cmd, options);
        }
    }
    public DataTable QueryDataTable(DbCommand cmd, QueryOptions? options = null)
    {
        using (var reader = ExecuteReader(cmd, options))
        {
            return QueryDataTable(reader, options);
        }
    }
    public void ShowMultipleResults(string sql)
    {
        var options = new QueryOptions();
        var db = this;
        MOutput.WriteHtmlEncodedPre(sql);
        using (var cmd = db.CreateCommand(options))
        {
            cmd.CommandText = sql;
            var reader = db.ExecuteReader(cmd, options);
            do
            {
                var dt = QueryDataTable(reader, options);
                //MOutput.WriteHtmlEncodedPre(reader.AsQueryable)
                MOutput.Write(HtmlValue.OfDataTable(dt));
            } while (reader.NextResult());
            //while (reader.NextResult())
            //{
            //    dt = QueryDataTable(reader, options);
            //    MOutput.Write(HtmlValue.OfDataTable(dt));
            //}
        }
    }
    public DataTable QueryDataTable(DbDataReader reader, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        DataRow row;
        var colCount = reader.FieldCount;

        ReadOnlyCollection<DbColumn> cols = reader.GetColumnSchema();
        var dt = new DataTable();

        foreach (DbColumn col in cols)
        {
            var dataCol = new DataColumn(col.ColumnName, col.DataType);
            dt.Columns.Add(dataCol);
        }

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
            row = dt.NewRow();
            var arr = new Object[colCount];
            reader.GetValues(arr);
            row.ItemArray = arr;
            dt.Rows.Add(row);
        }
        int rowsFound = dt.Rows.Count;
        if (rowsFound < options.MinRows)
        {
            throw new Exception($"MinRows expected is {options.MinRows}, but actual is {rowsFound}");
        }
        return dt;
    }

    //===================================================================================================
    public string BuildReadLastRowSql(Type entityType)
    {
        PropColMapper mapper = new(entityType, logger, new() { PrepareForWrite = true });

        var buf = new StringBuilder();
        string s = "uninit";
        var keyCol = mapper.GetTableKeyOrFail();
        if (IsSQLServer)
        {
            //s = string.Format("select * from {0} order by {1} desc", mapper.TableName, keyCol.ColumnName);
        }
        else if (IsOracle)
        {
            s = string.Format("select * from {0} order by {1} desc  fetch  first 1 rows only", mapper.TableName, keyCol.ColumnName);
        }
        else
        {
            throw new Exception("Unknown db");
        }
        return s;
    }

    //===================================================================================================
    public string BuildReadRowSql(Type entityType, int keyId)
    {
        PropColMapper mapper = new(entityType, logger, new() { PrepareForWrite = true });
        var keyCol = mapper.GetTableKeyOrFail();

        var buf = new StringBuilder();
        buf.AppendFormat("select * from {0} where {1} = {2}", mapper.TableName, keyCol.ColumnName, keyId);
        return buf.ToString();
    }
}
