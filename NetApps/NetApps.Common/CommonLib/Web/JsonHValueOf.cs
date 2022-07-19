using System.Text.Json;
using System.Text.Json.Serialization;

namespace System
{
    public static class JsonHValue
    {
        //============================================================================================
        public static string Of(Object o, JsonSerializerOptions? options = null)
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
                MaxDepth = 4,
                PropertyNameCaseInsensitive = true, //for deserialize
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //IgnoreNullValues = false,//default=false
                //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //when serialize, ignore props/fields with null values
                DefaultIgnoreCondition = JsonIgnoreCondition.Never, //when serialize, ignore props/fields with null values, default=Never
                IncludeFields = false, //default=false

            };
            var ret = JsonSerializer.Serialize(o, options);
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
}