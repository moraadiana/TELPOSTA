using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.WebRequestMethods;

namespace TELPOSTAStaff.pages
{
    public partial class VerifyOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }
            }

        }
        protected void lbtnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
        protected void lbtnLogin_Click(object sender, EventArgs e)
        {
          
            try
            {
                string otp = txtOTP.Text.Trim();
                string generatedOtp = Session["OtpCode"] as string;
                DateTime? generatedAt = Session["OtpGeneratedAt"] as DateTime?;
                string otpFromUser = otp.Trim();

                if (generatedOtp == null || generatedAt == null)
                {
                    Message( "OTP session expired. Please login again.");
                    Response.Redirect("~/Default.aspx");
                }

                if (generatedOtp.Equals(otpFromUser, StringComparison.OrdinalIgnoreCase) &&
                    DateTime.UtcNow <= generatedAt.Value.AddMinutes(5))
                {
                    
                    Response.Redirect("Dashboard.aspx",false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else if (DateTime.UtcNow > generatedAt.Value.AddMinutes(5))
                {
                    Message( "OTP has expired. Please request a new OTP.");
                    Response.Redirect("verifyOTP.aspx",false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    Message("Invalid OTP. Please try again.");
                   // return View("VerifyOTP");
                   
                }
            }
            catch (Exception ex)
            {
               Message( ex.Message);
               // return View("VerifyOTP");
            }
        }
        public bool ValidateOtp(string inputOtp)
        {
            var storedOtp = Session["OtpCode"] as string;
            var generatedAt = Session["OtpGeneratedAt"] as DateTime?;

            if (storedOtp == null || generatedAt == null)
                return false;

            if (storedOtp == inputOtp && DateTime.UtcNow <= generatedAt.Value.AddMinutes(5))
                return true;

            return false;
        }
        protected void lbtnResendOtp_Click(object sender, EventArgs e)
       
        {
            //if (Session["username"] == null)
            //Message( "Please login first.");
            //Response.Redirect("~/Default.aspx");
            //  return RedirectToAction("Login");
        

            string Email = Session["email"].ToString();
            string PhoneNo = Session["phone"]?.ToString();
            string Name = Session["staffName"]?.ToString();
            string maskedPhone = PhoneNo.Length >= 12
? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
: PhoneNo;


            string otp = GenerateOtp(6);
            Session["otp"] = otp;
            Session["OtpCode"] = otp;
            Session["OtpGeneratedAt"] = DateTime.UtcNow;

            string subject = "Telposta Staff Portal OTP - Resend";
            string body = $"Dear {Name}, your OTP for the Staff Portal is {otp} . It is valid for 5 minutes. ";
            Components.SentEmailAlerts(Email, subject, body);
            Components.SendSMSAlerts(PhoneNo, body);

            Message( $"A new OTP has been sent to your email: {Email} and phone: {maskedPhone}");
            // Response.Redirect($"pages/verifyotp.aspx?");

            Response.Redirect("verifyOTP.aspx", false);
            Context.ApplicationInstance.CompleteRequest();

        }

        public static string GenerateOtp(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        private void Message(string message)
        {
            string strScript = $"<script>alert('{message}');</script>";
            Page.RegisterStartupScript("ClientScript", strScript);
        }
    }
}