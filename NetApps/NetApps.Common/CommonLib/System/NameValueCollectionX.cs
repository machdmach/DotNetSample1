using System.Collections;
using System.Collections.Specialized;
using System.Linq;

using System.Text.RegularExpressions;
using System.Xml;

namespace System
{
    //[Serializable]
    //[DataContract]
    //public sealed class NameValuePair
    //{
    //    static void Test1()
    //    {
    //        var kv1 = new KeyValuePair<string, string>("aa", "11");
    //    }
    //    [DataMember]
    //    public String Name { get; set; }

    //    [DataMember]
    //    public String Value { get; set; }
    //}
    //===================================================================================================
    public sealed class NameValueCollectionX
    {
        //NameValueCollection toNavigateF3; //:
        //NameObjectCollectionBase toNavigateF3z; //:: ICollection, IEnumerable, ISerializable, IDeserializationCallback


        private static void Main()
        {
#if TEST
          int xx=3;
#endif
            NameValueCollection nvs = new NameValueCollection
            {
                ["a"] = "v1",
                ["a"] = "v2", //=nvs.Set (replace old val);

                [null] = "dd" //[] = dd; //OK
            };
            nvs.Get("afas");

            nvs.Add("a", null); //not added if null, makeSence b/c nvs.Get("a")=null
            nvs.Add("a", "22");
            nvs["a"] = "33";  //remove all oldVals and replaced by this newVal
            nvs["ab"] = "33ab";
            nvs["ad"] = "33ad";
            nvs["ac"] = "33ac";
            //         nvs.Add(null, "is null"); //OK blank.. for null, 

            //IXmlSerializable 
            //SerializableKeyValuePair
        }
        public static string? RegexGet(NameValueCollection nvs, string keyPattern)
        {
            //NameValueCollectionX.RegexGet(nvs, "fullName$"); //match xxddfullName
            //
            foreach (string key in nvs.Keys)
            {
                if (Regex.IsMatch(key, keyPattern))
                {
                    return nvs[key];
                }
            }
            return null;
        }
        //but can be used to store multiple values for each key

        //=================================================================
        public static NameValueCollection FromXml(String xml)
        {
            NameValueCollection nvs = new NameValueCollection();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            XmlNodeList nodes = xdoc.SelectNodes("/NameValueCollection/nv");
            //Log.Debug("k="+ nodes.Count);
            StringBuilder buf = new StringBuilder();
            foreach (XmlNode node in nodes)
            {
                string key = node.Attributes[0].Value;
                string value = node.FirstChild.Value;
                nvs.Add(key, value);
            }
            return nvs;
        }
        //=================================================================
        public static string ToXml(NameValueCollection nvc)
        {
            XmlDocument d = new XmlDocument();
            XmlDeclaration decl = d.CreateXmlDeclaration("1.0", "UTF-8", "no");
            d.AppendChild(decl);
            d.InsertAfter(d.CreateWhitespace("\n"), decl);

            XmlNode rootNode = d.CreateNode(XmlNodeType.Element, "NameValueCollection", null); //=<person xmlns="nsURI"/>
            d.AppendChild(rootNode);

            foreach (string key in nvc.Keys)
            {
                XmlNode node = d.CreateNode(XmlNodeType.Element, "nv", null);
                rootNode.AppendChild(node);

                XmlAttribute at = d.CreateAttribute("key");
                at.Value = key;
                node.Attributes.Append(at);

                XmlNode textNode = d.CreateNode(XmlNodeType.Text, null, null);
                textNode.Value = nvc[key];
                node.AppendChild(textNode);

                rootNode.InsertBefore(d.CreateWhitespace("  "), node);
                rootNode.InsertAfter(d.CreateWhitespace("\n"), node);
            }
            return d.OuterXml;
        }
        //************************************************************************
        public static bool IsNullOrEmpty(NameValueCollection nvs)
        {
            return nvs == null || nvs.Count == 0;
        }
        //************************************************************************
        public static String ToString(NameValueCollection nvs, string format)
        {
            StringBuilder buf = new();
            foreach (String k in nvs.Keys) //Keys: NameObjectCollectionBase.KeysCollection
            {
                string v = nvs[k]; //.ToString();//Not need cast for NVCol
                //buf.Append("[" + k + "] = [" + v + "]\n");
                buf.Append(string.Format(format, k, v));
            }
            return buf.ToString();
        }
        //************************************************************************
        public static bool Equals(NameValueCollection nvs, NameValueCollection nvs2)
        {
            if (nvs == null && nvs2 != null) { return false; }
            if (nvs != null && nvs2 == null) { return false; }

            if (nvs == null && nvs2 == null) { return false; }

            int k1 = nvs.Count;
            int k2 = nvs2.Count;
            if (k1 != k2) return false;

            for (int i = 0; i < k1; i++)
            {
                if (nvs.GetKey(i) != nvs2.GetKey(i)) { return false; }
                if (nvs.Get(i) != nvs2.Get(i)) { return false; }
            }
            return true;
        }
        //************************************************************************
        public static String ToString(NameValueCollection nvs)
        {
            if (nvs == null) { return "null"; }
            StringBuilder buf = new StringBuilder();
            foreach (String k in nvs.Keys) //Keys: NameObjectCollectionBase.KeysCollection
            {
                string v = nvs[k]; //.ToString();//Not need cast for NVCol
                buf.Append("[" + k + "] = [" + v + "]" + Environment.NewLine);
            }
            return buf.ToString();
        }
        //************************************************************************
        public static int GetHashCode(NameValueCollection nvs)
        {
            string s = ToString(nvs);
            return s.GetHashCode();
        }

