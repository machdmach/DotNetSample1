using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Libx;

[DataContract]
public enum DbEnvEnum
{
    [EnumMember]
    Unknown = 0,
    [EnumMember]
    Dev,
    [EnumMember]
    Test,
    [EnumMember]
    Prod,
    [EnumMember]
    FixCode
}
public enum DbmsType
{
    Unknown,
    SqlServer,
    Oracle,
    MySql,
    SQLite,
}

public abstract class MyDbConnection : DbConnectionString, IDisposable
{
    public Type ProviderType { get; protected set; }
    public string ProviderName => ProviderType.FullName;
    public string ProviderParamMarker { get; set; } //ParameterPrefix
    public string StdParamMarker { get; set; } = "@";
    public MLogger logger;
    public MLogger log => logger;
    //public bool IsLogging = true;

    //===================================================================================================
    public void Init(string connStr, MLogger logger)
    {
        this.ConnStrKey = GetFinalConnStrKeyName(connStr);
        this.logger = logger;

        IsOutdatedDb = ConnStrKey.IndexOf("11") > 0;  //prod11

        if (IsSQLServer)
        {
            ProviderParamMarker = "@";
        }
        else if (IsOracle)
        {
            ProviderParamMarker = ":";
        }
        else if (IsSQLite)
        {
            ProviderParamMarker = "@";
        }
        else
        {
            throw new Exception("unknown db type");
        }

    }
    //===================================================================================================
    public string CommandGetDateTime
    {
        get
        {
            string ret = IsOracle ? "select sysdate from dual" :
                         IsSQLServer ? "getDate()" :
                         throw new NotImplementedException("unknown db");
            return ret;
        }
    }

    //===================================================================================================
    public virtual void GetDatabaseInfo(DataSvcInfo info)
    {
        info ??= new DataSvcInfo();
        info.DbServerDateTime = GetServerDateTime();
        info.DbEnv = DbEnv.ToString();
    }

