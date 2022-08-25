namespace Libx;
public class CrudRegistry
{
    string appName;
    public CrudRegistry(string appName)
    {
        this.appName = appName;
    }

    //===================================================================================================
    public static Dictionary<string, CrudEntityEntry> crudEntities = new(StringComparer.OrdinalIgnoreCase);
    public void AddCrudEntity<T>(CrudEntityEntry ent)
    {
        ent.EntityType = typeof(T); 
        //var entity = new CrudEntityEntry() { EntityType = entType, ExcludeFields = Excludes };
        //crudEntities.Add(entName, entity);
        if (ent.Name == null)
        {
            ent.Name = ent.EntityType.Name;
        }
        crudEntities.Add(ent.Name, ent);
    }
    public static CrudEntityEntry GetCrudEntity(string entName)
    {
        var ent = crudEntities[entName];
        if (ent == null)
        {
            throw new Exception("Crud not found for entName: " + entName);
        }
        return ent;
    }

    //===================================================================================================
    public static string BuildLinks(Type controllerType, Type dbContextType)
    {
        string controllerName = controllerType.Name.Replace("Controller", "");
        string applicationPath = ConfigX.WebAppVirtualPath;
        var buf = new StringBuilder();
        int k = 0;
        foreach (var ent in crudEntities.Values)
        {
            //https://localhost:44321/api/LafAdminDataApi/ReadRow/LafLostEntity/-1
            string url = $"{applicationPath}/api/{controllerName}/ReadEntity/{ent.Name}/-1";
            string readRowUrl = $"{applicationPath}/api/{controllerName}/ReadRow/{ent.Name}/-1";

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
public class CrudEntityEntry: NamedSqlEntry
{
    //public Type EntityType { get; set; }
    public string ExcludeOps { get; set; }
}

