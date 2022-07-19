using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;

namespace Libx;
public class AutoDbContext: MyDbContext
{
    public AutoDbContext(string connStr, MLogger logger)
    {
        DbmsType = GetDbmsType(GetConnectionString(connStr));
        if (DbmsType == DbmsType.Oracle)
        {
            ProviderType = typeof(OracleConnection);
            DbConnStrBuilderType = typeof(OracleConnectionStringBuilder);
        }
        else if(DbmsType == DbmsType.SqlServer)
        {
            ProviderType = typeof(SqlConnection);
            DbConnStrBuilderType = typeof(SqlConnectionStringBuilder);
        }
        Init(connStr, logger);
    }

    //===================================================================================================
    public static DbmsType GetDbmsType(String connStr)
    {
        DbmsType ret;
        if (connStr.IndexOf("/") > 0)
        {
            ret = DbmsType.Oracle;
        }
        else
        {
            ret = DbmsType.SqlServer;
        }
        return ret;
    }
}
