using System.Collections.Specialized;

namespace Libx
{
    public class JqHtmlElement // ICloneable<>
    {
        //public bool IsReadable = true;
        //public JqHtmlElement[] Children;
        //public List<JqHtmlElement> Children;
        //        public string StartTagPrefix; //" "
        //public string StartTagSufffix;  //\n
        //public int ColumnIndex = -1;
        //    <table border='0' cellpadding='3px' style='padding: 3px; border-collapse: collapse; border-spacing: 0'>
        //===============================================================================
        public string TagName;
        public string ID;
        public string Name;
        public string ClassName;
        //public string ColHeaderHtml;
        public NameValueCollection AttributeNVs;
        public String AttributesStr;
        public NameValueCollection StyleDeclarations;
        public String StyleStr;
        public List<String> ClassNames;
        public String ClassNamesStr;
        public JqHtmlElement ParentColumn;

        public JqHtmlElement(string tagName)
        {
            TagName = tagName;
            // bufz2 = new StringBuilder();
        }
        public JqHtmlElement(JqHtmlElement src)
        {
            TagName = src.TagName;
            ClassName = src.ClassName;
            //StartTagPrefix = src.StartTagPrefix;
            //StartTagSufffix = src.StartTagSufffix;

            AddClass(src);
            Attr(src);
            Css(src);
            // bufz2 = new StringBuilder();
        }
        public JqHtmlElement CloneObj()
        {
            var rval = new JqHtmlElement(this);
            return rval;
        }
        public JqHtmlElement Clone()
        {
            var rval = new JqHtmlElement(this);
            return rval;
        }

        ////===============================================================================
        //public JqElement(StringBuilder sb)
        //{
        //    this.bufz2 = sb;
        //}
        //===================================================================================================
        //===================================================================================================
        public JqHtmlElement Attr(JqHtmlElement otherElt)
        {
            var col2 = otherElt.AttributeNVs;
            if (col2 != null)
            {

                foreach (string propertyName in col2.AllKeys)
                {
                    //AttributeCol.Add(propertyName, col2[propertyName]);
                    Attr(propertyName, col2[propertyName]);
                }
            }
            return this;
        }
        public JqHtmlElement Attr(String propertyName, object val)
        {
            return Attr(propertyName, val.ToString());
        }
        public JqHtmlElement RemoveAttr(String propertyName)
        {
            if (AttributeNVs != null)
            {
                AttributeNVs.Remove(propertyName);
            }
            if (AttributesStr != null)
            {
                AttributesStr = null; //reset
            }
            return this;
        }
        public JqHtmlElement Attr(String propertyName, string val)
        {
            //$("#w3s").attr("href");
            //$( "#greatphoto" ).attr( "title", "Photo by Kelly Clark" );

            if (AttributeNVs == null)
            {
                AttributeNVs = new NameValueCollection();
            }
            AttributeNVs[propertyName] = val;
            if (AttributesStr != null)
            {
                AttributesStr = null; //reset
            }
            return this;
        }

        //===================================================================================================
        public string GetAttributes()
        {
            if (AttributesStr == null)
            {
                if (AttributeNVs != null)
                {
                    var buf = new StringBuilder();
                    foreach (string propertyName in AttributeNVs)
                    {
                        string val = AttributeNVs.Get(propertyName);
                        buf.Append(' ');
                        buf.Append(propertyName);
                        buf.Append("='");
                        buf.Append(val);
                        buf.Append("'");
                    }
                    AttributesStr = buf.ToString();
                }
            }
            return AttributesStr;
        }

        public String InnerHtml;
        public JqHtmlElement Html(string html)
        {
            InnerHtml = html;
            return this;
        }

