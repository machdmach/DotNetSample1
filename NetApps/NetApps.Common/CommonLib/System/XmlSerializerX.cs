using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace System
{
    //<Class1 p1="abc"> only if p1 has [XmlAttribute]
    //For a class to be serialized with XML serialization, class must be public
    //and has a parameterless constructor.

    //.InvalidOperationException: To be XML serializable, types which inherit from 
    //ICollection must have an implementation of Add(System.Object) at all levels 
    //of their inheritance hierarchy. System.Collections.Specialized.NameObjectCollection 
    //does not implement Add(System.Object).

    //IDeserializationCallback works only for Runtime Serializers, i.e. SoapFormatter
    //and BinaryFormtter, and CustomFormtter:ISerializable

    //XmlSerializer doens't work for IDeserializationCallback, ISerializable..., [OnSerialized]
    //It is not really Serializable, just read and write xml stuff.... 
    //XML Serialization doesn't support serialization events.

    public class XmlSerializerX
    {
        private static void Test1()
        {
            ShoppingList o = new ShoppingList();
            o.AddItem(new Item("table", 123.45));
            o.AddItem(new Item("laptop", 343.44));

            Type t = typeof(ShoppingList);

            //LogX.Debug_Pre(File.ReadAllText(file));


            //LogX.Debug(o);
        }
        //static string file = @"c:\temp\XmlSerializer_out.xml";

        public static void SerializeObject_ToFile(object obj, string pfname)
        {
            Type objType = obj.GetType();
            using (Stream stream = new FileStream(pfname, FileMode.Create))
            {
                XmlSerializer xs = new XmlSerializer(objType);
                xs.Serialize(stream, obj);
                stream.Close();
            }
        }
        public static T DeserializeObject_FromFile<T>(string pfname) where T : class
        {
            T rval;
            Type objType = typeof(T);
            //using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (FileStream fs = new FileStream(pfname, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xs = new XmlSerializer(objType);
                rval = (T)xs.Deserialize(fs);
                fs.Close();
            }
            return rval;
        }
        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }
        public object Deserialize(string input, Type toType)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(toType);

            using (StringReader sr = new StringReader(input))
                return ser.Deserialize(sr);
        }
    }


    [XmlRoot("shoppingList")]
    public class ShoppingList : IDeserializationCallback
    {
        public void OnDeserialization(object sender)
        {
            throw new Exception("Never works for XmlSerializer");
        }

        private readonly ArrayList listShopping;

        public ShoppingList()
        {
            listShopping = new ArrayList();
        }

        [XmlElement("item")]
        public Item[] Items
        {
            get
            {
                Item[] items = new Item[listShopping.Count];
                listShopping.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Item[] items = value;
                listShopping.Clear();
                foreach (Item item in items)
                    listShopping.Add(item);
            }
        }
        public string newMember = "newMemberOK";
        // [XmlText] //causing identation not in place. significant space improtant.
        public string xmlText = "XmlText val";

        //[XmlAnyAttribute]  public string xmlAnyAttribute = "xmlAnyAttribute";
        //Exception, not For non-array types

        //[XmlAnyElement]   public string xmlAnyElement = "XmlAnyElement val";
        //Exception for XmlNode only

        //[XmlEnum]   public FileMode fileMode = FileMode.OpenOrCreate;
        //?? Exception For non-array types, you may use the following attributes: XmlAttribute, XmlText, XmlElement, or XmlAnyElement.


        public int AddItem(Item item)
        {
            return listShopping.Add(item);
        }







    }

    // Items in the shopping list
    public class Item
    {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("price")]
        public double price;

        public Item()
        {
            nvs.Add("k1", "v1");
        }

        public Item(string Name, double Price)
        {
            name = Name;
            price = Price;
        }
        public byte property1 = 12;
        [XmlIgnore]
        public NameValueCollection nvs = new NameValueCollection();


    }
}
