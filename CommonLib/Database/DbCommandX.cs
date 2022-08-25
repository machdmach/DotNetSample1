namespace Libx;
///<see cref="DbCommand"/>
public class DbCommandX
{
    //========================================================================
    public static String GetParameterValue(DbParameter sp)
    {
        String retval;
        switch (sp.DbType)
        {
            case DbType.StringFixedLength: //Char:
            case DbType.AnsiStringFixedLength:
            case DbType.AnsiString: //.NChar:
            case DbType.String: //.NText:
                                //case DbType.NVarChar:
                                //case DbType.Text:
            case DbType.Time:
            //case DbType.VarChar:
            case DbType.Xml:
            case DbType.Date:
            case DbType.DateTime:
            case DbType.DateTime2:
            case DbType.DateTimeOffset:
                retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                break;
            //case DbType.Bit:
            case DbType.Boolean:
                //retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                retval = SqlServerBitToString(sp.Value);
                break;

            default:
                retval = sp.Value.ToString().Replace("'", "''");
                break;
        }

        return retval;
    }
    //========================================================================
    public static string SqlServerBitToString(Object o)
    {
        string rval;
        if (o == null) { rval = "0"; }
        else
        {
            Boolean b = (Boolean)o;
            if (b == true) rval = "1";
            else rval = "0";
        }
        return rval;
    }
    //========================================================================
    public static String ToStringLikeLINQ(DbCommand c)
    {
        StringBuilder buf = new StringBuilder();
        buf.AppendLine(c.CommandText);
        foreach (IDbDataParameter p in c.Parameters)
        {
            buf.AppendLine(ToStringLikeLINQ(p));
        }
        return buf.ToString();
    }
    //========================================================================
    public static String ToStringLikeLINQ(IDbDataParameter p)
    {
        //SqlParameter p;

        //-- @p12: Input VarChar (Size = 8000; Prec = 0; Scale = 0) [Null]
        string rval = string.Format("-- {0}: {1} {2} (Size = {3}; Prec = {4}; Scale = {5})[{6}]",
            p.ParameterName,
            p.Direction.ToString(),         //Input, Output, InputOutput, ReturnValue
            p.DbType.ToString(),
            p.Size,
            p.Precision,
            p.Scale,
            p.Value);
        return rval;
    }
    //===================================================================================================
    public static void ExeUpdateOneRow(string stmt, DbConnection con)
    {
        using (DbCommand cmd = con.CreateCommand())
        {
            cmd.CommandText = stmt;

            //log.Warn(cmd.CommandText);
            int k = cmd.ExecuteNonQuery();
            //log.Debug("SQL Executed");

            if (k != 1)
            {
                throw new Exception("Failed to update 1 row updated k rows:" + k + ", " + stmt);
            }
        }
    }
    public static void ExeInsertOneRow(string stmt, DbConnection con)
    {
        DbCommand cmd = con.CreateCommand();
        cmd.CommandText = stmt;
        //log.Warn(cmd.CommandText);

        int k = cmd.ExecuteNonQuery();
        if (k != 1)
        {
            throw new Exception("Failed to insert 1 row updated k rows:" + k + ", " + stmt);
        }
    }
    /*
			public override object ExecuteInsert(Database db, IDbCommand cmd, string PrimaryKeyName)
			{
				if (PrimaryKeyName != null)
				{
					cmd.CommandText += string.Format(" returning {0} into :newid", PrimaryKeyName);
					var param = cmd.CreateParameter();
					param.ParameterName = ":newid";
					param.Value = DBNull.Value;
					param.Direction = ParameterDirection.ReturnValue;
					param.DbType = DbType.Int64;
					cmd.Parameters.Add(param);
					db.ExecuteNonQueryHelper(cmd);
					return param.Value;
				}
				else
				{
					db.ExecuteNonQueryHelper(cmd);
					return -1;
				}
			}
     */
    public static void ExecuteNonQueryOneRow(DbCommand cmd, ResponseData resData = null, string label = null)
    {
        string cmdStr = DbCommandX.ToStr(cmd);
        MOutput.WriteHtmlEncodedPre("DbCommandX.ExecuteNonQueryOneRow: " + cmdStr);
        if (resData != null)
        {
            resData.addSql(cmdStr, label);
        }

        //cmd.ExecuteScalar();

        //try { }
        int k = cmd.ExecuteNonQuery();
        if (k != 1)
        {
            throw new Exception(string.Format("Failed to insert/update/delete 1 row, affected {0} rows, CmdStr={1}", k, cmdStr));
        }
    }
    public static int ExecuteNonQuery(DbCommand cmd, int expectedRowsCountAffected = -1)
    {
        string cmdStr = DbCommandX.ToStr(cmd);
        MOutput.WriteHtmlEncodedPre("DbCommandX.ExecuteNonQuery: " + cmdStr);

        //cmd.ExecuteScalar();

        //try { }
        int k = cmd.ExecuteNonQuery();
        if (expectedRowsCountAffected != -1 && k != expectedRowsCountAffected)
        {
            throw new Exception(string.Format("Failed to insert/update {2} row, inserted {0} rows, CmdStr={1}", k, cmdStr, expectedRowsCountAffected));
        }
        return k;
    }

