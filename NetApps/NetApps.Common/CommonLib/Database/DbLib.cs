using System.Configuration;
using System.Data;
using System.Data.Common;

namespace System;

public class DbLib
{
    //=================================================================================================
    public static DataTable CreateDataAdapter(string sql)
    {
        if (NotRunThis)
        {
            DataTable allInstalledProviders = DbProviderFactories.GetFactoryClasses();
            //MOutput.WriteDataTable("All Installed Db Providers", allInstalledProviders);
        }

        var table = new DataTable();
        ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings["conn1"];
        DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(connSettings.ProviderName);

        using (DbConnection connection = dbProviderFactory.CreateConnection())
        {
            if (connection == null)
            {
                throw new InvalidOperationException(connSettings.ProviderName);
            }

            connection.ConnectionString = connSettings.ConnectionString;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;


                using (var adapter = dbProviderFactory.CreateDataAdapter())
                {
                    if (adapter == null)
                    {
                        throw new InvalidOperationException("Cannot create Data Adapter for " + connSettings.ProviderName);
                    }

                    adapter.SelectCommand = command;

                    adapter.Fill(table);
                }
            }
        }
        return table;
    }
    //=================================================================================================
    public static int GetOneCell_Int32(DbConnection conn, string sql, int defaultVal = 0)
    {
        string s = GetOneCell(conn, sql) + "";
        if (s == null)
        {
            return defaultVal;
        }
        else
        {
            int rval = Int32.Parse(s);
            MOutput.WriteLine("sql result=" + rval);
            return rval;
        }
    }

    //=================================================================================================
    public static object GetOneCell(DbConnection conn, string sql)
    {
        object rval = null;
        try
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                //AppLog.LogSql(cmd);

                //ExecuteReader requires the command to have a transaction when the connection assigned to the command is in a pending local transaction
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        rval = rdr[0];
                    }
                }
            }
        }
        catch (DbException ex)
        {
            throw new Exception("sql=" + sql, ex);
        }
        return rval;
    }
    //=================================================================================================
    public static List<int> QueryOneColumn_Int32(DbConnection conn, string sql)
    {
        var rval = new List<Int32>();
        try
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                //AppLog.LogSql(cmd);

                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    int val = (Int32)rdr[0];
                    rval.Add(val);
                }
                rdr.Close();
            }
        }
        catch (DbException ex)
        {
            throw new Exception("sql=" + sql, ex);
        }
        return rval;
    }
    //=================================================================================================
    public static List<String> QueryOneColumn_String(DbConnection conn, string sql)
    {
        var rval = new List<String>();
        try
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                //AppLog.LogSql(cmd);

                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var val = (String)rdr[0];
                    rval.Add(val);
                }
                rdr.Close();
            }
        }
        catch (DbException ex)
        {
            throw new Exception("sql=" + sql, ex);
        }
        return rval;
    }
}
