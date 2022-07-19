using Microsoft.Data.Sqlite;
namespace Libx;

public class SQLiteDbContext : MyDbContext
{
    public SQLiteDbContext(string connStr, MLogger logger)
    {
        DbmsType = DbmsType.SQLite;
        ProviderType = typeof(SqliteConnection);
        DbConnStrBuilderType = typeof(SqliteConnectionStringBuilder);

        Init(connStr, logger);
    }
}
