using System.IO;
using System.Security;

namespace Libx
{
    public class AppEnvX : EnvironmentX
    {
        private static bool _isFirstTimeRunning = true;
        public static bool IsFirstTimeRunning
        {
            get
            {
                bool rval = _isFirstTimeRunning;
                _isFirstTimeRunning = false;
                return rval;
            }
        }
    }

    public class EnvironmentX
    {
        //===================================================================================================
        public static string MapPath(String relPath)
        {
            string ApplicationPhysicalPath = AppDomain.CurrentDomain.BaseDirectory;

            string pfname;
            if (string.IsNullOrEmpty(ApplicationPhysicalPath))
            {
                throw new Exception("AppBaseDir is not initialized");
            }
            if (relPath.StartsWith("~"))
            {
                relPath = relPath.Substring(1);
            }
            if (relPath.StartsWith("/"))
            {
                relPath = relPath.Substring(1);
                pfname = Path.Combine(ApplicationPhysicalPath, relPath);
                //docRoot = HttpContex.Current.Request.MapPath(docRoot);
            }
            else
            {
                pfname = relPath;
            }
            //MOutput.WriteLine(pfname);
            return pfname;
        }

        //===================================================================================================

        public static DateTime AppStartedDateTime { get; internal set; } = DateTime.Now;

        private static void Main()
        {

            ////LogX.Debug(
            //    Environment.ExpandEnvironmentVariables(
            //       "OS=%OS% SystemRoot is: %SYstemRooT%"));
            ////LogX.Debug(
            //    Environment.ExpandEnvironmentVariables(
            //       "JAVA_HOME=%JAVA_HOME%"));
            ////LogX.Debug(ToStringX());
        }

        public static string GetEnvironmentVariable(string envVarName, bool throwsOnNotFound = true)
        {
            string rval = Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.User);
            if (rval == null)
            {
                throw new Exception("Can't find environment variable with name: " + envVarName);
            }
            if (rval.Trim() == "")
            {
                throw new Exception("blank environment variable with name: " + envVarName);
            }
            return rval;
        }
        public static string GetMyPassword()
        {
            return GetPassword("bradleym");
        }
        public static string GetPassword(string username)
        {
            String pfname = @"G:\Pass\" + username;
            string pass = File.ReadAllText(pfname);
            pass = pass.Trim();
            return pass;
        }


        public static String GetGlobalConfig(string key, bool throwOnNotFound = true)
        {
            //var nvs = new Dictionary<String, String>();
            //nvs.Add("n1", "v1");
            //nvs.Add("n222", "v222");


            string settingsPfname = @"C:\wdo\PublicWebsites\WebApiApps\WebConfigs\My.GlobalSettings.json";

            //Dictionary<string, string> nvs = new Dictionary<string, string>();
            //System.Xml.Serialization.XmlSerializerX.SerializeObject_ToFile(nvs, );

            ////    var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(nvs);
            //var json = JsonConvert.SerializeObject(nvs, Formatting.Indented);

            ////MOutput.WriteHtmlEncoded(json);
            //System.IO.File.WriteAllText(settingsPfname, json);
            string jsonStr = System.IO.File.ReadAllText(settingsPfname);

            //var nvs = (Dictionary<string, string>)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonStr, typeof(Dictionary<string, string>));
            Dictionary<string, string> nvs = null; // JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
            //throw new NotImplementedException("sadf234");

            key = key.ToLower();
            nvs = DictionaryX.LowercaseKeys(nvs);

            // nvs[key];
            nvs.TryGetValue(key, out string rval);

            if (string.IsNullOrWhiteSpace(rval))
            {
                if (throwOnNotFound)
                {
                    throw new Exception("No value found for key: " + key + ", in file: " + settingsPfname);
                }
            }
            return rval;
        }


        //public static bool IsProd
        //{
        //    get
        //    {
        //        string appEnv = AppSettingsX.GetString("app.environment");
        //        return appEnv == "prod";
        //    }
        //}
        ////============================================================================
        //public static bool IsDebug
        //{
        //    get
        //    {
        //        HttpContext ctxt = HttpContext.Current;
        //        if (ctxt == null)
        //        {
        //            //throw new Exception("How can HttpContext be null, must be calling from WinForm app");
        //            return false;
        //        }
        //        if (ctxt.Session == null)
        //        {
        //            //throw new Exception("How can HttpContext.Session be null?"); yes on remoteServer
        //            return false;
        //        }
        //        if (IsLocalDev)
        //        {
        //            return true;
        //        }
        //        //return ctxt.Session["debug"] == "on";
        //        HttpCookie ck = ctxt.Request.Cookies["debug"];
        //        return ck != null && ck.Value == "debug";
        //    }
        //}

        ////============================================================================
        //public static bool IsLocalDev
        //{
        //    get
        //    {
        //        return IsLocalHost;
        //    }
        //}

