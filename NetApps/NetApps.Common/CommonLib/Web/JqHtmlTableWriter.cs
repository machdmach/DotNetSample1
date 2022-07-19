using System.Linq;

namespace System
{
    public class JqHtmlTableWriter
    {
#if docs1
#endif
        /*
#actual-table { border-collapse: collapse; }
#actual-table td {
  width: 20%;
  padding: 10px;
  vertical-align: top;
}
#actual-table td:nth-child(even) {
  background: #ccc;
}
#actual-table td:nth-child(odd) {
  background: #eee;
}         
 */
        //public string TDClassName;   //td
        //public string THClassName;   //th


        //tableStyle: "wg1-table",
        //        headerStyle: "wg1-header",
        //        rowStyle: "wg1-row1",
        //        alternatingRowStyle: "wg1-altRow",
        //        selectedRowStyle: "wg1-selectedRow"

        public StringBuilder buf; // = new StringBuilder();

        public JqHtmlElement THead;// = new JqHtmlElement("th");
        public JqHtmlElement HeadTR;// = new JqHtmlElement("tr");

        public JqHtmlElement TABLE;// = new JqHtmlElement("table");
        public JqHtmlElement TR;// = new JqHtmlElement("tr");

        public JqHtmlElement TRAlt;// = new JqHtmlElement("tr");
        public JqHtmlElement TH;// = new JqHtmlElement("th");
        public JqHtmlElement TD;// = new JqHtmlElement("td");
        public List<JqHtmlElement> ColTDList = new List<JqHtmlElement>();// = new JqHtmlElement("td");

        public List<JqHtmlElement> ColHeaders = new List<JqHtmlElement>();
        //public List<String> ColHeaders = new List<string>();
        public int ColHeadersBufIndex = 0;

        public void SetDefaultOptions()
        {
        }
        public void ClearOptions()
        {
            //TableStyleInline = "border-collapse:collapse; border-spacing:0";
            //TableStyleClasses = "wg1-table";
            //THeadTRStyleClasses = "wg1-header";
            //RowStyleClasses = "wg1-row1";
            //AlternatingRowStyleClasses = "wg1-altRow";

            //TDStyleClasses = "wg1-td";
            //THStyleClasses = "wg1-th";

            //TDStyleInline = "";
            //THStyleInline = "";

            //TableAttributes = "border='0'";
            //TRAttributes = "valign='top'";
            //TDAttributes = ""; //"style='padding:3px'";
        }

        public static int TabIDCounter = 100;
        public string TabID => TABLE.ID;
        //===============================================================================
        public JqHtmlTableWriter()
        {
            buf = new StringBuilder();
            Init();
        }
        public JqHtmlTableWriter(StringBuilder sb)
        {
            buf = sb;
            Init();
        }
        protected virtual void Init()
        {
            //TabID = "jqTab" + TabIDCounter++;

            THead = new JqHtmlElement("thead");
            TABLE = new JqHtmlElement("table") { ID = "jqHtmlTab" + TabIDCounter++ };
            TR = new JqHtmlElement("tr");
            HeadTR = new JqHtmlElement("tr") { ClassName = "trHead" };
            TRAlt = new JqHtmlElement("tr") { ClassName = "trAlt" };
            TH = new JqHtmlElement("th");
            TD = new JqHtmlElement("td");

            TR.AddClass("hover-hilite");
            TRAlt.AddClass("hover-hilite");

            //TD.StartTagPrefix = " ";
        }

        //public String DataRowStart = "<div class='row rgRow'>";
        //public String DataRowAltStart = "<div class='row rgRowAlt'>";
        //public String DataCellStart = "<div class='{0}'>";

        public bool TableStarted = false;
        //===============================================================================
        public virtual JqHtmlTableWriter StartTable(string tableCaption = null)
        {
            if (!TableStarted)
            {
                buf.AppendFormat("<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~ {0} Start -->\n", TabID);
                TABLE.BuildStartTag(buf);
                buf.AppendLine();

                if (tableCaption != null)
                {
                    buf.AppendFormat("<caption>{0}</caption>", tableCaption);
                    buf.AppendLine();
                }

                ColHeadersBufIndex = buf.Length;
                TableStarted = true;
            }
            return this;
        }

