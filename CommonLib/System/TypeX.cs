using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace Libx;
///<see cref="TypeInfoX"/>
public class TypeX
{
    //IsNullable(typeof(int?)) ==> true
    //IsNullable(typeof(int)) ==> false
    public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

    public static string ToString(Type t)
    {
        NameObjectCollection nvs = new NameObjectCollection
        {
            { "FullName", t.FullName }, //
            { "BaseType", t.BaseType.FullName }, //
            { "AssemblyQualifiedName", t.AssemblyQualifiedName }, //
            { "Name", t.Name }, //
            { "Module", t.Module.FullyQualifiedName } //
        };
        return nvs.ToStringValue();


    }

    //static void Main()
    private static void Main()
    {
        //                    System2.Diagnostics.FileStreamTraceListener,CommonLib

        //C2 c2 = new C2(3);

        //ValueType vt;

        //NameValueCollection nvs = new NameValueCollection();
        //Type t = typeof(AppDomain);
        // t = typeof(AppDomainSetup);
        //String s = "abc";

        Type t = "".GetType();


        Binder binder = Type.DefaultBinder;

        MethodInfo mi = t.GetMethod("IndexOf", BindingFlags.Public | BindingFlags.Instance, binder, new Type[] { typeof(string) }, null);
        //Assert.IsNotNull(mi, "IndexOf method not found");

        object o = "azzbc";

        //TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter);
        //delegate TOutput Converter<TInput, TOutput>(TInput input);
        object[] @params = Array.ConvertAll(new int[] { 1 }, delegate (int x) { return (object)x; });
        object[] @params2 = Array.ConvertAll(new string[] { "b" }, delegate (string x) { return (object)x; });

        //int index = (int)mi.Invoke(o, BindingFlags.InvokeMethod, binder, @params2, CultureInfo.CurrentCulture);
        //LogX.Debug("Index=" + index);

        //process_MemberInfo(t);
        //int x = s.Length;
        //MethodInfo mi;



    }

    private T Generic1<T>() where T : struct { return default(T); }

    private T Generic1<T>(T t) where T : struct { return default(T); }

    //T Generic1() where T : struct { return default(T); } //Error: not non-generic declaration
    private void Generic1<T, U>() where T : class, new() { }//new must be last param
                                                            //void Generic3<T>() where T : ValueType { } //err Can't be special class.



    private interface I1
    {
        void m2();
        //const int x = 3; //Interface can't contain fields.
    }

    private class C1 //:x2
    {
        public int xx = 3;
        public C1(int x) { }
        public static int cw(String s)
        {
            //LogX.Debug(s);
            return 1;
        }

        private static readonly int asdf = cw("Class1 static field1 init");
        static C1() { cw("Class1 class static init"); }

        private static readonly int asdf2 = cw("Class1 static field2 init");
    }

    private class C2 : C1
    {
        public C2(int x) : base(x) { }

        private static readonly int asdf = cw("Class2 static field1 init");
        static C2() { cw("Class2 class static init"); }

        private static readonly int asdf2 = cw("Class2 static field2 init");
    }

