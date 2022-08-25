namespace Libx
{
    public class ConfigX
    {
        public static string MachineNameLower;
        public static string BaseDirLower;

        public static int AppRunId { get; private set; }
        public static string AppRunIdLong { get; private set; }
        public static DateTime AppStartedDateTime { get; private set; } = DateTime.Now;

        public static string AppName { get; set; } = "uninit";
        public static bool IsDevMachine { get; private set; }
        public static bool IsIISExpress { get; private set; }
        public static string WebAppVirtualPath { get; set; }

        public static bool IsAdminApp => AppSettingsX.GetString("IsAdminApp") == "1";
        public static bool IsSampleApp => AppSettingsX.GetString("IsSampleApp", "0") == "1";

        static ConfigX()
        {
            if (true)
            {
                ///<seeAlso cref="DateTimeX.UnixEpochNowSeconds" cref="DateTimeX.toISOString" />
                var dt1 = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan ts = DateTime.UtcNow - dt1;

                AppRunId = (int)ts.TotalSeconds;
                AppRunIdLong = DateTime.Now.ToString("s").Replace(':', '-'); //s=sortable, 2022-05-03T18-31-47
            }
            MachineNameLower = Environment.MachineName.ToLower();

            //Hard-code dev machine name, this is used in dev only
            IsDevMachine = (MachineNameLower == "desktop-0f8sm8p" || MachineNameLower == "hpi712");

            BaseDirLower = AppDomain.CurrentDomain.BaseDirectory.ToLower();
            IsIISExpress = BaseDirLower.Contains(@"\bin\debug\");
        }

        //===================================================================================================
        public static string authority = "?"; //hostname:port
        public static string hostNameLower = "?"; //hostname:port
        public static int port = 0;
        public static int portStart = 0;
        public static void SetRequestUri(Uri url)
        {
            //if (requestUri == null)
            if (url.Authority != authority)
            {
                //requestUri = url;
                authority = url.Authority;
                hostNameLower = url.Host.ToLower();
                port = url.Port;
                portStart = port / 100; //9040 becomes 90; 80 becomes 0
            }
        }

        //===================================================================================================
        public static String TestingEmailTo
        {
            get
            {
                string rval = AppSettingsX.GetString("Testing.EmailTo");
                return rval;
            }
        }
        public static String ErrorEmailTo => AppSettingsX.GetString("Error.EmailTo");

        //===================================================================================================
        public static String _SmtpHost = null;
        public static String SmtpHost
        {
            get
            {
                if (_SmtpHost == null)
                {
                    _SmtpHost = AppSettingsX.GetString("SmtpHost");
                }
                return _SmtpHost;
            }
        }
        //===================================================================================================
        public static bool IsAppEnvDev => AppEnv == AppEnvEnum.Dev;
        public static bool IsAppEnvTest => AppEnv == AppEnvEnum.Test;
        public static bool IsAppEnvProd => AppEnv == AppEnvEnum.Prod;
        private static AppEnvEnum _appEnv = AppEnvEnum.Unknown;
        public static AppEnvEnum AppEnv
        {
            get
            {
                //AppEnvEnum _appEnv = AppEnvEnum.Unknown;
                if (_appEnv == AppEnvEnum.Unknown)
                {
                    string appEnvStr = AppSettingsX.GetString("AppEnv");
                    bool parsedOK = Enum.TryParse(appEnvStr, out _appEnv);
                    if (!parsedOK)
                    {
                        throw new Exception("AppSetting AppEnv, invalid value: " + appEnvStr);
                    }
                }
                return _appEnv;
            }
        }
    }
}
