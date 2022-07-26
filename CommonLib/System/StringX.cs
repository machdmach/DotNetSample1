using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Libx;
/// <summary>
/// <seealso cref="Byte.ToString" format="X2"/>
/// </summary>
public static class StringX
{
    public static string ToggleSubstrings(string target, string substr1, string substr2, bool caseSenstive = true)
    {
        string ret;
        StringComparison strComp = caseSenstive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
        int i1 = target.IndexOf(substr1, strComp);
        if (i1 >= 0)
        {
            if (caseSenstive)
            {
                ret = target.ReplaceStringCI(substr1, substr2);
            }
            else
            {
                ret = target.Replace(substr1, substr2);
            }
        }
        else
        {
            int i2 = target.IndexOf(substr2, strComp);
            if (i2 >= 0)
            {
                if (caseSenstive)
                {
                    ret = target.ReplaceStringCI(substr2, substr1);
                }
                else
                {
                    ret = target.Replace(substr2, substr1);
                }
            }
            else
            {
                throw new Exception(string.Format("either '{0}' or '{1}' is not in target: '{2}'", substr1, substr2, target));
            }
        }
        return ret;
    }

    //===================================================================================================
    public static string Reverse(string s)
    {
        char[] charArr = new char[s.Length];
        int forward = 0;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            charArr[forward++] = s[i];
        }
        return new string(charArr);
    }

    //===================================================================================================
    public static bool Is1CharDiff(string s1, string s2)
    {
        if (s1 == null)
        {
            if (s2 == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (s2 == null)
            {
                return false;
            }
        }
        if (Math.Abs(s1.Length - s2.Length) > 1)
        {
            return false;
        }
        string shorter, longer;
        if (s1.Length < s2.Length)
        {
            shorter = s1;
            longer = s2;
        }
        else
        {
            shorter = s2;
            longer = s1;
        }
        int i = 0;
        for (; i < shorter.Length; i++)
        {
            if (shorter[i] != longer[i])
            {
                break; //first char diff found
            }
        }
        bool ret = true;
        if (i < shorter.Length)
        {
            i++;
            if (shorter.Length == longer.Length)
            {
                for (; i < shorter.Length; i++)
                {
                    if (shorter[i] != longer[i])
                    {
                        ret = false;
                        break;
                    }
                }
            }
            else
            {
                for (; i < shorter.Length; i++)
                {
                    if (shorter[i] != longer[i + 1])
                    {
                        ret = false;
                        break;
                    }
                }
            }
        }
        return ret;
    }
    //===================================================================================================
    public static string FormatWith(this string format, params object[] args)
    {
        if (format == null)
            throw new ArgumentNullException("format");

        return string.Format(format, args);
    }
    //===================================================================================================
    public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
    {
        if (format == null)
            throw new ArgumentNullException("format");

        return string.Format(provider, format, args);
    }
    //===================================================================================================
    public static string FormatWith(this string format, object source)
    {
        //return FormatWith(format, null, source);
        if (format == null)
            throw new ArgumentNullException("format");

        return string.Format(format, source);
    }

    //===================================================================================================
    public static string SafeSubstring(string input, int startIndex, int length)
    {
        // Todo: Check that startIndex + length does not cause an arithmetic overflow
        if (input.Length >= (startIndex + length))
        {
            return input.Substring(startIndex, length);
        }
        else
        {
            if (input.Length > startIndex)
            {
                return input.Substring(startIndex);
            }
            else
            {
                return input; // string.Empty;
            }
        }
    }

    //===================================================================================================
    public static string ReplaceNonPrintableCharacters(string s, string replaceWith)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            byte b = (byte)c;
            if (b < 32)
                result.Append(replaceWith);
            else
                result.Append(c);
        }
        return result.ToString();
    }

    //===================================================================================================
    public static string RemoveNonAsciiChars(String s)
    {
        char[] charArr = s.ToCharArray();
        int charArrLen = charArr.Length;

        StringBuilder buf = new StringBuilder(charArrLen);
        for (int i = 0; i < charArrLen; i++)
        {
            char c = charArr[i];
            int x = c;
            if ((31 < x && x < 127)
             || x == 9  //tab
             || x == 10 //0A LF
             || x == 13 //0D CR
             || x == 12 //0C FF
               )
            {
                buf.Append(c);
            }
        }
        return buf.ToString();
    }

    //===================================================================================================
    public static string Repeat(String s, int n)
    {
        StringBuilder buf = new StringBuilder();
        for (int i = 0; i < n; i++)
        {
            buf.Append(s);
        }
        return s.ToString();
    }

    public static String SetLineNumbers(string s)
    {
        //if (string.IsNullOrEmpty(s)) return "arg is blank"; //---------------------------

        StringBuilder buf = new StringBuilder();
        //string[] sa = s.Split(new char[] {'\r', '\f', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        //string[] sa = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] sa = s.Split(new char[] { '\n' }, StringSplitOptions.None);
        int len = sa.Length;
        int indent = (len - 1).ToString().Length;
        string identStrFormat = "{0," + indent + "}. ";
        for (int i = 0; i < sa.Length; i++)
        {
            //string s = s[i];            
            string indentStr = String.Format(identStrFormat, i);
            buf.Append(indentStr);
            buf.Append(sa[i]);
            buf.Append("\n");
        }
        return buf.ToString();
    }
    //===================================================================================================
    public static String SetLineNumbers_InHtmlTable(string s)
    {
        if (string.IsNullOrEmpty(s)) return "arg is blank"; //---------------------------

        StringBuilder buf = new StringBuilder();
        StringBuilder buf2 = new StringBuilder();

        //string[] sa = s.Split(new char[] {'\r', '\f', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        string[] sa = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int len = sa.Length;
        int indent = (len - 1).ToString().Length;
        string identStrFormat = "{0," + indent + "}. ";

        for (int i = 0; i < sa.Length; i++)
        {
            //string s = s[i];            
            string indentStr = String.Format(identStrFormat, i);
            buf.Append(indentStr);
            buf.Append("\n");

            buf2.Append(sa[i]);
            buf2.Append("\n");
        }
        string rval = "<table border=0><tr><td><pre>" + buf.ToString() + "</pre></td><td><pre>" + buf2.ToString() + "</pre></td></tr></table>";
        return rval;
    }

    //===================================================================================================
    public static String CreateRandomString(int len)
    {
        byte[] bytes = new byte[len];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        string rval = Convert.ToBase64String(bytes);
        return rval.Substring(0, len);
    }

    //===================================================================================================
    public static readonly string EmptyText = "~`-EMPTY-~"; //*EMPTY*
    public const string NullText = "~`-NULL-~";  //*NULL* const can't be static

    //==================================================================
    public static string InitCap(String s) //Oracle.InitCap
    {
        //Oracle FULL_NAME = Full_Name (keep the underscore, as space)
        return ToTitleCase(s);
    }

    public static string ToTitleCase(String s)
    {
        //This Is A Title Case
        //IDictionaryX x;  //Err, need Webx.IDictioaryX;

        if (s == null) return null; // throw new ArgumentNullException("s");

        //System.Globalization.TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        //s = s.Replace('_', ' ');
        //s = ti.ToTitleCase(s);

        var len = s.Length;
        if (len == 0)
        {
            return s; //-------------
        }
        StringBuilder buf = new StringBuilder(len);
        buf.Append(char.ToUpper(s[0]));
        for (int i = 1; i < len; i++)
        {
            if (s[i - 1] == ' ') buf.Append(char.ToUpper(s[i]));
            else buf.Append(char.ToLower(s[i]));
        }
        return buf.ToString();
    }

    //===================================================================================================
    public static string ToSentenceCase(String s)
    {
        if (s == null) return null; //throw new ArgumentNullException("s");

        //System.Globalization.TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        //s = s.Replace('_', ' ');

        var len = s.Length;
        if (len == 0)
        {
            return s; //-------------
        }
        StringBuilder buf = new StringBuilder(len);
        buf.Append(char.ToUpper(s[0]));
        if (len > 1)
        {
            buf.Append(s.Substring(1));
        }
        return buf.ToString();

    }
    //===================================================================================================
    static public string LoweredCasedSentenceToTitleCase(String text)
    {
        var buf = new StringBuilder();
        if (text == null) { return null; }
        if (text.Length == 1) { return Char.ToUpper(text[0]).ToString(); }

        int len = text.Length;
        char c;
        bool isLastCharSpace = true;
        for (int i = 0; i < text.Length; i++)
        {
            c = text[i];
            if (c == ' ' || c == '-')
            {
                isLastCharSpace = true;
                buf.Append(c);
            }
            else
            {
                if (isLastCharSpace)
                {
                    buf.Append(Char.ToUpper(c));
                }
                else
                {
                    buf.Append(c);
                }
                isLastCharSpace = false;
            }          
        }
        string ret = buf.ToString();
        ret = ret.Replace(" And ", " and ");

        ret = ret.Replace(" Of ", " of ");
        ret = ret.Replace(" To ", " to ");
        ret = ret.Replace(" The ", " the ");
        return ret;
    }
    //===================================================================================================
    static public string RemoveCrlfx(String input)
    {
        // joining lines in multiline strings
        string t13 = @"this is 
a split line";
        string p13 = @"\s*\r?\n\s*";
        string r13 = Regex.Replace(t13, p13, " ");
        //LogX.Debug("r13=" + r13);
        return input;
    }
    //===================================================================================================
    public static string Capitalize(string text)
    {
        // text = "the quick red fox jumped over the lazy brown dog.";
        //Log.Debug("text=[" + text + "]");
        string result = "";
        string pattern = @"\w+|\W+";

        foreach (Match m in Regex.Matches(text, pattern))
        {
            // get the matched string
            string x = m.ToString();
            // if the first char is lower case
            if (char.IsLower(x[0]))
                // capitalize it
                x = char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);
            // collect all text
            result += x;
        }
        return result;
    }

    public static string RemoveDuplicates(String input, string substr)
    {
        //Regex.Replace("xxxabcabcz\n\n\nabc\nd", "(.*)suffix$", "$1");
        //LogX.Debug("input= " + input);
        //LogX.Debug("substr= " + substr);
        String rval;

        Regex regex = new Regex("(.*?)(" + substr + "){2,}", RegexOptions.Singleline);
        // ?=Non-greedy
        Match m = regex.Match(input);
        Match mSaved = null;
        StringBuilder buf = new StringBuilder();
        while (m.Success)
        {
            buf.Append(m.Groups[1]);
            buf.Append(m.Groups[2]);
            mSaved = m;
            m = m.NextMatch();
        }
        if (mSaved == null)
        {
            rval = input;
        }
        else
        {
            buf.Append(input.Substring(mSaved.Index + mSaved.Length));
            rval = buf.ToString();
        }
        //LogX.Debug("rval= " + rval);
        return rval;
    }
    //===================================================================================================
    public static Int32 ExtractSubnumber(string s, string errMesg)
    {
        var ret = ExtractSubnumber(s);
        if (!ret.HasValue)
        {
            throw new Exception(errMesg);
        }
        return ret.Value;
    }
    //===================================================================================================
    public static Int32? ExtractSubnumber(string s)
    {
        Match m = Regex.Match(s, @"[\d]+", RegexOptions.Singleline);
        Int32? ret;
        if (m.Success)
        {
            ret = Int32.Parse(m.Value);
        }
        else
        {
            ret = null;
        }

        return ret;

    }
    //===================================================================================================
    public static bool IsNumeric(string s)
    {

        bool result;
        if (s != null && s.Length > 0)
        {
            result = true;
            for (int i = 0; i < s.Length; ++i)
            {
                if (!char.IsDigit(s[i]))
                {
                    result = false;
                    break;
                }
            }
        }
        else
        {
            result = false;
        }
        return result;
    }
    /// <summary>
    ///    string s = "asbc <!-- abcd tag3 s bcd --> dd. ";
    ///    RemoveSubstringBetween_Inclusive(s, "<!--", "-->"));
    /// </summary>
    /// <param name="s"></param>
    /// <param name="substrStart"></param>
    /// <param name="substrEnd"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    public static string RemoveSubstringBetween_Inclusive(String s, string substrStart, string substrEnd, StringComparison comparisonType = StringComparison.InvariantCulture)
    {
        int i1 = s.IndexOf(substrStart, comparisonType);
        if (i1 < 0) return s;

        StringBuilder buf = new StringBuilder();
        int i2 = 0;
        int substrEndLen = substrEnd.Length;
        while (i1 > 0)
        {
            buf.Append(s.Substring(i2, i1 - i2));
            i2 = s.IndexOf(substrEnd, i1);

            //Assert.Greater(i2, 0, "missing end substring: " + substrEnd);
            i2 += substrEndLen;
            i1 = s.IndexOf(substrStart, i2);
        }
        buf.Append(s.Substring(i2));
        return buf.ToString();
    }

    //===================================================================================================
    public static string CompressWhiteSpaces(String s)
    {
        if (string.IsNullOrEmpty(s)) return s; //------------------------------------
        string ret = Regex.Replace(s, "[ \r\f\n\t]+", " ");
        ret = ret.Trim();
        return ret;
    }
    public static string CompressSpaces(String s)
    {
        return Regex.Replace(s, "[ ]{2,}", " ");
        //return RegexX.RemoveDuplicates(s, " ");
    }
    public static string CompressBlankLines(string lines)
    {
        return Regex.Replace(lines, "[\r\f\n]+", "\n");
    }

    public static int CountSubstring(string s, string substr)
    {
        if (s == null || substr == null) return 0; //-------------------

        int i = s.IndexOf(substr);
        int substrLen = substr.Length;
        int rval = 0;
        while (i >= 0)
        {
            rval++;
            i = s.IndexOf(substr, i + substrLen);
        }
        return rval;
    }
    //===================================================================================================
    public static string ReplaceLastSubstring(string s, string substr, string replacement)
    {
        int i = s.LastIndexOf(substr);
        if (i < 0) return s;

        return s.Substring(0, i) + replacement + s.Substring(i + substr.Length);
    }
    public static StringCollection BreakLines(string s, int maxCharsPerLine)
    {
        StringCollection rval = new StringCollection();

        int i;
        while (s.Length > maxCharsPerLine)
        {
            i = maxCharsPerLine - 1;
            while (i > 0 && !Char.IsWhiteSpace(s[i]))
            {
                i--;
            }
            if (i == 0) { i = maxCharsPerLine; }

            string line = s.Substring(0, i);
            s = s.Substring(i);
            rval.Add(line);
        }
        rval.Add(s);

        return rval;
    }
    public static string ReplaceStringCI(this string str, string oldValue, string newValue)
    {
        string rval = ReplaceString_CI(str, oldValue, newValue);
        return rval;
    }
    public static string ReplaceString_CI_Info = "";
    public static string ReplaceString_CI(string str, string oldValue, string newValue)
    {
        StringBuilder sb = new StringBuilder();

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);
        if (index == -1)
        {
            //MOutput.WriteFormat("not found, replacing {0} with {1}<br>\n", oldValue, newValue);
            return str;
        }
        //MOutput.WriteFormat("replacing {0} with {1}<br>\n", oldValue, newValue);

        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            ReplaceString_CI_Info = oldValue + " = " + newValue + ", index=" + index;

            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.InvariantCultureIgnoreCase);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }

    //===================================================================================================
    public static bool TryReplaceStringCI(this string str, string oldValue, string newValue, out string result)
    {
        StringBuilder sb = new StringBuilder();

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);
        if (index == -1)
        {
            //MOutput.WriteFormat("not found, replacing {0} with {1}<br>\n", oldValue, newValue);
            result = str;
            return false;
        }
        //MOutput.WriteFormat("replacing {0} with {1}<br>\n", oldValue, newValue);

        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            ReplaceString_CI_Info = oldValue + " = " + newValue + ", index=" + index;

            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.InvariantCultureIgnoreCase);
        }
        sb.Append(str.Substring(previousIndex));

        result = sb.ToString();
        return true;
    }
    //===================================================================================================
    public static String GetSubstringBetween(string s, string text1, string text2)
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
}
