namespace Libx;
public class UpdateCommandBuilder : UpsertCommandBuilder
{
    public UpdateCommandBuilder(DbCommand dbCommand, CrudOptions options) : base(dbCommand, options)
    {
        IsInserting = false;
    }
    //===================================================================================================
    public String BuildUpdateStatement(string wherec = null)
    {
        if (_nvs.Count == 0)
        {
            throw new Exception("NVCol is empty");
        }
        //String rval = "update " + _tableName + " set \n";

        var buf = new StringBuilder("update ");
        buf.Append(TableName);
        buf.AppendLine(" set");

        for (int i = 0; i < _nvs.Count; i++)
        //foreach (string name in _nvs.Keys)
        {
            //name = name.ToLower();
            //Object oVal = _nvs[name];
            //int     type  = colTypes[i];
            string name = _nvs.Keys[i];
            string val = (string)_nvs[i];

            //if (val == null || val.Length == 0 || val.Equals("''"))
            //{
            //   continue;
            //}
            //rval += name + " = " + val + ",\n";
            buf.Append(name);
            buf.Append(" = ");
            buf.Append(val);
            if (i < _nvs.Count - 1) //not is last value
            {
                buf.AppendLine(",");
            }
        }
        //rval = rval.Substring(0, rval.Length - 2);            
        //buf.Length -= 2;
        //buf.Remove(buf.Length - 2, 2);
        //MOutput.WriteLine(buf.ToString());

        if (wherec != null)
        {
            //rval += "\n where " + wherec;
            buf.AppendLine();
            buf.Append(" where ");
            buf.Append(wherec);
        }
        //return rval;
        return buf.ToString();
    }
}
