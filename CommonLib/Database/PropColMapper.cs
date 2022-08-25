using System.ComponentModel.DataAnnotations;
using System.Reflection;
//https://martinfowler.com/eaaCatalog/dataMapper.html

namespace Libx;
public class PropColMapperOptions
{
    public bool PrepareForWrite { get; set; }
}
public class PropColMapper
{
    public int PropertyCount = 0;

    public Dictionary<string, PropCol> GetKeyedByColumnNamePropCols()
    {
        Dictionary<string, PropCol> ret = new(new NormalizeColumnNameEqualityComparer());
        AllPropCols.ForEach(e => ret.Add(e.ColumnName, e));
        return ret;
    }
    public Dictionary<string, PropCol> KeyedByPropNamePropCols = new(new NormalizeColumnNameEqualityComparer());
    public List<PropCol> AllPropCols = new(); // => KeyedByPropNamePropCols.Values.OrderBy(e => e.PropertyOrder).ToList();
    public List<PropCol> MappedPropCols => AllPropCols.Where(e => e.ColumnDataType != null).ToList(); //new List<PropCol>();
    public List<PropCol> UnMappedPropCols => AllPropCols.Where(e => e.ColumnDataType == null).ToList(); //new List<PropCol>();
    public List<String> UnMappedPropNames => UnMappedPropCols.Select(e => e.PropertyName).ToList();
    public List<String> UnMappedColumnNames = new List<String>();
    protected MLogger logger;
    public string TableName;
    public PropCol TableKey;
    public PropCol GetTableKeyOrFail()
    {
        var ret = TableKey;
        if (ret == null) throw new Exception("TableKey not defined");
        return ret;
    }
    //===================================================================================================
    public PropColMapper(Type type, MLogger logger, PropColMapperOptions options = null)
    {
        options ??= new PropColMapperOptions();

        this.logger = logger;
        //Type type = typeof(T);
        BindingFlags targetBindingFlags;
        if (options.PrepareForWrite)
        {
            targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty; // | BindingFlags.DeclaredOnly;
        }
        else
        {
            targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty; // | BindingFlags.DeclaredOnly;
        }

        var props = type.GetProperties(targetBindingFlags); //.Where(p => p.CanWrite).ToList();

        var tabAttr = type.GetCustomAttribute<DbTabAttribute>(false);
        if (tabAttr != null)
        {
            TableName = tabAttr.TableName;
        }

        foreach (var prop in props)
        {
            var pc = new PropCol(prop);
            pc.PropertyOrder = PropertyCount++;
            if (pc.IsPrimaryKey)
            {
                TableKey = pc;
            }
            AllPropCols.Add(pc);
            KeyedByPropNamePropCols.Add(pc.PropertyName, pc);
        }
    }

    //===================================================================================================
    public void AddPropColMapping(String propName, string colName)
    {
        //var pc = AllPropCols.FirstOrDefault(e => e.PropertyName == propName);
        KeyedByPropNamePropCols.TryGetValue(colName, out PropCol? pc);
        if (pc == null)
        {
            throw new Exception("pop not found with name: " + propName);
        }
        logger.LogInfo("propName mapped: " + propName);
        pc.ColumnName = colName;
    }
    //===================================================================================================
    public void AddPropColMappings(Dictionary<string, string> addlPropColNamePairs)
    {
        if (addlPropColNamePairs == null)
        {
            logger.LogInfo("addlPropColNamePairs is null");
            return;
        }
        logger.LogHtml(HtmlValue.OfList(addlPropColNamePairs, new() { Caption = nameof(AddPropColMappings) }));
        foreach (var kv in addlPropColNamePairs)
        {
            AddPropColMapping(kv.Key, kv.Value);
        }
    }
    //===================================================================================================
    public void MapTableColumns(DbConnection dbc)
    {
        if (string.IsNullOrEmpty(TableName))
        {
            throw new Exception("TableName is not found");
        }
        MapTableColumns(TableName, dbc);
    }
    //===================================================================================================
    public void MapTableColumns(string tableName, DbConnection dbc)
    {
        string sql = $"select * from {tableName} where 1=2";
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            MapDbColumns(cmd);
        }
    }
    //===================================================================================================
    public void MapDbColumns(DbCommand cmd)
    {
        logger.LogSql(cmd);
        using (var rdr = cmd.ExecuteReader())
        {
            MapDbColumns(rdr);
        }
    }
    //===================================================================================================
    public void MapDbColumns(DbDataReader reader)
    {
        //var nameMaps = nameMapsP ?? new List<PropCol>();
        //reader.GetColumnSchema();

        //var props = KeyedAllPropCols; //AllPropCols_keyedByColName();
        var props = GetKeyedByColumnNamePropCols();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            string colName = reader.GetName(i);

            //var pc = props.FirstOrDefault(p => NamesEqual(p.ColumnName, colName));
            //props.TryGetValue(NormalizeColumnName(colName), out pc);
            props.TryGetValue(colName, out PropCol? pc);

            if (pc == null)
            {
                UnMappedColumnNames.Add(colName);
                //continue;
            }
            else
            {
                pc.ColumnName = colName;
                pc.ColumnOrdinal = i;
                pc.ColumnDataType = reader.GetFieldType(i);
                pc.ColumnDataTypeName = reader.GetDataTypeName(i);

                pc.PropColTypesMatched = (pc.ColumnDataType == pc.PropDataType);
                if (!pc.PropColTypesMatched)
                {
                    string mesg = "Types do not match: " + pc.toStringShort();
                    logger.LogInfo(mesg);
                }
                //MappedPropCols.Add(pc);
            }
        }
        logger.LogInfo("unmapped columns: " + String.Join(", ", UnMappedColumnNames));
        logger.LogInfo("unmapped props: " + String.Join(", ", UnMappedPropNames));
    }
    ////===================================================================================================
    //internal static string NormalizeColumnName(string colName)
    //{
    //    return colName.Trim().Replace("_", "").ToLower();
    //}

    ////===================================================================================================
    //internal static bool NamesEqualzz(string name1, string name2)
    //{
    //    if (name1 == name2)
    //    {
    //        return true;
    //    }
    //    name1 = name1.Trim().Replace("_", "").ToLower();
    //    name2 = name2.Trim().Replace("_", "").ToLower();
    //    return name1 == name2;
    //}
}