using System.Net.Http;
using System.Security.Authentication;
namespace Libx;
public partial class MyHttpClient //#HttpClientBase
{
    public MLogger logger;
    protected HttpClient httpClient;
    public MyHttpCacheStore CacheStore { get; set; }

    //public HttpStatusCode StatusCode { get; set; }
    //public HttpRequestMessage httpReq { get; set; }
    public HttpResponseMessage httpRes { get; set; }
    protected Func<Uri, HttpRequestMessage> createRequestMesg;
    public string TextFor404 { get; set; } //NotFound
    public string TextFor302AndExternal { get; set; } //Redirect

    public string UrlSent { get; set; }
    public string UrlRedirected { get; set; }
    public bool RequestWasRedirected { get; set; }
    public bool RequestWasRedirectedToExternal { get; set; }

    //===================================================================================================
    public Task<HttpContent> SendAsync(string url)
    {
        var uri = new Uri(url);
        return SendAsync(uri);
    }
    public Task<HttpContent> SendAsync(Uri uri)
    {
        HttpRequestMessage httpReq = createRequestMesg(uri);
        return SendAsync(httpReq);
    }
    public async Task<HttpContent> SendAsync(HttpRequestMessage httpReq)
    {
        //httpReq.Method = HttpMethod.Post;
        //httpReq.Method = HttpMethod.Get;
        Uri uri = httpReq.RequestUri;
        UrlSent = httpReq.RequestUri.AbsoluteUri;

        logger.LogInfo("Sending Async request to: " + uri);
        httpRes = await httpClient.SendAsync(httpReq);
        //httpRes.EnsureSuccessStatusCode(); //HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
        if (httpRes.StatusCode != HttpStatusCode.OK)
        {
            string errMesg = $"*** HttpStatusCode.NotOK: url ={uri.AbsoluteUri}, HttpStatusCode= {httpRes.StatusCode}";
            logger.LogInfo(errMesg);
            //throw new Exception(errMesg);
        }
        this.RequestWasRedirected = false;
        this.RequestWasRedirectedToExternal = false;
        this.UrlRedirected = null;

        string urlFinal = httpRes.RequestMessage.RequestUri.AbsoluteUri;
        if (urlFinal != UrlSent)
        {
            this.RequestWasRedirected = true;
            this.UrlRedirected = urlFinal;
        }
        else
        {
        }
        if (RequestWasRedirected)
        {
            //httpRes.StatusCode = HttpStatusCode.Redirect;
            this.RequestWasRedirectedToExternal = UriX.IsCors(UrlRedirected, UrlSent);
            if (RequestWasRedirectedToExternal)
            {
                logger.LogInfo($"Request redirected to {(RequestWasRedirectedToExternal ? "(External)" : "")}: {UrlRedirected}");
            }
        }
        //var bytes = await client.GetByteArrayAsync(uri);
        //String html = Encoding.GetEncoding("utf-8").GetString(bytes, 0, bytes.Length - 1);
        //httpRes.RequestMessage;

        HttpContent res = httpRes.Content;
        return res;
    }

    //===================================================================================================
    public HttpClient CreateHttpClient(string scheme) //Uri uri) //, HttpClient httpClient)
    {
        string schemeLower = scheme.ToLower();
        //if (httpClient != null) return httpClient; //-------------------------------
        HttpClient ret;
        if (schemeLower == "https")
        {
            //if (RunThis) return; //---------
            //if (NotRunThis)
            //{
            //    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //    ServicePointManager.SecurityProtocol =
            //            SecurityProtocolType.Tls
            //          | SecurityProtocolType.Tls11
            //          | SecurityProtocolType.Tls12
            //          | SecurityProtocolType.Tls13;
            //    ////| SecurityProtocolType.Ssl3;//System.NotSupportedException: The requested security protocol is not supported.
            //}
            //else
            {
                var clientHandler = new HttpClientHandler();
                //clientHandler.AllowAutoRedirect = false; //#redirect
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                clientHandler.SslProtocols =
                        SslProtocols.Tls
                      | SslProtocols.Tls11
                      | SslProtocols.Tls12;
                //| SslProtocols.Tls13; //ExceptioN: Tls13 not support, 

                ret = new HttpClient(clientHandler);
                logger.LogInfo("Tls done setup");
            }
        }
        else if (schemeLower == "http")
        {
            ret = new HttpClient();
        }
        else
        {
            throw new Exception("invalid scheme: " + scheme);
        }
        return ret;
    }

    //===================================================================================================
    public async Task<bool> DoesURLExistz(string url, string mimeType = null)
    {
        bool rval;
        try
        {
            var uri = new Uri(url);
            HttpRequestMessage httpReq = createRequestMesg(uri);
            httpReq.Method = HttpMethod.Head;
            //httpReq.AllowAutoRedirect = true;

            var res = await this.SendAsync(httpReq);
            //var res = await this.SendAsync(url);

            if (httpRes.StatusCode == HttpStatusCode.OK)
            {
                //    if (mimeType != null)
                //    {
                //        //if (httpRes.Content..ContentType == mimeType) //#todo
                //        {
                //            rval = true;
                //        }
                //    }
                //    else
                //    {
                //        rval = true;
                //    }
                rval = true;
            }
            else
            {
                rval = false;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("error accessing URL: " + url, ex);
        }
        return rval;
    }
}