        //===================================================================================================
        public string Attr(String propertyName)
        {
            if (AttributeNVs == null)
            {
                return null;
            }
            return AttributeNVs.Get(propertyName); //null if not found
        }
        //===================================================================================================
        public JqHtmlElement ClearAttr()
        {
            AttributeNVs = null;
            AttributesStr = null;
            return this;
        }
        //===================================================================================================
        //===================================================================================================
        public JqHtmlElement Css(JqHtmlElement otherElt)
        {
            var col2 = otherElt.StyleDeclarations;
            if (col2 != null)
            {
                foreach (string propertyName in col2.AllKeys)
                {
                    Css(propertyName, col2[propertyName]);
                }
            }
            return this;
        }
        //===================================================================================================
        public JqHtmlElement ClearCss()
        {
            StyleDeclarations = null;
            StyleStr = null;
            return this;
        }
        //===================================================================================================
        public JqHtmlElement Css(String propertyName, object val)
        {
            return Css(propertyName, val.ToString());
        }
        public JqHtmlElement Css(String propertyName, string val)
        {
            //$("p").css("background-color");  //get; 
            //css("propertyname","value"); //set

            //$("p").css("background-color", "yellow");
            //css({"propertyname":"value","propertyname":"value",...});
            //$("p").css({"background-color": "yellow", "font-size": "200%"});

            if (StyleDeclarations == null)
            {
                StyleDeclarations = new NameValueCollection();
            }
            StyleDeclarations[propertyName] = val;

            if (StyleStr != null)
            {
                StyleStr = null; //reset
            }
            return this;
        }
        //===================================================================================================
        public string GetCssDeclarations()
        {
            if (StyleStr == null)
            {
                if (StyleDeclarations != null)
                {
                    var buf = new StringBuilder();
                    foreach (string propertyName in StyleDeclarations)
                    {
                        string val = StyleDeclarations.Get(propertyName);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            if (buf.Length > 0)
                            {
                                buf.Append(' ');
                            }
                            buf.Append(propertyName);
                            buf.Append(":");
                            buf.Append(val);
                            buf.Append(";");
                        }
                    }
                    StyleStr = buf.ToString();
                }
            }
            return StyleStr;
        }
        //===================================================================================================
        public string Css(String propertyName)
        {
            if (StyleDeclarations == null)
            {
                return null;
            }
            return StyleDeclarations.Get(propertyName); //null if not found
        }
        //===================================================================================================
        //===================================================================================================
        public JqHtmlElement AddClass(JqHtmlElement otherElt)
        {
            var col2 = otherElt.ClassNames;
            if (col2 != null)
            {
                foreach (string className in col2)
                {
                    AddClass(className);
                }
            }
            return this;
        }
        //===================================================================================================
        public JqHtmlElement AddClass(String className)
        {
            // $("h1, h2, p").addClass("blue");
            //$("div").addClass("important");
            //    $("h1, h2, p").removeClass("blue");
            //$( "p" ).removeClass( "myClass noClass" ).addClass( "yourClass" );

            if (ClassNames == null)
            {
                ClassNames = new List<string>();
            }
            if (!ClassNames.Contains(className))
            {
                ClassNames.Add(className);
            }

            if (ClassNamesStr != null)
            {
                ClassNamesStr = null; //reset
            }
            return this;
        }
        //===================================================================================================
        public JqHtmlElement RemoveClass(String className)
        {
            // $("h1, h2, p").addClass("blue");
            //$("div").addClass("important");
            //    $("h1, h2, p").removeClass("blue");
            //$( "p" ).removeClass( "myClass noClass" ).addClass( "yourClass" );

            if (ClassNames == null)
            {
                return this;
            }
            bool found = ClassNames.Remove(className);

            if (ClassNamesStr != null)
            {
                ClassNamesStr = null; //reset
            }
            return this;
        }

        //===================================================================================================
        public string GetClassNames()
        {
            if (ClassNamesStr == null)
            {
                //if (ClassName != null)
                //{
                //    if (!ClassCol.Contains(ClassName))
                //    {
                //        ClassCol.Add(ClassName);
                //    }
                //}
                StringBuilder buf = new StringBuilder();

                //if (ColumnIndex >= 0)
                //{
                //    buf.Append(" c");
                //    buf.Append(ColumnIndex);
                //}

                if (ClassNames != null)
                {
                    foreach (string className in ClassNames)
                    {
                        buf.Append(' ');
                        //if (!string.IsNullOrWhiteSpace(val))
                        {
                            buf.Append(className);
                        }
                    }
                    ClassNamesStr = buf.ToString();
                }
                if (ClassName != null)
                {
                    buf.Append(' ');
                    buf.Append(ClassName);
                }

                if (buf.Length > 0)
                {
                    ClassNamesStr = buf.ToString(1, buf.Length - 1);
                }
            }
            //MOutput.WriteLine("ClassNames: "+ ClassNames);
            return ClassNamesStr;
        }
        //===================================================================================================
        public static void Test_css()
        {
            var e = new JqHtmlElement("tr");
            MOutput.WriteHtmlEncodedPre(e.GetCssDeclarations());
            e.Css("color", "red");
            MOutput.WriteHtmlEncodedPre(e.GetCssDeclarations());
            e.Css("color", null);
            MOutput.WriteHtmlEncodedPre(e.GetCssDeclarations());
            e.Css("color", "blue");
            e.Css("padding", "5px 15px 5px 15px");
            MOutput.WriteHtmlEncodedPre(e.GetCssDeclarations());

            //-----------------------------------------------
            MOutput.WriteHtmlEncodedPre(e.GetAttributes());
            e.Attr("border", "1");
            MOutput.WriteHtmlEncodedPre(e.GetAttributes());
            e.Attr("border", null);
            e.Attr("padding", "5px 15px 5px 15px");
            MOutput.WriteHtmlEncodedPre(e.GetAttributes());

            //-----------------------------------------------
            MOutput.WriteHtmlEncodedPre(e.GetClassNames());
            e.AddClass("class1");
            MOutput.WriteHtmlEncodedPre(e.GetClassNames());
            e.AddClass("class2");
            MOutput.WriteHtmlEncodedPre(e.GetClassNames());
            e.RemoveClass("class2");
            MOutput.WriteHtmlEncodedPre(e.GetClassNames());

            //e.BuildTag(buf);

            //-----------------------------------------------
            MOutput.WriteHtmlEncodedPre(e.BuildStartTag());
        }