        //public static bool? isLocalDev_File = null;
        //public static bool IsLocalHost
        //{
        //    get
        //    {
        //        HttpContext context = HttpContext.Current;
        //        if (context == null)
        //        {
        //            //Log.Debug(new Exception("HttpContext is null???")); //infiniteLoop
        //            if (isLocalDev_File == null)
        //            {
        //                isLocalDev_File = File.Exists(@"C:\isLocalDev.txt");
        //            }
        //            return isLocalDev_File.Value;
        //            //return true;
        //        }
        //        return context.Request.IsLocal ||
        //               context.Request.UserHostAddress.StartsWith("192.168."); //pc1 is the client and pc2 is the server.

        //        //return context.Request.UserHostAddress.StartsWith("127.") ||
        //        //       context.Request.UserHostAddress.Equals("::1") ||
        //        //       context.Request.Url.Host.ToLower().Equals("toshiba") ||
        //        //       context.Request.Url.Host.ToLower().Equals("lh") ||
        //        //       context.Request.Url.Host.ToLower().Equals("localhost") ||
        //        //       context.Request.UserHostAddress.StartsWith("192.168.");
        //        //            [UserHostAddress] = [fe80::78ab:9cef:d9ca:77a4%11]

        //    }
        //}

        ////============================================================================
        //public static bool IsRemoteHost
        //{
        //    get
        //    {
        //        return !IsLocalHost;
        //    }
        //}
        ////============================================================================
#if ZZ
      public static ApplicationStage ApplicationStage
      {
         get
         {
            if (IsProduction) { return ApplicationStage.Production; }
            if (IsUserTest) { return ApplicationStage.UserTest; }
            if (IsRemoteDev) { return ApplicationStage.RemoteDev; }
            return ApplicationStage.LocalDev;
            
         }
      }
#endif

        //============================================================================
#if XX
      abc
      public static bool IsDebugRemoteAddr
      {
         get
         {
            string t;
            //string DebugRemoteAddrs = ConfigurationManager.AppSettings[t = "Debug.Remote_Addrs"];
            string DebugRemoteAddrs = (String)AppSettingsExpressionBuilder.GetAppSetting(t = "Debug.Remote_Addrs");
            if (string.IsNullOrEmpty(DebugRemoteAddrs))
            {
               throw new Exception("AppSettings not exist: " + t);
            }
            string currentRemoteAddr = HttpContext.Current.Request.ServerVariables[t = "REMOTE_ADDR"];
            if (string.IsNullOrEmpty(currentRemoteAddr))
            {
               throw new Exception("AppSettings not exist: " + t);
            }
            bool rval = DebugRemoteAddrs.IndexOf(currentRemoteAddr) >= 0;
            //Log.Debug("DebugRemoteAddrs/currentRemoteAddr = " + DebugRemoteAddrs + "/" + currentRemoteAddr);
            //Log.Debug("HttpContext.Current.Debug.IsEnabled = " + HttpContext.Current.Debug.IsEnabled);
            return rval;
         }
      }
#endif
        //============================================================================
        public static NameObjectCollection ToNVs()
        {
            NameObjectCollection nvs = new NameObjectCollection();

            OperatingSystem os = Environment.OSVersion;

            //IDictionary allEnvs = Environment.GetEnvironmentVariables(); //=EnvironmentVariableTarget.Process
            //   IDictionary allEnvs = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process); //=
            //Log.Debug(IDictionaryX.ToString(allEnvs));
            //Log.Debug(Environment.StackTrace);

            string s;
            //SecurityException

            try { s = Environment.CommandLine; }
            catch (SecurityException e) { s = e.Message; }
            nvs.Add("CommandLine", s);

            nvs.Add("OSVersion", Environment.OSVersion.ToString());
            nvs.Add("Environment.SpecialFolder.Favorites", Environment.GetFolderPath(Environment.SpecialFolder.Favorites));
            nvs.Add("Environment.MachineName (NetBIOSName)", Environment.MachineName);//WSB-152246

            //nvs.Add("UserDomainName", Environment.UserDomainName); //exception
            try { s = Environment.UserDomainName; }
            catch (SecurityException e) { s = e.Message; }
            nvs.Add("UserDomainName [DOMAIN1]", s);

            nvs.Add("UserInteractive", Environment.UserInteractive + ""); //true
            nvs.Add("CLR Version", Environment.Version + ""); //[2.0.50727.832],  [4.0.30319.1]

            //nvs.Add("WorkingSet", Environment.WorkingSet + ""); //process MemAounty. 6M
            try { s = "" + Environment.WorkingSet; }
            catch (SecurityException e) { s = e.Message; }
            nvs.Add("WorkingSet", s); //[147533824]



            nvs.Add("TickCount", Environment.TickCount + ""); //millis elapsed since sys restarted

            //nvs.Add("SystemDirectory", Environment.SystemDirectory); //
            try { s = Environment.SystemDirectory; }
            catch (SecurityException e) { s = e.Message; }
            nvs.Add("SystemDirectory", s);

            //nvs.Add("CurrentDirectory", Environment.CurrentDirectory); //
            try { s = Environment.CurrentDirectory; }
            catch (SecurityException e) { s = e.Message; }
            nvs.Add("CurrentDirectory", s);

            return nvs;
        }
        public static string ToStringX()
        {
            return ToNVs().ToStringValue();
        }
        public static string ToHtml()
        {
            return ToNVs().ToHtml();
        }


    }
}