    //===================================================================================================
    public static int ExecuteNonQueryUptoNRows(DbCommand cmd, int expectedRowsCountAffectedMax, ResponseData resData = null)
    {
        var k = ExecuteNonQueryManyRows(cmd, 0, expectedRowsCountAffectedMax, resData);
        if (k == 0)
        {
            throw new Exception("No rows updated: "); // + cmdStr);
        }
        return k;
    }

    //===================================================================================================
    public static int ExecuteNonQueryManyRows(DbCommand cmd, int expectedRowsCountAffectedMin, int expectedRowsCountAffectedMax, ResponseData resData = null)
    {
        string cmdStr = DbCommandX.ToStr(cmd);
        MOutput.WriteHtmlEncodedPre("DbCommandX.ExecuteNonQueryUptoNRows: " + cmdStr);
        if (resData != null)
        {
            resData.addSql(cmdStr);
        }

        //cmd.ExecuteScalar();

        int k = cmd.ExecuteNonQuery();
        if (k > expectedRowsCountAffectedMax)
        {
            throw new Exception(string.Format("Failed to insert/update {2} row, inserted {0} rows, CmdStr={1}", k, cmdStr, expectedRowsCountAffectedMax));
        }
        return k;
    }

    public static Object ExecuteScalar(DbCommand cmd, ResponseData resData = null, string label = null)
    {
        string cmdStr = DbCommandX.ToStr(cmd);
        MOutput.WriteHtmlEncodedPre("DbCommandX.ExecuteScalar: " + cmdStr);
        if (resData != null)
        {
            resData.addSql(cmdStr, label);
        }

        //Nullable<object> rval = cmd.ExecuteScalar();
        object? rval = cmd.ExecuteScalar();
        MOutput.WriteLine("SQL Result: " + rval);

        //try { }
        //int k = cmd.ExecuteNonQuery();
        //if (k != 1)
        //{
        //    throw new Exception(string.Format("Failed to insert/update 1 row, inserted {0}, CmdStr={1}", k, cmdStr));
        //}
        return rval;
    }
    //---------------------------------------------------------------
    public static String ToStr(IDbCommand cmd)
    {
        //DbMaintCommandBuilder   - SqlMaintStatementBuilder
        //DbSelectCommandBuilder  - SqlSelectStatementBuilder
        //DbWhereClauseBuilder    - SqlWhereClauseBuilder
        //DbCommandText           - SqlText  - SqlLiteral 
        //DbCommand       

        if (cmd == null)
        {
            return "Arguments.DbCommand is null";
        }
        var buf = new StringBuilder();
        if (cmd.Parameters.Count == 0)
        {
            buf.Append(cmd.CommandText);
        }
        else
        {
            IDataParameterCollection col = cmd.Parameters;

            buf.AppendLine(cmd.CommandText);
            buf.Append("Params={");
            int k = 0;
            foreach (DbParameter p in col)
            {
                buf.Append(p.ParameterName);
                buf.Append("=");
                buf.Append("[");
                buf.Append(p.Value);
                buf.Append("]");
                if (k < col.Count - 1)
                {
                    buf.Append(", ");
                }
                k++;
            }
            buf.Append("}");
        }
        if (cmd.Transaction != null)
        {
            buf.Append(" Transaction: "+ cmd.Transaction.IsolationLevel);
        }
        return buf.ToString();
    }
    /*
    using( SqlConnection cn = new SqlConnection("server=(local);Database=Northwind;user id=sa;password=;")) 
{ 
SqlCommand cmd = new SqlCommand("CustOrderOne", cn); 
cmd.CommandType=CommandType.StoredProcedure ; 
SqlParameter parm=new SqlParameter("@CustomerID",SqlDbType.NChar) ; 
parm.Value="ALFKI"; 
parm.Direction =ParameterDirection.Input ; 
cmd.Parameters.Add(parm); 
SqlParameter parm2=new SqlParameter("@ProductName",SqlDbType.VarChar); 
parm2.Size=50; 
parm2.Direction=ParameterDirection.Output; 
cmd.Parameters.Add(parm2); 
SqlParameter parm3=new SqlParameter("@Quantity",SqlDbType.Int); 
parm3.Direction=ParameterDirection.Output; 
cmd.Parameters.Add(parm3); 
cn.Open(); 
cmd.ExecuteNonQuery(); 
cn.Close(); 
Console.WriteLine(cmd.Parameters["@ProductName"].Value); 
Console.WriteLine(cmd.Parameters["@Quantity"].Value.ToString()); 
Console.ReadLine(); 
*/
}
