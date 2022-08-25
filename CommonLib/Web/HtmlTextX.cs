using System.Text.RegularExpressions;
namespace Libx;
public class HtmlText
{
    static public string ExtractText(String htmlText)
    {
        htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<!--", "-->", StringComparison.InvariantCulture);
        htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<script", "</script>", StringComparison.InvariantCultureIgnoreCase);
        htmlText = StringX.RemoveSubstringBetween_Inclusive(htmlText, "<style", "</style>", StringComparison.InvariantCultureIgnoreCase);

        string pattern = "<[^>]*>";

        htmlText = Regex.Replace(htmlText, pattern, " ");

        htmlText = Regex.Replace(htmlText, "^[ ]+", "", RegexOptions.Multiline); //remove spaces in blank lines
        htmlText = Regex.Replace(htmlText, "[\r\f\n]{2,}", "\n"); //compress blank lines

        //htmlText = htmlText.Replace("&nbsp;", " ");
        htmlText = WebUtility.HtmlDecode(htmlText);
        return htmlText;
    }
    //===================================================================================================
    /// <summary>
    /// Strips HTML tags
    /// #striphtml #striptags
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static string GetInnerText(string html)
    {
        Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
        string txt = reg.Replace(html, "");

        //htmlText = htmlText.Replace("&nbsp;", " ");
        txt = WebUtility.HtmlDecode(txt);
        txt = StringX.CompressWhiteSpaces(txt);
        return txt;
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
    public static string HtmlEncodeAndPre(string text)
    {
        text ??= "";
        text = text.Trim();
        text = WebUtility.HtmlEncode(text);
        text = "<pre style='overflow:visible'>" + text + "</pre>";
        return text;
    }

    //===================================================================================================
    public static string HtmlEncode(String s, bool encodeSpaces = false)
    {
        return WebUtility.HtmlEncode(s); //-------------------------------
        //if (string.IsNullOrEmpty(s)) { return s; }
        //int len = s.Length;
        //StringBuilder buf = new StringBuilder(len);
        //for (int i = 0; i < len; i++)
        //{
        //    char c = s[i];
        //    if (c == '&') buf.Append("&amp;");
        //    else if (c == '>') buf.Append("&gt;");
        //    else if (c == '<') buf.Append("&lt;");
        //    else if (c == '"') buf.Append("&quot;");
        //    else if (c == '\'') buf.Append("&apos;");
        //    else if (encodeSpaces)
        //    {
        //        if (c == '\n') { buf.Append("<br/>"); }
        //        else if (c == '\f') { }
        //        else if (c == '\t') { buf.Append("&nbsp;&nbsp;&nbsp; "); }
        //        else if (c == ' ')
        //        {
        //            buf.Append(' ');
        //            for (++i; i < len; i++)
        //            {
        //                c = s[i];
        //                if (c == ' ')
        //                {
        //                    buf.Append("&nbsp;");
        //                }
        //                else
        //                {
        //                    i--;
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            buf.Append(c);
        //        }
        //    }
        //}
        //return buf.ToString();
    }
}
