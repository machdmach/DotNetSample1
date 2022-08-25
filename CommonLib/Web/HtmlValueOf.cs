using System.Collections.Specialized;
using System.IO;
namespace Libx;
public class ToHtmlOptions
{
    public String Caption { get; set; }
    public string DataFormat { get; set; }
    public bool ReverseRows { get; set; }
    public bool EncodeHtml { get; set; }
    public bool OrderByPropNames { get; set; }
    public bool ShowRowNumber { get; set; } = true;
    public bool UnpivotOnOneRow { get; set; }
}

public static class HtmlValue
{
    public static string OfAny(Object o, ToHtmlOptions? options = null)
    {
        options ??= new ToHtmlOptions();
        options.Caption += String.Format(" ({0})", o.GetType().Name);

        if (o == null || o == DBNull.Value)
        {
            return "~null~";
        }
        Type type = o.GetType();
        var ti = new TypeInfoX(type);
        if (ti.IsSimpleType)
        {
            return o.ToString();
        }
        else if (o is DataTable dt)
        {
            return OfDataTable(dt, options);
        }
        if (o is IEnumerable<object> lst)
        {
            return OfList(lst, options);
        }
        //else if (type.IsGenericType)
        //{
        //    //var payload2 = o as IEnumerable<object>;
        //    //if (payload2 != null)
        //    if (o is IEnumerable<object> lst)
        //    {
        //        return OfList(lst, options);
        //        //var dt = DataTableX.FromNTupleList(lst, 500, false);
        //        //return DataTableX.ToHtml(dt, true);
        //    }
        //    //else
        //    //{
        //    //}
        //    //var genericTypeDef = type.GetGenericTypeDefinition();

        //    //if (genericTypeDef == typeof(System.Collections.Generic.List<>) ||
        //    //    genericTypeDef == typeof(System.Collections.Generic.IEnumerable<>) ||
        //    //    genericTypeDef == typeof(System.Linq.Enumerable) ||
        //    //    genericTypeDef == typeof(System.Collections.IEnumerable)
        //    //    )
        //    //{
        //    //    //System.Linq.Enumerable< x;
        //    //    var dt = DataTableX.FromNTupleList(, 500, false);
        //    //    s += DataTableX.ToHtml(dt, true);
        //    //    s += "<br>";
        //    //}
        //}
        return OfObject(o, options);
    }
    //============================================================================================
    public static string OfObject(Object o, ToHtmlOptions? options = null)
    {
        options ??= new ToHtmlOptions();
        options.Caption += String.Format(" ({0})", o.GetType().Name);

        if (o == null)
        {
            return "Object is null";
        }
        var nvs = ObjectX.GetPropFieldNameValues(o);
        if (options.OrderByPropNames)
        {
            nvs = nvs.Sort();
        }
        string rval = nvs.ToHtml(options);
        return rval;
    }

    //=================================================================
    public static String OfList<T>(IEnumerable<T> en, ToHtmlOptions? options = null)
    {
        options ??= new ToHtmlOptions();
        StringBuilder buf = new();
        try
        {
            if (options.ReverseRows)
            {
                en = en.Reverse();
            }
            if (options.Caption != null)
            {
                buf.AppendFormat("<h3>{0}</h3>", options.Caption);
            }
            var dt = DataTableX.FromNTupleList(en);
            buf.Append(DataTableX.ToHtml(dt, options));
        }
        catch (Exception ex)
        {
            buf.AppendLine(ex.ToString());
        }
        return buf.ToString();
    }
    //========================================================================================
    public static string OfDataTable(DataTable dt, ToHtmlOptions? options = null)
    {
        return DataTableX.ToHtml(dt, options);
    }

    public static String Of(NameValueCollection nvs, ToHtmlOptions? options = null)
    {
        options ??= new ToHtmlOptions();
        StringBuilder buf1 = new StringBuilder();
        StringWriter buf = new StringWriter(buf1);

        buf.WriteLine("<table border='1' style='padding:9px; border-collapse:collapse;border-spacing:0'>");
        if (options.Caption != null)
        {
            buf.Write("<h3>");
            buf.Write(options.Caption);
            buf.WriteLine("</h3>");
        }

        if (nvs == null)
        {
            buf.Write("<tr><td>is null</td</tr>");
        }
        else if (nvs.Count == 0)
        {
            buf.WriteLine("<tr><td>is Empty</td></tr>");
        }
        else
        {
            int i = 0;
            foreach (String k in nvs.Keys) 
            {
                object vObj = nvs[k];
                string v;
                if (vObj == null)
                {
                    v = "null";
                }
                else
                {
                    Type t = vObj.GetType();
                    if (t == typeof(Nullable<DateTime>))
                    {
                        v = "dt";
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        object convertedValue = Convert.ChangeType(vObj, Nullable.GetUnderlyingType(t));
                        v = convertedValue.ToString();
                    }
                    else
                    {
                        v = vObj.ToString();
                    }
                    if (options.EncodeHtml)
                    {
                        v = System.Net.WebUtility.HtmlEncode(v);
                    }
                }

                if (i++ % 2 == 0)
                {
                    buf.Write("<tr style='vertical-align:top;background-color:#DADADA'>");
                }
                else
                {
                    buf.Write("<tr style='vertical-align:top;background-color:#BACACA'>");
                    //hw.Write("<tr bgcolor='#BACACA'>");
                }

                buf.Write("<td>" + i + "</td>");

                buf.Write("<td>");
                buf.WriteLine(k);
                buf.Write("</td>");
                buf.Write("<td>");
                buf.WriteLine(v);
                buf.WriteLine("</td></tr>");
            }
        }
        buf.WriteLine("</table>");
        buf.Close();
        return buf.ToString();
    }
}