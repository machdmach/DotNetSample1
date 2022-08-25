using System.Linq;
using System.Net;

namespace Libx
{
    //===================================================================================================
    public class ResponsiveGridWriter
    {
        public static void Test1()
        {
            var g = new ResponsiveGridWriter();
            g.buf.Append("<div class='container'>");
            g.AddHeaderRow(new string[] { "c1", "c2" });
            g.AddRow(new string[] { "d1", "d2" });
            g.AddRow(new string[] { "2d1", "2d2" });
            g.buf.Append("</div>");
            string s = g.GetGridHtml();
            MOutput.WriteLine(s);
            MOutput.WriteHtmlEncodedPre(s);
        }

        //public class Options
        //{
        //    //public bool ShowCaption { get; set; } = false;
        //    //public bool ShowColumnHeaders { get; set; } = true;
        //}
        //public Options options;

        public bool[] ColumnSortable { get; set; }
        public string[] CellClassesArr;
        //public void Init(int colCount)
        //{
        //    //if (CellClassesArr == null)
        //    //{
        //    //    CellClassesArr = new List<string>(colCount);
        //    //    for (int i = 0; i < colCount; i++)
        //    //    {
        //    //        string cellClasses = "col-sm-2";
        //    //        CellClassesArr.Add(cellClasses);
        //    //    }
        //    //}
        //}
        public string CellClasses = "col-sm-3 rgCell";
        public bool HtmlEncodeData = false;
        public int dataRowCount = 0;
        public StringBuilder buf = new StringBuilder();
        public string GetGridHtml()
        {
            if (dataRowCount > 0)
            {
                //buf.AppendLine("</div>");  //end divData
            }
            return buf.ToString();
        }

        //===============================================================================
        public ResponsiveGridWriter()
        {
        }
        //===============================================================================
        public ResponsiveGridWriter(StringBuilder sb)
        {
            buf = sb;
        }
        //===============================================================================
        public ResponsiveGridWriter StartTable()
        {
            //buf.AppendLine("<div class='wg1'>");
            //string startTag = String.Format("<table {0} style='{1}' class='{2}'>", TableAttributes, TableStyleInline, TableStyle);
            //buf.Append(startTag);
            return this;
        }


        //===================================================================================================
        public string GetCellClasses(int colIndex)
        {
            string ret = CellClasses;
            if (CellClassesArr != null)
            {
                ret = CellClassesArr[colIndex];
            }
            return ret;
        }
        //public String HeaderRowStart = "<article class='container-fly-row-header'> <div class='container'>";
        //public String HeaderRowEnd = "</div></article>";
        //public String HeaderCellStart = "<p class='{0}'>";
        //public String HeaderCellEnd = "</p>";

        public String HeaderRowStart = "<div class='row rgRowHeader'>";
        public String HeaderCellStart = "<div class='{0}'>";
        public String HeaderCellEnd = "</div>";
        public String HeaderRowEnd = "</div>";

        public void UseTemplate2()
        {
            HeaderRowStart = "<div class='row'>";
            HeaderCellStart = "<div class='{0}'>";
            HeaderCellEnd = "</div>";
            HeaderRowEnd = "</div>";
            //-------
            DataRowStart = "<div class='row'>";

            DataRowAltStart = "<div class='row'>";
            DataCellStart = "<div class='{0}'>";
            DataCellEnd = "</div>";
            DataRowEnd = "</div>";
        }
        //===================================================================================================
        public void AddHeaderRow(params String[] values)
        {
            var lst = values.ToList();
            AddHeaderRow(lst);
        }
        public string ColumnHeaderOnclick = "return rg1.columnHeaderClicked(this);'";

        //===================================================================================================
        private int colHeaderCount = 0;
        public void StartHeaderRow()
        {
            buf.AppendLine(HeaderRowStart);
            colHeaderCount = 0;
        }
        //===================================================================================================
        public void EndHeaderRow()
        {
            buf.AppendLine(HeaderRowEnd);
        }
        //===================================================================================================
        public void AddColumnHeader(string colHeader, bool isSortable = true)
        {
            var thText = colHeader;
            var cellClasses = GetCellClasses(colHeaderCount);

            //if (ColumnSortable == null || ColumnSortable[i])
            if (isSortable)
            {
                //th = string.Format("<p class='col-xs-6 col-sm-2'><a href='#' onclick='return bsColHeaderClicked(this);' zid='bs{0}'> {0} </a> </p>", header);
                //th = string.Format("<p class='{1}'><a href='#' onclick='return bsColHeaderClicked(this);' zid='bs{0}'> {0} </a> </p>", thText, cellClasses[i]);
                var id = "rgCol" + colHeaderCount + "_" + Environment.TickCount;
                thText = string.Format("<a href='#' onclick='{1}' id='{2}'> {0} </a>", thText, ColumnHeaderOnclick, id);
            }
            buf.AppendFormat(HeaderCellStart, cellClasses);
            buf.Append(thText);
            buf.AppendLine(HeaderCellEnd);
            colHeaderCount++;
        }