        //************************************************************************
        //[Test]
        public void LowerCaseKeys__Test()
        {

            NameValueCollection nvs = new NameValueCollection
            {
                { "Key1", "Value1" },
                { "KEY2", "Value2" },
                { "key3", "Value3" }
            };
            nvs = LowerCaseKeys(nvs);
            //Assert.AreEqual("key1", nvs.Keys[0]);
            //Assert.AreEqual("key2", nvs.Keys[1]);
            //Assert.AreEqual("key3", nvs.Keys[2]);
            //Assert.AreEqual("Value3", nvs["key3"]);
            //Log.Debug("nvs=" + nvs);
        }
        public static NameValueCollection LowerCaseKeys(NameValueCollection nvs)
        {
            if (nvs == null) return null;
            NameValueCollection rval = new();
            for (int i = 0; i < nvs.Count; i++)
            {
                rval.Add(nvs.Keys[i].ToLower(), nvs[i]);
                //Log.Debug(nvs[i].ToLower());
            }
            return rval;
        }
        public static NameValueCollection TitleCaseKeys(NameValueCollection nvs)
        {
            if (nvs == null) return null;
            NameValueCollection rval = new NameValueCollection();
            for (int i = 0; i < nvs.Count; i++)
            {
                //rval.Add(StringX.ToTitleCase(nvs.Keys[i]), nvs[i]);
                if (RunThis) throw new NotImplementedException();
                //Log.Debug(nvs[i].ToLower());
            }
            return rval;
        }
        public static Hashtable ToHashtable(NameValueCollection nvs)
        {
            if (nvs == null) return null;
            Hashtable rval = new Hashtable();
            foreach (string key in nvs.Keys)
            {
                string szkey = key;
                rval.Add(szkey, nvs[szkey]);
            }
            return rval;
        }

        public static NameValueCollection Clone(NameValueCollection nvs)
        {
            if (nvs == null) return null;

            NameValueCollection rval = new NameValueCollection();
            foreach (string key in nvs.Keys)
            {
                string szkey = key;
                rval.Add(szkey, nvs[szkey]);
            }
            return rval;
        }
        public static string GetOrDie(NameValueCollection nvs, string key)
        {
            string rval = nvs[key];
            if (rval == null)
            {
                throw new Exception("[" + key + "] key found in nvs, avail keys=" + nvs.Keys);
            }
            if (rval == "")
            {
                throw new Exception("value for [" + key + "] key is blank");
            }
            return rval;
        }
        public static string Get(NameValueCollection nvs, string key, string defaultVal)
        {
            string rval = nvs[key];
            if (rval == null)
            {
                rval = defaultVal;
            }
            if (rval == "")
            {
                //throw new Exception("value for [" + key + "] key is blank");
            }
            return rval;
        }
        public static string GetIgnoreCase(NameValueCollection nvs, string key)
        {
            string keyLower = key.ToLower();
            string rval = null;
            foreach (var k in nvs.AllKeys)
            {
                if (k != null && k.ToLower() == keyLower)
                {
                    rval = nvs[k];
                }
            }
            return rval;
        }
        public static string GetCI_orError(NameValueCollection nvs, string key, string errMesg = null)
        {
            string keyLower = key.ToLower();
            string ret = null;
            foreach (var k in nvs.AllKeys)
            {
                if (k != null && k.ToLower() == keyLower)
                {
                    ret = nvs[k];
                }
            }
            if (ret == null)
            {
                errMesg ??= string.Format("No key found for {0}, avail keys={1}", key, String.Join(", ", nvs.AllKeys));
                throw new Exception(errMesg);
            }
            return ret;
        }

        public static int GetInt(NameValueCollection nvs, string key)
        {
            string rval = nvs[key];
            if (rval == null)
            {
                throw new Exception("[" + key + "] key found in nvs, avail keys=" + nvs.Keys);
            }
            if (rval == "")
            {
                throw new Exception("value for [" + key + "] key is blank");
            }
            return Int32.Parse(rval);
        }
    }
}