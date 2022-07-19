namespace System.Data
{
    public class SqlSelectStatementBuilder2
    {
        public int currIndex = 0;
        //protected internal ArrayList colNames = ArrayList.Synchronized(new ArrayList(10));
        protected internal List<string> colNames = new List<String>();

        public string FromClause { get; set; }
        public string WhereClause { get; set; }

        public int Select(String col, bool flag = true)
        {
            if (flag)
            {
                colNames.Add(col);
                return colNames.Count - 1;
                //currIndex++;
                //return currIndex;
            }
            return -1;
        }

        public string BuildSelectStmt()
        {
            var buf = new StringBuilder();
            buf.Append("select");
            var colNames = this.colNames;

            for (int i = 0; i < colNames.Count; i++)
            {
                buf.Append("   ");
                //if (i != 0)  //comma first
                //{
                //    buf.Append(",");
                //}
                //else
                //{
                //    buf.Append(",");
                //}
                string colName = colNames[i];
                colName = SqlReservedKeywords2.SafeGuardToken(colName);

                if (i < (colNames.Count - 1)) //comma at the end
                {
                    colName = colName + ",";
                }
                buf.AppendLine(colName);
            }
            return buf.ToString();
        }
    }

}