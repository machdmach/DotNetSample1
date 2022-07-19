using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// <seeAlso cref="DataColumn"/>
    /// </summary>
    public partial class DataTableX
    {
        private static void Main()
        {
            //using (OracleConnection con = OracleConnectionX.Open_DBPUBS_UPROD())
            //{
            //   colNames = DataTableX.GetColumnNames("cdis_cd", con);
            //}
            DataTable t1 = DataTableX.Create_AspNet_Users(1);
            //DataSetX ds;
            // ds.GetXml

            //LinqX x;
            string[] strArr = { "aa", "bb" };
            var l = from r in strArr
                    select new { name = r, IsAnonymous = 2 };

            DataTable t2 = DataTableX.FromNTupleList(l);
            //log.Debug(DataTableX.ToHtml(t2));

            return;

            //DataTable dt = new DataTable();
            //dt.Columns.Add("c1");

            //dt.Rows.Add("v1");
            //dt.Rows.Add("v2");
            //DataRow r = dt.Rows[0];
            //dt.Rows.Remove(dt.Rows[0]);
            //Log.Debug(dt.Rows.Count);
            //object[] ia = r.ItemArray;

            ////dt.Columns[0].
            //DataColumn col = dt.Columns["c1"];
            //DataColumnX cx;
            ////DataColumnX.GetItemArray(col);0
            ////col.Table;

            //DataRow newR = dt.NewRow();
            //newR.ItemArray = new string[] { "bb" };
            //Log.Debug(newR.RowState); //Detached.

            //Log.Debug(r.RowState); //Detached
            ////DataRow r = dt.Rows[1]; //Exc no row @position1
            ////Log.Debug(r);

            //return;
            //DataTable dt2 = DataTableX.Create_AspNet_Users(10);
            //Log.Debug(DataTableX.ToString(dt2));

        }
        //===================================================================================================
        public static void RemoveNullColumns(DataTable dt)
        {
            var cols = GetNullColumns(dt);
            foreach (var col in cols)
            {
                MOutput.WriteLineFormat("Removing column {0}", col.ColumnName);
                dt.Columns.Remove(col);
            }
        }

        //===================================================================================================
        public static List<DataColumn> GetNullColumns(DataTable dt)
        {
            var rowsCount = dt.Rows.Count;
            var colsCount = dt.Columns.Count;
            int[] colNullCount = new int[colsCount];

            for (int i = 0; i < rowsCount; i++)
            {
                DataRow r = dt.Rows[i];
                var cols = dt.Columns;
                for (int j = 0; j < colsCount; j++)
                {
                    object val = r[j];
                    if (val == DBNull.Value)
                    {
                        colNullCount[j]++;
                    }
                }
            }
            var rval = new List<DataColumn>();

            MOutput.WriteLine("Total rows: " + rowsCount);
            for (int j = 0; j < colsCount; j++)
            {
                DataColumn col = dt.Columns[j];
                //MOutput.WriteLineFormat("{0} = {1}", col.ColumnName, colNullCount[j]);

                if (colNullCount[j] == rowsCount)
                {
                    //MOutput.WriteLineFormat("Removing column {0}, nullCount: {1}", col.ColumnName, colNullCount[j]);
                    //dt.Columns.Remove(col);
                    rval.Add(col);
                }
            }
            return rval;
        }
        //****************************************************************
        public static DataTable GetSchemaTable(String tab, DbConnection conn)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from " + tab + " where 1=2";

            //return DbCommandX.GetColumnNames(cmd);
            //log.Warn(cmd.CommandText);

            using (DbDataReader rdr = cmd.ExecuteReader())
            {
                DataTable schemaTable = rdr.GetSchemaTable();
                return schemaTable;
            }
            //SchemaTable:
            // ColumnName    | ColumnSize | DataType        | AllDBNull 
            // KEY_ID        | 22         | System.Decimal  | TRUE
            // DOC_TYPE      | 100        | System.String   | FALSE
            // OBSOLETE_DATE | 7          | System.DateTime
            // BaseLine_FLAG | 1          | System.String
        }
        //****************************************************************                         
        public static void CheckColumnExists(String tableName, string colName, DbConnection conn)
        {
            colName = colName.ToLower();
            string[] colNames = DataTableX.GetColumnNames(tableName, conn);

            foreach (string col in colNames)
            {
                if (col.ToLower() == colName)
                {
                    return;
                }
            }
            StringBuilder sb = new StringBuilder();
            colNames.ToList().ForEach(e => sb.Append(e.ToString() + "\n"));
            string err = string.Format("Column {0} does not exist in table {1}\n Available Colums are: {2}", colName, tableName, sb);
        }
        //****************************************************************                         
        public static bool ContainsColumn(String tableName, string colName, DbConnection conn)
        {
            colName = colName.ToLower();
            string[] colNames = DataTableX.GetColumnNames(tableName, conn);
            foreach (string col in colNames)
            {
                if (col.ToLower() == colName)
                {
                    return true;
                }
            }
            return false;
        }
        //****************************************************************                         
        public static string[] GetColumnNames(String tab, DbConnection conn)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from " + tab + " where 1=2";

            //return DbCommandX.GetColumnNames(cmd);
            //log.Warn(cmd.CommandText);
            using (DbDataReader rdr = cmd.ExecuteReader())
            {
                DataTable schemaTable = rdr.GetSchemaTable();
                return DataTableX.GetColumnNames(schemaTable);
            }
        }

        public static string[] GetColumnNames(DataTable schemaTable)
        {
            String[] rval = new string[schemaTable.Rows.Count];
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow row = schemaTable.Rows[i];
                Object[] oa = row.ItemArray;
                String colName = (String)oa[0];
                //int columnSize = (int)oa[(int)nameIdx["ColumnSize"]];
                //Type dataType = (Type)oa[(int)nameIdx["DataType"]];
                //Sytem.String, System.Decimal, System.DateTime
                rval[i] = colName.ToLower();
            }
            return rval;
        }

        //****************************************************************
        public static void ColumnNamesToTitleCase(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn dc = dt.Columns[i];
                var colName = dc.ColumnName.Replace("_", " ");
                dc.ColumnName = StringX.ToTitleCase(dc.ColumnName);
            }
        }
        //****************************************************************
        public static void AddRow(DataTable tab, NameValueCollection nvs)
        {
            //log.Debug("addRows for nvs: " + NameValueCollectionX.ToString(nvs));

            //Todo: nvs = NameValueCollectionX.LowerCaseKeys(nvs);
            DataRow r = tab.NewRow();
            object[] sa = new object[tab.Columns.Count];

            for (int i = 0; i < tab.Columns.Count; i++)
            {
                DataColumn dc = tab.Columns[i];
                string colName = dc.ColumnName.ToLower();
                string val = nvs[colName];
                if (string.IsNullOrEmpty(val))
                {
                    sa[i] = DBNull.Value;
                    //log.Debug("null value for column: " + colName);
                }
                else
                {
                    sa[i] = val;
                }
            }
            //tab.AcceptChanges();
            tab.Rows.Add(r);  //must add r to Rows.
            r.ItemArray = sa;
        }
        //****************************************************************
        public static void AddRow(DataTable tab, NameObjectCollection nvs)
        {
            //log.Debug("addRows for nvs: " + nvs.ToStringX());

            nvs = NameObjectCollection.LowerCaseKeys(nvs);
            DataRow r = tab.NewRow();
            object[] sa = new object[tab.Columns.Count];

            for (int i = 0; i < tab.Columns.Count; i++)
            {
                DataColumn dc = tab.Columns[i];
                string colName = dc.ColumnName.ToLower();
                object val = nvs[colName];
                if (val == null)
                {
                    sa[i] = DBNull.Value;
                    //log.Debug("null value for column: " + colName);
                }
                else
                {
                    sa[i] = val;
                }
            }
            //tab.AcceptChanges();
            tab.Rows.Add(r);  //must add r to Rows.
            r.ItemArray = sa;
        }

        public static void ToList_eg()
        {
            List<DataRow> rows = DataTableX.Create_AspNet_Users(30).AsEnumerable().ToList();
            //if bind rows to LinqDatSource:
            // System.Web.HttpException: DataBinding: 'System.Data.DataRow' does not contain a property with the name 'Username'.

            //LinqDataSourceSelectEventArgs.Result = 
            // from r in DataTableX.Create_AspNet_Users(30).AsEnumerable()
            //  select new { username = r.ItemArray[1] };

        }
        //========================================================================================
        public static string ToHtml(DataTable dt, bool htmlEncode = true, bool showRowNumber = true)
        {
            if (dt == null)
            {
                return "datatable is null";
            }
            if (dt.Rows.Count == 0)
            {
                return "Data table is Empty"; // enter code here
            }
            if (dt.Rows.Count == 1)
            {
                dt = DataTableX.TurnSideway(dt);
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<table class='auditDataTab' style='border: solid 1px Silver; border-collapse: collapse;'>");

            builder.AppendLine("<thead>");
            builder.AppendLine("<tr >");

            //vertical-align: top;

            if (showRowNumber)
            {
                builder.Append("<th style='border: solid 1px Silver; padding:3px'><b>");
                builder.Append("#");
                builder.AppendLine("</b></td>");
            }
            foreach (DataColumn c in dt.Columns)
            {
                builder.Append("<th style='border: solid 1px Silver; padding:3px'><b>");
                builder.Append(c.ColumnName);
                builder.AppendLine("</b></td>");
            }
            builder.AppendLine("</tr>");
            builder.AppendLine("</thead>");

            builder.AppendLine("<tbody>");
            int k = 0;
            foreach (DataRow r in dt.Rows)
            {
                k++;
                builder.Append("<tr >");
                if (showRowNumber)
                {
                    builder.Append("<td style='border: solid 1px Silver; padding:3px'>");
                    builder.Append(k);
                    builder.AppendLine("</td>");

                }
                foreach (DataColumn c in dt.Columns)
                {
                    object val = r[c];
                    string sVal = "";
                    if (val == null)
                    {
                        //sVal = "null";
                    }
                    else if (val == DBNull.Value)
                    {
                        //sVal = "dbNull";
                    }
                    else
                    {
                        sVal = val.ToString();
                    }
                    if (htmlEncode)
                    {
                        sVal = System.Net.WebUtility.HtmlEncode(sVal);
                    }

                    builder.Append("<td style='border: solid 1px Silver; padding:3px'>");
                    builder.Append(sVal);
                    builder.AppendLine("</td>");
                }
                builder.AppendLine("</tr>");
            }
            builder.AppendLine("</tbody>");
            builder.AppendLine("</table>");
            return builder.ToString();
        }
        //****************************************************************
        //params argument can be nothing/unspecified
        public static String ToString(DataTable tab, params string[] titles)
        {
            string title = "";
            if (titles.Length > 0) { title = titles[0]; }

            const string FS = "\t";  //Awk, NR, NF, Record Number, Field Number.
            const string RS = "\n";  //Awk
            StringBuilder buf = new StringBuilder(title + "\n");
            buf.Append("#");
            buf.Append(FS);
            for (int i = 0; i < tab.Columns.Count; i++)
            {
                DataColumn dc = tab.Columns[i];
                string columnName = dc.ColumnName;
                //columnName = StringX.ToTitleCase(columnName);
                buf.Append(columnName);
                buf.Append(FS);
            }
            buf.Append(RS);

            if (tab.Rows.Count == 0)
            {
                buf.Append("No rows exist");
            }
            else
            {
                buf.Append("-----------------------" + RS);
                //foreach (DataRow r in tab.Rows)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow r = tab.Rows[i];
                    buf.Append(i);
                    buf.Append(FS);
                    for (int j = 0; j < tab.Columns.Count; j++)
                    {
                        DataColumn dc = tab.Columns[j];
                        object o = r[j];
                        buf.Append(o.ToString());
                        buf.Append(FS);
                    }
                    buf.Append(RS);
                }
            }
            return buf.ToString();
        }
        //**************************************************************** 
        //[Deprecated]
        public static DataTable Create_AspNet_Users(int numberOfRows)
        {
            return GetSampleDataTable(numberOfRows);
        }
        //**************************************************************** 
        public static DataTable GetSampleDataTable(int numberOfRows)
        {
            //DataTable dt = DataTableX.Create_AspNet_Users(10);
            //Log.Debug(DataTableX.ToString(dt));

            DataTable dt = new DataTable("aspnet_users", "au_ns1");
            //dc.Caption = dc.ColumnName
            string UserIdN = null;
            string ApplUserIdicationIdN = null;
            string UserNameN = null;
            string LoweredUserNameN = null;
            string MobileAliasN = null;
            string IsAnonymousN = null;
            string LastActivityDateN = null;
            string ageN = null;

            //           dt.Columns.Add(new DataColumn(UserIdN = "UserId", typeof(Guid)));
            dt.Columns.Add(new DataColumn(UserIdN = "UserId", typeof(Guid)));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };

            //dt.Columns.Add(new DataColumn(ApplUserIdicationIdN="ApplUserIdicationId", typeof(string)));
            dt.Columns.Add(new DataColumn(UserNameN = "UserName", typeof(string)));
            //dt.Columns.Add(new DataColumn(LoweredUserNameN="LoweredUserName", typeof(string)));
            //dt.Columns.Add(new DataColumn(MobileAliasN="MobileAlias", typeof(string)));
            dt.Columns.Add(new DataColumn(IsAnonymousN = "IsAnonymous", typeof(bool)));
            dt.Columns.Add(new DataColumn(LastActivityDateN = "LastActivityDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn(ageN = "age", typeof(Decimal)));


            Random ran = new Random();
            for (int i = 0; i < numberOfRows; i++)
            {
                DataRow r = dt.NewRow();
                List<Object> items = new List<object>();
                //if (UserIdN != null) { items.Add(Guid.NewGuid()); }
                if (UserIdN != null) { items.Add(Guid.NewGuid()); }

                if (ApplUserIdicationIdN != null) { items.Add("app1"); }
                if (UserNameN != null) { items.Add("User" + ran.Next(1, 9) + "_" + +i); }
                if (LoweredUserNameN != null) { items.Add("user" + ran.Next(1, 9) + "_" + +i); }
                if (MobileAliasN != null) { items.Add("mobileALias" + i); }
                if (IsAnonymousN != null) { items.Add(i % 2); }
                if (LastActivityDateN != null) { items.Add(DateTime.Now.AddDays(ran.Next(-600, 0))); }
                if (ageN != null) { items.Add(ran.Next(1, 100)); } //.ne RandomX.n items.Add((10 + i) % 100); }
                //r.ItemArray = new object[] { Guid.NewGuid(), "app1", "user" + i, "user" + i, "mobileALias" + i, i % 2, DateTime.Now.AddDays(i), i };

                //r.ItemArray = IEnumerableX.ToArray<object>(items);//OK too
                r.ItemArray = items.ToArray(); // IEnumerableX.ToArray(items);

                dt.Rows.Add(r);
            }
            dt.AcceptChanges();
            return dt;
        }
        public static Object ConvertObjz(string s, Type type)
        {
            Object rval;
            if (type == typeof(Boolean))
            {
                rval = Boolean.Parse(s);
            }
            else if (type == typeof(Byte))
            {
                rval = Byte.Parse(s);
            }
            else if (type == typeof(SByte))
            {
                rval = SByte.Parse(s);
            }
            else if (type == typeof(Char))
            {
                rval = Char.Parse(s);
            }
            else if (type == typeof(Int16))
            {
                rval = Int16.Parse(s);
            }
            else if (type == typeof(Int32))
            {
                rval = Int32.Parse(s);
            }
            else if (type == typeof(Int64))
            {
                rval = Int64.Parse(s);
            }
            else if (type == typeof(UInt16))
            {
                rval = UInt16.Parse(s);
            }
            else if (type == typeof(UInt32))
            {
                rval = UInt32.Parse(s);
            }
            else if (type == typeof(UInt64))
            {
                rval = UInt64.Parse(s);
            }

            else if (type == typeof(Single))
            {
                rval = Single.Parse(s);
            }
            else if (type == typeof(Double))
            {
                rval = Double.Parse(s);
            }
            else if (type == typeof(Decimal))
            {
                rval = Decimal.Parse(s);
            }


            else if (type == typeof(DateTime))
            {
                rval = DateTime.Parse(s);
            }
            else if (type == typeof(TimeSpan))
            {
                rval = TimeSpan.Parse(s);
            }

            else
            {
                rval = s;
            }
            return rval;
        }
        ////===================================================================================================
        //public List<T> ConvertTo<T>(DataTable datatable) where T : new()
        //{
        //    List<T> Temp = new List<T>();
        //    try
        //    {
        //        List<string> columnsNames = new List<string>();
        //        foreach (DataColumn DataColumn in datatable.Columns)
        //            columnsNames.Add(DataColumn.ColumnName);
        //        Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
        //        return Temp;
        //    }
        //    catch
        //    {
        //        return Temp;
        //    }

        //}
        ////===================================================================================================
        //public T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        //{
        //    T obj = new T();
        //    try
        //    {
        //        string columnname = "";
        //        string value = "";
        //        PropertyInfo[] Properties;
        //        Properties = typeof(T).GetProperties();
        //        foreach (PropertyInfo objProperty in Properties)
        //        {
        //            columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
        //            if (!string.IsNullOrEmpty(columnname))
        //            {
        //                value = row[columnname].ToString();
        //                if (!string.IsNullOrEmpty(value))
        //                {
        //                    if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
        //                    {
        //                        value = row[columnname].ToString().Replace("$", "").Replace(",", "");
        //                        objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
        //                    }
        //                    else
        //                    {
        //                        value = row[columnname].ToString().Replace("%", "");
        //                        objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
        //                    }
        //                }
        //            }
        //        }
        //        return obj;
        //    }
        //    catch
        //    {
        //        return obj;
        //    }
        //}

        //===================================================================================================
        //http://stackoverflow.com/questions/1427484/convert-datatable-to-listt
        public static List<T> ToList<T>(DataTable dt) where T : new()
        {
            int dtCount = dt.Rows.Count;


            List<T> list = new List<T>();

            Dictionary<String, int> colNameIndexMap = new Dictionary<string, int>();
            int colIndex = 0;

            foreach (DataRow r in dt.Rows)
            {
                var colName = r[0].ToString();
                colNameIndexMap.Add(colName, colIndex);
                colIndex++;
            }

            Type oType = typeof(T);
            PropertyInfo[] oProps = oType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

            int cIndex = 0;
            foreach (DataColumn c in dt.Columns)
            {
                if (cIndex == 0)
                {
                    cIndex++;
                    continue;
                }
                T t = new T();
                foreach (PropertyInfo pi in oProps)
                {
                    Type colType = pi.PropertyType;
                    string pName = pi.Name;
                    int colIndx = colNameIndexMap[pName];
                    DataRow r = dt.Rows[colIndx];
                    object colVal = r[c];
                    //Type colValType = colVal.GetType();

                    //object colValTyped = DataTableX.ConvertObj(colVal, colType);
                    object colValTyped = Convert.ChangeType(colVal, colType);

                    pi.SetValue(t, colValTyped, new Object[] { });
                }
                list.Add(t);
                cIndex++;
            }
            return list;
        }
        //public DataTable ToDataTable<T>(IEnumerable<T> collection)
        //{
        //    DataTable newDataTable = new DataTable();
        //    Type impliedType = typeof(T);
        //    PropertyInfo[] _propInfo = impliedType.GetProperties();
        //    foreach (PropertyInfo pi in _propInfo)
        //        newDataTable.Columns.Add(pi.Name, pi.PropertyType);

        //    foreach (T item in collection)
        //    {
        //        DataRow newDataRow = newDataTable.NewRow();
        //        newDataRow.BeginEdit();
        //        foreach (PropertyInfo pi in _propInfo)
        //            newDataRow[pi.Name] = pi.GetValue(item, null);
        //        newDataRow.EndEdit();
        //        newDataTable.Rows.Add(newDataRow);
        //    }
        //    return newDataTable;
        //}
        //*******************************************************************
        //Usage: DataTable dt = DataTable.ValueOf(ds, rec => new object[] { ds });
        //Err: this IEnumerable<T> varlist,  Extension method must be in non-generic static class
        //public static DataTable ValueOf<T>(IEnumerable<T> varlist, CreateRowDelegate<T> fnzz)
        public static DataTable FromNTupleList<T>(IEnumerable<T> varlist, int maxRows = 1000, bool throwsOnExceedingMaxRows = true)
        {
            return FromNTupleList2(varlist, maxRows, throwsOnExceedingMaxRows);
        }
        public static DataTable FromNTupleList2(IEnumerable varlist, int maxRows = 1000, bool throwsOnExceedingMaxRows = true)
        {
            if (varlist == null) { return null; }

            Type[] dbTypes = new Type[]
            {
                typeof(Boolean),
                typeof(Byte),
                typeof(Char),
                typeof(DateTime ),
                typeof(Decimal ),
                typeof(Double ),
                typeof(Int16 ),
                typeof(Int32 ),
                typeof(Int64 ),
                typeof(SByte ),
                typeof(Single ),
                typeof(String ),
                typeof(TimeSpan ),
                typeof(UInt16 ),
                typeof(UInt32 ),
                   typeof(UInt64 ),
            };
            //Dictionary<string, Type> nameType = new Dictionary<string, Type>();

            DataTable rval = new DataTable();
            Type oType = null;
            foreach (object rec in varlist)
            {
                oType = rec.GetType();
                break;
            }
            if (oType == null)
            {
                return rval; //------------------------------
            }
            if (oType.IsPrimitive || oType == typeof(string))
            {
                return FromSingleTupleList(varlist, "Item1");
            }

            //if (tableName.Length > 0)
            //{
            //    rval = new DataTable(tableName[0]);
            //}
            //Type oType = typeof(T);
            PropertyInfo[] oProps = oType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (oProps.Length == 0)
            {
                throw new Exception("No properties found for type: " + oType);
            }

            foreach (PropertyInfo pi in oProps)
            {
                Type colType = pi.PropertyType;
                //if (colType.IsGenericType)
                //{
                //    //nameType.Add(pi.Name, pi.PropertyType);
                //    colType = typeof(string);
                //}
                //if (colType == typeof(object)) { colType = typeof(string); }
                if (!dbTypes.Contains(colType))
                {
                    if (colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        //Nullable<Int32>
                        colType = colType.GetGenericArguments()[0];
                    }
                    else
                    {
                        colType = typeof(string);
                    }
                }

                rval.Columns.Add(new DataColumn(pi.Name, colType));
                //rval.Columns.Add(new DataColumn(pi.Name, typeof(String)));
                //The data source for GridView with id 'gv1' did not have any properties or 
                //attributes from which to generate columns.  Ensure that your data source has content
            }

            int rowCount = 0;
            foreach (object rec in varlist)
            {
                if (rowCount++ > maxRows)
                {
                    if (throwsOnExceedingMaxRows)
                    {
                        throw new Exception("To Many rows, exceeding: " + maxRows);
                    }
                    break;
                }
                //if (rowCount++ > 100) { return;  }

                DataRow dr = rval.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    //try
                    {
                        object val = null;
                        Type colType = pi.PropertyType;
                        if (!dbTypes.Contains(colType))
                        {
                            if (colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            {
                                val = pi.GetValue(rec, null);
                            }
                            else
                            {
                                val = pi.PropertyType.ToString();
                            }
                        }
                        else
                        {
                            try
                            {
                                val = pi.GetValue(rec, null);
                            }
                            catch (Exception e)
                            {
                                val = "Error: " + e.Message;
                            }
                            //if (val.
                            //val = "x";
                        }
                        if (val == null) { val = DBNull.Value; }


                        //String was not recognized as a valid Boolean.Couldn't store  in AllowContentTypes Column.  Expected type is Boolean. ---> System.FormatException: String was not recognized as a valid Boolean.   
                        //String must be exactly one character long.]

                        dr[pi.Name] = val;
                        //try
                        //{
                        //    dr[pi.Name] = val;
                        //}
                        //catch (Exception e)
                        //{
                        //    //dr[pi.Name] = DBNull.Value;
                        //}
                    }
                    //catch (Exception e)
                    //{
                    //    dr[pi.Name] = "PropertyType=" + pi.PropertyType + "<pre>" + e.ToString() + "</pre>";
                    //}
                    //finally { }
                }
                rval.Rows.Add(dr);
            }
            return rval;
        }
        public delegate object[] CreateRowDelegate<T>(T t);
        //*******************************************************************
        public static DataTable FromDoubleTupleCollection(NameValueCollection nvs)
        {
            //DataTableX x;
            DataTable dt = new DataTable();
            foreach (string key in nvs.Keys)
            {
                dt.Columns.Add(new DataColumn(key)); //, typeof(string)));
            }
            DataTableX.AddRow(dt, nvs);
            return dt;
        }
        public static DataTable ValueOf(NameObjectCollection nvs)
        {
            DataTable dt = new DataTable();
            foreach (string key in nvs.Keys)
            {
                object o = nvs[key];
                DataColumn dc = new DataColumn(key, o.GetType());
                dt.Columns.Add(dc);
            }
            AddRow(dt, nvs);
            return dt;
        }

        public static DataTable FromDoubleTupleCollection(NameValueCollection nvs, string keyColumnName, String valueColumnName)
        {
            //DataTableX x;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(keyColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(valueColumnName, typeof(string)));


            for (int i = 0; i < nvs.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = nvs.GetKey(i);
                dr[1] = nvs.Get(i);
                dt.Rows.Add(dr);
            }
            return dt;
        }

        //<ItemTemplate>
        //   <%# DataBinder.Eval(Container.DataItem, "[0]")%>
        //</ItemTemplate>
        public static DataTable FromSingleTupleList(IEnumerable en)
        {
            //DataTableX x;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn());

            foreach (object o in en)
            {
                DataRow dr = dt.NewRow();
                dr[0] = o;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static DataTable FromSingleTupleList(IEnumerable en, string columnName)
        {
            //DataTableX x;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(columnName, typeof(object)));

            foreach (object o in en)
            {
                DataRow dr = dt.NewRow();
                dr[0] = o;
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public static string GetInfo(DataTable dt)
        {
            throw new NotImplementedException();
        }
        public static void Copy_eg()
        {
            DataTable dt = null;
            dt = dt.Copy();
        }
        //========================================================================
        public static DataTable TurnSideway(DataTable dt)
        {
            DataTable newTable = new DataTable();
            int oldColCount = dt.Columns.Count;
            int newColCount = dt.Rows.Count;

            newTable.Columns.Add("ColumnName");
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                newTable.Columns.Add(Convert.ToString(i));
            }
            for (int k = 0; k < oldColCount; k++)
            {
                DataRow r = newTable.NewRow();
                r[0] = dt.Columns[k].ToString();
                for (int j = 1; j <= newColCount; j++)
                {
                    r[j] = dt.Rows[j - 1][k];
                }
                newTable.Rows.Add(r);
            }
            return newTable;
        }

        ////=================================================================================================
        //public static DataTable ValueOf(string sql, string tableName = null)
        //{
        //    var dbc = new DbConnectionX();
        //    using (var conn = dbc.Open())
        //    {
        //        var rval = ValueOf(conn, sql, tableName);
        //        return rval;
        //    }
        //}
        ////=================================================================================================
        //public static DataTable ValueOf(DbConnectionX conn, string sql, string tableName = null)
        //{
        //    var rval = ValueOf(conn.Connection, sql, tableName);
        //    return rval;
        //}

        //=================================================================================================
        public static DataTable QueryData(DbConnection conn, string sql)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            DataTable dt = new DataTable();
            //var sqlLog = new SqlLog(log, "Running sql to return DataTable", sql);
            bool runOK = false;
            try
            {
                var cmd = conn.CreateCommand();
                //SqlCommand cmd = new SqlCommand(sql, db);
                cmd.CommandText = sql;
                //log.Debug("sql=" + sql);
                var dr = cmd.ExecuteReader(); //CommandBehavior.CloseConnection);
                //dr.Read();
                //var c0 = dr[0];

                if (dr.HasRows)
                {
                    dt.Load(dr);
                }
                //SqlDataAdapter dap = new SqlDataAdapter(myQuery, con);
                //DataTable dt = new DataTable();
                //dap.Fill(dt);
                //if (tableName != null)
                //{
                //    dt.TableName = tableName;
                //}
                runOK = true;
            }
            finally
            {
                //db.Close();
                if (!runOK)
                {
                    MOutput.WriteHtmlEncodedPre(sql);
                }
                //sqlLog.Flush(runOK);
            }
            return dt;
        }
        //=================================================================================================
        public static int GetCount(DbConnection conn, string tabName)
        {
            string sql = "select count(*) from " + tabName;
            int rval = 0;
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rval = (int)rdr[0];
                    }
                    else
                    {
                        throw new Exception("Something is worng");
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
        //===================================================================================================
        public static DataRow GetOneRow(String tableName, string keyName, object keyValue)
        {
            //if (keyValue == null)
            //{
            //    keyValue = "max";
            //}

            if (string.IsNullOrWhiteSpace(keyName))
            {
                //keyName = DataTableX.GetPrimaryKeyNames(tableName).FirstOrDefault();
                throw new Exception("keyName can't be null");
            }
            //TODO: var sqlLog = new SqlLog(log, "Getting One Row");
            string sql;
            //if (keyValue == "max") // DbConstants.MaxValue)
            //{
            //    sql = string.Format("select top 1 * from {0} order by {1} desc", tableName, keyName);
            //}
            //else if (keyValue == "min") //DbConstants.MinValue)
            //{
            //    sql = string.Format("select top 1 * from {0} order by {1} asc", tableName, keyName);
            //}
            //else
            {
                sql = string.Format("select top 1 * from {0} where {1}={2}", tableName, keyName, keyValue);
            }

            //sqlLog.SQL = sql;

            DataTable dt = null;// QueryData(sql);

            //sqlLog.ResultCount = dt.Rows.Count;
            //sqlLog.Flush();

            DataRow dr = null;
            if (dt.Rows.Count > 0)
            {
                dr = dt.Rows[0];
            }
            return dr;
            //return dt;
        }

        public static void GetFirstNRows(DataTable dt, int p)
        {
            //DataTable dtPage = dt.Rows.Cast<System.Data.DataRow>().Skip((pageNum - 1) * pageSize).Take(pageSize).CopyToDataTable();
        }
    }
}
