namespace Libx;
public class QueryRegistry
{
    string appName;
    public QueryRegistry(string appName)
    {
        this.appName = appName;
    }

    //===================================================================================================
    public static Dictionary<string, NamedSqlEntry> NamedSqlList = new(StringComparer.OrdinalIgnoreCase);
    public NamedSqlEntry AddNamedSql(NamedSqlEntry ent)
    {
        NamedSqlList.Add(ent.Name, ent);
        return ent;
    }
    public NamedSqlEntry AddNamedSql<T>(NamedSqlEntry ent)
    {
        ent.EntityType = typeof(T);
        NamedSqlList.Add(ent.Name, ent);
        return ent;
    }
    public static NamedSqlEntry GetNamedSql(string queryName, MyDbConnection db)
    {
        var entry = NamedSqlList[queryName];
        if (entry == null)
        {
            throw new Exception("sql not found for queryName: " + queryName);
        }
        return entry;
    }
    public static string BuildLinks(Type controllerType, Type dbContextType)
    {
        //Type t1 = typeof(DbContextType);
        //var o1 = Activator.CreateInstance(t1);

        string controllerName = controllerType.Name.Replace("Controller", "");
        //var o1 = Activator.CreateInstance(controllerType) as MvcController_DbActions;

        string applicationPath = ConfigX.WebAppVirtualPath;

        var buf = new StringBuilder();
        //string url = MvcLib.ResolveToAbsolutePath(e.Url);
        //buf.AppendFormat("<li> <a href='{0}'> {1} </a></li>\n", url, e.Label);

        int k = 0;
        foreach (var ent in NamedSqlList.Values)
        {
            string url = $"{applicationPath}/api/{controllerName}/RunNamedQuery?q={ent.Name}";
            //url = MvcLib.ResolveToAbsolutePath(url);
            if (ent.EntityType == null)
            {
                url = url.Replace("RunNamedQuery", "RunNamedQueryRaw");
            }

            string link = $"<a href='{url}'> {ent.Name}</a>";
            buf.AppendLine(link);
            buf.AppendLine("<br>");
            k++;
        }
        if (k == 0)
        {
            buf.AppendLine("NamedSqlList is empty");
        }
        return buf.ToString();
    }
}
//===================================================================================================
//===================================================================================================
public class NamedSqlEntry
{
    public string Name { get; set; }
    public string Sql { get; set; }
    public int MaxRows { get; set; }
    public int Order { get; set; }
    public Type DbContexClass { get; set; }
    public Type EntityType { get; set; }
    private static int EntryCount = 0;
    public bool IsChildLookup { get; set; }
    public string TestUrlSuffix { get; set; }
    
    public NamedSqlEntry() { }
    public NamedSqlEntry(string name, string sql, Type dbContexClass = null, int maxRows = 2000)
    {
        Name = name;
        Sql = sql;
        MaxRows = maxRows;
        Order = EntryCount++;
        DbContexClass = dbContexClass;
    }
}

