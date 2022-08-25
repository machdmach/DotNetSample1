using System.Net.Http;
namespace Libx;
public partial class MyHttpClient
{
    public MyHttpClient(MLogger logger, Func<Uri, HttpRequestMessage> createRequestMesg = null, HttpClient httpClient = null)
    {
        this.logger = logger;
        this.createRequestMesg = createRequestMesg ?? MyHttpClientHelper.CreateHttpRequestMessage;
        this.httpClient = httpClient ?? CreateHttpClient("https");
        //this.httpClient = httpClient ?? new HttpClient();
        //httpReq = new HttpRequestMessage();
        //httpReq.Method = HttpMethod.Post;
    }
    //===================================================================================================
    public async Task DownloadFile(string url, string pfname)
    {
        //if (!Directory.Exists(pfname))
        //    throw new DirectoryNotFoundException("File not found."               

        HttpContent content = await SendAsync(url);
        //byte[] bytes = await content.ReadAsByteArrayAsync();
        //File.WriteAllBytes(pfname, bytes);

        using Stream contentStream = await content.ReadAsStreamAsync();
        using Stream stream = new FileStream(pfname, FileMode.Create, FileAccess.Write, FileShare.None, 10000, true);
        await contentStream.CopyToAsync(stream);
    }
    //===================================================================================================
    public async Task<String> GetStringAsync(HttpRequestMessage req)
    {
        HttpContent content = await SendAsync(req);
        string res = await httpRes.Content.ReadAsStringAsync();
        return res;
    }
    //===================================================================================================
    public async Task<String> GetStringAsync(string url)
    {
        HttpContent content = await SendAsync(url);
        string res = await httpRes.Content.ReadAsStringAsync();
        return res;
    }
    //===================================================================================================
    public String GetStringSync_z(string url = null)
    {
        var task = Task.Run(
             async () => await GetStringAsync(url)
         );
        task.Wait();
        var ret = task.Result;
        return ret;
    }

    //===================================================================================================
    public async Task<T> GetObject_z<T>(string url = null) where T : class
    {
        string str = await GetStringAsync(url);
        T res = MyJsonConvert.DeserializeObject<T>(str);
        return res;
    }
    //===================================================================================================
    public async Task<ResponseData> GetResponseDataAsync(string url = null)
    {
        string str = await GetStringAsync(url);
        var res = MyJsonConvert.DeserializeObject<ResponseData>(str);
        return res;
    }
    //===================================================================================================
    public ResponseData GetResponseDataSync_z(string url = null)
    {
        var task = Task.Run(
           async () => await GetResponseDataAsync(url)
        );
        task.Wait();
        var ret = task.Result;
        return ret;
    }
    //===================================================================================================
    public async Task<T> GetPayloadAsync<T>(string url = null) where T : class
    {
        var resData = await GetResponseDataAsync(url);
        if (resData.payload == null)
        {
            MOutput.WriteHtmlOfAny(resData, "ResponseData received");
            throw new Exception("payload is null");
        }
        logger.LogInfo("payload type=" + resData.payload.GetType().Name);
        string payloadStr = resData.payload.ToString();
        Type type = typeof(T);
        T res = MyJsonConvert.DeserializeObject<T>(payloadStr);
        return res;
    }
    //===================================================================================================
    public T GetPayloadSync_z<T>(string url = null) where T : class
    {
        var task = Task.Run(
             async () => await GetPayloadAsync<T>(url)
        );
        task.Wait();
        var ret = task.Result;
        return ret;
    }

    //===================================================================================================
    public async Task FetchWebpage(WebPageEntity wp)
    {
        string url = wp.URL;
        var t1 = DateTime.Now;

        string pfname = CacheStore?.GetFilePathFromUrl(url);
        string htmlText = CacheStore?.TryRetrieveText(pfname);
        if (htmlText == null)
        {
            //Retrieving Content from the internet

            htmlText = await this.GetStringAsync(url);

            wp.StatusCode = httpRes.StatusCode;
            //if (htmlText.Contains("<title>Page not found - Geonetric</title>"))
            if (htmlText.Contains("Page Not Found"))
            {
                wp.StatusCode = HttpStatusCode.NotFound;
            }

            //--------
            if (wp.StatusCode == HttpStatusCode.NotFound)
            {
                htmlText = TextFor404 + ""; //Append a blank so the value can't be null
            }
            else if (RequestWasRedirectedToExternal) //wp.StatusCode == HttpStatusCode.Redirect)
            {
                htmlText = TextFor302AndExternal + "\nRedirected to: " + UrlRedirected;
            }
            CacheStore?.SaveText(pfname, htmlText);
        }
        else
        {
            //Content is from cache/file

            wp.StatusCode = HttpStatusCode.OK;
            if (htmlText == TextFor404)
            {
                wp.StatusCode = HttpStatusCode.NotFound;
            }
            else if (TextFor302AndExternal != null && htmlText.StartsWith(TextFor302AndExternal))
            {
                wp.StatusCode = HttpStatusCode.Redirect;
            }
        }
        wp.HtmlContentOrginal = htmlText;

        //string pfname = @"C:\temp\htmlDownloaded.html";
        //System.IO.File.WriteAllText(pfname, htmlText);
        //htmlText = System.IO.File.ReadAllText(pfname);

        //wp.ContentType = httpClient.HttpResponseMesg.
        wp.ContentType = "text/html";

        //wpEx.ResponseInfo = HtmlValue.OfObject(res);
        wp.ContentLength = htmlText.Length; //.ToString("N3");
        var t2 = DateTime.Now - t1;
        wp.Latency = t2.TotalMilliseconds;
    }
}