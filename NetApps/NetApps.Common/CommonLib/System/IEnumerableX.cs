using System.Collections.Specialized;
using System.Linq;

namespace System.Collections
{
    public class IEnumerableX
    {
        //IEnumerable ee;
        //has only 1 member: IEnumerator GetEnumerator();

        //===================================================================================================
        public static List<List<T>> Split<T>(IEnumerable<T> list, int eltCountPerSubList, bool appendDefault = true)
        {
            var rval = new List<List<T>>();
            int k = 0;
            List<T> subList = new List<T>();
            foreach (T elt in list)
            {
                if (k % eltCountPerSubList == 0)
                {
                    subList = new List<T>();
                    rval.Add(subList);
                }
                subList.Add(elt);
                k++;
            }
            if (appendDefault && subList.Count() > 0)
            {
                for (int i = subList.Count(); i < eltCountPerSubList; i++)
                {
                    subList.Add(default(T));
                }
            }
            return rval;
        }

        //private readonly SortedList sl;

        //****************************************************************
        public static IEnumerable Sort(IEnumerable en)
        {
            String[] sa = ToStringArray(en);
            Array.Sort(sa);
            return sa;
        }
        //****************************************************************
        public static List<T> ToList<T>(IEnumerable<T> en)
        {
            List<T> rval = new List<T>();
            IEnumerator<T> enumerator = en.GetEnumerator();
            while (enumerator.MoveNext())
            {
                rval.Add(enumerator.Current);
            }
            return rval;
        }
        //****************************************************************
        public static T[] ToArray<T>(IEnumerable<T> en)
        {
            List<T> list = en as List<T>;
            if (list == null)
            {
                list = ToList(en);
            }
            T[] rval = new T[list.Count];
            IEnumerator<T> enumerator = en.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                rval[i++] = enumerator.Current;
                //enumerator.Reset();
            }
            return rval;
        }
        //****************************************************************
        public static StringCollection ToStringCollection(IEnumerable en)
        {
            StringCollection rval = new StringCollection();
            IEnumerator enumerator = en.GetEnumerator();
            while (enumerator.MoveNext())
            {
                rval.Add(enumerator.Current.ToString());
                //enumerator.Reset();
            }
            /*foreach (object o in en)
            {           
                rval.Add((string)o);
            }*/
            return rval;
        }
        //****************************************************************
        public static string[] ToStringArray(IEnumerable en)
        {
            StringCollection strCol = new StringCollection();
            IEnumerator enumerator = en.GetEnumerator();
            while (enumerator.MoveNext())
            {
                strCol.Add(enumerator.Current.ToString());
                //enumerator.Reset();
            }
            string[] rval = new string[strCol.Count];
            strCol.CopyTo(rval, 0);
            return rval;
        }
        //****************************************************************
        //ok public static IEnumerable Reverse(IEnumerable en) { return null; }
        //****************************************************************
        public static IEnumerable<T> Reverse<T>(IEnumerable<T> en)
        {
            if (en == null) { yield return default(T); }
            //List<T> list = ToList<T>(en); //OK
            List<T> list = ToList(en);//OK
            //return list.Reverse();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                yield return list[i];
            }
        }

        //****************************************************************
        //****************************************************************
        public static string ToString<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> en)
        {
            StringBuilder buf = new StringBuilder();
            IEnumerator<KeyValuePair<TKey, TValue>> enumerator = en.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, TValue> kv = enumerator.Current;
                buf.Append("[" + kv.Key.ToString() + "]");
                buf.Append(" = ");
                buf.Append("[" + kv.Value.ToString() + "]");
                buf.Append("\n");
            }
            return buf.ToString();
        }
        //****************************************************************
        public static string ToStringFormat(IEnumerable en, String format)
        {
            //"<span style="font-family:Times New Roman">Times New Roman </span><br />";
            if (string.IsNullOrEmpty(format))
            {
                //format = "<span style='font-family:{0}'>{0}</span><br />";
                throw new Exception("bad format: " + format);
            }
            StringBuilder buf = new StringBuilder();
            foreach (Object o in en)
            {
                //Log.Debug(o);

                buf.Append(String.Format(format, o.ToString()));
            }
            return buf.ToString();
        }
        //****************************************************************
        public static String ToString(IEnumerable en)
        {
            return ToString(en, ", ");
        }
        public static void ToString_eg(IEnumerable en)
        {
            StringBuilder sb = new StringBuilder();
            en.Cast<object>().ToList().ForEach(e => sb.Append(e.ToString() + "\n"));
            if (sb.Length > 1) sb.Remove(sb.Length - 1, 1);
        }
        public static String ToString(IEnumerable en, string seperator)
        {
            //if (seperator.Length > 1) { throw new ArgumentException("params.Length must be 0 or 1", "seperator"); }
            //string sep = (seperator.Length == 0)? ", ": seperator[0];
            string sep = seperator;


            if (en == null) { return "null"; }
            StringBuilder buf = new StringBuilder();
            foreach (Object o in en) //err if en is null
            {
                buf.Append(o.ToString());
                buf.Append(sep);
            }
            if (buf.Length >= sep.Length)
            {
                buf.Remove(buf.Length - sep.Length, sep.Length);
            }
            else
            {
                buf.Append("");
            }
            return buf.ToString();
        }
        //public ICollection ToICollection() { }
        // can't add items to ICollection

        //=================================================================
        public static String ToHtml(IEnumerable en, String captionTitle = null, string dataFormat = null)
        {
            //if (en.GetEnumerator() is IDictionaryEnumerator)
            //{
            //}
            StringBuilder buf = new StringBuilder();
            //StringWriter sw = new StringWriter(buf);
            //Html32TextWriter hw = new Html32TextWriter(sw);
            //hw.WriteLine("....");
            if (captionTitle != null)
            {
                //hw.WriteLine("<br/>");
                //hw.WriteEncodedText(captionTitle + ":");
                buf.AppendFormat("<h3>{0}</h3>", captionTitle);
                buf.AppendLine("<br/>");
            }

            buf.AppendLine("<table border=1 cellspacing=0 cellpading=1>");

            //if (captionTitle != null)
            //{
            //    hw.Write("<caption title='Caption=");
            //    hw.WriteEncodedText(captionTitle);
            //    hw.WriteLine("'>");
            //    hw.WriteEndTag("caption");
            //}

            if (en == null)
            {
                buf.AppendLine("<tr><td>is null</td</tr>");
            }
            else
            {
                //IEnumerable en = req.Form;
                //IEnumerator enr = en.GetEnumerator();
                //foreach (object o in en)
                //{
                // buf.Append(o + "<br>"); //Keys only !values
                //}
                int i = 0;
                foreach (object obj in en) //Keys: NameObjectCollectionBase.KeysCollection
                {
                    string val = obj.ToString();
                    String tr_bgColor = (i++ % 2 == 0) ? "#FFFFFF" : "lightgrey"; // "CADBEE";
                    buf.AppendLine("<tr bgcolor='" + tr_bgColor + "'> <td>" + i + "</td><td>");

                    var data = System.Net.WebUtility.HtmlEncode(val);
                    if (dataFormat != null)
                    {
                        data = String.Format(dataFormat, data);
                    }
                    buf.Append(data);


                    buf.AppendLine("</td></tr>");
                }
                if (i == 0)
                {
                    buf.AppendLine("<tr><td>is Empty</td</tr>");
                }
            }
            buf.AppendLine("</table>");

            return buf.ToString();
        }

        public static int Count(IEnumerable fa)
        {
            int rval = 0;
            foreach (object o in fa)
            {
                rval++;
            }
            return rval;
        }
    }
}