        //===============================================================================
        public virtual void BuildColHeaders()
        {
            if (ColHeaders.Count > 0)
            {
                var sb = new StringBuilder();
                //THead.BuildStartTag(sb);
                //sb.AppendLine();

                HeadTR.BuildStartTag(sb);
                sb.AppendLine();
                foreach (var headElt in ColHeaders)
                {
                    sb.Append("  ");
                    headElt.BuildStartTag(sb);
                    sb.Append(headElt.InnerHtml);
                    headElt.BuildEndTag(sb);
                }
                HeadTR.BuildEndTag(sb);
                //THead.BuildEndTag(sb);
                buf.Insert(ColHeadersBufIndex, sb.ToString());
            }
        }
        //===============================================================================
        public virtual string EndTable()
        {
            if (TableStarted)
            {
                //buf.AppendLine("</table>");
                TABLE.BuildEndTag(buf);
                buf.AppendFormat("<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~ {0} End-->\n", TabID);
                BuildColHeaders();
                TableStarted = false;
            }
            return buf.ToString();
        }
        //===================================================================================================
        public virtual void AddHeaderRow(params String[] values)
        {
            var lst = values.ToList();
            AddHeaderRow(lst);
        }
        //===================================================================================================
        public virtual void AddRow2(JqHtmlElement td1, string s1, JqHtmlElement td2, object obj)
        {
            //string s2 = (obj == null) ? "" : obj.ToString();
            //AddRow_NValues(new List<string> { s1, s2 });
            StartRow();
            AddCell(td1, s1);
            AddCell(td2, obj);
            EndRow();
        }

        //===================================================================================================
        public virtual void AddRow(string s1, object obj)
        {
            //string s2 = (obj == null) ? "" : obj.ToString();
            AddRow_NValues(new List<object> { s1, obj });
        }
        public int currRowNumber = 1;
        public virtual void AddRow_NValues(params Object[] values)
        {
            var lst = values.ToList();
            AddRow_NValues(lst);
        }
        //===================================================================================================
        public virtual JqHtmlElement GetCurrentTR()
        {
            var rval = (currRowNumber % 2 == 0) ? TRAlt : TR;
            return rval;
        }
        //===================================================================================================
        public int ColumnCount;
        public virtual void AddHeaderRow(IList<String> values)
        {
            StartHeaderRow();
            //ColumnCount = 0;            
            foreach (String val in values)
            {
                //ColumnCount++;
                AddHeaderCell(TH, val);
            }
            EndHeaderRow();
        }
        //===================================================================================================
        public virtual void AddRow_NValues(IList<Object> values)
        {
            StartRow();
            int i = 0;
            foreach (object val in values)
            {
                var td = (ColTDList.Count > i) ? ColTDList[i] : TD;
                AddCell(td, val);
                i++;
            }
            EndRow();
        }
        //===================================================================================================
        public virtual void AddHeaderCell(JqHtmlElement elt, string val)
        {
            if (!RowStarted) { StartHeaderRow(); }
            buf.Append("  ");
            elt.BuildStartTag(buf);
            buf.Append(val);
            elt.BuildEndTag(buf);
            ColumnCount++;
        }
        //===================================================================================================
        public virtual void AddCell(JqHtmlElement elt, string val)
        {
            if (!RowStarted) { StartRow(); }
            if (currRowNumber == 1)
            {
                if (elt.ParentColumn != null)
                    ColHeaders.Add(elt.ParentColumn);
            }
            buf.Append("  ");
            elt.BuildStartTag(buf);
            buf.Append(val);
            elt.BuildEndTag(buf);
        }

        //===================================================================================================
        public virtual void AddCell(JqHtmlElement elt, object val)
        {
            string s = val as string;
            if (s == null)
            {
                s = val?.ToString();
            }
            AddCell(elt, s);
        }
        //===================================================================================================
        public virtual void AddCell(JqHtmlElement elt, Boolean? val)
        {
            string s;
            if (val.HasValue)
            {
                s = val.Value ? "Yes" : "";
            }
            else
            {
                s = "";
            }
            AddCell(elt, s);
        }

        //===================================================================================================
        public bool RowStarted = false;
        public virtual void StartRow(JqHtmlElement tr = null)
        {
            if (!RowStarted)
            {
                if (!TableStarted) { StartTable(); }
                if (tr == null)
                {
                    tr = GetCurrentTR();
                }
                tr.BuildStartTag(buf);
                buf.AppendLine();
                RowStarted = true;
            }
        }
        //===================================================================================================
        public virtual void StartHeaderRow()
        {
            if (!RowStarted)
            {
                if (!TableStarted) { StartTable(); }
                if (THead != null)
                {
                    THead.BuildStartTag(buf);
                }
                HeadTR.BuildStartTag(buf);
                ColumnCount = 0;
                RowStarted = true;
            }
        }
        //===================================================================================================
        public virtual void EndRow()
        {
            //var tr = GetCurrentTR();
            TR.BuildEndTag(buf);
            RowStarted = false;
            currRowNumber++;
        }
        //===================================================================================================
        public virtual void EndHeaderRow()
        {
            HeadTR.BuildEndTag(buf);
            if (THead != null)
            {
                THead.BuildEndTag(buf);
            }
            RowStarted = false;
            //currRowIndex++;
        }

