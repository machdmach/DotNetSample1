namespace System
{
    public class JqResponsiveGridWriter : JqHtmlTableWriter
    {
        //https://github.com/liabru/jquery-match-height
        //https://css-tricks.com/fluid-width-equal-height-columns/

        //===============================================================================
        public JqResponsiveGridWriter() { }
        public JqResponsiveGridWriter(StringBuilder sb) : base(sb) { }
        //Tobe called by base class
        protected override void Init()
        {
            //THead = null;

            TABLE = new JqHtmlElement("div") { ID = "jqHtmlGrid" + TabIDCounter++ };
            TR = new JqHtmlElement("div")
            {
                ClassName = "trx"
            };
            TR.AddClass("row");

            HeadTR = TR.CloneObj();

            TRAlt = new JqHtmlElement(TR)
            {
                ClassName = "trAlt"
            };

            TR.AddClass("hover-hilite");
            TRAlt.AddClass("hover-hilite");

            TD = new JqHtmlElement("div")
            {
                ClassName = "tdx"
            };

            TH = new JqHtmlElement(TD)
            {
                ClassName = "thx"
            };
        }
        ////===================================================================================================
        //public override void StartHeaderRow()
        //{
        //    base.StartHeaderRow();

        //    if (!RowStarted)
        //    {
        //        if (!TableStarted) { StartTable(); }
        //        //if (THead != null)
        //        {
        //            HeadTR.BuildStartTag(buf);
        //        }
        //        ColumnCount = 0;
        //        //TR.BuildTag(buf);
        //        buf.AppendLine();
        //        RowStarted = true;
        //    }
        //}
        ////===================================================================================================
        //public override void EndHeaderRow()
        //{
        //    //TR.BuildEndTag(buf);
        //    //if (THead != null)
        //    {
        //        HeadTR.BuildEndTag(buf);
        //    }
        //    RowStarted = false;
        //    //currRowIndex++;
        //}

        //===================================================================================================
        public new static void Tests()
        {
            //Eg1(new JqResponsiveGridWriter());
            var tw = new JqResponsiveGridWriter();


            tw.TABLE.Css("margin", "1px")
                    .Nada();
            //.Css("border-collapse", "collapse")
            //.Css("font-weight", "bold")
            //.Css("border", "1px solid #ccc")

            //tw.TRAlt.Css("background-color", "lightgray");

            //tw.TR.ClassName = "tr";
            tw.TR//.Attr("tr1", "11")
                 //.Css("padding", "11px")
                 //.Css("border", "1px solid #ccc")
                 .Nada();

            //table>tbody>

            tw.TD//.Attr("valign", "top")
                .Css("padding", "5px")
                .Css("background-color", "lightblue")
                //.Css("border", "1px solid #ccc")
                .AddClass("col-sm-3")
                .Nada();
            tw.TH = tw.TD.CloneObj();

            StringBuilder css = new StringBuilder();
            //css.AppendFormat("#{0} td:nth-child(2) {{font-weight: bolder;}}", tw.TabID);
            css.Append(".row { border: 1px solid #ccc;}");
            tw.FlushStyleSheet(css.ToString());

            tw.AddHeaderRow("h1", "h2", "h3");
            for (int i = 0; i < 2; i++)
            {
                string longString = new string('x', 20) + " " + new string('y', 20);

                tw.AddRow_NValues("aa", new string('b', 20), longString);
            }
            MOutput.Write(tw.EndTable());
            MOutput.WriteHtmlEncodedPre(tw.EndTable());

        }
    }
}