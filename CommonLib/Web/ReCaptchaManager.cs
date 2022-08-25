using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Libx
{
    //Usage:
    //<script src='https://www.google.com/recaptcha/api.js'></script>
    //public async Task<IActionResult>FormSubmit()
    //{
    //    base.Init();

    //    var gData = new ReCaptchaServerRequestData() { G_Recaptcha_Response = Request.Form[ReCaptchaServerRequestData.G_Recaptcha_Response_FieldName] };
    //    var re = new ReCaptchaManager();
    //    var gRes = re.Check(gData);
    //    if (gRes.Success)
    //    {
    //        MOutput.WriteLine("OK");
    //    }
    //    MOutput.Write(NameValueCollectionX.ToHtml(Request.Form));
    //    return PassThroughView("zz");
    //}    

    //===================================================================================================
    public class ReCaptchaManager
    {
        private readonly MLogger logger;
        public ReCaptchaManager(MLogger? loggerP)
        {
            logger = loggerP;
        }
        public static string DataSiteKey = AppSettingsX.GetString("recaptcha-data-sitekey");

        public static void Test1()
        {
            var data = new ReCaptchaServerRequestData
            {
                G_Recaptcha_Response = "OK"
            };

            //var re = new ReCaptchaManager(Temp.Logger);
            //re.Check(data);
        }
        //When your users submit the form where you integrated reCAPTCHA, you'll get as part of the payload a string 
        //with the name "g-recaptcha-response". In order to check whether Google has verified that user, 
        //send a POST request with these parameters:

        public ReCaptchaServerResponseData Check(ReCaptchaServerRequestData data)
        {
            string postUrl = "https://www.google.com/recaptcha/api/siteverify";

            WebRequest request = WebRequest.Create(postUrl);
            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            byte[] postDataByteArray = data.ToPostData();
            request.ContentLength = postDataByteArray.Length;

            try
            {
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(postDataByteArray, 0, postDataByteArray.Length);
                reqStream.Close();
            }
            catch (WebException ex)
            {
                logger.LogWarning(ex, "Bad DNS server, return OK to caller though");
                ReCaptchaServerResponseData rval = new ReCaptchaServerResponseData
                {
                    Success = true
                };
                return rval;
            }

            WebResponse response = request.GetResponse();
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream resStream = response.GetResponseStream();
            StreamReader resStreamReader = new StreamReader(resStream);
            string responseFromServer = resStreamReader.ReadToEnd();

            resStreamReader.Close();
            resStream.Close();
            response.Close();

            ReCaptchaServerResponseData googleRes = JsonConvert.DeserializeObject<ReCaptchaServerResponseData>(responseFromServer);
            //{
            //"success": false,
            //"error-codes": [
            //  "missing-input-response",
            //  "missing-input-secret"
            //]
            //}

            //{
            //  "success": true,
            //  "challenge_ts": "2017-01-20T20:23:44Z",
            //  "hostname": "localhost"
            //}
            MOutput.WriteHtmlOfAny(googleRes);
            if (googleRes.ErrorCodes != null)
            {
                foreach (var ec in googleRes.ErrorCodes)
                {
                    MOutput.WriteLine("ErrorCode: " + ec);
                }
            }
            MOutput.WriteHtmlEncodedPre(responseFromServer);
            return googleRes;

        }
    }
    //===================================================================================================
    public class ReCaptchaServerRequestData
    {
        public const string G_Recaptcha_Response_FieldName = "g-recaptcha-response";

        public string Secret { get; set; }
        public string G_Recaptcha_Response { get; set; }
        public string RemoteIP { get; set; }

        public byte[] ToPostData()
        {
            Secret = AppSettingsX.GetString("recaptcha-secretekey"); // "6LcvFyATAAAAAFGVNOakLk9SBjBUJscFP5sDFi_U";
            //recaptcha-data-sitekey

            var buf = new StringBuilder();
            buf.Append("secret=" + Secret);
            buf.Append("&response=" + G_Recaptcha_Response);
            buf.Append("&remoteip=" + RemoteIP);
            //var postData = "thing1=hello";
            //postData += "&thing2=world";
            //var data = Encoding.ASCII.GetBytes(postData);

            string postData = buf.ToString();
            MOutput.WriteLine(postData);
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);

            return byteArray;
        }
    }
    //===================================================================================================
    public class ReCaptchaServerResponseData
    {
        //When your users submit the form where you integrated reCAPTCHA, you'll get as part of the payload a string 
        //with the name "g-recaptcha-response". In order to check whether Google has verified that user, 
        //send a POST request with these parameters:

        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime? ChallengeTimestamp { get; set; }

        public String HostName { get; set; }
        //  "challenge_ts": "2017-01-20T20:23:44Z",
        //  "hostname": "localhost"

        [JsonProperty("error-codeS")]
        public List<string> ErrorCodes { get; set; }
    }

}