        //===================================================================================================
        //An element in HTML represents some kind of structure or semantics and generally consists of a start tag, content, and an end tag. 

        public string BuildStartTag(string tagName = null)
        {
            var buf = new StringBuilder();
            BuildStartTag(buf);
            return buf.ToString();
        }
        //===================================================================================================
        public void BuildStartTag(StringBuilder buf) //, int colNum = 0)
        {
            //if (StartTagPrefix != null)
            //{
            //    buf.Append(StartTagPrefix);
            //}

            buf.Append("<");
            buf.Append(TagName);

            //tw.TABLE.Attr("id", tabID);
            if (!string.IsNullOrEmpty(ID))
            {
                buf.AppendFormat(" id='{0}'", ID);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                buf.AppendFormat(" name='{0}'", Name);
            }

            string attrs = GetAttributes();
            if (attrs != null)
            {
                buf.Append(attrs);
            }
            //ring colClass = null
            string classNames = GetClassNames();

            if (classNames != null)
            {
                buf.Append(" class='");
                buf.Append(classNames);
                buf.Append("'");
            }
            string style = GetCssDeclarations();
            if (style != null)
            {
                buf.Append(" style='");
                buf.Append(style);
                buf.Append("'");
            }
            buf.Append(">");
            //if (StartTagSufffix != null)
            //{
            //    buf.Append(StartTagSufffix);
            //}
            //return buf.ToString();
        }
        //===================================================================================================
        public void BuildEndTag(StringBuilder buf)
        {
            //</tr>
            buf.Append("</");
            buf.Append(TagName);
            buf.AppendLine(">");
        }
        //===================================================================================================
        public string InlineCssToStyleSheet(string outerEltID)
        {
            StringBuilder buf = new StringBuilder();
            InlineCssToStyleSheet(buf, outerEltID);
            return buf.ToString();
        }
        //===================================================================================================
        public void InlineCssToStyleSheet(StringBuilder buf, string outerEltID)
        {
            //string style = GetCssDeclarations();
            string cssRuleSet = GetCssRuleSet();
            if (cssRuleSet != null)
            {
                buf.Append("  ");
                // #tab123 td { color:red;}
                if (outerEltID != null)
                {
                    buf.Append("#");
                    buf.Append(outerEltID);
                    buf.Append(" ");
                }
                buf.Append(cssRuleSet);
                ClearCss();
            }
        }
        //===================================================================================================
        public string GetCssRuleSet(string outerSelector = null)
        {
            //rule-set: selector (h1) {declaration-block}
            string rval = null;
            string style = GetCssDeclarations();
            if (style != null)
            {
                var buf = new StringBuilder();
                // #tab123 td { color:red;}

                if (outerSelector != null)
                {
                    buf.Append(outerSelector);
                    buf.Append(" ");
                }
                if (ClassName != null)
                {
                    buf.Append(".");
                    buf.Append(ClassName);
                    //AddClass(ClassName);
                }
                else
                {
                    buf.Append(TagName);
                }

                buf.Append(" {");
                buf.Append(style);
                buf.AppendLine("}");
                rval = buf.ToString();
            }
            return rval;
        }
        //===================================================================================================
        public void Nada() { }

    }

}