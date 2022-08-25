namespace Libx;
public abstract partial class MyDbContext: MyDbContext_Base
{
    public DbTableInfo TableInfo => new DbTableInfo(this);

    public InsertCommandBuilder CreateInsertCommandBuilder(InsertOptions? options = null)
    {
        var cmd = CreateCommand(options);
        var cb = new InsertCommandBuilder(cmd, options);
        cb.ParameterPrefix = ProviderParamMarker;
        return cb;
    }
    //===================================================================================================
    public void InsertRow(Object data, InsertOptions? options = null)
    {
        //options = CheckOptions(options);
        var mapper = new PropColMapper(data.GetType(), logger);
        mapper.AddPropColMappings(options?.propColNameMaps);
        mapper.MapTableColumns(dbc);

        var cb = CreateInsertCommandBuilder(options);
        cb.TableName = mapper.TableName;

        var cmd = cb.DbCommand;
        foreach (PropCol pc in mapper.MappedPropCols)
        {
            object val = pc.PropInfo.GetValue(data);
            cb.SetValue(pc.ColumnName, val);
        }
        cmd.CommandText = cb.BuildInsertStatement();
        ExecuteNonQuery(cmd, options);
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
}
