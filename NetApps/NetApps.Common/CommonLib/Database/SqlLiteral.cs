using System.Text.RegularExpressions;

namespace System.Data
{
    public class SqlText { }
    public class SqlLiteral
    {
        public const char SQUOTE = '\'';


        public static bool IsValidNumValForSearch(String val)
        {
            // DbType
            if (val == null || val.Length == 0)
                return true;
            if (val.Equals("!") || val.Equals("%"))
                return true;

            return Regex.IsMatch(val, "^[0-9]*$");
        }
        //******************************************************************************
        public static String RemoveDoubleWildcards(String s)
        {
            while (s.IndexOf("%%") >= 0)
            {
                s = s.Replace("%%", "%");
            }
            return s;
        }
        //******************************************************************************
        public static String WildcardSpecialChars(String s)
        {
            if (s.IndexOf("%") >= 0)
                return s;

            StringBuilder buf = new StringBuilder("%");
            int i = 0;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if (Char.IsLetterOrDigit(c))
                {
                    buf.Append(c);
                }
                else if (c == SQUOTE)
                {
                    buf.Append(c);
                }
                else if (c == ' ')
                {
                    buf.Append('%');
                }
                else
                {
                    if (i != 0) { buf.Append('%'); }
                    buf.Append(c);
                    buf.Append('%');
                }
            }
            buf.Append("%");
            String rval = buf.ToString();
            rval = rval.Replace("''", "%''%");
            rval = rval.Replace("%%", "%");
            return rval;
        }
        //******************************************************************************
        public static String EscapeSQuote(String s)
        {
            if (s == null)
                return "null";
            if (s.IndexOf(SQUOTE) < 0)
            {
                return s;
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\'')
                    buf.Append("''");
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }
        //******************************************************************************
        public static String EncloseSQuote(String s)
        {
            //if (s==null) throw new Exception("null param");
            if (s == null)
                return "null";
            return SQUOTE + s + SQUOTE;
        }
        //******************************************************************************
        public static String EscapeEncloseSQuote(String s)
        {
            if (s == null)
            {
                return "null";
            }

            if (s.IndexOf('\'') >= 0)
            {
                s = EscapeSQuote(s);
            }
            //if (s.IndexOf('@') >= 0)
            //{
            //    //if (s.Length > 0)
            //    {
            //        s = s.Replace("@", "@@");
            //    }
            //}

            return '\'' + s + '\'';
        }
        //******************************************************************************
        public static String EscapeEncloseSQuoteAndAmpersand(String s)
        {
            if (s == null)
            {
                return "null";
            }

            if (s.IndexOf('\'') >= 0)
            {
                s = EscapeSQuote(s);
            }

            if (s.IndexOf('@') >= 0)
            {
                s = s.Replace("@", "@@");
            }

            return '\'' + s + '\'';
        }

        public virtual bool ContainsWildCards(String s)
        {
            return s.IndexOf('*') > 0 || s.IndexOf('%') > 0;
        }
    }
}