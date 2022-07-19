namespace Libx;
public abstract partial class MyDbContext
{
    public List<Dictionary<string, object>> QueryDictionary(DbDataReader reader, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        var fieldCount = reader.FieldCount;

        List<Dictionary<string, object>> ret = new();

        int k = 0;
        while (reader.Read())
        {
            if (++k > options.MaxRows)
            {
                if (options.ThrowOnMaxRowsExceeded)
                {
                    throw new Exception($"The number of rows returning has exceeeded MaxRows: {options.MaxRows}");
                }
                break;
            }

            Dictionary<string, object> dict = new();
            for (var i = 0; i < fieldCount; i++)
            {
                var colName = reader.GetName(i);
                object val = reader.GetValue(i);
                dict.Add(colName, val);
            }
            ret.Add(dict);

            if (ret.Count > 1)
            {
                throw new Exception("Cannot return more than 1 row for entity type of Dictionary[string, object]");
            }
        }
        int rowsFound = ret.Count;
        if (rowsFound < options.MinRows)
        {
            throw new Exception($"MinRows expected is {options.MinRows}, but actual is {rowsFound}");
        }
        return ret;
    }



    //===================================================================================================
    public List<LookupEntity> ReadLookups_byName(string queryName, int parentKeyId = 0, QueryOptions? options = null)
    {
        //object[] args = { queryName.Length, queryName };
        //object[] args2 = new object[2] [ queryName.Length, queryName ];
        //string result1 = String.Format("Temperature on {0:d}:\n{1,11}: {2} degrees (hi)\n{3,11}: {4} degrees (lo)",    {1,11} postition 11 string length=11

        //Random rnd = new Random();
        //int[] numbers = new int[4];
        //int total = 0;
        //for (int ctr = 0; ctr <= 2; ctr++)
        //{
        //    int number = rnd.Next(1001);
        //    numbers[ctr] = number;
        //    total += number;
        //}
        //numbers[3] = total;
        //object[] values = new object[numbers.Length];
        //numbers.CopyTo(values, 0);
        //Console.WriteLine("{0} + {1} + {2} = {3}", values);


        options = CheckOptions(options);
        string sql = LookupRegistry.GetLookupQuery(queryName, this);
        if (parentKeyId != 0)
        {
            sql = String.Format(sql, parentKeyId);
        }
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return ReadLookups(cmd, options);
        }
    }
    //===================================================================================================
    public List<LookupEntity> ReadLookups(string sql, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return ReadLookups(cmd, options);
        }
    }
    //===================================================================================================
    public List<LookupEntity> ReadChildLookups(string sql, int parentKeyId, QueryOptions? options = null)
    {

        options = CheckOptions(options);
        using (var cmd = dbc.CreateCommand())
        {
            cmd.CommandText = sql;
            return ReadLookups(cmd, options);
        }
    }
    public List<LookupEntity> ReadLookups(DbCommand cmd, QueryOptions? options = null)
    {
        options = CheckOptions(options);
        var ret = new List<LookupEntity>();
        //logger.Log_Sql(cmd);
        using (var reader = ExecuteReader(cmd, options))
        {
            int k = 0;
            while (reader.Read())
            {
                if (++k > options.MaxRows)
                {
                    if (options.ThrowOnMaxRowsExceeded)
                    {
                        throw new Exception($"The number of rows returning has exceeeded MaxRows: {options.MaxRows}");
                    }
                    break;
                }
                var ent = new LookupEntity
                {
                    KeyId = reader.GetInt32(0),
                    Value = reader.GetString(1)
                };
                ret.Add(ent);
            }
        }
        return ret;
    }


}
