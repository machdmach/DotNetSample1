using System.Collections;
namespace Libx;
public static class MOutput
{
    public static MLogger Logger => MyAppContext.Logger;

    //===================================================================================================
    public static void Clear()
    {
        MyAppContext.Logger.Clear();
        WriteLine("MOutput cleared");
    }
    //===================================================================================================
    public static String ToHtml()
    {
        var ret = MyAppContext.Logger.ToHtml(new ToHtmlOptions { Caption = "MOutput Content", ReverseRows = true });
        return ret;
    }
    //===================================================================================================
    //static string colorsxxx = @"
    //#fce5cd    
    //#f4cccc    
    //#fff2cc    
    //#d9ead3    
    //#ea9999    
    //#cfe2f3    
    //#99CCFF    
    //#FFCC99    
    //#FFFFCC    
    //#FF9999    
    //#EBFCFD    
    //#CCCCFF
    //";
    //private static readonly string colors = @"#EBFCFD, #FFFFFF";

    //===================================================================================================
    //private static readonly int currentColorIndex = 0;
    public static bool WriteToConsole = false;
    //public static bool ShowCallerStack = true;
    public static void Write(string content)
    {
        if (WriteToConsole)
        {
            Console.Write(content);
        }
        else
        {
            MyAppContext.Logger.LogHtml(content);
        }
    }
    public static string WriteLine(string content)
    {
        string rval = content + "<br/>\n";
        Write(rval);
        return rval;
    }
    //===================================================================================================
    public static void WriteHtmlOfAny(Object obj, string title = null)
    {
        var options = new ToHtmlOptions() { Caption = title };
        WriteLine(HtmlValue.OfAny(obj, options));
    }
    public static void WriteLineFormat(string format, params object[] args)
    {
        string s = string.Format(format, args);
        s += "<br>\n";
        Write(s);
    }
    //===================================================================================================
    public static void WriteHtmlTable<T>(String title, IEnumerable<T> list, int maxRows = 1000, bool throwsOnExceedingMaxRows = true)
    {
        WriteLineFormat("<h4> {0}, count: {1} </h4>", title, list?.Count());
        if (list == null)
        {
            WriteLine("list is null");
            return;
        }
        string ret;
        if (typeof(T) == typeof(string))
        {
            ret = IEnumerableX.ToHtml(list);
        }
        else
        {
            var dt = DataTableX.FromNTupleList(list, maxRows, throwsOnExceedingMaxRows);
            ret = DataTableX.ToHtml(dt);
        }
        Write(ret);
    }
    //===================================================================================================
    public static void WriteHtmlEncodedPre(object contentx)
    {
        string content = contentx.ToString();
        content = System.Net.WebUtility.HtmlEncode(content);
        content = "<pre style='overflow:visible'>" + content + "</pre>";
        Write(content);
    }
    //===================================================================================================
    public static void WriteErrorHtmlEncoded(Exception ex)
    {
        string content = ex.ToString();
        content = System.Net.WebUtility.HtmlEncode(content);
        content = "<pre style='overflow:visible'>" + content + "</pre>";
        //MOutput.TurnOn();
        Write(content);
        //MOutput.TurnOff();
    }
    //===================================================================================================
    public static void Write_TextArea(object contentObj, string label = null, bool encode = true)
    {
        String content = contentObj.ToString();
        if (encode)
        {
            content = System.Net.WebUtility.HtmlEncode(content);
        }
        //content = content.Replace('"', 'x');
        //content = content.Replace('\'', 'x');

        StringBuilder buf = new StringBuilder();
        if (label != null)
        {
            buf.AppendLine(label + "<br>");
        }

        buf.AppendLine("<textarea rows=30 cols=130 wrap=off onclick='select()'>");
#if css1
<style type='text/css'>
textarea{
white-space:nowrap;
overflow:scroll;
}
</style>
#endif
        buf.AppendLine(content);
        buf.AppendLine("</textarea>");
        Write(buf.ToString());
    }

}