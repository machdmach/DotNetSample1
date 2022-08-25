namespace Libx;

public abstract class UpsertCommandBuilder
{
    //public class Options
    //{
    //    //[Obsolete]
    //    //public bool IsParameterized { get; set; }
    //    public bool CommaAtTheBeginning { get; set; }
    //    public bool ShowColumnNameAfterValue { get; set; }
    //    public bool OneColumnOnEachLine { get; set; }
    //    //public bool IncludePrimaryKeys { get; set; }
    //    //public DateTime? DateTimeColumnValue { get; set; }
    //    public string OutputPrimaryKeyName { get; set; }
    //}
    public CrudOptions options;
    public bool IsInserting;

    public bool SkipDefaultValues => IsInserting && !options.InsertDefaultValues;

    public DbCommand DbCommand;
    //public virtual string ParameterPrefix { get { return "@"; } } //: for Oracle, @ for MsSQL
    public string ParameterPrefix = "?"; //ParameterMarker
    public bool IsParameterizedValues = true;
    protected readonly NameObjectCollection _nvs = new();
    public string TableName;
    public List<String> ParameterizedNames = new();

    //public UpsertCommandBuilder(DbConnection conn) : this(conn.CreateCommand()){}
    public UpsertCommandBuilder(DbCommand dbCommand, CrudOptions options)
    //public void Init(DbCommand dbCommand, CrudOptions options)
    {
        this.options = options;
        //dbCommand.Connection
        this.DbCommand = dbCommand;
        //OracleCommand

        //var cmdTypeStr = dbCommand.GetType().ToString();
        ////string connStr = dbCommand.Connection.ConnectionString.ToLower();
        ////if (connStr.Contains("initial catalog"))
        //if (cmdTypeStr.Contains("Oracle"))
        //{
        //    ParameterPrefix = ":";  //Oracle
        //}
        //else
        //{
        //    ParameterPrefix = "@"; //MsSql
        //}
    }

    //******************************************************************************

