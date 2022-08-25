using System.IO;
using System.Net.Http;
using System.Security.Authentication;
namespace Libx;
public class MyHttpClientHelper
{
    public static HttpRequestMessage CreateHttpRequestMessage(Uri uri)
    {
        var reqMesg = new HttpRequestMessage();
        reqMesg.RequestUri = uri;
        //reqMesg.Method = HttpMethod.Post;
        reqMesg.Method = HttpMethod.Get;
        return reqMesg;
    }
}
//*******************************************************************************************************
public class HttpResponseMessageX
{
    public static String ToString(HttpResponseMessage mesg)
    {
        return HtmlValue.OfObject(mesg);
    }
}