        //===================================================================================================
        public string StyleSheetContainer;
        public virtual void FlushStyleSheet(string extraStyles = null)
        {
            //sharedCssStr += string.Format("#{0} > div:hover {{background-color:lightyellow;}}", tw.TabID);

            var tw = this;
            string tabID = tw.TabID;
            //tw.TABLE.Attr("id", tabID);

            // #tab123 td { color:red;}
            buf.AppendLine();
            buf.AppendLine("<style type='text/css'>");

            string ssContainer = StyleSheetContainer;
            if (ssContainer == null)
            {
                ssContainer = tabID;
            }
            //buf.AppendFormat("#{0} ", tableID);
            tw.TRAlt.InlineCssToStyleSheet(buf, ssContainer);
            tw.TR.InlineCssToStyleSheet(buf, ssContainer);

            //buf.AppendFormat("#{0} ", tableID);
            tw.TH.InlineCssToStyleSheet(buf, ssContainer);
            tw.TD.InlineCssToStyleSheet(buf, ssContainer);

            if (extraStyles != null)
            {
                extraStyles = extraStyles.TrimEnd();
                buf.AppendLine(extraStyles);
            }

            buf.AppendLine("</style>");
        }

        //===================================================================================================
        public static void Eg1(JqHtmlTableWriter tw)
        {
            //var tw = new JqHtmlTableWriter();

            tw.TABLE.Css("margin", "1px")
                    //.Css("border-collapse", "collapse")
                    //.Css("font-weight", "bold")
                    //.Css("border", "1px solid #ccc")
                    .Nada();

            tw.HeadTR.Css("background-color", "#c3d3d3").Css("font-weight", "bold");

            //tw.TRAlt.Css("background-color", "lightgray");

            tw.TR//.Attr("tr1", "11")
                .Css("background-color", "#eee")
                 //.Css("padding", "11px")
                 //.Css("border", "1px solid #ccc")
                 .Nada();
            tw.TRAlt.Css("background-color", "#dedede");

            //table>tbody>

            tw.TD//.Attr("valign", "top")
                .Css("padding", "5px")
                .Css("border", "1px solid #ccc")
                .Nada();

            tw.TH.Css(tw.TD);

            StringBuilder css = new StringBuilder();
            css.AppendFormat("#{0} td:nth-child(2) {{font-weight: bolder;}}", tw.TabID);
            tw.FlushStyleSheet(css.ToString());

            tw.AddHeaderRow("h1", "h2", "h3");
            for (int i = 0; i < 2; i++)
            {
                tw.AddRow_NValues("aa", new string('b', 20), "c");
            }
            MOutput.Write(tw.EndTable());
            MOutput.WriteHtmlEncodedPre(tw.EndTable());

        }
        //===================================================================================================
        public static void Tests()
        {
            Eg1(new JqHtmlTableWriter());

            var buf = new StringBuilder();
            var tw = new JqHtmlTableWriter();


            //tw.TABLE.StyleCol = new NameValueCollection();
            //tw.TABLE.ClassCol = new List<string>();
            //tw.TABLE.AttributeCol = new NameValueCollection();

            tw.TABLE.Css("margin", "20px")
                     .Css("c", 11)
                     .AddClass("class1")
                     .AddClass("Class2")
                     .Css("color", "green")
                     .Attr("border", 1)
                     .Css("padding", "11px")
                     ;
            tw.TABLE.ClassNamesStr = "c1 c2 c3";

            tw.TR.Attr("tr1", "11")
                 .Css("padding", "11px")
                ;

            tw.TRAlt.Css("background-color", "lightgray")
                    .Attr(tw.TR)
                    .Css(tw.TR)
                    ;


            tw.TD.Attr("xx", "11")
                //Css("color", "red")
                .Css("padding", "3px")
                ;

            for (int i = 0; i < 5; i++)
            {
                tw.AddRow_NValues("aa", "b", "c");
            }
            MOutput.Write(tw.EndTable());
            MOutput.WriteHtmlEncodedPre(tw.EndTable());
        }


    }
}