namespace Libx;
public class InsertCommandBuilder : UpsertCommandBuilder
{
    public InsertCommandBuilder(DbCommand dbCommand, CrudOptions options) : base(dbCommand, options)
    {
        IsInserting = true;
    }
    //===================================================================================================
    public String BuildInsertStatement()
    {
        //String names = "";
        var namesBuf = new StringBuilder();
        //String values = "";
        var valuesBuf = new StringBuilder();
        if (_nvs.Count == 0)
        {
            throw new Exception("NVCol is empty");
        }
        int k = 0;
        int lastK = _nvs.Keys.Count - 1;
        foreach (string name in _nvs.Keys)
        {
            bool isFirstCol = k == 0;
            bool isLastKcol = k == lastK;

            //name = name.ToLower();
            //Object oVal = _nvs[name];
            //int     type  = colTypes[i];
            string val = (string)_nvs[name];

            if (ParameterizedNames.Contains(name))
            {
                val = ParameterPrefix + name;
                //AddParameterImpl(name, 0, val);
            }

            //if (val == null || val.Length == 0 || val.Equals("''"))
            //{
            //   continue;
            //}
            string namex = name;
            if (options.CommaAtTheBeginning)
            {
                namex = ", " + namex;
                val = ", " + val;
            }
            else
            {
                if (!isLastKcol)
                {
                    namex = namex + ", ";
                    val = val + ", ";
                }
            }
            if (options.ShowColumnNameAfterValue)
            {
                val = val + " --" + name;
            }
            if (options.OneColumnOnEachLine)
            {
                namex = namex + Environment.NewLine;
                val = val + Environment.NewLine;
            }

            //string fieldDelim = ",";
            //names += (name + fieldDelim) + "\n";
            //values += (val + fieldDelim) + " --" + name + "\n";
            //values += (val + fieldDelim) + "\n";
            namesBuf.Append(namex);
            valuesBuf.Append(val);
            //values += val;
            k++;
        }
        //namesBuf.Trim();
        //valuesBuf.Trim();

        //if (options.CommaAtTheEnd)
        //{
        //    names = names.Substring(0, (names.Length - 2));
        //    values = values.Substring(0, (values.Length - 2));
        //    //names = Regex.Replace(names, ",[ \n\f]*$", "");
        //    //values = Regex.Replace(values, ", [ \n\f]*$,", "");
        //}
        //else
        //{
        //    names = names.Substring(1, (names.Length - 2));
        //    values = values.Substring(1, (values.Length - 2));
        //    //names = Regex.Replace(names, "^[ ]*,", "");
        //    //values = Regex.Replace(values, "^[ ]*,", "");
        //}
        //names = names.Substring(0, (names.Length - 2) - (0));
        //values = values.Substring(0, (values.Length - 2) - (0));

        string names = namesBuf.ToString().Trim();
        string values = valuesBuf.ToString().Trim();

        String rval;
        if (options.OutputPrimaryKeyName != null)
        {
            rval = string.Format("insert into {0} (\n{1}\n) output inserted.{3} values (\n{2}\n)", TableName, names, values, options.OutputPrimaryKeyName);
            //cmd.ExcecuteScalar();
        }
        else
        {
            rval = string.Format("insert into {0} (\n{1}\n) values (\n{2}\n)", TableName, names, values);

        }
        return rval;
    }

}
