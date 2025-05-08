using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Services.Description;
using PensionPortal.NAVWS;

namespace PensionPortal
{
    public class Components
    {
        public static Pension ObjNav
        {
            get
            {
                // Enforce TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Ignore SSL certificate validation (use only for testing)
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                var webservice = new Pension();
                try
                {
                    var credentials = new NetworkCredential(
                        ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"]
                    );
                    webservice.Credentials = credentials;
                    webservice.PreAuthenticate = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error initializing web service: " + ex.Message);
                }
                return webservice;
            }
        }
        public static void SendEmailAlerts(string address, string subject, string message)
        {
            try
            {
                //string email = "dynamicsselfservice@gmail.com";
                //string password = "ydujienvejtdojgv";
                string email = "erp@telpostapension.org";
                string password = "rtbbfthfnthfhhyx";

                var loginInfo = new NetworkCredential(email, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.office365.com", 587);

                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress(address));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
            }
            catch (Exception Ex)
            {
                Ex.Data.Clear();
            }
        }
        public static void SendEmailAlertswithAttachment(  string subject, string message, string attachmentPath)
        {
            try
            {
               
                string email = "erp@telpostapension.org";
                string password = "rtbbfthfnthfhhyx";

               string toaddress = "info@telpostapension.org";

                //string toaddress = "dmoraa@appkings.co.ke";

                var loginInfo = new NetworkCredential(email, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.office365.com", 587); 

                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress(toaddress));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(attachmentPath) && System.IO.File.Exists(attachmentPath))
                {
                    msg.Attachments.Add(new Attachment(attachmentPath));
                }

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Error sending email: " + Ex.Message);
            }
        }
       

    }
}