        //===================================================================================================
        public void AddHeaderRow(IList<String> values)
        {
            //Init(headers.Count());

            StartHeaderRow();
            for (int i = 0; i < values.Count; i++)
            {
                var thText = values[i];
                var cellClasses = GetCellClasses(i);

                //string th;
                //if (ColumnSortable == null || ColumnSortable[i])
                if (ColumnSortable != null && ColumnSortable[i])
                {
                    //th = string.Format("<p class='col-xs-6 col-sm-2'><a href='#' onclick='return bsColHeaderClicked(this);' zid='bs{0}'> {0} </a> </p>", header);
                    //th = string.Format("<p class='{1}'><a href='#' onclick='return bsColHeaderClicked(this);' zid='bs{0}'> {0} </a> </p>", thText, cellClasses[i]);
                    var id = "rgCol" + i + "_" + Environment.TickCount;
                    thText = string.Format("<a href='#' onclick='{1}' id='{2}'> {0} </a>", thText, ColumnHeaderOnclick, id);
                }
                else
                {
                    //throw new Exception("axdfde");
                    //thText = string.Format("<a href='#' onclick='{1}' id='{2}'> {0} </a>", thText, ColumnHeaderOnclick, id);
                }
                //buf.AppendLine(th);

                buf.AppendFormat(HeaderCellStart, cellClasses);
                buf.Append(thText);
                buf.AppendLine(HeaderCellEnd);

            }
            EndHeaderRow();

            var ret = buf.ToString();
        }

        //===================================================================================================
        public void AddDataRows(IList<IList<String>> dataRows)
        {
            for (int i = 0; i < dataRows.Count; i++)
            {
                var dataRow = dataRows[i];
                AddRow(dataRow);
            }
        }
        //public String DataRowStart = "<article class='container-fly-row'> <div class='container'>";
        //public String DataRowAltStart = "<article class='container-fly-row-alt'> <div class='container'>";
        //public String DataRowEnd = "</div></article><br class='clear'/>";
        //public String DataCellStart = "<p class='{0}'>";
        //public String DataCellEnd = "</p>";

        public String DataRowStart = "<div class='row rgRow'>";

        public String DataRowAltStart = "<div class='row rgRowAlt'>";
        public String DataCellStart = "<div class='{0}'>";
        public String DataCellEnd = "</div>";
        public String DataRowEnd = "</div>";

        //===================================================================================================
        public void AddRow(params String[] values)
        {
            var lst = values.ToList();
            AddRow(lst);
        }
        public void AddCells(params String[] values)
        {
            var lst = values.ToList();
            AddRow(lst, false);
        }
        public string AddRow(IList<String> values, bool withRowStartAndEnd = true)
        {
            //if (dataRowCount == 0)
            //{
            //    buf.AppendLine("<div class='divData'>");
            //}
            //Init(headers.Count());
            // &#124; = |

            if (withRowStartAndEnd)
            {
                if (dataRowCount % 2 == 0)
                {
                    buf.AppendLine(DataRowStart);
                }
                else
                {
                    buf.AppendLine(DataRowAltStart);
                }
            }
            for (int i = 0; i < values.Count; i++)
            {
                var data = values[i];
                if (HtmlEncodeData)
                {
                    data = WebUtility.HtmlEncode(data);
                }
                var cellClasses = GetCellClasses(i);
                //buf.AppendFormat("<p class='col-xs-6 col-sm-2'>{0}</p>\n", data);
                buf.AppendFormat(DataCellStart, cellClasses);
                buf.Append(data);
                buf.AppendLine(DataCellEnd);
            }
            if (withRowStartAndEnd)
            {
                buf.AppendLine(DataRowEnd);
            }
            dataRowCount++;
            return buf.ToString();
        }
        //===================================================================================================
    }
}


