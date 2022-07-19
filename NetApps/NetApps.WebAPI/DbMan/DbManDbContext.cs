namespace Libx.Mvc.App;
public class DbManDbContext : AutoDbContext
{
    static DbManDbContext()
    {
    }
    public static Type Type => typeof(DbManDbContext);
    public DbManDbContext(string connStr, MLogger logger) : base(connStr, logger)
    {
    }
}
