using System.Collections.Specialized;



namespace System
{
    public class StringArray
    {
        //protected //static ILog log = LogManagerX.GetLogger(MethodInfo.GetCurrentMethod().DeclaringType);
        private static void Main()
        {
            string[] sa1 = { "aa", "bb", "cc" };
            //string[] sa2 = { "aa", "bb", "cc" };
            //LogX.Debug(string.Join(",", Remove(sa1, "aa")));
            //LogX.Debug(string.Join(",", Remove(sa1, "bb")));
            //LogX.Debug(string.Join(",", Remove(sa1, "cc")));
            //LogX.Debug(string.Join(",", Remove(sa1, "ccd")));

            string[] sa2 = { "aa", "bb3", "cc4" };
            //LogX.Debug(string.Join(",", Union(sa1, sa2)));
            //LogX.Debug(string.Join(",", Minus(sa1, sa2)));


        }
        public static void ForEach_cm(String[] sa)
        {
            //strs.ForEach(new Action<string>(delegate(string s) { w.WriteLine(s); }));

            StringBuilder buf = new StringBuilder();

            Array.ForEach<string>(sa, new Action<string>(
               delegate (string s)
               {
                   buf.Append(s);
               }));
        }
        //public static string[] Union(string[] sa1, string[] sa2)


        public static string[] Union(string[] sa1, string[] sa2)
        {
            string[] rval = new string[sa1.Length + sa2.Length];
            Array.Copy(sa1, rval, sa1.Length);
            Array.Copy(sa2, 0, rval, sa1.Length, sa2.Length);
            return rval;
            /*
            StringCollection col = new StringCollection();
            col.AddRange(sa1);
            for (int i = 0; i < sa2.Length; i++)
            {
               if (!col.Contains(sa2[i]))
               {
                  col.Add(sa2[i]);
               }
            }
            string[] rval = new string[col.Count];
            col.CopyTo(rval, 0);
            return rval;
             */
        }
        public static string[] Minus(string[] sa1, string[] sa2)
        {
            //if elt in sa2, elt is removed from sa1
            StringCollection col = new StringCollection();
            col.AddRange(sa1);
            for (int i = 0; i < sa2.Length; i++)
            {
                if (col.Contains(sa2[i]))
                {
                    col.Remove(sa2[i]);
                }
            }
            string[] rval = new string[col.Count];
            col.CopyTo(rval, 0);
            return rval;
            //StringCollectionX.ToStringArray(col);
        }
        public static string[] Remove(string[] sa, string value)
        {
            List<string> list = new List<string>();
            foreach (string s in sa)
            {
                if (s != value)
                {
                    list.Add(s);
                }
            }
            return list.ToArray();
            /*
            int idx = Array.IndexOf(sa, value);
            if (idx < 0) return sa;
            string[] rval = new string[sa.Length-1];
            Array.Copy(sa, rval, idx);
            Array.Copy(sa, idx+1, rval, idx, sa.Length-idx-1);
            return rval;
             */
        }
    }
}
