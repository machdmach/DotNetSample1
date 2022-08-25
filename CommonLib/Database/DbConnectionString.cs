namespace Libx;
public class DbConnectionString
{
    public static Dictionary<string, string> ConnectionStrings;
    public string ConnStrKey = "UnInitx";
    public bool IsOutdatedDb { get; set; }

    public Type DbConnStrBuilderType { get; protected set; }
    public bool IsOracle => DbmsType == DbmsType.Oracle;
    public bool IsSQLServer => DbmsType == DbmsType.SqlServer;
    public bool IsSQLite => DbmsType == DbmsType.SQLite;
    public DbmsType DbmsType { get; protected set; }

    //===================================================================================================
    public static bool IsConnnectionString(string s)
    {
        return s.IndexOf('=') > 0 && s.IndexOf(';') > 0;
    }
    public static String GetFinalConnStrKeyName(string key1)
    {
        if (IsConnnectionString(key1)) return key1; //------------------------

        string val1 = ConnectionStrings[key1]; 
        if (IsConnnectionString(val1)) return key1; //------------------------

        string key2 = val1;
        string val2 = ConnectionStrings[key2];
        if (IsConnnectionString(val2)) return key2; //------------------------

        //if (IsConnnectionString(val2)) throw new Exception("too many aliases");

        return val2; //------------------------
    }

    //===================================================================================================
    public String GetConnectionString()
    {
        return GetConnectionString(ConnStrKey);
    }
    public static String GetConnectionString(string connStr)
    {
        connStr = GetFinalConnStrKeyName(connStr);
        string ret;
        if (IsConnnectionString(connStr))
        {
            ret = connStr;
        }
        else
        {
            ret = ConnectionStrings[connStr];
        }
        return ret;
    }

    //===================================================================================================
    public virtual String GetConnectionInfo()
    {
        var csb = (DbConnectionStringBuilder)Activator.CreateInstance(DbConnStrBuilderType, GetConnectionString());
        //logger.LogInformation(HtmlValue.OfAny(csb));
        //string rval = string.Format("connStrName:{0}, ds:{1}, db:{2}", conStrName, scsb.DataSource, scsb.InitialCatalog);
        string rval = ConnStrKey;
        if (ConfigX.IsDevMachine)
        {
            //rval += " = " + string.Format("{0}@{1} -", scsb.UserID, scsb.DataSource);
            rval += " = " + string.Format("{0}", csb.ConnectionString);
            //rval += " = " + string.Format("{0}@{1} -", csb.ConnectionString);
        }
        rval += ", dbEnv=" + DbEnv.ToString();
        return rval;
    }

    //===================================================================================================
    private DbEnvEnum _dbEnv = DbEnvEnum.Unknown;
    public DbEnvEnum DbEnv
    {
        get
        {
            if (_dbEnv == DbEnvEnum.Unknown) _dbEnv = GetDbEnv(ConnStrKey);
            return _dbEnv;
        }
    }
    public bool IsDbEnvProd => DbEnv == DbEnvEnum.Prod;
    public static DbEnvEnum GetDbEnv(string connStrKey)
    {
        if (string.IsNullOrEmpty(connStrKey))
        {
            throw new Exception("connStrKey is null");
        }
        string connInfo = connStrKey.ToLower();
        DbEnvEnum rval;

        if (connInfo.IndexOf(".prod") > 0)
        {
            rval = DbEnvEnum.Prod;
        }
        else if (connInfo.IndexOf(".qa") > 0 || connInfo.IndexOf(".test") > 0)
        {
            rval = DbEnvEnum.Test;
        }
        else if (connInfo.IndexOf(".dev") > 0)
        {
            rval = DbEnvEnum.Dev;
        }
        else
        {
            rval = DbEnvEnum.Unknown;
            //rval = DbEnvEnum.FixCode;
            //throw new Exception("Unknown DbEnv: " + connInfo);
        }
        return rval;
    }
}
