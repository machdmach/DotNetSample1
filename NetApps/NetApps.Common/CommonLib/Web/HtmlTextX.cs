using System.Text.RegularExpressions;


namespace System
{
    //http://dotnetperls.com/Content/Encode-HTML-String.aspx

    public class HtmlText
    {
        private static void Main()
        {

            //HtmlText ht;
        }
        //<a href=http://www.abc.com
        //<a href="http://www.abc.com/fully/qualified/page.htm'
        //<a href=/path/to/page1.htm
        //<a href=relative/path

        public static String SetLineNumbers_InHtmlTable(string s)
        {
            return StringX.SetLineNumbers_InHtmlTable(s);
        }



        //public static string Highlight(String s)
        //{
        //    return Highlight(s, Color.Yellow);
        //}
        //public static string Highlight(String s, Color hiliteColor)
        //{
        //    return String.Format("<span style=\"background-color:#{0}\">{1}</span>",
        //       ColorX.ToRRGGBB(hiliteColor), s);
        //}

        //--------------------------------------------------------------------
        //SMGL Standard Generalized Markup Language 
        static public string ExtractText(String htmlText)
        {
            htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<!--", "-->", StringComparison.InvariantCulture);
            htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<script", "</script>", StringComparison.InvariantCultureIgnoreCase);
            htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<style", "</style>", StringComparison.InvariantCultureIgnoreCase);

            string pattern = @"<[^>]*>";
            htmlText = Regex.Replace(htmlText, pattern, " ");

            htmlText = Regex.Replace(htmlText, "^[ ]+", "", RegexOptions.Multiline); //remove spaces in blank lines
            htmlText = Regex.Replace(htmlText, "[\r\f\n]{2,}", "\n"); //compress blank lines

            //htmlText = htmlText.Replace("&nbsp;", " ");
            htmlText = System.Net.WebUtility.HtmlDecode(htmlText);

            return htmlText;


            /*
            String s = "dd";

            // find links in simple html
            string t18 = @"
   <html>
   <a href=""first.htm"">first tag text</a>
   <a href=""next.htm"">next tag text</a>
   </html>
   ";
            string pattern = @"<[^>]*>";
            s = Regex.Replace(s, pattern, " ");

            MatchCollection mc = Regex.Matches(s, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            StringBuilder buf = new StringBuilder();
            foreach (Match m in mc)
            {
               //log.Debug("Match=" + m);
               //log.Debug("Group1=" + m.Groups[1]);
            }
            return buf.ToString();
             * */
        }
        //===============================================================
        static public string[] GetNumbers(String input)
        {
            // extracting all numbers from a string
            string t14 = @"
test 1
test 2.3
test 47
";
            string p14 = @"(\d+\.?\d*|\.\d+)";
            MatchCollection mc14 = Regex.Matches(t14, p14);
            //  foreach (Match m in mc14)
            //LogX.Debug("Match=" + m);

            //  DBNull db;


            return null;
        }

        //===================================================================================================
        public static string GetInnerHtml(string s, string eltName)
        {
            string tagOpen = '<' + eltName;
            string tagClose = "</" + eltName + ">";
            String sShort = s;
            if (sShort.Length > 100)
            {
                sShort = s.Substring(0, 100);
            }

            int i1 = s.IndexOf(tagOpen, StringComparison.InvariantCultureIgnoreCase);
            if (i1 < 0)
            {
                throw new Exception("open tag not found for " + eltName + " in: " + sShort);
            }
            i1 = s.IndexOf('>', i1);
            if (i1 < 0)
            {
                throw new Exception("open tag 222 not found for " + eltName);
            }
            i1 += 1;

            int i2 = s.IndexOf(tagClose, StringComparison.InvariantCultureIgnoreCase);
            if (i2 < 0)
            {
                throw new Exception("close tag not found for " + eltName);
            }
            string rval = s.Substring(i1, i2 - i1);
            return rval;
        }
        //===================================================================================================
        public static String GetTextBetween(string s, string text1, string text2)
        {
            int i1 = s.IndexOf(text1);
            if (i1 <= 0)
            {
                throw new Exception("i1");
            }
            i1 = i1 + text1.Length;

            int i2 = s.IndexOf(text2, i1);
            if (i2 <= 0)
            {
                throw new Exception("i2");
            }
            s = s.Substring(i1, i2 - i1);
            return s;
        }

        //===================================================================================================
        public static string GetBodyText(string s)
        {
            s = s.Trim();
            int i1_body = s.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase);
            if (i1_body >= 0)
            {
                i1_body = s.IndexOf('>', i1_body);
                s = s.Substring(i1_body + 1);
            }
            int i2_body = s.IndexOf("</body>", StringComparison.InvariantCultureIgnoreCase);
            if (i2_body >= 0)
            {
                s = s.Substring(0, i2_body);
            }
            return s.Trim();
        }
        //===================================================================================================
        public static string HtmlDecode(string text)
        {
            string rval;
            if (string.IsNullOrWhiteSpace(text))
            {
                rval = text;
            }
            else
            {
                System.Net.WebUtility.HtmlDecode(text);
                rval = text;
            }
            return rval;
        }
        //===================================================================================================
        public static string HtmlEncodeAndPre(string text)
        {
            text ??= "";
            text = text.Trim();
            text = System.Net.WebUtility.HtmlEncode(text);
            text = "<pre style='overflow:visible'>" + text + "</pre>";
            return text;
        }

        //===================================================================================================
        public static string HtmlEncode(string text)
        {
            string rval;
            if (string.IsNullOrWhiteSpace(text))
            {
                rval = text;
            }
            else
            {
                //StringWriter stringWriter = new StringWriter();
                //using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                //{
                //    //// Write a DIV with encoded text.
                //    //writer.RenderBeginTag(HtmlTextWriterTag.Div);
                //    writer.WriteEncodedText(text);
                //    //writer.RenderEndTag();
                //}
                //rval = stringWriter.ToString();
                rval = System.Net.WebUtility.HtmlEncode(text);
            }
            return rval;
        }
        //===================================================================================================
        public static string HtmlEncode(String s, bool encodeSpaces)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            //HttpContext Context = HttpContext.Current;
            //s = Context.Server.HtmlEncode(s);
            //if (!encodeSpaces)
            //{
            //    return s;
            //}

            //s = s.Replace("\n", "<br>");
            //while (s.Contains("  "))
            //{
            //    s = s.Replace("  ", "&nbsp; ");
            //}
            int len = s.Length;
            StringBuilder buf = new StringBuilder(len);

            for (int i = 0; i < len; i++)
            {
                char c = s[i];
                if (c == '&') buf.Append("&amp;");
                else if (c == '>') buf.Append("&gt;");
                else if (c == '<') buf.Append("&lt;");
                else if (c == '"') buf.Append("&quot;");
                else if (c == '\'') buf.Append("&apos;");
                else if (encodeSpaces)
                {
                    if (c == '\n') { buf.Append("<br/>"); }
                    else if (c == '\f') { }
                    else if (c == '\t') { buf.Append("&nbsp;&nbsp;&nbsp; "); }
                    else if (c == ' ')
                    {
                        buf.Append(' ');
                        for (++i; i < len; i++)
                        {
                            c = s[i];
                            if (c == ' ')
                            {
                                buf.Append("&nbsp;");
                            }
                            else
                            {
                                i--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        buf.Append(c);
                    }
                }
            }
            return buf.ToString();
        }
    }
}
