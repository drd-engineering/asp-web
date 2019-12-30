using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.View;
using DRD.Service;

namespace DRD.Service
{
    public class EmailService
    {
        //private bool mailSent = false;

        public async Task Send(string senderEmail, string senderName, string recipientEmail, string subject, string body, bool isBodyUrl, string[] fileNames)
        {
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(senderEmail, senderName);
            mail.To.Add(recipientEmail);

            //set the content
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            //screen scrape the html
            if (isBodyUrl)
            {
                string html = HttpContent(body);
                mail.Body = html;
                //mail.IsBodyHtml = true;
            }
            else
                mail.Body = body;

            if (fileNames.Length > 0)
            {
                Attachment attach;
                foreach (string filename in fileNames)
                {
                    attach = new Attachment(filename);
                    mail.Attachments.Add(attach);
                }
            }

            var configGenerator = new AppConfigGenerator();
            var emailsmtp = configGenerator.GetConstant("EMAIL_SMTP")["value"];
            var emailport = configGenerator.GetConstant("EMAIL_PORT")["value"];
            var emailuser = configGenerator.GetConstant("EMAIL_USER")["value"];
            var emailpassword = configGenerator.GetConstant("EMAIL_PASSWORD")["value"];
            
            //send the message
            SmtpClient smtp = new SmtpClient();
            smtp.Host = emailsmtp;// "smtp.gmail.com";
            smtp.Port = int.Parse(emailport);// 587;
            smtp.UseDefaultCredentials = false;// true;
            smtp.Credentials = new System.Net.NetworkCredential(emailuser, emailpassword);// "klaxononline@gmail.com", "klaxon123");
            smtp.EnableSsl = true;
            //smtp.Timeout = 5000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            await Task.Yield();
            smtp.Send(mail);//, "Klaxon");

        }

        private async void AsyncVoidMethod()
        {
            await Task.Delay(100);
        }

        public async Task SendNotification(string SendTo, string[] cc, string subject, string body, string path)
        {
            SmtpClient client = new SmtpClient();
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(SendTo));
            foreach (string ccmail in cc)
            {
                message.CC.Add(new MailAddress(ccmail));
            }
            message.Subject = subject;
            message.Body = body;
            message.Attachments.Add(new Attachment(path));
            //message.Attachments.Add(a);
            try
            {
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;
                await Task.Yield();
                client.Send(message);
            }
            catch (Exception ex)
            { 
                ex.ToString();
            }
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = "";// (string)e.UserState; 

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }

            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            //mailSent = true;

        }

        private string HttpContent(string url)
        {
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
            StreamReader sr = new StreamReader(objRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public string CreateHtmlBody(string path)
        {
            string body = string.Empty;
            using(StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        public void sendEmailInbox(ActivityItem activity)
        {
/*            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            EmailTools emailtools = new EmailTools();

            string body = string.Empty;
            if (System.Web.HttpContext.Current != null)
                body = emailtools.CreateHtmlBody(System.Web.HttpContext.Current.Server.MapPath("/doc/emailtemplate/InboxNotif.html"));
            else
                body = emailtools.CreateHtmlBody(@"c:\doc\emailtemplate\InboxNotif.html");


            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            body = body.Replace("{_URL_}", strUrl);
            body = body.Replace("{_NAME_}", activity.UserName);

            body = body.Replace("//images", "/images");

            var dbx = new DrdContext(Constant.CONSTRING);
            var resultx = dbx.ApplConfigs.ToList();
            var emailfrom = resultx.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;

            var task = emailtools.Send(emailfrom, admName + " Administrator", activity.Email, admName + " Inbox Reception", body, false, new string[] { });*/
        }
    }
}
