using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using TrusteePortal.NAVWS;
using System.Net.Http;

namespace TrusteePortal
{
    public class Components
    {
        private static readonly HttpClient client = new HttpClient();
        public static Trustee ObjNav
        {
            get
            {
                // Enforce TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Ignore SSL certificate validation (use only for testing)
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                var webservice = new Trustee();
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
        public static void SendSMSAlerts(string phoneNo, string subject, string message)
        {
            try
            {
                string token = "jyiu4RdISCceoUTackl2BLBULCfEO2";
                string senderID = "TelPosta";

                string fullMessage = $"{subject}: {message}";
                string encodedMessage = System.Web.HttpUtility.UrlEncode(fullMessage);

                string url = $"https://api2.uwaziimobile.com/send?token={token}&phone={phoneNo}&text={encodedMessage}&senderID={senderID}";

                var response = client.GetAsync(url).GetAwaiter().GetResult();
                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Console.WriteLine("SMS sent. Response: " + responseString);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Error sending SMS: " + Ex.Message);
                Ex.Data.Clear();
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

    }
}