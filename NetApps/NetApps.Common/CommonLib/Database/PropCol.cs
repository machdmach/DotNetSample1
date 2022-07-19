using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Common
{
    public class NormalizeColumnNameEqualityComparer : IEqualityComparer<String>
    {
        internal static string NormalizeColumnName(string colName)
        {
            return colName.Trim().Replace("_", "").ToLower();
        }

        public bool Equals(string? x, string? y)
        {
            return NormalizeColumnName(x) == NormalizeColumnName(y);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return NormalizeColumnName(obj).GetHashCode();
        }
    }

    //public class ColPropMapping
    public class PropCol : Prop
    {
        public string ColumnName { get; set; }
        public int ColumnOrdinal { get; set; }
        public int PropertyOrdinal { get; set; }
        public string Label { get; set; }
        public int PropertyOrder { get; set; } = 0;
        public bool NoInsert { get; set; }
        public bool NoUpdate { get; set; }

        ///<see cref="DataColumn.ReadOnly"/>
        public bool IsReadOnly => NoInsert && NoUpdate;

        ///<see cref="DataColumn.AllowDBNull"/>
        ///<see cref="DbColumn.AllowDBNull"/>
        ///<see cref="RequiredAttribute"/>
        public bool Required { get; set; }

        public DbType DbType { get; set; }

        public Type ColumnDataType { get; set; }
        public String ColumnDataTypeName { get; set; }
        public bool PropColTypesMatched { get; set; }

        /// <see cref="DbColumn.IsKey"/>
        public bool IsPrimaryKey { get; set; }

        //===================================================================================================
        //public PropCol() { }
        public PropCol(PropertyInfo prop) : base(prop)
        {
            //var attr = prop.GetCustomAttribute<DbColAttribute>(false);
            var attrs = prop.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr is DbColAttribute dbColAttr)
                {
                    if (!string.IsNullOrWhiteSpace(dbColAttr.ColumnName))
                    {
                        ColumnName = dbColAttr.ColumnName.Trim();
                    }
                    NoInsert = dbColAttr.NoInsert;
                    NoUpdate = dbColAttr.NoUpdate;
                    //IsPrimaryKey = dbColAttr.PrimaryKey;
                }
                else if (attr is KeyAttribute keyAttr)
                {
                    IsPrimaryKey = true;
                }
                else if (attr is DisplayAttribute dispAttr)
                {
                    Label = dispAttr.Name;
                }
                else if (attr is RequiredAttribute requiredAttr)
                {
                    Required = true;
                }
                else if (attr is StringLengthAttribute strLenAttr)
                {

                }
            }
            //DataColumn dc;
            if (ColumnName == null)
            {
                ColumnName = PropertyName;
            }
        }
        //===================================================================================================
        public string toStringShort()
        {
            var buf = new StringBuilder();
            var pc = this;
            string mesg = $"Property: {pc.PropertyName}({pc.PropDataType.FullName}) ~~ Column: {pc.ColumnName}({pc.ColumnDataType?.FullName})-{pc.ColumnDataTypeName}";
            buf.Append(mesg);
            return buf.ToString();
        }

        //===================================================================================================
        public void ReadDbAndSetValue(Object ent, DbDataReader reader)
        {
            var pc = this;
            var colOrdinal = pc.ColumnOrdinal;
            object val = reader.GetValue(colOrdinal);

            //if (reader.IsDBNull(colOrdinal))
            //if (val == DBNull.Value)
            //{
            //    //System.InvalidCastException: Column contains NULL data
            //    return; //---------------------------------------
            //}

            //if (pc.IsString)
            //{
            //    //if (reader.IsDBNull(colOrdinal))
            //    //{
            //    //    return;
            //    //}
            //    string sVal = reader.GetString(colOrdinal);
            //    //pc.PropInfo.SetValue(ent, sVal);
            //    if (sVal != null)
            //    {
            //        pc.PropInfo.SetValue(ent, sVal);
            //    }
            //    //return;
            //}

            //if (reader.IsDBNull(colOrdinal))
            if (val == DBNull.Value)
            {
                if (pc.PropDataType.IsValueType)
                {
                    if (pc.IsNullable)
                    {
                        return; //-----------------
                    }
                    else
                    {
                        throw new Exception("Err9238: Property is NotNullable but the value from db is null. " + pc.toStringShort());
                    }
                }
                else
                {
                    return; //-----------------
                }
            }
            //DBNull.Value;
            if (pc.PropColTypesMatched)
            {
                pc.PropInfo.SetValue(ent, val); //reader.GetValue(colOrdinal));
            }
            else
            {
                //object val = reader.GetValue(colOrdinal);
                try
                {
                    //info.SetValue(myobj, val1 == DBNull.Value ? null : (int?)Convert.ToInt32(val1), null);
                    //pc.PropInfo.SetValue(ent, val);
                    //pc.PropInfo.SetValue(ent, val == DBNull.Value ? null : val); // (int?)Convert.ToInt32(val));
                    //pc.PropInfo.SetValue(ent, val == DBNull.Value ? null : val); // (int?)Convert.ToInt32(val));

                    if (pc.IsValueType)
                    {
                        val = Convert.ChangeType(val, pc.PropDataType);
                        //System.Exception: Invalid cast from 'System.Int16' to 'System.Nullable`1[[System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.
                    }
                    pc.PropInfo.SetValue(ent, val);
                }
                catch (Exception ex)
                {
                    //System.ArgumentException: Object of type 'System.Decimal' cannot be converted to type 'System.Int32'.
                    //System.ArgumentException: Object of type 'System.Int16' cannot be converted to type 'System.Nullable`1[System.Int32]'.
                    //Property: FoundYearNumber(System.Nullable`1[[System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]) ~~ Column: FoundYearNumber(System.Int16)-Int16
                    throw new Exception(ex.Message + "\n" + pc.toStringShort() + "\n" + ex.ToString());
                }
            }

            //FormData ff;
            //    if (reader.GetFieldType(i) == property.PropertyType)
            //    {
            //        property.SetValue(item, reader[i]);
            //    }
            //    else if (reader.GetProviderSpecificFieldType(i) == typeof(SqlXml))
            //    {
            //        //var serializer = new XmlSerializer(property.PropertyType);
            //        //property.SetValue(item, serializer.Deserialize(reader.GetXmlReader(i)));
            //    }
            //}
        }
    }

    //===================================================================================================
    public class QueryOptions
    {
        public int MinRows = 0;
        public int MaxRows = 2000;
        public bool ThrowOnMaxRowsExceeded = false;
        public bool ThrowIfAnyColumnsUnMapped = false;

        public object? Parameters { get; set; }
        public int? CommandTimeout { get; set; }
        public DbTransaction Transaction { get; set; }
        public CommandType? CommandType { get; set; }

        //public List<PropCol> nameMaps = new List<PropCol>();
        public Dictionary<string, string> propColNameMaps = new();

        public QueryOptions()
        {
            //nameMaps = new List<OrmColumnProp>();
        }
        //public void AddPropColNameMapping(string propName, string colName)
        //{
        //    nameMaps.Add(new PropCol { PropertyName = propName, ColumnName = colName });
        //}
        //void eg1()
        //{
        //    var options = new QueryOptions { nameMaps = new List<PropCol> { new PropCol { ColumnName = "KEY_ID", PropertyName = "KeyId" } } };
        //}
    }
    //===================================================================================================
    public class CrudOptions: QueryOptions
    {
        public int MinAffectedRows { get; set; } = 1;
        public int MaxAffectedRows { get; set; } = 1;
        //public bool ThrowOnMinAffectedRows = false;
        //public bool ThrowOnMaxAffectedRowsExceeded = false;
        public bool ThrowIfAffectedRowsOutOfRange { get; set; } = true;

        //[Obsolete]
        //public bool IsParameterized { get; set; }
        public bool CommaAtTheBeginning { get; set; }
        public bool InsertDefaultValues { get; set; }

        public bool ShowColumnNameAfterValue { get; set; }
        public bool OneColumnOnEachLine { get; set; }
        //public bool IncludePrimaryKeys { get; set; }
        //public DateTime? DateTimeColumnValue { get; set; }
        public string OutputPrimaryKeyName { get; set; }

        public IDbCommand SetupCommandz(IDbConnection cnn, Action<IDbCommand, object> paramReader)
        {
            var cmd = cnn.CreateCommand();
            //var init = GetInit(cmd.GetType());
            //init?.Invoke(cmd);
            if (Transaction != null)
                cmd.Transaction = Transaction;
            //cmd.CommandText = CommandText;
            if (CommandTimeout.HasValue)
            {
                cmd.CommandTimeout = CommandTimeout.Value;
            }
            //else if (SqlMapper.Settings.CommandTimeout.HasValue)
            //{
            //    cmd.CommandTimeout = SqlMapper.Settings.CommandTimeout.Value;
            //}
            if (CommandType.HasValue)
                cmd.CommandType = CommandType.Value;
            paramReader?.Invoke(cmd, Parameters);
            return cmd;
        }

    }
    public class InsertOptions : CrudOptions
    {
    }
}