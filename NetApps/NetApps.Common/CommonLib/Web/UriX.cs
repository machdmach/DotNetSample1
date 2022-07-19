using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace System
{
    public class UriX
    {
        static UriX()
        {
            for (int i = 0; i < nonTextExtensions.Length; i++)
            {
                nonTextExtensions[i] = "." + nonTextExtensions[i];
            }
        }
        //===================================================================================================
        public static string[] nonTextExtensions = new string[] {
                "bat", "pdf", "csv",
                "ppt", "pptx", "vsdx", "docx", "doc", "xls", "xlsx", "mdb",  //microsoft
                "mp3", "wav", "wma", "avi", "mp4", //audios, videos
                "png", "jpg", "gif", "svg", "bmp", "psd", "dwg", "tiff", "tif", "zip", "gz"
            };

        //===================================================================================================
        public static bool IsTextContentPage(string urlLower)
        {
            bool isNonHtmlPage = nonTextExtensions.Any(e => urlLower.EndsWith(e));
            return !isNonHtmlPage;
        }

        //===================================================================================================
        //void UriTemplate1()
        //{
        //    Uri baseUri = new Uri("http://contoso.com/bookmarkservice");
        //    UriTemplate uriTemplate = new UriTemplate(
        //       "users/{username}/bookmarks/{id}");
        //    // generate a new bookmark URI
        //    Uri newBookmarkUri = uriTemplate.BindByPosition(baseUri, "skonnard", "123");
        //    // match an existing bookmark URI
        //    UriTemplateMatch match = uriTemplate.Match(baseUri, newBookmarkUri);
        //    System.Diagnostics.Debug.Assert(match != null);
        //    Console.WriteLine(match.BoundVariables["username"]);
        //    Console.WriteLine(match.BoundVariables["id"]);
        //}
        private static void Main()
        {
            NameValueCollection nvs = new NameValueCollection
            {
                { "key_id", "12" }
            };
            nvs.Set("price_to", "ab");
            nvs.Add("price_to", "ab"); //2 values ab and ab2
            nvs.Set("price_to", "ab3"); //2 values ab and ab2

            nvs.Add("price_from", "a&b");

            String qs = UriX.ConstructQueryString(nvs);
            //LogX.Debug(qs);
            qs = "xx/bb/?a=b&" + qs;
            //LogX.Debug(NameValueCollectionX.ToString(ToNameValueCollection(qs)));

            //HttpRequest req;


            UriBuilder b = new UriBuilder();

            Uri baseUri = new Uri("http://microsoft.com");
            Uri u = new Uri(baseUri, "samples/page 1.aspx?q=more#? &> abc"); //ctor always EscapUriString
            //LogX.Debug(NameObjectCollection.ToString(ToNVs(u)));
        }

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

                //buf.Append(Uri.EscapeDataString(val));
                buf.Append(val);

                ////buf.Append(HttpUtility.UrlEncode(val));
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
            qs = qs.Trim();
            if (qs.Length == 0)
            {
                return nvs;
            }

            int x = qs.IndexOf('?');
            if (x < 0 && qs.IndexOf('=') < 0)
            {
                return nvs;
            }

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
        //public static string ReplaceQueryValue(string name, string newVal)
        //{
        //a=b
        //}
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
            Uri uriO = new(uri);
            string leftPart = uriO.GetLeftPart(UriPartial.Path);

            nvs[name] = value;
            return leftPart + "?" + ConstructQueryString(nvs);
        }

        //public static String CurrentHost_eg
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request.Url.Host;
        //    }
        //}
        //public static Uri CurrentUri_eg
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request.Url;
        //    }
        //}
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
        public static void CheckUserDataInput_AbsoluteURL(string url, string label = "The URL")
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
        public static string ToHtml(Uri uri)
        {
            NameObjectCollection nvs = ToNVs(uri);
            return nvs.ToHtml(new ToHtmlOptions { Caption = "Uri" });
        }

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

        //public static string UrlDecode_eg(String s)
        //{
        //    return HttpContext.Current.Server.UrlDecode(s);
        //    //UrlDecode(string s, TextWriter output);
        //}
        //public static string UrlEncode_eg(String s)
        //{
        //    return HttpContext.Current.Server.UrlEncode(s);
        //    //UrlEncode(string s, TextWriter output);
        //}
        //public static string UrlPathEncode_eg(String s)
        //{

        //    return HttpContext.Current.Server.UrlPathEncode(s);
        //}
        //public static string UrlTokenEncode_eg(byte[] ba)
        //{
        //    return HttpServerUtility.UrlTokenEncode(ba);
        //}
        //public static byte[] UrlTokenDecode_eg(string s)
        //{
        //    return HttpServerUtility.UrlTokenDecode(s);
        //}

        //===================================================================================================
        public static string ExtractFileName(string url)
        {
            //http://localhost:63422/Views/ContentMigrator_.aspx
            string s = url;
            int slashIdx = s.LastIndexOf('/');
            if (slashIdx > 0)
            {
                s = s.Substring(slashIdx + 1);
            }
            int questionMark = s.IndexOf('?');
            if (questionMark > 0)
            {
                s = s.Substring(0, questionMark);
            }
            //MOutput.Write("res=" + s);
            return s;
        }
        //===================================================================================================
        public static string GetFileLeafRef(string url)
        {
            //VirtualPathUtility.GetFileName(url);

            string rval = url;
            int idx1 = url.LastIndexOf('/');
            if (idx1 >= 0)
            {
                rval = url.Substring(idx1 + 1);
            }
            else
            {
                idx1 = url.LastIndexOf('\\');
                if (idx1 >= 0)
                {
                    rval = url.Substring(idx1 + 1);
                }
            }
            int i1_queryString = rval.IndexOf('?');
            if (i1_queryString > 0)
            {
                rval = rval.Substring(0, i1_queryString);
            }
            return rval;
        }

        //===================================================================================================
        public static string GetDirLeafRef(string url)
        {
            //VirtualPathUtility.GetDirectory(url);

            string rval = url;
            int idx1 = url.LastIndexOf('/');
            if (idx1 < 0)
            {
                idx1 = url.LastIndexOf('\\');
            }

            if (idx1 >= 0)
            {
                rval = url.Substring(0, idx1);
            }
            return rval;
        }
        //===================================================================================================
        public static bool DoesURLExist(string url, string mimeType = null)
        {
            // allows for validation of SSL conversations
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls
                  | SecurityProtocolType.Tls11
                  | SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";
            req.AllowAutoRedirect = true;

            bool rval = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (mimeType != null)
                    {
                        if (response.ContentType == mimeType)
                        {
                            rval = true;
                        }
                    }
                    else
                    {
                        rval = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error accessing URL: " + url, ex);
            }
            return rval;
        }
    }
}
