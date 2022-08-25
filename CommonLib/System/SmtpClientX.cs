using System.IO;
using System.Net;
using System.Net.Mail;

namespace Libx
{
    public class SmtpClientX
    {
        //==================================================================
        public static string ToHtml(SmtpClient client, MailMessage mesg)
        {
            //LinkedResource lr;
            //MediaTypeNames.Application.
            //AlternateView av1 = AlternateView.CreateAlternateViewFromString(

            var nvs = new NameObjectCollection();

            if (client != null)
            {
                nvs.Add("Host", client.Host);
                nvs.Add("Port", client.Port);

                NetworkCredential credential = (NetworkCredential)client.Credentials;
                if (credential == null)
                {
                    nvs.Add("Credentials", "null");
                }
                else
                {
                    nvs.Add("Credentials.UserName", credential.UserName);
                    nvs.Add("Credentials.P8ssw0d", "zz"); // + credential.Password);
                    nvs.Add("Credentials.Domain", credential.Domain);
                }
            }

            nvs.Add("MailMessage.From", mesg.From);

            if (true)
            {
                var buf = new StringBuilder();
                foreach (MailAddress ma in mesg.To)
                {
                    buf.Append(ma.Address + ", ");
                }
                string emailList = buf.ToString();
                emailList = emailList.Trim(',', ' ');
                nvs.Add("MailMessage.To", emailList);
            }

            nvs.Add("IsBodyHtml", mesg.IsBodyHtml); //
            nvs.Add("Subject", mesg.Subject);
            nvs.Add("Body", System.Net.WebUtility.HtmlEncode(mesg.Body));

            if (true)
            {
                var buf = new StringBuilder();
                foreach (MailAddress ma in mesg.CC)
                {
                    buf.Append(ma.Address + ", ");
                }
                string emailList = buf.ToString();
                emailList = emailList.Trim(',', ' ');
                nvs.Add("MailMessage.CC.To", emailList);
            }

            if (true)
            {
                var buf = new StringBuilder();
                foreach (MailAddress ma in mesg.ReplyToList)
                {
                    buf.Append(ma.Address + ", ");
                }
                string emailList = buf.ToString();
                emailList = emailList.Trim(',', ' ');
                nvs.Add("MailMessage.ReplyTo", emailList);
            }
            return nvs.ToHtml();
        }
        //===================================================================================================
        public static string GetEmailAddressesStr(MailAddressCollection addresses)
        {
            var buf = new StringBuilder();
            foreach (MailAddress ma in addresses)
            {
                buf.Append(ma.Address + ", ");
            }
            return buf.ToString();
        }

        public static string LastEmailSent = "";
        //===================================================================================================
        public static MailMessage LastMailMessageObject_Sent = null;
        public static String GetLastEmail_Sent_Html()
        {
            string rval = "UnInit";
            if (LastMailMessageObject_Sent != null)
            {
                rval = SmtpClientX.ToHtml(null, LastMailMessageObject_Sent);
            }
            return rval;
        }

        //===================================================================================================
        public static async void SendEmailMessageAsync(MailMessage mail)
        {
            if (mail.From == null)
            {
                //string emailFrom = AppSettingsX.GetString("SDD.EmailFrom");
                //string emailFromDisplayName = AppSettingsX.GetString("SDD.EmailFrom.DisplayName", false);
                //mail.From = new MailAddress(emailFrom, emailFromDisplayName);
            }
            if (string.IsNullOrEmpty(mail.Subject))
            {
                //mail.Subject = "No Subjct";
            }
            using (var client = new SmtpClient())
            {
                client.Host = ConfigX.SmtpHost;
                client.Port = 25;
                client.EnableSsl = false;

                LastMailMessageObject_Sent = mail;
                MOutput.WriteLine(GetLastEmail_Sent_Html());
                await client.SendMailAsync(mail);
                //MOutput.WriteLine("<hr>Emailed to: " + SmtpClientX.GetEmailAddressesStr(mail.To));
                //MOutput.WriteLine(mail.Body + "<hr>");

            }
        }
        //===================================================================================================
        public static LinkedResource CreateLinkedResourceFromUrl(String imgUrl, string contentId)
        {
            string mimeType = "todo"; //MimeMapping.GetMimeMapping(imgUrl);
            //string mimeType2 = MediaTypeNames.Image.Jpeg;

            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(imgUrl);
            MemoryStream ms = new MemoryStream(imageBytes);
            LinkedResource resource = new LinkedResource(ms, mimeType)
            {
                ContentId = contentId
            };
            return resource;
        }
        //===================================================================================================
        public static void CreateLinkedResourceFromUrl_eg()
        {
            string imgUrl = "http://a.png";
            string body = "<img src='cid:imageID1'/>";
            MailMessage mm = null;

            LinkedResource resource = SmtpClientX.CreateLinkedResourceFromUrl(imgUrl, "imageID1");
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            htmlView.LinkedResources.Add(resource);
            mm.AlternateViews.Add(htmlView);
        }
    }
    //===================================================================================================
    public class MailMessageEntity
    {
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.mailmessage.headers?view=net-6.0
        public MailMessageEntity()
        {
            IsBodyHtml = true;
        }

        public MailMessage BuildMailMessage()
        {
            MailMessage m = new MailMessage
            {
                Subject = Subject,
                Body = Body,
                From = new MailAddress(From),
                Sender = new MailAddress(From),
                IsBodyHtml = IsBodyHtml
            };
            m.To.Add(RecipientList);
            return m;
        }
        public string Subject { get; set; }
        public string From { get; set; }
        public string ReplyToList { get; set; }
        public string RecipientList { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public bool IsBodyHtml { get; set; }

        //public AlternateViewCollection AlternateViews { get; }
        //public AttachmentCollection Attachments { get; }
        //public MailAddressCollection Bcc { get; }
        ////public Encoding BodyEncoding { get; set; }
        ////public TransferEncoding BodyTransferEncoding { get; set; }
        //public MailAddressCollection CC { get; }
        //public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
        ////public NameValueCollection Headers { get; }
        ////public Encoding HeadersEncoding { get; set; }
        //public MailPriority Priority { get; set; }
        //public MailAddress ReplyTo { get; set; }
        //public MailAddressCollection ReplyToList { get; }
        //public MailAddress Sender { get; set; }
        ////public Encoding SubjectEncoding { get; set; }

    }
}
