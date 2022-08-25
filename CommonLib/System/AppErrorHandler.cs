using System.Net.Mail;

namespace Libx;
public class AppErrorHandler
{
    public static Exception LastErrorLogged;
    public static DateTime LastErrorLogged_DateTime;

    public static Exception LastInternalLoggerError;
    public static DateTime LastInternalLoggerError_DateTime;

    static TimedCounter dosEmailErrors = new("EmailSysAdminErrors", 2,  TimeSpan.FromMinutes(10));
    public static async Task TryEmailSystemAdminErrorAsync(Exception ex, string extraInfo = null)
    {
        if (dosEmailErrors.Done)
        {
            MOutput.WriteLine(dosEmailErrors.ToHtml("Not OK, max reached"));
            dosEmailErrors.Increment();
            return;  //---------------------------------------------------
        }
        MOutput.WriteLine(dosEmailErrors.ToHtml("OK"));

        string body = "";
        try
        {
            body += "<pre>";
            body += ex.ToString();
            body += "</pre>";

            body += "<hr>\f\n";
            body += extraInfo;

            body += MyAppContext.GetInfo_Html();

            var mail = new MailMessageEntity
            {
                From = ConfigX.AppEnv + ".Error@lasairport.com",
                RecipientList = ConfigX.ErrorEmailTo,
                Subject = ConfigX.AppName + " Error",
                IsBodyHtml = true
            };

            body = dosEmailErrors.ToHtml() + body;
            mail.Body = body;

            using (SmtpClient client = new SmtpClient())
            {
                client.Host = ConfigX.SmtpHost;
                client.Port = 25;
                client.EnableSsl = false;

                MOutput.WriteLine("<hr>Sending Email to: " + mail.RecipientList);
                await client.SendMailAsync(mail.BuildMailMessage());
            }
            dosEmailErrors.Increment();

            AppErrorHandler.LastErrorLogged = ex;
            AppErrorHandler.LastErrorLogged_DateTime = DateTime.Now;
        }
        catch (Exception internalLoggerExc)
        {
            AppErrorHandler.LastInternalLoggerError = internalLoggerExc;
            AppErrorHandler.LastInternalLoggerError_DateTime = DateTime.Now;
            //Log to EventLog?
            MOutput.WriteErrorHtmlEncoded(internalLoggerExc);
        }
    }
}