    //===================================================================================================
    private static void Main()
    {
        //Log.Debug(DBNull.Value == null);
        //Log.Debug(DBNull.Value);


        //SqlMaintStatementBuilder stmt = new SqlMaintStatementBuilder("psa_requests");
        //stmt["key_id"] = Guid.NewGuid().ToString();
        //stmt["full_name"] = "John Smith";
        //stmt["age"] = 123;
        //stmt["blanks"] = " "; //=null
        ////stmt["null"] = null; //error nullExc
        ////stmt.SetNonString("age2", "123");
        //stmt["x"] = DBNull.Value;
        //stmt["timestamp"] = DateTime.Now;

        //log.Debug(stmt.InsertStatement);
        //stmt.WhereClause = "key_id=" + 111;
        //log.Debug(stmt.UpdateStatement);
    }
    //=========================================================================================
    public virtual void SetValueExact(string name, object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value cannot be null");
        }
        string val = value + "";
        _nvs[name] = val;
    }
    //=========================================================================================
    public virtual void SetString(String name, string val)
    {
        if (IsParameterizedValues)
        {
            SetParam(name, val, DbType.String);
        }
        else
        {
            SetStringTrusted(name, val);
        }
    }
    //===================================================================================================
    public void SetParam(String name, object value, DbType dbType)
    {
        DbParameter param = DbCommand.CreateParameter();
        //.Parameters.Add(name, dbType);
        param.ParameterName = name;
        param.Value = value;
        param.DbType = dbType;
        param.Direction = ParameterDirection.Input;

        DbCommand.Parameters.Add(param);
        ParameterizedNames.Add(name);
        SetValueExact(name, ParameterPrefix + name);
    }

    //=========================================================================================
    //public Action<String, int, object> AddParameterImpl;
    //public void SetValueParameterizedz(String name, int dataType, object value)
    //{
    //    ParameterizedNames.Add(name);
    //    //MOutput.WriteLine("dataType value=" + dataType);
    //    AddParameterImpl(name, dataType, value);
    //    SetValueExact(name, "param");
    //}

    //===================================================================================================
    public virtual void SetStringTrusted(String colName, string val) //, string defaultVal)
    {
        string s = val;
        if (val != null)
        {
            s = s.Trim();
        }

        if (s == null)
        {
            SetValueNull(colName);
        }
        else
        {
            s = SqlLiteral.EscapeEncloseSQuoteAndAmpersand(s);
        }
        SetValueExact(colName, s);
    }
    //===================================================================================================
    public virtual void SetStringOrDefault(String name, string val, string defaultVal)
    {
        if (string.IsNullOrWhiteSpace(val))
        {
            val = defaultVal;
        }
        SetString(name, val);  //def can be from subclass
    }
    //===================================================================================================
    public virtual void SetStringCropTo(String name, string value, int maxLen)
    {
        string croppedValue = value;
        if (!string.IsNullOrEmpty(croppedValue))
        {
            croppedValue = croppedValue.Trim();
            if (croppedValue.Length > 50)
            {
                croppedValue = croppedValue.Substring(0, 50);
                croppedValue = croppedValue.Trim();
            }
        }
        SetString(name, croppedValue);
    }

    //=========================================================================================
    public const string dd_MMM_yyyy = "dd-MMM-yyyy";
    public virtual string DbDateTimeFormat =>
            //return "yyyy-MM-dd HH:mm:ss"; //'2008-11-12 19:30:10'
            "dd-MMM-yyyy HH:mm:ss"; //'15-Mar-2008 19:30:10'
    public virtual string GetDateTimeDbToken(DateTime dt)
    {
        string dtStr = dt.ToString(DbDateTimeFormat);
        string quotedVal = string.Format("'{0}'", dtStr);
        return quotedVal;
    }

    public virtual void SetDateTime(String colName, DateTime? dt)
    {
        if (dt == null || !dt.HasValue)
        {
            SetValueNull(colName);
        }
        else
        {
            if (dt == default(DateTime))
            {
                SetValueNull(colName);
            }
            else
            {
                string val = GetDateTimeDbToken(dt.Value);
                SetValueExact(colName, val);
            }
        }
    }
    //===================================================================================================
    public virtual void SetBoolean(String colName, object boolObj)
    {
        if (boolObj == null || boolObj == DBNull.Value)
        {
            SetValueNull(colName);
        }
        else
        {
            if (!(boolObj is Boolean))
            {
                throw new Exception("value is not of type Boolean, actual=" + boolObj.GetType());
            }
            Boolean b = (Boolean)boolObj;
            string v = b ? "1" : "0";
            SetValueExact(colName, v);
        }
    }
    //===================================================================================================
    public virtual void SetBLOB(String colName, byte[] blob)
    {
        throw new NotImplementedException("");
    }
    //=========================================================================================
    public void SetValueNull(String colName)
    {
        if (SkipDefaultValues)
        {
            //nothing
        }
        else
        {
            SetValueExact(colName, "null");
        }
    }


    //=========================================================================================
    //public void SetValues(IDataRecord rec)
    //{
    //    // DataRow dr;
    //    for (int i = 0; i < rec.FieldCount; i++)
    //    {
    //        string colName = rec.GetName(i);
    //        object value = rec.GetValue(i);
    //        SetValue(colName, value);
    //    }
    //}
    //******************************************************************************
    //public void SetDefaultValue(DataColumn dc)
    //{
    //    string colName = dc.ColumnName;
    //    var dataType = dc.DataType;
    //    object val = DBNull.Value;
    //    if (dataType == typeof(string))
    //    {
    //        val = colName;
    //    }
    //    //DbType dbType = DbTypeX.TypeMap[dc.DataType];
    //    SetValue(colName, val, dataType);
    //}

    //===================================================================================================
    public void SetValue(String colName, object val)
    {
       
        if (val == null || val.GetType() == typeof(System.DBNull))
        {
            if (SkipDefaultValues)
            {
                return; //-----------------------
            }
            SetValueNull(colName);
        }
        var defaulVal = ObjectX.GetDefaultValue(val.GetType());
        if (colName == "zxRECORD_ID")
        {
            throw new Exception("val=" + val + ", def=" + defaulVal);
        }
        if (val.Equals(defaulVal))
        {
            if (SkipDefaultValues)
            {
                return; //-----------------------
            }
        }

        var t = new TypeInfoX(val.GetType());
        //if (t.IsNullable)
        //{
        //    //var str = val.ToString();
        //    //if (str.Length == 0)
        //    //{
        //    //    SetValueNull(colName);
        //    //    return; //----------------------------
        //    //}
        //    //val = str;
        //    //val = Convert.ChangeType(val, t.UnderlyingType); //not needed
        //    //Nullable<string> t2 = val;
        //}
        if (t.IsString)
        {
            SetString(colName, (string)val);
        }
        else if (t.IsNumeric)
        {
            SetValueExact(colName, val.ToString());
        }
        else if (t.IsDateTime)
        {
            SetDateTime(colName, (DateTime)val);
        }
        else if (t.IsBoolean)
        {
            SetBoolean(colName, val);
        }
        else if (t.IsChar)
        {
            SetString(colName, val.ToString());
        }
        else
        {
            if (ConfigX.IsAppEnvProd)
            {
                SetValueExact(colName, val + "");
            }
            else
            {
                throw new Exception(colName+ ", unknown type: " + t.UnderlyingType.FullName);
            }
        }
    }
}