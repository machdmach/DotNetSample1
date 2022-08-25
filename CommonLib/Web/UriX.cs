using System.Collections.Specialized;
using System.Text.RegularExpressions;
namespace Libx;
#if docs
#endif
public class UriX
{
    public static Uri CreateUri(string url)
    {
        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException("Bad URI format: " + url, "url");
        }
        return uri;
    }
    public static Uri CreateUri(Uri baseUri, string relativeUrl)
    {
        //Uri uri = CreateUri(relativeUrl);
        Uri ret;
        try
        {
            ret = new Uri(baseUri, relativeUrl);
        }
        catch (UriFormatException ex)
        {
            throw new Exception("Bad URI format: " + relativeUrl + ", baseUrl=" + baseUri.AbsoluteUri);
        }
        return ret;
    }

    public static Uri CreateUriFromFullyUrl(string url)
    {
        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException("Bad URI format: " + url, "url");
        }
        if (string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host))
        {
            throw new Exception("Url missing scheme or host: " + url);
        }
        return uri;
    }
    //static UriX()
    //{
    //    for (int i = 0; i < nonTextExtensions.Length; i++)
    //    {
    //        nonTextExtensions[i] = "." + nonTextExtensions[i];
    //    }
    //}
    ////===================================================================================================
    //public static string[] nonTextExtensions = new string[] {
    //        "ppt", "pptx", "vsdx", "docx", "doc", "xls", "xlsx", "mdb",  //microsoft
    //        "pdf", "csv", //misc apps
    //        "mp3", "wav", "wma", "avi", "mp4", //audios, videos
    //        "png", "jpg", "gif", "svg", "bmp", "psd", "dwg", "tiff", "tif", "zip", "gz", //images
    //        "bat", "exe", "dll", "jar", //executables
    //        "db" //Thumbs.db
    //        ,"eps"
    //};
    //===================================================================================================
    public static string[] HtmlPageExtensions = new string[] { ".html", ".htm", ".shtml" };

    public static bool IsHtmlPage(string url)
    {
        return IsWebFile(url); //todo: exclude css and js
    }
    /// <summary>
    /// Determine if the resource located the the url is a web file.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>True if the resource located at the url is a web (html, css, js...) file </returns>
    /// <exception cref="Exception">if url is null or empty</exception>
    public static bool IsWebFile(string url)
    {
        bool ret;
        if (string.IsNullOrEmpty(url))
        {
            throw new Exception("url is empty");
        }
        int slashIdx = url.LastIndexOf('/');
        if (slashIdx > 0)
        {
            url = url.Substring(slashIdx + 1);
        }
        if (url.IndexOf('.') < 0)
        {
            //url with no extension
            ret = true;
        }
        else
        {
            Uri baseUri = new Uri("http://abc.com");
            Uri uri = new Uri(baseUri, url);
            string absPathLower = uri.AbsolutePath.ToLower();
            ret = HtmlPageExtensions.Any(e => absPathLower.EndsWith(e));
        }
        //MOutput.Write("IsWebFile: " + url + " is" + ret);
        return ret;
    }

    public static bool IsUserContentFile(string url) => !IsWebFile(url); //#isUserFile

    //public static bool IsWebFile(string url) => !IsUserContentFile(url);
    //public static bool IsUserContentFile(string url)
    //{
    //    string urlLower = url.ToLower();
    //    bool isNonHtmlPage = nonTextExtensions.Any(e => urlLower.EndsWith(e));
    //    return isNonHtmlPage;
    //}

    //===================================================================================================
    public static string ConstructQueryString(NameValueCollection nvs)
    {
        StringBuilder buf = new StringBuilder();
        for (int i = 0; i < nvs.Count; i++)
        {
            if (i > 0) { buf.Append('&'); }
            string key = nvs.Keys[i];
            string val = nvs[i];
            buf.Append(key);
            buf.Append('=');

            buf.Append(val);
            //buf.Append(Uri.EscapeDataString(val));
            //buf.Append(HttpUtility.UrlEncode(val));
        }
        return buf.ToString();
    }
    //===================================================================================================
    public static string GetQueryStringParameterValue(string url, string name, string defaultVal)
    {
        name = name.ToLower();
        string rval = null;
        if (url.ToLower().Contains(name + "="))
        {
            var nvs = UriX.ParseQueryStringNVs(url);
            rval = nvs[name];
        }
        if (string.IsNullOrEmpty(rval))
        {
            rval = defaultVal;
        }
        return rval;
    }
    //===================================================================================================
    public static NameValueCollection ParseQueryStringNVs(string qs)
    {
        NameValueCollection nvs = new NameValueCollection();
        if (qs == null) { return nvs; } //--------------------------------
        qs = qs.Trim();
        if (qs.Length == 0) { return nvs; } //--------------------------------

        int x = qs.IndexOf('?');
        if (x < 0 && qs.IndexOf('=') < 0) { return nvs; } //--------------------------------

        if (x >= 0)
        {
            qs = qs.Substring(x + 1);
        }
        string[] nvsArr = qs.Split('&'); //, StringSplitOptions.RemoveEmptyEntries);
        foreach (string nv in nvsArr)
        {
            string[] nvArr = nv.Split('=');
            string val;
            if (nvArr.Length == 1)
            {
                val = String.Empty;
            }
            else if (nvArr.Length == 2)
            {
                val = Uri.EscapeDataString(nvArr[1]);
            }
            else
            {
                //throw new Exception("bad qs: " + qs);
                throw new UserInputDataException("bad nv: " + nv);
                //%20href%3Dhttp//ccentspdev04m/_layouts/15/settings.aspx?Source=http%3A%2F%2Fccentspdev04m%2FPages%2Fdefault%2Easpx
                //%20%20%20at%20System.UriX.ParseQueryStringNVs(String%20qs)%20in%20c:\ws\MProjects\CommonLib\System\ASystem\UriX.cs:line%2093%20%20%20at%20SharepointPal.WebUI.Views.GetSpUtilsText.GetContent()%20in%20c:\ws\MProjects\SharepointPal\SharepointPal.Web\Views\GetSpUtilsText.ashx.cs:line%2046%20%20%20at%20SharepointPal.WebUI.Views.GetSpUtilsText.ProcessRequest(HttpContext%20context)%20in%20c:\ws\MProjects\SharepointPal\SharepointPal.Web\Views\GetSpUtilsText.ashx.cs:line%2027
            }
            string name = nvArr[0];
            //string val = HttpUtility.UrlDecode(nvArr[1]);
            nvs.Add(name.ToLower(), val);
        }
        return nvs;
    }
    public static string RemoveQuery(string uri, string name)
    {
        NameValueCollection nvs = ParseQueryStringNVs(uri);
        Uri uriO = new Uri(uri);
        string leftPart = uriO.GetLeftPart(UriPartial.Path);

        nvs.Remove(name);
        if (nvs.Count == 0)
        {
            return leftPart;
        }
        else
        {
            return leftPart + "?" + ConstructQueryString(nvs);
        }
    }

    public static string RemoveQueryNV(string uri, string name, string value)
    {
        throw new Exception("not implemented");
        //return uri;
    }
    public static string SetQueryNV(string uri, string name, string value)
    {
        var nvs = ParseQueryStringNVs(uri);
        Uri uriO = new Uri(uri);
        string leftPart = uriO.GetLeftPart(UriPartial.Path);

        nvs[name] = value;
        return leftPart + "?" + ConstructQueryString(nvs);
    }
    public static bool IsValid(String uri)
    {
        //string regexPat = @"^http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
        //return Regex.IsMatch(uri, regexPat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var ret = false;
        if (Uri.TryCreate(uri, UriKind.Absolute, out Uri resultURI))
        {
            ret = resultURI.Scheme == Uri.UriSchemeHttp || resultURI.Scheme == Uri.UriSchemeHttps;
        }
        return ret;
    }
    //===================================================================================================
    /// <summary>
    /// Check if the user has entered an Absolute URL.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="label"></param>
    /// <exception cref="UserInputDataException">User-Friendly error message</exception>
    public static void EnsureAbsoluteURL(string url, string label = "The URL")
    {
        if (url == null)
        {
            throw new UserInputDataException(label + " must not be blank");
        }
        else if (url.Contains(":"))
        {
            throw new UserInputDataException(label + " must not contain colon character: " + url);
        }
        else if (!url.StartsWith("/"))
        {
            throw new UserInputDataException(label + " must start with a slash character: " + url);
        }

        try
        {
            var baseUrl = new Uri("https://domain1.com");
            var fqUri = new Uri(baseUrl, url);
            //Uri uri = new Uri(fqUrl, UriKind.Absolute);
        }
        catch (UriFormatException)
        {
            throw new UserInputDataException(label + " is invalid: " + url);
        }
    }

    //===================================================================================================
    public static string ToHtml(Uri uri)
    {
        NameObjectCollection nvs = ToNVs(uri);
        return nvs.ToHtml(new ToHtmlOptions { Caption = "Uri" });
    }
    public static NameObjectCollection ToNVs(Uri uri)
    {
        Uri o = uri;
        NameObjectCollection nvs = new NameObjectCollection
        {
            { "AbsolutePath", uri.AbsolutePath }, //
            { "AbsoluteUri", uri.AbsoluteUri }, //
            { "Authority", uri.Authority }, //Authority = Host+ Port
            { "DnsSafeHost", uri.DnsSafeHost }, //
            { "Fragment", uri.Fragment }, //#?%20&%3E%20abc
            { "GetLeftPart(UriPartial.Path)", uri.GetLeftPart(UriPartial.Path) }, //http://microsoft.com/samples/page%201.aspx
            { "Host", uri.Host }, //
            { "IsAbsoluteUri", uri.IsAbsoluteUri }, //
            { "IsLoopback", uri.IsLoopback }, //
            { "LocalPath", uri.LocalPath }, //
            { "OriginalString", uri.OriginalString }, //
            { "Query", uri.Query }, //?a=1&b=2
            { "Scheme", uri.Scheme }, //https
            { "UserInfo", uri.UserInfo }, //
            { "UserEscaped", uri.UserEscaped }, //False
            { "Segments", string.Join(", ", uri.Segments) }, //
            { "PathAndQuery", uri.PathAndQuery }, // n             t859t7y75656887

            { "SchemeAndServer", uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) },
            { "HostAndPort", uri.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped) }
        };

        return nvs;
    }

    //===================================================================================================
    public static string FlattenUrl(Uri uri)
    {
        //Uri uri = new Uri(baseUrl);
        string s = uri.Authority + uri.AbsolutePath;
        s = s.ToLower();
        s = s.Replace("http://", "");
        s = s.Trim('/', ' ');
        //if (s.EndsWith("/")) { s = s.Substring(0, s.Length - 1); }
        //s = PathX.FlattenPath(s);
        s = Regex.Replace(s, @"[\W]+", "_");  //Non-Word chars a-z0-9         
        return s;
    }

    //===================================================================================================
    public static string ExtractFileName(string url, bool uriEscapeResult = false)
    {
        ////http://localhost:63422/Views/ContentMigrator_.aspx
        //if (url.EndsWith("/"))
        //{
        //    //s = url.Substring(0, url.Length - 1);
        //    //return "";            
        //    throw new Exception("url cannot end with as slash: " + url);
        //}

        //string s = url;
        //int i1 = url.LastIndexOf('/');
        //if (i1 >= 0)
        //{
        //    s = url.Substring(i1 + 1);
        //}
        //else
        //{
        //    i1 = url.LastIndexOf('\\');
        //    if (i1 >= 0)
        //    {
        //        s = url.Substring(i1 + 1);
        //    }
        //}
        //int i1_queryString = s.IndexOf('?');
        //if (i1_queryString > 0)
        //{
        //    s = s.Substring(0, i1_queryString);
        //}
        //s = Uri.UnescapeDataString(s);
        //return s;

        //throw new Exception("1f");


        int i2 = url.IndexOf('?');
        if (i2 == -1)
        {
            i2 = url.IndexOf('#');
        }
        if (i2 == -1)
        {
            i2 = url.Length;
        }
        int i1 = url.LastIndexOf('/', i2 - 1);
        //if (i1 == -1)
        //{
        //    i1 = 0;
        //}
        i1++;

        int subsLen = i2 - i1;
        //subsLen++;

        string s = url.Substring(i1, subsLen);
        if (uriEscapeResult)
        {
            s = Uri.UnescapeDataString(s);
        }
        if (s == "")
        {
            s = "_home_index.html";
        }
        MOutput.WriteLine("filename extracted=" + s);
        return s;
    }

    //===================================================================================================
    public static string GetParentDirUrl(string url, List<string> virtualPaths = null)
    {
        Uri uri = CreateUri(url);
        var buf = new StringBuilder();
        buf.Append(uri.GetLeftPart(UriPartial.Authority));
        string path = uri.AbsolutePath;

        if (virtualPaths != null && virtualPaths.Exists(p => path.EndsWith(p, StringComparison.InvariantCultureIgnoreCase)))
        {
            buf.Append(path);
        }
        else
        {
            var parentPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parentPath))
            {
                parentPath = parentPath.Replace('\\', '/');
                buf.Append(parentPath);
            }
        }

        if (buf[buf.Length - 1] != '/')
        {
            buf.Append('/');
        }
        string rval = buf.ToString();
        return rval;
    }
    //===================================================================================================
    public static bool IsAPIUrl(string url)
    {
        bool ret = false;
        if (url.ToLower().Contains("/api/"))
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
        return ret;
    }
    //===================================================================================================
    /// <summary>
    /// Remove scheme, "www" and lower the url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="removeQuery"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static string NormalizeUrl(string url, bool removeQuery = false)
    {
        //harryreidairport.com/business/planning/?	
        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException("Invalid URI: " + url);
        }
        if (string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host))
        {
            throw new Exception("Url missing scheme or host: " + url);
        }
        string ret;
        if (removeQuery)
        {
            ret = uri.Authority + uri.AbsolutePath;
        }
        else
        {
            ret = uri.Authority + uri.PathAndQuery;
        }
        ret = ret.ToLower();
        if (ret.StartsWith("www."))
        {
            ret = ret.Substring(4);
        }

        while (ret.EndsWith("/") || ret.EndsWith("?"))
        {
            ret = ret.Substring(0, ret.Length - 1);
        }
        return ret;
    }
    //===================================================================================================
    /// <summary>
    /// 
    /// </summary>
    /// <param name="appUrl"></param>
    /// <param name="resourceUrl"></param>
    /// <returns>True if appUrl and resourceUrl do not have the same origins</returns>
    public static bool IsCors(string appUrl, string resourceUrl)
    {
        //Browser restricts requests initiated from scripts to access resources from the same origin as the
        //loaded app unless response from other origins includes the right CORS headers Access-Control-Allow-Origin: * or https://foo.example
        //origins (domain, scheme, or port)

        bool ret;
        if (string.IsNullOrEmpty(appUrl) || string.IsNullOrEmpty(resourceUrl))
        {
            ret = false;
        }
        else
        {
            Uri appUri = CreateUri(appUrl);
            Uri resourceUri = CreateUri(resourceUrl);

            string appOrigin = appUri.GetLeftPart(UriPartial.Authority);
            string resourceOrigin = resourceUri.GetLeftPart(UriPartial.Authority);
            if (string.IsNullOrEmpty(appOrigin) || string.IsNullOrEmpty(resourceOrigin))
            {
                ret = false;
            }
            else
            {
                ret = !appOrigin.Equals(resourceOrigin, StringComparison.OrdinalIgnoreCase);
            }
        }
        return ret;
    }
}
