using System.Collections;
using System.Diagnostics;

//Multi-Dimensional Arrays: Rectangular and Jagged.
//Retangular: is a single array with 2 or more dimensions

namespace Libx
{
    /// <summary>
    /// <seealso cref="Array"/>
    /// </summary>
    public class ArrayX
    {
        private static void Main()
        {
            int[,] squareArray = { { 1, 2, 3 }, { 4, 5, 6 } };
            //log.Debug(squareArray.GetLowerBound(1)); //= 0, of second dimension
            for (int i = 0; i < squareArray.GetLength(0); i++)
            {
                for (int j = 0; j < squareArray.GetLength(1); j++)
                {
                    //   Console.WriteLine(squareArray[i, j]);
                }
            }
            int[][] jag1 = new int[2][];
            int[][] jag = new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8, 9, 10 } };

            for (int i = 0; i < jag.GetLength(0); i++)
            {
                for (int j = 0; j < jag[i].GetLength(0); j++)
                {
                    //     Console.WriteLine(jag[i][j]);
                }
            }



            {
                string[] emptyArray = new string[0];
                //log.Debug("new string[0] len=" + emptyArray.Length);
                foreach (string s in emptyArray)
                {
                    //log.Debug("this should never print");
                }
            }
            {
                ArrayList al = new ArrayList();
                Array ar = al.ToArray(typeof(string));
                string[] sa = (string[])ar;
                //log.Debug("empty string[] len=" + sa.Length);
            }
            {
                string[] sa = { "a", "ab", "cc" };
                //log.Debug("sa2 = " + string.Join(",", sa));
                m1(sa);
                //m1({"11", "bb", "cc"}); //error
                m1(new string[] { "11", "bb", "cc" });
            }
            Debug.WriteLine("asdfasdf debug ok");
            //log.Debug("Arrays done");
        }

        private static void m1(string[] sa)
        {
            //log.Debug("m1, sa = " + string.Join(",", sa)); 
        }

        public static int IndexOf_cm(Array ar, string s)
        {
            return Array.IndexOf(ar, s);
        }

        //****************************************************************
        public static bool zzEquals_StringValues(Array a1, Array a2)
        {
            string s1 = ToString(a1);
            string s2 = ToString(a2);
            //Log.Debug(s1);
            //Log.Debug(s2);
            return ToString(a1) == ToString(a2);
        }
        //****************************************************************
        public static bool Equals<T>(T[] a1, T[] a2)
        {
            if (a1.Length != a2.Length) { return false; }

            for (int i = 0; i < a1.Length; i++)
            {
                T o1 = a1[i];
                T o2 = a2[i];
                //if (o1 != o2) { return false; } //err: op != can't be applied to type T
                //if (o1 == o2) { return false; } //err: op == can't be applied to type T
                //if (new Object() == new String('a', 100)) ; //OK, Value Type
                if (!o1.Equals(o2)) { return false; }
            }
            return true;
        }
        public static ArrayList ToArrayList(Array arr)
        {
            ArrayList rval = new ArrayList(arr.Length);
            foreach (object o in arr)
            {
                rval.Add(o);
            }
            return rval;
        }


        //****************************************************************

        public static String ToString(Array a)
        {
            return ToString(a, ", ");
        }


        public static String ToString(Array a, string seperatorInfix)
        {
            //string.Join(', ', colNames[]);

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            foreach (Object o in a)
            {
                buf.Append(o.ToString());
                buf.Append(seperatorInfix);
            }
            if (buf.Length > 1)
            {
                buf.Remove(buf.Length - seperatorInfix.Length, seperatorInfix.Length);
            }
            buf.Append("]");
            return buf.ToString();
        }
        public static String ToString(Array a, String itemPrefix, String itemSuffix)
        {
            return ToString(a, itemPrefix, itemSuffix, "", "");
        }
        public static String ToString(Array a, String itemPrefix, String itemSuffix, String prefix, String suffix)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(prefix);
            foreach (object o in a)
            {
                buf.Append(itemPrefix);
                buf.Append(o.ToString());
                buf.Append(itemSuffix);
            }
            buf.Append(suffix);
            return buf.ToString();
        }

    }
}
