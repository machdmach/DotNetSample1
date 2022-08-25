using System.Text.Json;
using System.Text.Json.Serialization;
namespace Libx;
public class MyJsonConvert
{
    public static T DeserializeObject<T>(string jsonText, JsonSerializerOptions options = null)
    {
        Type type = typeof(T);
        var ret = DeserializeObject(jsonText, type, options);
        return (T)ret;
    }
    public static object DeserializeObject(string jsonText, Type type, JsonSerializerOptions options = null)
    {
        //T res = (T)JsonConvert.DeserializeObject(str, type);
        options ??= new JsonSerializerOptions() //#json
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            MaxDepth = 9,
            PropertyNameCaseInsensitive = true, //for deserialize
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //IgnoreNullValues = false,//default=false
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //when serialize, ignore props/fields with null values
            DefaultIgnoreCondition = JsonIgnoreCondition.Never, //when serialize, ignore props/fields with null values, default=Never
            IncludeFields = false, //default=false

        };
        //JsonDocument
        //var ret = JsonSerializer.Serialize(o, options);
        object ret = JsonSerializer.Deserialize(jsonText, type, options);
        return ret;
    }
    public static T DeserializeObjectFromFile<T>(string pfname, JsonSerializerOptions options = null)
    {
        string jsonText = System.IO.File.ReadAllText(pfname);
        MOutput.WriteLine("DeserializeObjectFromFile: " + pfname);
        Type type = typeof(T);
        var ret = DeserializeObject(jsonText, type, options);
        return (T)ret;
    }

    //===================================================================================================
    public static void SerializeObjectToFile(object obj, string pfname, JsonSerializerOptions options = null)
    {
        var jsonText = MyJsonConvert.SerializeObject(obj);
        System.IO.File.WriteAllText(pfname, jsonText);
        MOutput.WriteLine("SerializeObjectToFile: " + pfname);
    }
    public static string SerializeObject(Object obj, JsonSerializerOptions? options = null)
    {
        //options ??= new ToHtmlOptions();
        //options.Caption += String.Format(" ({0})", o.GetType().Name);

        //if (o == null)
        //{
        //    return "Object is null";
        //}
        //var nvs = ObjectX.GetPropFieldNameValues(o);
        //if (options.OrderByPropNames)
        //{
        //    nvs = nvs.Sort();
        //}
        //string rval = nvs.ToHtml(options);
        options ??= new JsonSerializerOptions() //json
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            MaxDepth = 9,
            PropertyNameCaseInsensitive = true, //for deserialize
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //IgnoreNullValues = false,//default=false
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //when serialize, ignore props/fields with null values
            DefaultIgnoreCondition = JsonIgnoreCondition.Never, //when serialize, ignore props/fields with null values, default=Never
            IncludeFields = false, //default=false

        };
        var ret = JsonSerializer.Serialize(obj, options);
        return ret;
    }
    static void eg1()
    {
        //var serializer = new JsonSerializer();
        //var stringWriter = new StringWriter();
        //using (var writer = new JsonTextWriter(stringWriter))
        //{
        //    writer.QuoteName = false;
        //    serializer.Serialize(writer, options);
        //}
        //var json = stringWriter.ToString();
    }
}