using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Libx;
public class MyHttpContent //#postForm
{

    //===================================================================================================
    public static async Task<string> postForm()
    {
        //var client = new HttpClient();
        //client.Timeout = new TimeSpan(0, 0, 5);
        //Task<HttpResponseMessage> responseTask = client.PostAsync(url, content);

        //WebPageUtility.InitTLS();

        //HttpRequest req;
        var httpClient = new HttpClient();

        HttpRequestHeaders headers = httpClient.DefaultRequestHeaders;

        headers.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Mobile Safari/537.36");
        headers.Referrer = new Uri("https://www.notams.faa.gov/dinsQueryWeb/");

        HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept = headers.Accept;
        accept.Clear();
        //accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
        accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        //accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        //accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
        accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        //application/json, text/plain, */*

        var values = new Dictionary<string, string>
             {
                { "reportType", "Raw" },
                //{ "reportType", "Report" },
                { "actionType", "notamRetrievalByICAOs" },
                { "submit", "View NOTAMs" },
                //{ "", "" },
            };
        HttpContent content = new FormUrlEncodedContent(values);
        if (NotRunThis)
        {
            var bodyStr = "searchType=0&designatorsForLocation={HND}&designatorForAccountable=&latDegrees=&latMinutes=0&latSeconds=0&longDegrees=&longMinutes=0&longSeconds=0&radius=10&sortColumns=5+false&sortDirection=true&designatorForNotamNumberSearch=&notamNumber=&radiusSearchOnDesignator=false&radiusSearchDesignator=&latitudeDirection=N&longitudeDirection=W&freeFormText=&flightPathText=&flightPathDivertAirfields=&flightPathBuffer=4&flightPathIncludeNavaids=true&flightPathIncludeArtcc=false&flightPathIncludeTfr=true&flightPathIncludeRegulatory=false&flightPathResultsType=All+NOTAMs&archiveDate=&archiveDesignator=&offset=0&notamsOnly=false&filters=&minRunwayLength=&minRunwayWidth=&runwaySurfaceTypes=&predefinedAbraka=&predefinedDabra=&flightPathAddlBuffer=";
            //bodyStr = bodyStr.Replace("{HND}", IATA_LocationIdentifier);
            content = new StringContent(bodyStr, Encoding.UTF8, "application/json");
        }


        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded"); //"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",

        httpClient.Timeout = new TimeSpan(0, 0, 30);
        //MOutput.WriteLine("HttpClient PostAync to url: " + url);

        string htmlContent = "?";
        string url = "abc";
        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, content); //System.Net.WebException: Unable to connect to the remote server
                                                                                     //System.Threading.Tasks.TaskCanceledException: A task was canceled.
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new Exception("ServiceUnavailable:  " + response + url);
            }
            htmlContent = await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            //throw new UserInputDataException(ex.Message, ex);
        }
        string s = htmlContent;

        await Task.Yield();
        return null;
    }
    //===================================================================================================
    public void testCookies()
    {
        var baseAddress = new Uri("http://example.com");
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("CookieName", "cookie_value"));

        var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        {
            var content = new FormUrlEncodedContent(new[]
            {
               new KeyValuePair<string, string>("foo", "bar"),
               new KeyValuePair<string, string>("baz", "bazinga"),
            });
            HttpResponseMessage result = client.PostAsync("/test", content).Result;
            result.EnsureSuccessStatusCode();
        }
    }
    //===================================================================================================
    public static void SetContent()
    {
        var nvs = new[] {
           new KeyValuePair<string, string>("foo", "bar"),
           new KeyValuePair<string, string>("baz", "bazinga"),
        };
        var content = new FormUrlEncodedContent(nvs);
        var client = new HttpClient();
        HttpResponseMessage result = client.PostAsync("/test", content).Result;
        result.EnsureSuccessStatusCode();
    }
}
/*
       string Login = JsonConvert.SerializeObject(new LoginViewModel()
                {
                    Email = userFromDb.Email,
                    Password = "fdfdf@2239",
                    RememberMe = false
                }); ;
                StringContent LoginhttpContent = new(Login, Encoding.UTF8, "application/json");
                var Login_response = await _httpClient.PostAsync(HelperFunctions.getUrl(HelperFunctions.AcctounController.name, HelperFunctions.AcctounController.Login), LoginhttpContent);
                Assert.Equal(HttpStatusCode.OK, Login_response.StatusCode);
    
                //receive cookies from the login response
                var cookies = Login_response.Headers.GetValues(HeaderNames.SetCookie);
                 //Add the cookies to the DefaultRequestHeaders of the _httpClient
                _httpClient.DefaultRequestHeaders.Add("Cookie",cookies);
                var IsLoggedIn_response = await _httpClient.GetAsync(HelperFunctions.getUrl(HelperFunctions.AcctounController.name, HelperFunctions.AcctounController.IsLoggedIn));
                Assert.Equal("true",IsLoggedIn_response.Content.ReadAsStringAsync().Result);
 */