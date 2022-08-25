namespace Libx;
public sealed class DbTypeConvertor
{
    static DbTypeConvertor()
    {
        //http://msdn.microsoft.com/en-us/library/cc716729(v=vs.110).aspx
        //ordering by SqlDbType           

        AddMapping(typeof(Int64), DbType.Int64, SqlDbType.BigInt);
        AddMapping(typeof(byte[]), DbType.Binary, SqlDbType.VarBinary); //binary ssdb, FILESTREAM attribute (varbinary(max))
        AddMapping(typeof(bool), DbType.Boolean, SqlDbType.Bit);
        AddMapping(typeof(string), DbType.AnsiStringFixedLength, SqlDbType.Char);
        AddMapping(typeof(DateTime), DbType.Date, SqlDbType.Date);
        AddMapping(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime);
        AddMapping(typeof(DateTime), DbType.DateTime2, SqlDbType.DateTime2);
        AddMapping(typeof(DateTime), DbType.DateTime, SqlDbType.SmallDateTime);

        AddMapping(typeof(DateTime), DbType.DateTimeOffset, SqlDbType.DateTimeOffset);
        AddMapping(typeof(Decimal), DbType.Decimal, SqlDbType.Decimal);
        AddMapping(typeof(double), DbType.Double, SqlDbType.Float);
        AddMapping(typeof(byte[]), DbType.Binary, SqlDbType.Image);
        AddMapping(typeof(byte[]), DbType.Binary, SqlDbType.Binary);  //image
        AddMapping(typeof(Int32), DbType.Int32, SqlDbType.Int);

        AddMapping(typeof(Decimal), DbType.Decimal, SqlDbType.Money);
        AddMapping(typeof(string), DbType.StringFixedLength, SqlDbType.NChar);
        AddMapping(typeof(string), DbType.String, SqlDbType.NText);
        AddMapping(typeof(string), DbType.String, SqlDbType.NVarChar);

        AddMapping(typeof(Int16), DbType.Int16, SqlDbType.SmallInt);
        AddMapping(typeof(byte), DbType.Byte, SqlDbType.TinyInt);
        AddMapping(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier);
        AddMapping(typeof(object), DbType.Object, SqlDbType.Variant);
        AddMapping(typeof(string), DbType.String, SqlDbType.VarChar);
        AddMapping(typeof(String), DbType.Xml, SqlDbType.Xml);
        AddMapping(typeof(Object), DbType.Object, SqlDbType.Structured);

        EntryArray = entryList.ToArray();
    }
    //public static string GetDefaultStringValue(SqlDbType t)
    //{
    //    object obj = GetDefaultValue(t);
    //    if (typef
    //}

    public static String GetDefaultStringValue(SqlDbType t, bool isQuoted = true)
    {
        object oVal = "";
        string sVal = "";
        string sValQuoted = "'x'";
        switch (t)
        {
            case SqlDbType.BigInt:
            case SqlDbType.Int:
            case SqlDbType.SmallInt:
            case SqlDbType.TinyInt:
                oVal = 22;
                sVal = "22";
                sValQuoted = sVal;
                break;

            case SqlDbType.Bit:
                oVal = true;
                sVal = "1";
                sValQuoted = sVal;
                break;

            case SqlDbType.Date:
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
            case SqlDbType.DateTimeOffset:
                oVal = DateTime.Now;
                sVal = DateTime.Now.ToString("MM/dd/yyyy");
                sValQuoted = "'" + sVal + "'";
                break;

            case SqlDbType.Money:
            case SqlDbType.Float:
                oVal = 1.1;
                sVal = "1.1";
                sValQuoted = sVal;
                break;

            case SqlDbType.Structured:
                oVal = new object();
                sVal = "Structured";
                sValQuoted = "'" + sVal + "'";
                break;

        }
        if (isQuoted)
        {
            return sValQuoted;
        }
        else
        {
            return sVal;
        }
    }

    public static Type ToNetType(string s)
    {
        s = s.ToUpper();
        Type t = typeof(string);
        if (s.Contains("CHAR"))
        {
            t = typeof(String);
        }
        else if (s.Contains("DATE"))
        {
            t = typeof(DateTime);
        }
        else if (s.Contains("INT"))
        {
            t = typeof(Int32);
        }
        else if (s.Contains("NUM"))
        {
            t = typeof(Int32);
        }
        else
        {
            t = typeof(String);
        }
        return t;
    }

    public static string CorrectCasing(string s, bool isOracle)
    {
        if (isOracle)
        {
            if (s.TryReplaceStringCI("varchar", "Varchar", out string res)) return res;
            if (s.TryReplaceStringCI("char", "Char", out res)) return res;
            if (s.TryReplaceStringCI("dateTime", "DateTime", out res)) return res;
            if (s.TryReplaceStringCI("date", "Date", out res)) return res;
            if (s.TryReplaceStringCI("NUMBER", "Int32", out res)) return res;
            if (s.TryReplaceStringCI("int", "Int32", out res)) return res;
        }
        else
        {

            s = s.ReplaceStringCI("varchar", "VarChar");
            s = s.ReplaceStringCI("char", "Char");
            s = s.ReplaceStringCI("int", "Int");
            s = s.ReplaceStringCI("datetime", "DateTime");
            s = s.ReplaceStringCI("Number", "Int32");
            s = s.ReplaceStringCI("z", "");
            s = s.ReplaceStringCI("z", "");
            s = s.ReplaceStringCI("z", "");
            s = s.ReplaceStringCI("z", "");

        }
        return s;
    }