    //****************************************************************
    public static void process_MemberInfo(Type t)
    {
        MemberInfo[] mbis = t.GetMembers();
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("MemberTypes")); //, typeof(MemberTypes));
        dt.Columns.Add(new DataColumn("AccessModifiers", typeof(string)));
        dt.Columns.Add(new DataColumn("ReturnType", typeof(Type)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        //DbColumn c2;

        String[] commonNames = new string[] { "GetType", "GetHashCode", "Equals", "ToString" };
        ArrayList allNames = new ArrayList();

        NameValueCollection nvs = new NameValueCollection();
        StringBuilder nvsBuf = new StringBuilder();

        foreach (MemberInfo mbi in mbis)
        {
            string name = mbi.Name;
            if (Array.IndexOf(commonNames, name) >= 0) { continue; }

            if (allNames.IndexOf(name) >= 0) { continue; } //avoid duplicates
            allNames.Add(name);

            MemberTypes memberTypes = mbi.MemberType;
            string memberTypesStr = "?";
            string accessModifiers = "?";
            Type returnType = typeof(object);
            if ((memberTypes & MemberTypes.Method) != 0)
            {
                MethodInfo mi = (MethodInfo)mbi;
                if (name.StartsWith("get_"))
                {
                    name = name.Substring(4);
                }
                else
                {
                    name = name + "()";
                }
                if (mi.IsPublic)
                {
                    accessModifiers = "public";
                }
                memberTypesStr = Enum.GetName(typeof(MemberTypes), MemberTypes.Method);

                returnType = mi.ReturnType;
                //if (returnType == typeof(string)) returnType = null;
            }
            if (returnType == typeof(void)) { continue; }
            if (accessModifiers != "public") { continue; }
            dt.Rows.Add(memberTypesStr, accessModifiers, returnType, name);

            nvs.Add('"' + name + (returnType.Name.EndsWith("String") ? "" : ":" + returnType.Name) + "\",", name);
        }
        //Log.Debug(DataTableX.ToString(dt));

        string format = "nvs.Add({0,-30} o.{1}); //\n";
        //LogX.Debug(NameValueCollectionX.ToString(nvs, format));
        File.WriteAllText(@"c:\temp.text", NameValueCollectionX.ToString(nvs, format));
    }
    //****************************************************************
    public static void process_MethodInfo(Type t)
    {
        MethodInfo[] mis = t.GetMethods();
        foreach (MethodInfo mi in mis)
        {
            Type returnType = mi.ReturnType;
            string name = mi.Name;
            //LogX.Debug("name=" + name);

            if (returnType == typeof(void)) break;
            if (name.StartsWith("get_"))
            {
                //LogX.Debug("get method: "+ name);
            }
        }
    }
    //****************************************************************
    public static void process_PropertyInfo(Type t)
    {
        PropertyInfo[] pis;
        pis = t.GetProperties(BindingFlags.Public);
        //LogX.Debug(pis.Length); //=0;

        pis = t.GetProperties();
        //LogX.Debug(pis.Length); //=2;
        PropertyInfo pi = pis[0];
        foreach (PropertyInfo pi2 in pis)
        {
            //LogX.Debug(pi2.PropertyType.Name);
            //LogX.Debug(pi2.MemberType.GetType()); //Property, Method...
            //LogX.Debug(pi2.Name);  //Chars:char, Length:Int32
        }
    }

    //****************************************************************

    public static void Main2(string[] args)
    {
        NameValueCollection nvs = new NameValueCollection();
        Type t = typeof(string);


        string s = "abcdAAAb";

        s.ToLower();

        string propertyName = "Length";
        PropertyInfo pi = t.GetProperty(propertyName);
        //LogX.Debug(pi == null);
        //LogX.Debug(pi.GetValue(s, null));

        string methodName = "ToLower";
        MethodInfo mi = t.GetMethod(methodName, new Type[] { });
        //MethodInfo mi = t.GetMethod(methodName, null); //err Value can't be null


        //mi.Invoke(null, new object[] {null});//OK
        //mi.Invoke(null, new object[] { args });  
        //LogX.Debug(mi.Name);
        string val = "uninit";
        //val = mi.Invoke(s, new object[] { }).ToString();
        val = mi.Invoke(s, null).ToString();

        //LogX.Debug(val);

        //nvs.Add(".Code", dbe.Code.ToString());
        //nvs.Add(".Message", dbe.Message);
        //Log.Debug(NameValueCollectionX.ToString(nvs));
    }



    //===================================================================================================
    public static string GetShortName(Type type)
    {
        string rval = null;
        bool isNullable = false;
        bool isGenericList = false;
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //Nullable<Int32>
                isNullable = true;
            }
            else
            {
                isGenericList = true;
            }
            type = type.GetGenericArguments()[0];
        }
        rval = TypeX.AbbreviateTypeName(type.Name);
        if (isNullable)
        {
            rval += "?";
        }
        else if (isGenericList)
        {
            rval = string.Format("List<{0}>", rval);
        }
        return rval;
    }

    //===================================================================================================
    public static string AbbreviateTypeName(String unabbreviate)
    {
        string rval = unabbreviate;
        switch (unabbreviate)
        {
            case "Boolean":
                rval = "bool";
                break;
            case "Int16":
                rval = "short";
                break;
            case "Int32":
                rval = "int";
                break;
            case "Int64":
                rval = "long";
                break;

            case "String":
                rval = "string";
                break;
        }
        return rval;
    }
}
