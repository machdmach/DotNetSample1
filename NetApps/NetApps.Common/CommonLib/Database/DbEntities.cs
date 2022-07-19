namespace Libx;

//===================================================================================================
public class SqlStmtOptions
{
    //public bool CommaAtStart { get; set; }
    public bool CommaAtTheEnd { get; set; }
    public bool ShowColumnNameAfterValue { get; set; }
    public bool OneColumnOnEachLine { get; set; }
    public bool IncludePrimaryKeys { get; set; }
    public DateTime? DateTimeColumnValue { get; set; }
}

//===================================================================================================
public class DbTableEntity
{
    public DbTableEntity(String tabName) { TableName = tabName; }

    public bool IsOracle { get; set; }
    public String TableName { get; set; }
    public string ColumnNames { get; set; }
    public List<DbColumnEntity> Columns { get; set; }
    public string KeyColumnNames { get; set; }
}

//===================================================================================================
public class DbColumnEntity
{
    ///<see cref="DataColumn.AllowDBNull"/>
    ///<see cref="DbColumn.AllowDBNull"/>
    ///
    [DbCol("COLUMN_NAME")] public string ColumnName { get; set; }
    [DbCol("DATA_TYPE")] public string DataTypeName { get; set; }
    [DbCol("IS_NULLABLE")] public string IsNullable { get; set; }
    public bool AllowDBNull => IsNullable != null;
    [DbCol("CHARACTER_MAXIMUM_LENGTH")] public Decimal? CharMaxLen { get; set; }

    public Type NetDataType { get; set; }
    public DbType ColDbType { get; set; }
    //public SqlDbType DataSqlDbType { get; set; }

    public string MiscInfo
    {
        get
        {
            var buf = new StringBuilder();
            //zbuf.Append("NetDataType: " + NetDataType.FullName);
            buf.Append(", DataDbType: " + ColDbType);
            //buf.Append(", DataSqlDbType: " + DataSqlDbType);
            //zbuf.Append(", SchemaDbType: " + SchemaDbType);
            return buf.ToString();
        }
    }

    [DbCol("ORDINAL_POSITION")] public int OrdinalPosition { get; set; }
    [DbCol("COLUMN_DEFAULT")] public string ColumnDefault { get; set; }
    [DbCol("NUMERIC_PRECISION")] public Decimal? NumericPrecision { get; set; }
    [DbCol("NUMERIC_SCALE")] public Decimal? NumericScale { get; set; }
    [DbCol("TABLE_NAME")] public String TableName { get; set; }
}

//===================================================================================================
public class DbRoutineEntity
{
    public string Catalog { get; set; }
    public string Schema { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Definition { get; set; }
    public string SQL_DATA_ACCESS { get; set; } //--PROCEDURE=MODIFIES, FUNCTION=READS
    public DateTime Created { get; set; }
    public DateTime LastAltered { get; set; }
}

//===================================================================================================
public class DbParameterEntity
{
    //public string Catalog { get; set; }
    //public string Schema { get; set; }
    //public string RoutineName { get; set; }
    public string ParameterName { get; set; }
    public int OrdinalPosition { get; set; }
    public ParameterDirection Mode { get; set; } //Direction: IN, OUT, INOUT, RESULT
    public String ModeStr { get { return Mode.ToString(); } }
    //public bool IsResult { get; set; }
    public Type DataType { get; set; }
    public DbType DataDbType;
    public SqlDbType DataSqlDbType;
    public string DataTypeStr { get; set; }

}

//===================================================================================================
public class ForeignKeyConstraintEntity
{
    public string ConstraintName { get; set; }
    public string ReferencedTable { get; set; }   //Parent=Referenced
    public string ReferencedColumn { get; set; }
    public string ReferencingTable { get; set; }  //Child=Referencing
    public string ReferencingColumn { get; set; }
    public override string ToString()
    {
        //string s = string.Format("{0} parent {1}.{2} - child {3}.{4}", ConstraintName, ReferencedTable, ReferencedColumn, ReferencingTable, ReferencingColumn);
        string s = string.Format("{1}.{2} - {3}.{4}", ConstraintName, ReferencedTable, ReferencedColumn, ReferencingTable, ReferencingColumn);
        return s;
    }
}

//===================================================================================================
public class DbIndexEntity
{
    public string IndexName { get; set; }
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public bool IsPrimaryKey { get; set; }
    public int ColumnOrder { get; set; }
}