    //private static ArrayList _DbTypeList = new ArrayList();
    private static readonly List<DbTypeMapEntry> entryList = new List<DbTypeMapEntry>();
    private static readonly DbTypeMapEntry[] EntryArray;
    private static void AddMapping(Type type, DbType dbType, SqlDbType sqlDbType)
    {
        entryList.Add(new DbTypeMapEntry(type, dbType, sqlDbType));
    }

    //==============================================================================
    public static Type ToNetType(DbType dbType)
    {
        DbTypeMapEntry entry = Find(dbType);
        return entry.Type;
    }

    //==============================================================================
    public static Type ToNetType(SqlDbType sqlDbType)
    {
        DbTypeMapEntry entry = Find(sqlDbType);
        return entry.Type;
    }

    //==============================================================================
    public static DbType ToDbType(Type type)
    {
        DbTypeMapEntry entry = Find(type);
        return entry.DbType;
    }

    //==============================================================================
    public static DbType ToDbType(SqlDbType sqlDbType)
    {
        DbTypeMapEntry entry = Find(sqlDbType);
        return entry.DbType;
    }

    //==============================================================================
    public static SqlDbType ToSqlDbType(Type type)
    {
        DbTypeMapEntry entry = Find(type);
        return entry.SqlDbType;
    }

    //==============================================================================
    public static SqlDbType ToSqlDbType(DbType dbType)
    {
        DbTypeMapEntry entry = Find(dbType);
        return entry.SqlDbType;
    }
    //===================================================================================================
    public static DbTypeMapEntry Find(Type type)
    {
        for (int i = 0; i < EntryArray.Length; i++)
        {
            var entry = EntryArray[i];
            if (entry.Type == type)
            {
                return entry;
            }
        }
        throw new ApplicationException("Unknown .Net Type: " + type);
    }
    //===================================================================================================
    public static DbTypeMapEntry Find(DbType type)
    {
        for (int i = 0; i < EntryArray.Length; i++)
        {
            var entry = EntryArray[i];
            if (entry.DbType == type)
            {
                return entry;
            }
        }
        throw new ApplicationException("Unknown DbType: " + type);
    }
    //===================================================================================================
    public static DbTypeMapEntry Find(SqlDbType type)
    {
        for (int i = 0; i < EntryArray.Length; i++)
        {
            var entry = EntryArray[i];
            if (entry.SqlDbType == type)
            {
                return entry;
            }
        }
        throw new ApplicationException("Unknown SqlDbType: " + type);
    }
    //===================================================================================================
    public static SqlDbType ToSqlDbType(string typeStr)
    {
        if (typeStr.Equals("table type", StringComparison.InvariantCultureIgnoreCase))
        {
            return SqlDbType.Structured;
        }
        else
        {
            SqlDbType rval = (SqlDbType)Enum.Parse(typeof(SqlDbType), typeStr, true);
            return rval;
        }
    }
}
//===================================================================================================
public struct DbTypeMapEntry
{
    public Type Type;
    public DbType DbType;
    public SqlDbType SqlDbType;
    public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
    {
        Type = type;
        DbType = dbType;
        SqlDbType = sqlDbType;
    }
};


public static class DbTypeX
{
    //private static readonly DbType dbType1;
    //private static readonly SqlDbType sqlDbType1;
    //private static readonly DbTypeConvertor cv1;

    private static void CopyMe1()
    {
        var type = TypeMap[typeof(string)]; // returns DbType.String

    }

    public static DbType ToDbType(Type netFrameworkType)
    {
        DbType dbType = DbTypeX.TypeMap[netFrameworkType];
        return dbType;
    }

    public static Dictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>();
    static DbTypeX()
    {
        TypeMap[typeof(byte)] = DbType.Byte;
        TypeMap[typeof(sbyte)] = DbType.SByte;
        TypeMap[typeof(short)] = DbType.Int16;
        TypeMap[typeof(ushort)] = DbType.UInt16;
        TypeMap[typeof(int)] = DbType.Int32;
        TypeMap[typeof(uint)] = DbType.UInt32;
        TypeMap[typeof(long)] = DbType.Int64;
        TypeMap[typeof(ulong)] = DbType.UInt64;
        TypeMap[typeof(float)] = DbType.Single;
        TypeMap[typeof(double)] = DbType.Double;
        TypeMap[typeof(decimal)] = DbType.Decimal;
        TypeMap[typeof(bool)] = DbType.Boolean;
        TypeMap[typeof(string)] = DbType.String;
        TypeMap[typeof(char)] = DbType.StringFixedLength;
        TypeMap[typeof(Guid)] = DbType.Guid;
        TypeMap[typeof(DateTime)] = DbType.DateTime;
        TypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
        TypeMap[typeof(byte[])] = DbType.Binary;

        TypeMap[typeof(byte?)] = DbType.Byte;
        TypeMap[typeof(sbyte?)] = DbType.SByte;
        TypeMap[typeof(short?)] = DbType.Int16;
        TypeMap[typeof(ushort?)] = DbType.UInt16;
        TypeMap[typeof(int?)] = DbType.Int32;
        TypeMap[typeof(uint?)] = DbType.UInt32;
        TypeMap[typeof(long?)] = DbType.Int64;
        TypeMap[typeof(ulong?)] = DbType.UInt64;
        TypeMap[typeof(float?)] = DbType.Single;
        TypeMap[typeof(double?)] = DbType.Double;
        TypeMap[typeof(decimal?)] = DbType.Decimal;
        TypeMap[typeof(bool?)] = DbType.Boolean;
        TypeMap[typeof(char?)] = DbType.StringFixedLength;
        TypeMap[typeof(Guid?)] = DbType.Guid;
        TypeMap[typeof(DateTime?)] = DbType.DateTime;
        TypeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
        //TypeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
    }
}
