using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace Libx
{
    //NameObjectCollectionBase is not IDictionary
    public class NameObjectCollection : NameObjectCollectionBase, ICloneable
    {
        private static void Main()
        {
            String s = "abcDDD";
            NameObjectCollection nvs = new NameObjectCollection()
            .Add("aa", s) //
            .Add("Length", s.Length) //
            .Add("ToUpper()", s.ToUpper())
            .Add("ToLower()", s.ToLower())
            .Add("StartsWith(\"abc\")", s.StartsWith("abc"))
            .Add("Replace", s.Replace("DD", "bb"))
            .Add("Substring", s.Substring(2)) //

            .This();
            //Log.Debug(nvs.ToStringX());         
        }
        public object Clone()
        {
            //return base.MemberwiseClone(); //not work
            NameObjectCollection rval = new NameObjectCollection();
            foreach (string key in Keys)
            {
                rval.Add(key, this[key]);
            }
            return rval;
        }
        //---------------------------------------
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
        public void RemoveAt(int i)
        {
            BaseRemoveAt(i);
        }
        public void AddAt(int i, object o)
        {
            BaseSet(i, o);
        }
        public void ContainsKey(String key)
        {
            throw new NotImplementedException("z");
        }
        public string[] AllKeys
        {
            get
            {
                string[] rval = new string[Count];
                //NameValueCollection c; c.AllKeys
                int i = 0;
                foreach (string key in Keys)
                {
                    rval[i++] = key;
                }
                return rval;
            }
        }

        public object Get(int index)
        {
            return BaseGet(index);
        }
        public object Get(string name)
        {
            return BaseGet(name);
        }
        public object this[int index]
        {
            get => BaseGet(index);
            set => BaseSet(index, value);
        }
        public object this[string name]
        {
            get => BaseGet(name);
            set => BaseSet(name, value);
        }
        public NameObjectCollection This() { return this; }
        public NameObjectCollection Add(string name, object value)
        {
            BaseAdd(name, value);
            return this;
        }
        public void Add(NameObjectCollection nvs)
        {
            foreach (string key in nvs.Keys)
            {
                BaseAdd(key, nvs[key]);
            }
        }

        //===================================================================================================
        public static NameObjectCollection LowerCaseKeys(NameObjectCollection nvs)
        {
            if (nvs == null) return null;
            var rval = new NameObjectCollection();
            for (int i = 0; i < nvs.Count; i++)
            {
                rval.Add(nvs.Keys[i].ToLower(), nvs[i]);
                //Log.Debug(nvs[i].ToLower());
            }
            return rval;
        }

        //===================================================================================================
        public NameObjectCollection Sort()
        {
            //List<KeyValuePair<String, Object>>
            var nvs = this;
            if (nvs == null) return null;
            var rval = new NameObjectCollection();
            List<String> keys = nvs.Keys.OfType<String>().ToList();
            keys.Sort();
            foreach (String key in keys)
            {
                rval.Add(key, nvs.Get(key));
            }
            return rval;
        }

        //===================================================================================================
        public NameObjectCollection TruncateKeyPrefix(string prefix)
        {
            NameObjectCollection rval = new NameObjectCollection();
            int prefixLen = prefix.Length;
            foreach (String k in Keys) //Keys: NameObjectCollectionBase.KeysCollection
            {
                string newK = k;
                if (k.StartsWith(prefix))
                {
                    newK = k.Substring(prefixLen);
                }
                rval.Add(newK, this[k]);
            }
            return rval;
        }

        //===================================================================================================
        //public NameObjectCollection GetAll_WithKeyNotPrefixWith(string prefix)
        public NameObjectCollection GetAll_WithKeyPrefixWith(string prefix)
        {
            //NameValueCollection nvs;         
            NameObjectCollection rval = new NameObjectCollection();
            foreach (String k in Keys) //Keys: NameObjectCollectionBase.KeysCollection
            {
                if (k.StartsWith(prefix))
                {
                    rval.Add(k, this[k]);
                }
            }
            return rval;
        }
        //public string ToSqlUpdateStatement(string tab, string wherec)
        //{
        //    StringBuilder buf = new StringBuilder();
        //    buf.Append("update " + tab + " set\n");
        //    foreach (String k in this.Keys) //Keys: NameObjectCollectionBase.KeysCollection
        //    {
        //        Object o = this[k];
        //        if (o is DateTime)
        //        {
        //        }
        //        string val = o.ToString();
        //        val = "'" + val + "'";
        //        buf.Append(k + " = " + val);
        //        buf.Append(",\n");
        //    }
        //    buf.Append(" where " + wherec);
        //    return buf.ToString();
        //}


        public static NameObjectCollection ValueOf(DataRow dr)
        {
            //Log.Debug("addRows for nvs: " + NameValueCollectionX.ToString(nvs));
            NameObjectCollection rval = new NameObjectCollection();
            DataTable dt = dr.Table;
            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                object item = dr.ItemArray[i];
                string colName = dt.Columns[i].ColumnName.ToLower();
                rval.Add(colName, item);
            }
            return rval;
        }
        //===================================================================================================
        public static NameObjectCollection ValueOf(NameValueCollection nvs)
        {
            NameObjectCollection rval = new NameObjectCollection();
            foreach (string key in nvs.Keys)
            {
                rval.Add(key, nvs[key]);
            }
            return rval;
        }
        //===================================================================================================
        public static NameObjectCollection ValueOf(IDictionaryEnumerator en)
        {
            NameObjectCollection rval = new NameObjectCollection();
            while (en.MoveNext())
            {
                rval.Add(en.Key.ToString(), en.Value);
            }
            return rval;
        }

        //===================================================================================================
        public NameValueCollection ToNameValueCollection()
        {
            var nvs = this;
            var rval = new NameValueCollection();
            foreach (string key in nvs.Keys)
            {
                var val = nvs[key];
                string? sval = val?.ToString();
                //string sval = (val == null) ? null : val.ToString()+ ", "+ val.GetType().Name;
                rval.Add(key, sval);
            }
            return rval;
        }
        ////============================================================================================
        //public static NameObjectCollection GetPublicProperties(Object o)
        //{
        //    bool includeNullableIndicator = true;
        //    //Dictionary<string, object> nvs = new Dictionary<string, object>();
        //    var nvs = new NameObjectCollection();
        //    Type t = o.GetType();

        //    BindingFlags bindingFlags = BindingFlags.Public  | BindingFlags.Instance | BindingFlags.GetProperty;

        //    PropertyInfo[] props = t.GetProperties(bindingFlags);
        //    //props = t.GetProperties();
        //    foreach (PropertyInfo p in props)
        //    {
        //        string propName = p.Name;
        //        if (includeNullableIndicator)
        //        {
        //            if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        //            {
        //                propName = propName + "?";
        //            }
        //        }

        //        object val;
        //        try
        //        {
        //            val = p.GetValue(o, null);
        //            //[ArgumentException: An item with the same key has already been added.]
        //        }
        //        //[TargetParameterCountException: Parameter count mismatch.]
        //        catch (TargetParameterCountException ex)
        //        {
        //            val = "Error: TargetParameterCountException: " + ex.Message + ", propName: " + propName + ", propType=" + p.PropertyType;
        //        };
        //        nvs.Add(propName, val);
        //    }
        //    //nvs = nvs.Sort();
        //    return nvs;
        //}

        //************************************************************************
        public string ToHtml(ToHtmlOptions? options = null)
        {
            var nvs = ToNameValueCollection();
            string s = HtmlValue.Of(nvs, options);
            return s;
        }
        //===================================================================================================
        public String ToStringValue(string format = "{0} = {1}<br>\n")
        {
            //format = "[{0}] = [{1}]\n";
            var nvs = this;
            StringBuilder buf = new StringBuilder();
            foreach (String key in nvs.Keys) //Keys: NameObjectCollectionBase.KeysCollection
            {
                object o = nvs.Get(key);
                if (o == null)
                {
                    //throw new Exception("no value found for key: "+ k);
                    o = "null";
                }
                string val = o.ToString();//Not need cast for NVCol
                //buf.Append("[" + k + "] = [" + v + "]\n");
                buf.AppendFormat(format, key, val);
            }
            return buf.ToString();
        }
    }
}
