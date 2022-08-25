namespace Libx;
public class LookupRegistry
{
    //MyDbConnection db;
    Type dbContextType;
    public LookupRegistry(Type dbContextType)
    {
        this.dbContextType = dbContextType;
    }

    public static Dictionary<string, NamedSqlEntry> LookupQueries = new(StringComparer.OrdinalIgnoreCase);

    public NamedSqlEntry AddLookupQuery(string queryName, string sql)
    {
        var ent = new NamedSqlEntry(queryName, sql);
        LookupQueries.Add(queryName, ent);
        return ent;
    }
    public NamedSqlEntry AddChildLookupQuery(string queryName, string sql)
    {
        var ent = new NamedSqlEntry(queryName, sql) { IsChildLookup = true };
        LookupQueries.Add(queryName, ent);
        return ent;
    }

    public static string GetLookupQuery(string queryName, MyDbConnection db)
    {
        var entry = LookupQueries[queryName];
        if (entry == null)
        {
            throw new Exception("sql not found for queryName: " + queryName);
        }
        return entry.Sql;
    }
    public static string BuildLookupLinks(Type controllerType, Type dbContextType)
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
        foreach (var ent in LookupQueries.Values)
        {
            string url = $"{applicationPath}/api/{controllerName}/ReadActiveLookups/{ent.Name}";
            //url = MvcLib.ResolveToAbsolutePath(url);

            if (ent.IsChildLookup)
            {
                url = url.Replace("ReadActiveLookups", "ReadActiveChildLookups");
            }
            if (ent.TestUrlSuffix != null)
            {
                url += ent.TestUrlSuffix;
            }

            string link = $"<a href='{url}'> Lookups - {ent.Name}</a>";
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
