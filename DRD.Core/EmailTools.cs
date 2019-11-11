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
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Service
{
    public class EmailTools
    {
        //private bool mailSent = false;

        public async Task Send(string from, string fromDisplayName, string to, string subject, string body, bool isBodyUrl, string[] fileNames)
        {
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(from, fromDisplayName);
            mail.To.Add(to);

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

            var db = new DrdContext(ConfigConstant.CONSTRING);
            var result = db.ApplConfigs.ToList();
            var emailsmtp = result.Where(c => c.Code == "EMAILSMTP").FirstOrDefault().Value;
            var emailport = result.Where(c => c.Code == "EMAILPORT").FirstOrDefault().Value;
            var emailuser = result.Where(c => c.Code == "EMAILUSER").FirstOrDefault().Value;
            var emailpwd = result.Where(c => c.Code == "EMAILPWD").FirstOrDefault().Value;

            //send the message
            SmtpClient smtp = new SmtpClient();
            smtp.Host = emailsmtp;// "smtp.gmail.com";
            smtp.Port = int.Parse(emailport);// 587;
            smtp.UseDefaultCredentials = false;// true;
            smtp.Credentials = new System.Net.NetworkCredential(emailuser, emailpwd);// "klaxononline@gmail.com", "klaxon123");
            smtp.EnableSsl = true;
            smtp.Timeout = 5000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            await Task.Yield();
            smtp.Send(mail);//, "Klaxon");

            //SmtpClient smtp = new SmtpClient(emailsmtp, int.Parse(emailport));
            //smtp.UseDefaultCredentials = false;
            //smtp.EnableSsl = true;
            //smtp.Credentials = new System.Net.NetworkCredential(emailuser, emailpwd); 
            ////smtp.Port = Convert.ToInt32("25");
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            ////using (var smtpClient = new SmtpClient())
            ////{

            ////await smtp.SendMailAsync(mail);
            //smtp.Send(mail);
            //////}
            //////this.AsyncVoidMethod();


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
    }
}
