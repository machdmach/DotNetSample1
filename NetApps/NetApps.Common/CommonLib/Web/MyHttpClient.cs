using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace System
{
    public class MyHttpClient //#HttpClientX
    {
        private void eg1()
        {
            //var content = new FormUrlEncodedContent(values);
            //using (var wb = new WebClient())
            //{
            //    var data = new NameValueCollection();
            //    data["username"] = "myUser";
            //    data["password"] = "myPassword";
            //    var response = wb.UploadValues(url, "POST", data);
            //}

            //using (var client = new HttpClient())
            //{
            //    //var responseString = await response.Content.ReadAsStringAsync();
            //    //var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);
            //    //var responseString = await response.Content.ReadAsStringAsync();
            //    //var responseString2 = await client.GetStringAsync("http://www.example.com/recepticle.aspx");
        }
        protected Uri baseUri;
        public MyHttpClient() { }
        public MyHttpClient(string baseUrl)
        {
            baseUri = new Uri(baseUrl);
        }
        public Uri BuildRequestUri(string relUrl)
        {
            return new Uri(baseUri, relUrl);
        }

        //===================================================================================================
        public async Task<String> GetStringAsync(string url = null)
        {
            //HttpRequest r;
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            var uri = BuildRequestUri(url);

            //var dataApiUrl = "https://rest.locuslabs.com/v1/venue/las/poi/get-all-ids";
            MOutput.WriteLine("Calling data API: " + uri);

            var httpRes = await client.GetAsync(uri);
            if (httpRes.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("data api error, httpResponse.StatusCode != HttpStatusCode.OK: " + httpRes.StatusCode + ", url=" + uri);
            }
            string res = await httpRes.Content.ReadAsStringAsync();
            return res;
        }
        //===================================================================================================
        public String GetStringSync(string url = null)
        {
            var task = Task.Run(
                 async () => await GetStringAsync(url)
             );
            task.Wait();
            var ret = task.Result;
            return ret;
        }

        //===================================================================================================
        public async Task<T> GetObjectz<T>(string url = null) where T : class
        {
            string str = await GetStringAsync(url);
            Type type = typeof(T);
            T res = (T)JsonConvert.DeserializeObject(str, type);
            return res;
        }
        //===================================================================================================
        public async Task<ResponseData> GetResponseDataAsync(string url = null)
        {
            string str = await GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<ResponseData>(str);
            return res;
        }
        //===================================================================================================
        public ResponseData GetResponseDataSync(string url = null)
        {
            //string str = GetStringSync(url);
            //var res = JsonConvert.DeserializeObject<ResponseData>(str);
            //return res;
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
                MOutput.WriteObject(resData, "ResponseData received");
                throw new Exception("payload is null");
            }
            MOutput.WriteLine("payload type=" + resData.payload.GetType().Name); //JObject
            string payload = resData.payload.ToString();
            Type type = typeof(T);
            T res = (T)JsonConvert.DeserializeObject(payload, type);
            return res;
        }
        //===================================================================================================
        public T GetPayloadSync<T>(string url = null) where T : class
        {
            var task = Task.Run(
                 async () => await GetPayloadAsync<T>(url)
            );
            task.Wait();
            var ret = task.Result;
            return ret;

            //var resData = GetResponseDataSync(url);
            //if (resData.payload == null)
            //{
            //    MOutput.WriteObject(resData, "ResponseData received");
            //    throw new Exception("payload is null");
            //}
            //MOutput.WriteLine("payload type=" + resData.payload.GetType().Name); //JObject
            //string payload = resData.payload.ToString();
            //Type type = typeof(T);
            //T res = (T)JsonConvert.DeserializeObject(payload, type) as T;
            //return res;
        }

        //===================================================================================================
        private void test1()
        {
            var baseAddress = new Uri("http://example.com");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                   new KeyValuePair<string, string>("foo", "bar"),
                   new KeyValuePair<string, string>("baz", "bazinga"),
                });
                cookieContainer.Add(baseAddress, new Cookie("CookieName", "cookie_value"));
                HttpResponseMessage result = client.PostAsync("/test", content).Result;
                result.EnsureSuccessStatusCode();
            }
        }
    }
}
