using System.Data;
using System.Reflection;

namespace Libx
{
    public class DataFieldDef
    {
        public string Name { get; set; }
        public string Label { get; set; } //DisplayName
        public string jsReferenceType { get; set; }
        public string jsPrimitiveType { get; set; } // six primitive types as undefined , null , boolean , number , string , and symbol , and a reference type object

        public String TypeShortName { get; set; }
        public Type Type { get; set; }
        public bool Required { get; set; }
        public int MinLen { get; set; }
        public int MaxLen { get; set; }
        //public string JSName { get; set; }
        public object DefaultVal { get; set; }
        public object Value { get; set; }

        public Type PropertyType => Type;

        //===================================================================================================
        public static DataFieldDef GetDataFieldDef(PropertyInfo prop)
        {
            string fieldName = prop.Name;
            Type fieldType = prop.PropertyType;
            return GetDataFieldDef(fieldName, fieldType);
        }
        //===================================================================================================
        public static DataFieldDef GetDataFieldDef(string fieldName, Type fieldType, bool isNullable = true)
        {

            var def = new DataFieldDef
            {
                Name = fieldName,

                TypeShortName = TypeX.GetShortName(fieldType),
                Label = DataFieldDefHelper.FieldNameToLabel(fieldName)
            };


            //def.JSName = Char.ToLower(pName[0]) + pName.Substring(1);
            //def.JSName = pName;

            //var type = prop.PropertyType;

            string jsRefType;
            def.Required = false;
            //if (fieldType == typeof(string))
            //{
            //    jsType = "String";
            //}
            //else
            //{
            //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Data_structures
            if (fieldType.IsGenericType &&
                    fieldType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                fieldType = Nullable.GetUnderlyingType(fieldType);
                def.Required = false;
            }
            if (!isNullable)
            {
                def.Required = true;
            }
            //------
            if (fieldType == typeof(DateTime))
            {
                jsRefType = "Date";
            }
            else if (fieldType == typeof(Boolean))
            {
                jsRefType = "Boolean";
            }
            else if (fieldType == typeof(Int32) || fieldType == typeof(Int64)
                || fieldType == typeof(Decimal) || fieldType == typeof(Double) || fieldType == typeof(Single))
            {
                jsRefType = "Number";
            }
            else
            {
                jsRefType = "String"; //have to be titlecase, String, for eval('String') to work in js code getFieldDefs
            }
            //}
            //jsType = jsType.ToLower(); // def.Type = eval(p.value); //tslint:disable-line
            def.jsReferenceType = jsRefType;
            def.jsPrimitiveType = jsRefType == "String" ? "string" :
                                  jsRefType == "Number" ? "number" :
                                  jsRefType == "Boolean" ? "boolean" :
                                  jsRefType;

            def.Type = fieldType;

            return def;
        }
        //===================================================================================================
        public static List<DataFieldDef> GetDataFieldDefs(Type type)
        {
            var defs = new List<DataFieldDef>();

            BindingFlags targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty; // | BindingFlags.DeclaredOnly;
            PropertyInfo[] targetProps = type.GetProperties(targetBindingFlags);

            foreach (PropertyInfo prop in targetProps)
            {
                var def = GetDataFieldDef(prop);
                defs.Add(def);
            }
            return defs;
        }
        //===================================================================================================
        public static List<DataFieldDef> GetDataFieldDefs(Dictionary<string, object> dict)
        {
            var defs = new List<DataFieldDef>();

            //BindingFlags targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.DeclaredOnly;
            //PropertyInfo[] targetProps = type.GetProperties(targetBindingFlags);

            var en = dict.GetEnumerator();
            while (en.MoveNext())
            {
                KeyValuePair<string, object> nv = en.Current;
                defs.Add(GetDataFieldDef(nv.Key, nv.Value.GetType()));
            }
            return defs;
        }
        //===================================================================================================
        public static List<DataFieldDef> GetDataFieldDefs(string mssqlScript)
        {
            var defs = new List<DataFieldDef>();
            var s = mssqlScript;
            var lines = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Trim();

                var tokens = line.Split("[]{}\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    string name = tokens[0];
                    string sqlType = tokens[1];
                    Type netType = DbTypeConvertor.ToNetType(sqlType);
                    bool nullable = !line.ToUpper().Contains("NOT NULL");
                    var def = GetDataFieldDef(name, netType, nullable);
                    defs.Add(def);
                }
            }
            return defs;
        }
        //===================================================================================================
        public static List<DataFieldDef> GetDataFieldDefs_mssqlScript_Test()
        {
            string sql = @"
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [VersionNumberFormat] [tinyint] NULL,
    [UniqueFlightId] [varchar](20) NOT NULL,
    [DateOfFlightInfoValidity] [datetime2](7) NULL,
    [MessageType] [tinyint] NULL,
    [FlightId] [varchar](20) NOT NULL,
    [CallSign] [varchar](20) NULL,
    [TailNumber] [varchar](20) NULL,
    [AircraftType] [varchar](20) NULL,
    [ModeSUniqueId] [varchar](10) NULL,
    [OriginAirport] [varchar](4) NULL,
    [DestinationAirport] [varchar](4) NULL,
    [EstimatedTimeOfDeparture_hhmm_UTC] [varchar](4) NULL,
    [EstimatedTimeOfArrival_hhmm_UTC] [varchar](4) NULL,
    [TrackingSource] [tinyint] NULL,
    [LastUpdatedDate] [datetime2](7) NULL,
    [LastUpdatedBy] [varchar](20) NULL,
    [CreatedDate] [datetime2](7) NOT NULL,
    [CreatedBy] [varchar](20) NOT NULL,
";

            var ret = GetDataFieldDefs(sql);
            //MOutput.WriteHtmlTable("defs", ret);
            return ret;
        }

    }
}
