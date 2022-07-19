using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace System
{
    /*
    A generic type with one type argument 
     Type.GetType("MyGenericType`1[MyType]") //NOT WORK...
 
    A generic type with two type arguments 
     Type.GetType("MyGenericType`2[MyType,AnotherType]")
 
    A generic type with two assembly-qualified type arguments 
     Type.GetType("MyGenericType`2[[MyType,MyAssembly],[AnotherType,AnotherAssembly]]")
 
    An assembly-qualified generic type with an assembly-qualified type argument 
     Type.GetType("MyGenericType`1[[MyType,MyAssembly]],MyGenericTypeAssembly")
 
    A generic type whose type argument is a generic type with two type arguments 
     Type.GetType("MyGenericType`1[AnotherGenericType`2[MyType,AnotherType]]")

    http://p3net.mvps.org/CHowSharp/2007/April/04222007.aspx 
    */
    public class DictionaryX //<K, V>
    {
        ~DictionaryX() { }

        private static void Main()
        {
            Type t = typeof(Dictionary<int, Int64>);
            //LogX.Debug(t.FullName);
            //LogX.Debug(t.Name);

            t = Type.GetType("System.Collections.Specialized.Dictionary`2[System.Int32,System.Int32]");
            //LogX.Debug("expected: False: " + (t == null)); //True

            t = Type.GetType("Dictionary`2");
            //LogX.Debug(t == null); //True

            t = Type.GetType("Dictionary<int, Int64>");
            //LogX.Debug(t == null); //True

            //Dictionary<string, string> d = new Dictionary<string, string>();
            Hashtable d = new Hashtable
            {
                //both Exc if key already exist.
                //buckets only for internal only.

                { "k1", "v1" }
            };
            //d.Add("k1", "v1");
            //d.Add("k1", "v12");  //Exc


            //LogX.Debug(d);
        }
        public static string ToString(IDictionary<string, object> nvs)
        {
            string format = "{0}={1}\n<br/>";
            return ToString(nvs, format);
        }
        public static string ToString(IDictionary<string, object> nvs, string format)
        {
            StringBuilder buf = new StringBuilder();
            List<string> list = nvs.Keys.ToList();
            list.Sort();
            foreach (String k in list)
            {
                string v = nvs[k].ToString(); ;
                buf.Append(string.Format(format, k, v));
            }
            return buf.ToString();
        }
        public static Dictionary<String, T> LowercaseKeys<T>(IDictionary<string, T> nvs)
        {
            var rval = new Dictionary<String, T>();
            foreach (String k in nvs.Keys)
            {
                var v = nvs[k];
                string kLower = k.ToLower();
                rval.Add(kLower, v);
            }
            return rval;
        }

        //public static Dictionary<T1, T2> LowerCaseKeys (Dictionary<T1, UTF32Encoding>)
        //{
        //    var nvs2 = new Dictionary<String, string>();
        //    List<string> list = nvs.Keys.ToList();
        //    foreach (String k in list)
        //    {
        //        nvs.Add(k.ToLower(), nvs[k]);
        //    }
        //    nvs = nvs2;
        //}

        public static string ToHtml_eg(Dictionary<string, string> dict)
        {
            var nvs = NameObjectCollection.ValueOf(dict.GetEnumerator());
            return nvs.ToHtml();
        }

        public static IDictionary<string, string> ToDictionary(NameValueCollection source)
        {
            //return source.AllKeys.ToDictionary(k => k, k => source[k]);
            return null;
        }

    }
}
