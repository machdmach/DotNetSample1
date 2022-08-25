namespace Libx
{
    public class MyAppContext
    {
        public static MLogger Logger;
        static MyAppContext()
        {
            Logger = new MyLogger() 
            {
                EntryQueue = new CircularQueue<LogEntry>(600)
            };
        }

        //===================================================================================================
        public static String GetInfo_Html(bool isFromCommandLine = false)
        {
            var nvs = GetMoreInfo(isFromCommandLine);
            string rval = nvs.ToHtml();

            if (true)
            {
                string sectionHeaderFormat = "<h4>{0}</h4>\n";
                var buf = new StringBuilder();

                buf.AppendFormat(sectionHeaderFormat, "LastErrorLogged");
                if (AppErrorHandler.LastErrorLogged != null)
                {
                    buf.AppendLine(AppErrorHandler.LastErrorLogged.ToString());
                    buf.AppendLine("On " + AppErrorHandler.LastErrorLogged_DateTime);
                }

                buf.AppendFormat(sectionHeaderFormat, "LastSystemErrorLogged");
                if (AppErrorHandler.LastInternalLoggerError != null)
                {
                    buf.AppendLine(AppErrorHandler.LastInternalLoggerError.ToString());
                    buf.AppendLine("On " + AppErrorHandler.LastInternalLoggerError_DateTime);
                }


                buf.AppendFormat(sectionHeaderFormat, "LastRan SQL");
                //buf.AppendLine(AppLog.LastRunSql);

                // buf.AppendFormat(sectionHeaderFormat, "Last Email Sent");
                // buf.AppendLine(SmtpClientX.GetLastEmail_Sent_Html());

                rval += buf.ToString();
            }

            return rval;
        }
        //===================================================================================================
        public static NameObjectCollection GetMoreInfo(bool isFromCommandLine = false)
        {
            var nvs = new NameObjectCollection();
            try
            {
                //nvs.Add("DebugReasons", RequestDataHelper.DebugReasons);
                nvs.Add("AppStartedDateTime", EnvironmentX.AppStartedDateTime);

                nvs.Add("System.Environment.MachineName", System.Environment.MachineName);
                nvs.Add("System.Net.Dns.GetHostName()", System.Net.Dns.GetHostName());
                nvs.Add("System.Environment.UserName ", System.Environment.UserName);
                //nvs.Add("System.Security.Principal.WindowsIdentity.GetCurrent().Name", System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                //nvs.Add("", );
                //if (isFromCommandLine)
                {
                    nvs.Add("Environment.CommandLine", Environment.CommandLine);
                    nvs.Add("Environment.CurrentDirectory", Environment.CurrentDirectory);
                    nvs.Add("Environment.GetCommandLineArgs", string.Join(", ", Environment.GetCommandLineArgs()));
                    //nvs.Add("", );
                }
            }
            catch (Exception ex)
            {
                nvs.Add("Ex1", ex.ToString());
            }
            return nvs;
        }
    }
}
