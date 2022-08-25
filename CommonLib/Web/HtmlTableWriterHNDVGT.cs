using System.Linq;


namespace Libx
{
    public class HtmlTableWriterHNDVGT
    {
        //tableStyle: "wg1-table",
        //        headerStyle: "wg1-header",
        //        rowStyle: "wg1-row1",
        //        alternatingRowStyle: "wg1-altRow",
        //        selectedRowStyle: "wg1-selectedRow"

        public StringBuilder buf = new StringBuilder();
        public String TableStyleInline = "border-collapse:collapse; border-spacing:0";
        public string TableStyle = "wg1-table";
        public string THeadTRStyle = "wg1-header";
        public string RowStyle = "wg1-row1";
        public string AlternatingRowStyle = "wg1-altRow";

        public string TDStyle = "wg1-td";
        public string THStyle = "wg1-th";

        public string TableAttributes = "border='0'";
        public string TRAttributes = "valign='top'";
        public string TDAttributes = ""; //"style='padding:3px'";

        public void ClearStyles()
        {
            TableStyleInline = null;
            TableStyle = null;
            THeadTRStyle = null;
            RowStyle = null;
            AlternatingRowStyle = null;
            TDStyle = null;
            THStyle = null;

            TableAttributes = null;
            TRAttributes = null;
            TDAttributes = null;
        }
        //===============================================================================
        public HtmlTableWriterHNDVGT()
        {
        }
        ////===============================================================================
        //public HtmlTableWriter(StringBuilder sb)
        //{
        //    this.buf = sb;
        //}
        public bool TableStarted = false;
        //===============================================================================
        public void StartTable()
        {
            if (!TableStarted)
            {
                //buf.AppendLine("<div class='wg1'>");
                string startTag = String.Format("<table {0} style='{1}' class='{2}'>", TableAttributes, TableStyleInline, TableStyle);
                buf.Append(startTag);
                TableStarted = true;
            }
            //return this;
        }
        //===================================================================================================
        public void AddHeaderRow(params String[] values)
        {
            var lst = values.ToList();
            AddHeaderRow(lst);
        }
        //===================================================================================================
        public void AddHeaderRow(IList<String> values)
        {
            if (!TableStarted) { StartTable(); }

            buf.AppendLine("<thead>");
            buf.AppendFormat("<tr class='{0}'>\n", THeadTRStyle);
            foreach (String val in values)
            {
                buf.AppendFormat("<th scope='col'>{0}</th>\n", val);
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
        public bool endTR = true;
        public bool startTR = true;
        public void Reset()
        {
            endTR = true;
            startTR = true;
        }
        public void AddRow_NValues(IList<String> values)
        {
            if (!TableStarted) { StartTable(); }

            currentRowIndex++;
            string curRowStyle = RowStyle;
            if (currentRowIndex % 2 == 1)
            {
                curRowStyle = AlternatingRowStyle;
            }
            if (startTR)
            {
                buf.AppendFormat("<tr {0} class='{1}'>\n", TRAttributes, curRowStyle);
            }
            foreach (String val in values)
            {
                buf.AppendFormat("<td {0} class='{1}'>{2}</td>\n", TDAttributes, TDStyle, val);
            }
            if (endTR)
            {
                buf.AppendLine("</tr>");
            }
        }

        //===============================================================================
        public string EndTable()
        {
            if (TableStarted)
            {
                buf.AppendLine("</table>");
                TableStarted = false;
            }
            //buf.AppendLine("</div>");
            return buf.ToString();
        }

    }
}