    //===================================================================================================
    public DateTime GetServerDateTime()
    {
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = CommandGetDateTime;
            var ret = DbCommandX.ExecuteScalar(cmd);
            logger.LogInfo("SQL Server datetime is: " + ret);
            return (DateTime)ret;
        }
    }

    //===================================================================================================
    public virtual DbConnection CreateDbConnection(string connStr)
    {
        DbConnection ret = (DbConnection)Activator.CreateInstance(ProviderType, connStr);
        return ret;
    }
    protected DbConnection dbc => GetDbConnection();
    protected DbConnection _dbc = null;
    public DbConnection GetDbConnection()
    {
        if (_dbc == null)
        {
            string constr = GetConnectionString();
            _dbc = CreateDbConnection(constr);
            try
            {
                _dbc.Open();
            }
            catch (DbException ex)
            {
                throw new Exception(GetConnectionInfo() + ", " + ex.Message, ex);
            }
            logger.LogInfo("++DB opened: " + GetConnectionInfo());
            if (OnConnectionOpened != null)
            {
                OnConnectionOpened(_dbc);
            }
        }
        return _dbc;
    }
    public Action<DbConnection> OnConnectionOpened = null;

    //===================================================================================================
    #region IDisposable Support
    private bool disposed = false; // To detect redundant calls
    protected virtual void Dispose(bool disposing)
    {
        if (Transaction != null)
        {
            this.RollbackTransaction();
        }

        if (!disposed)
        {
            if (disposing)
            {
                //dispose managed state (managed objects).
                if (_dbc != null)
                {
                    try
                    {
                        _dbc.Dispose();
                        logger.LogInfo("--Database connection disposed");
                    }
                    catch (Exception ex)
                    {
                        MOutput.WriteErrorHtmlEncoded(ex);
                        //do nothing
                    }
                }
            }
            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            disposed = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~DbConnection() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public virtual void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    public void Close()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion

    public void Transaction_eg()
    {
        //var dbc = conn.GetConnection();
        if (true)
        {
            using (var tx = dbc.BeginTransaction())
            {
                //DbCommandX.ExecuteNonQueryUptoNRows(cmd, 2);
                tx.Commit();
            }
        }
        //or
        if (true)
        {
            var cmd = dbc.CreateCommand();
            var tx = dbc.BeginTransaction(); //#transaction
            cmd.Transaction = tx;
            try
            {
                //DbCommandX.ExecuteNonQueryUptoNRows(cmd, 2);
                tx.Commit();
            }
            catch (Exception ex)
            {
                Rollback(tx, ex);
            }
        }

    }
    public void TransactionScope_eg()
    {
        using (var scope = new TransactionScope())
        {
            //using (var conn = new SqlConnection(/*...*/))
            //{
            //    //As many nested commands, etc, using the above connection.
            //    //but don't need to create a SqlTransaction object nor
            //    //in any way reference the scope variable
            //    //scope.
            //}
            scope.Complete();
        }
    }
    //===================================================================================================
    public DbTransaction Transaction { get; private set; }
    public DbTransaction StartTransaction()
    {
        Transaction = dbc.BeginTransaction();
        return Transaction;
    }
    public void CommitTransaction()
    {
        if (Transaction == null)
        {
            throw new Exception("No transaction to commit");
        }
        //if (Transaction != null)
        {
            Transaction.Commit();
            Transaction = null;
        }
        logger.LogInfo("Transaction committed");
    }
    public void RollbackTransaction(Exception ex = null)
    {
        if (Transaction != null)
        {
            Transaction.Rollback();
            logger.LogInfo(ex, "Transaction Rollbacked");
            Transaction = null;
        }
        //try
        //{
        //    Transaction.Rollback();
        //}
        //catch (Exception ex2)
        //{
        //    // This catch block will handle any errors that may have occurred
        //    // on the server that would cause the rollback to fail, such as a closed connection.
        //    logger.LogInformation("Rollback Exception Type: " + ex2.GetType());
        //    logger.LogInformation("  Message: {0}", ex2.Message);
        //}
        //finally
        //{
        //    //System.InvalidOperationException: This SqlTransaction has completed; it is no longer usable.
        //    throw ex;
        //}
    }
    public void Rollback(DbTransaction tx, Exception ex)
    {
        logger.LogInfo(ex, "Rollback because exception");
        try
        {
            tx.Rollback();
        }
        catch (Exception ex2)
        {
            // This catch block will handle any errors that may have occurred
            // on the server that would cause the rollback to fail, such as a closed connection.
            logger.LogInfo("Rollback Exception Type: " + ex2.GetType());
            logger.LogInfo("  Message: {0}", ex2.Message);
        }
        finally
        {
            //System.InvalidOperationException: This SqlTransaction has completed; it is no longer usable.
            throw ex;
        }
    }

    //===================================================================================================
    public static QueryOptions CheckOptions(QueryOptions? options = null)
    {
        var ret = options ?? new QueryOptions();
        return ret;
    }
    //===================================================================================================
    public static QueryOptions CheckOptions(CrudOptions? options)
    {
        var ret = options ?? new CrudOptions();
        return ret;
    }
    //===================================================================================================
    public static InsertOptions CheckOptions(InsertOptions? options)
    {
        var ret = options ?? new InsertOptions();
        return ret;
    }
    //===================================================================================================
    public DbCommand CreateCommand(QueryOptions? options = null)
    {
        options = CheckOptions(options);
        var cmd = dbc.CreateCommand();
        if (Transaction != null)
        {
            //ExecuteReader requires the command to have a transaction when the connection assigned to the command is in a pending local transaction
            cmd.Transaction = Transaction;
        }
        if (options.Transaction != null)
        {
            cmd.Transaction = options.Transaction;
        }

        if (options.CommandTimeout.HasValue)
        {
            cmd.CommandTimeout = options.CommandTimeout.Value;
        }
        if (options.CommandType.HasValue)
        {
            cmd.CommandType = options.CommandType.Value;
        }
        return cmd;
    }

    //===================================================================================================
    public void SetParameters(IDbCommand cmd, object cmdParams)
    {
        var paramNames = GetParameterNames(cmd);
        var mapper = new PropColMapper(cmdParams.GetType(), logger);
        var props = mapper.KeyedByPropNamePropCols;
        //int markerLen = StdParamMarker.Length;

        foreach (var paramName in paramNames)
        {
            var prop = props.GetValueOrDefault(paramName); //.Substring(markerLen));
            if (prop == null)
            {
                logger.LogInfo(HtmlValue.OfAny(props.Keys, new() { Caption = "DbCommand Parameter Keys" }));
                throw new Exception("No param value found for: " + paramName); // + ", available params are: "+ props.Keys);
            }
            var p = cmd.CreateParameter();
            p.ParameterName = ProviderParamMarker + paramName;
            p.Value = prop.PropInfo.GetValue(cmdParams);
            //p.DbType = DbType.Int32;
            cmd.Parameters.Add(p);
        }
    }

    //===================================================================================================
    public List<string> GetParameterNames(IDbCommand cmd)
    {
        string sql = cmd.CommandText;
        var newSql = new StringBuilder();

        string paramMarker = StdParamMarker;

        List<string> ret = new List<string>();
        string regexPattern = @$"[^{paramMarker}]{paramMarker}([a-zA-Z_][\w]*)";
        Regex regex = new Regex(regexPattern, RegexOptions.Compiled);
        // statement = Regex.Replace(statement, @":(?=(?:'[^']*'|[^'\n])*$)", "@");
        //string output = Regex.Replace(input, @"(?<=\W):(?=\w+)", "@");

        foreach (Match match in regex.Matches(sql))
        {
            string val = match.Groups[1].Value;
            ret.Add(val);
        }
        cmd.CommandText = sql.Replace(StdParamMarker, ProviderParamMarker);
        return ret;
    }
}
