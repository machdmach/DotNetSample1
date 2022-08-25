namespace Libx;
public partial class DbTableInfo
{
    protected MyDbContext db;
    public DbTableInfo(MyDbContext db) { this.db = db; }

    //===================================================================================================
    public DataTable GetRawColumns(string tableName)
    {
        string sqlServer = @"select col.*
            from INFORMATION_SCHEMA.COLUMNS  col
             join INFORMATION_SCHEMA.TABLES tab on col.TABLE_NAME = tab.TABLE_NAME
            {wherec}
           order by tab.TABLE_NAME, col.ORDINAL_POSITION
";
        string oracle = @"select col.*
            from USER_TAB_COLUMNS  col
             join USER_TABLES tab on col.TABLE_NAME = tab.TABLE_NAME
           {wherec}
            order by tab.TABLE_NAME, col.COLUMN_ID
";
        string sql = db.IsSQLServer ? sqlServer : oracle;
        if (tableName != "*")
        {
            sql = sql.Replace("{wherec}", $"where tab.TABLE_NAME='{tableName}'");
        }
        else
        {
            sql = sql.Replace("{wherec}", "");
        }
        return db.QueryDataTable(sql);
    }

    //===================================================================================================
    public List<DbColumnEntity> GetColumns(string tableName)
    {
        //select Row_Number = row_number() over (order by tab.TABLE_NAME, col.ORDINAL_POSITION),
        string sqlServer = @"select
             tab.TABLE_NAME,
             col.COLUMN_NAME,
             col.ORDINAL_POSITION,
             col.COLUMN_DEFAULT,
             col.IS_NULLABLE, 
             col.DATA_TYPE,
             col.CHARACTER_MAXIMUM_LENGTH,
             col.NUMERIC_PRECISION,
             col.NUMERIC_SCALE 
            from INFORMATION_SCHEMA.COLUMNS  col
             join INFORMATION_SCHEMA.TABLES tab on col.TABLE_NAME = tab.TABLE_NAME
            {wherec}
           order by tab.TABLE_NAME, col.ORDINAL_POSITION
";
        string oracle = @"select
             tab.TABLE_NAME,
             col.COLUMN_NAME,
             cast(col.COLUMN_ID as INTEGER) as ORDINAL_POSITION,
             col.DATA_DEFAULT as COLUMN_DEFAULT,
             col.NULLABLE as IS_NULLABLE, 
             col.DATA_TYPE,
             col.CHAR_LENGTH as CHARACTER_MAXIMUM_LENGTH,
             col.DATA_PRECISION as Numeric_Precision,
             col.DATA_SCALE as Numeric_Scale 
            from USER_TAB_COLUMNS  col
             join USER_TABLES tab on col.TABLE_NAME = tab.TABLE_NAME
           {wherec}
            order by tab.TABLE_NAME, col.COLUMN_ID
";
        string sql = db.IsSQLServer ? sqlServer : oracle;
        if (tableName != "*")
        {
            sql = sql.Replace("{wherec}", $"where tab.TABLE_NAME='{tableName}'");
        }
        else
        {
            sql = sql.Replace("{wherec}", "");
        }
        var colList = db.Query<DbColumnEntity>(sql).ToList();
        colList.ForEach(ent =>
        {
            if (ent.IsNullable == "NO" || ent.IsNullable == "N") { ent.IsNullable = null; }
            if (ent.CharMaxLen == 0) { ent.CharMaxLen = null; }
        });
        return colList;

        //var dt = db.QueryDataTable(sql);
        //MOutput.WriteHtmlValueOf(dt);
        //MOutput.WriteHtmlValueOf(dt.Columns);
        //var colList = new List<DbColumnEntity>();
        //foreach (DataRow r in dt.Rows)
        //{
        //    var ent = new DbColumnEntity();
        //    int i = -1; //RowNumber
        //    ent.TableName = (String)r[++i];
        //    ent.ColumnName = (String)r[++i];
        //    //throw new Exception("zz" + r[++i].GetType());

        //    ent.OrdinalPosition = Int32.Parse("" + r[++i]);


        //    ent.ColumnDefault = r[++i] + ""; //DbNull = {}
        //    ent.IsNullable = (String)r[++i];
        //    ent.SchemaDbType = (r[++i] + "").ToUpper();
        //    ent.NetDataType = DbTypeConvertor.ToNetType(ent.SchemaDbType);

        //    //ent.SchemaDbType = DbTypeConvertor.CorrectCasing(ent.SchemaDbType, DbConnectionX.IsOracleConnection(dbc.Connection.ConnectionString));

        //    //SqlDbType sqlDbType;
        //    //Enum.TryParse(ent.SchemaDbType, out sqlDbType);

        //    //ent.DataSqlDbType = sqlDbType;
        //    //ent.DataSqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), ent.DataTypeStr, true);
        //    //ent.DataDbType = DbTypeConvertor.ToDbType(ent.DataSqlDbType);

        //    //ent.DataType = DbTypeX.ToDbType(
        //    //https://gist.github.com/abrahamjp/858392

        //    //ent.CharMaxLen = r.IsNull(++i) ? "" : "" + r[i]; //Int32
        //    //ent.NumericPrecision = r.IsNull(++i) ? "" : "" + r[i]; //Byte
        //    //ent.NumericScale = r.IsNull(++i) ? "" : "" + r[i];  //Int32

        //    if (ent.IsNullable == "NO") { ent.IsNullable = ""; }
        //    colList.Add(ent);
        //}
        //if (tableName != "*")
        //{
        //    colList = colList.Where(r => r.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
        //    if (colList.Count < 1)
        //    {
        //        throw new Exception("No columns found for table: " + tableName);
        //    }
        //}
    }
}
