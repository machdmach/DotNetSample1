using System.Linq;


namespace Libx
{
    public class HtmlTableWriter2
    {
        //tableStyle: "wg1-table",
        //        headerStyle: "wg1-header",
        //        rowStyle: "wg1-row1",
        //        alternatingRowStyle: "wg1-altRow",
        //        selectedRowStyle: "wg1-selectedRow"

        public StringBuilder buf = new StringBuilder();

        public string THeadTRStyleClasses;

        public string TableStyleClasses;
        public string RowStyleClasses;
        public string AlternatingRowStyleClasses;
        public string THStyleClasses;
        public string TDStyleClasses;

        public String TableStyleInline;
        public string THStyleInline;
        public string TDStyleInline;

        public string TableAttributes;
        public string TRAttributes;
        public string TDAttributes;

        public void SetDefaultOptions()
        {
            TableStyleInline = null;
            TableStyleClasses = null;
            THeadTRStyleClasses = null;
            RowStyleClasses = null;
            AlternatingRowStyleClasses = null;
            TDStyleClasses = null;
            THStyleClasses = null;

            TableAttributes = null;
            TRAttributes = null;
            TDAttributes = null;
        }
        public void ClearOptions()
        {
            TableStyleInline = "border-collapse:collapse; border-spacing:0";
            TableStyleClasses = "wg1-table";
            THeadTRStyleClasses = "wg1-header";
            RowStyleClasses = "wg1-row1";
            AlternatingRowStyleClasses = "wg1-altRow";

            TDStyleClasses = "wg1-td";
            THStyleClasses = "wg1-th";

            TDStyleInline = "";
            THStyleInline = "";

            TableAttributes = "border='0'";
            TRAttributes = "valign='top'";
            TDAttributes = ""; //"style='padding:3px'";
        }

        //===============================================================================
        public HtmlTableWriter2()
        {
        }
        //===============================================================================
        public HtmlTableWriter2(StringBuilder sb)
        {
            buf = sb;
        }
        public bool TableStarted = false;
        //===============================================================================
        public HtmlTableWriter2 StartTable()
        {
            if (!TableStarted)
            {
                //string startTag = String.Format("<table {0} style='{1}' class='{2}'>", TableAttributes, TableStyleInline, TableStyleClasses);
                buf.Append("<table ");
                buf.Append(TableAttributes);
                if (!string.IsNullOrEmpty(TableStyleInline))
                {
                    buf.AppendFormat(" style='{0}'", TableStyleInline);
                }
                if (!string.IsNullOrEmpty(TableStyleClasses))
                {
                    buf.AppendFormat(" class='{0}'", TableStyleClasses);
                }

                buf.AppendLine(">");
                TableStarted = true;
            }
            return this;
        }
        //===================================================================================================
        public void AddHeaderRow(params String[] values)
        {
            var lst = values.ToList();
            AddHeaderRow(lst);
        }
        //===================================================================================================
        public int ColumnCount;
        public void AddHeaderRow(IList<String> values)
        {
            if (!TableStarted) { StartTable(); }

            buf.AppendLine("<thead>");
            if (!string.IsNullOrEmpty(THeadTRStyleClasses))
            {
                buf.AppendFormat("<tr class='{0}'>\n", THeadTRStyleClasses);
            }
            ColumnCount = 0;
            foreach (String val in values)
            {
                buf.AppendFormat("<th scope='col'>{0}</th>\n", val);
                ColumnCount++;
            }
            buf.AppendLine("</tr>");
            buf.AppendLine("</thead>");
        }

        //===================================================================================================
        public void AddRow(string s1, object obj)
        {
            string s2 = (obj == null) ? "" : obj.ToString();
            AddRow_NValues(new List<string> { s1, s2 });
        }
        public int currentRowIndex = -1;
        public void AddRow_NValues(params String[] values)
        {
            var lst = values.ToList();
            AddRow_NValues(lst);
        }
        //===================================================================================================
        public void AddRow_NValues(IList<String> values)
        {
            if (!TableStarted) { StartTable(); }

            currentRowIndex++;
            string curRowStyleClasses = RowStyleClasses;
            if (currentRowIndex % 2 == 1)
            {
                curRowStyleClasses = AlternatingRowStyleClasses;
            }
            buf.Append("<tr ");
            buf.Append(TRAttributes);

            if (!string.IsNullOrEmpty(TableStyleInline))
            {
                buf.AppendFormat(" class='{0}'", curRowStyleClasses);
            }
            buf.AppendLine(">");

            foreach (String val in values)
            {
                //buf.AppendFormat("<td {0} class='{1}' style='{2}'>{3}</td>\n", TDAttributes, TDStyleClasses, TDStyleInline, val);
                buf.Append("<td ");
                buf.Append(TDAttributes);

                if (!string.IsNullOrEmpty(TDStyleInline))
                {
                    buf.AppendFormat(" class='{0}'", TDStyleClasses);
                }
                if (!string.IsNullOrEmpty(TDStyleInline))
                {
                    buf.AppendFormat(" style='{0}'", TDStyleInline);
                }
                buf.Append(">");
                buf.Append(val);
                buf.AppendLine("</td>\n");
            }
            buf.AppendLine("</tr>");
        }

        //===============================================================================
        public string EndTable()
        {
            buf.AppendLine("</table>");
            TableStarted = false;
            //buf.AppendLine("</div>");
            return buf.ToString();
        }

    }
}