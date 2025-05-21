using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;
using TrusteePortal.Models;
using TrusteePortal.NAVWS;

namespace TrusteePortal.Controllers
{
    public class LoginController : Controller
    {


        Trustee webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Account account)
        {
            try
            {
                string PFno = account.PFno;
                string emailAddress = account.Email;
                string phoneNo = account.PhoneNo;
                string password = account.Password.Trim();
                bool isValid = webportals.CheckValidTrusteeNo(PFno);
               // Console.WriteLine($"CheckValidPensionerNo returned: {isValid}");
                bool changedpwd = false;

                if (webportals.CheckValidTrusteeNo(PFno))
                {
                    string responsepwd = webportals.CheckTrusteePasswordChanged(PFno);
                    if (!string.IsNullOrEmpty(responsepwd))
                    {
                        if (responsepwd == "Yes")
                        {
                            changedpwd = true;
                        }
                    }
                    if (changedpwd == true)
                    {
                        string response = webportals.CheckTrusteeLogin(PFno, password);
                        if (!string.IsNullOrEmpty(response))
                        {
                            string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                            string returnMsg = responseArr[0];
                            if (returnMsg == "SUCCESS")
                            {
                                string trusteeNo = responseArr[2];
                                string trusteeName = responseArr[3];
                                string trusteeEmail = responseArr[4];
                                string PhoneNo = responseArr[5];
                               

                                Session["trusteeNo"] = trusteeNo;
                                Session["trusteeName"] = trusteeName;
                                Session["trusteeEmail"] = trusteeEmail;
                                Session["PhoneNo"] = PhoneNo;


                                string otp = GenerateOtp(6);
                                Session["otp"] = otp;
                                Session["OtpCode"] = otp;
                                Session["OtpGeneratedAt"] = DateTime.UtcNow;
                                string maskedPhone = PhoneNo.Length >= 12
  ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
  : PhoneNo;

                                string subject = "Telposta Trustee Portal OTP";
                               // string body = $"{otp} ";
                                string body = $"Dear {trusteeName}, your OTP for the Trustee Portal is {otp} . It is valid for 5 minutes. ";
                                Components.SendEmailAlerts(trusteeEmail, subject, body);
                                Components.SendSMSAlerts(PhoneNo, body);
                                TempData["success"] = $"An OTP has been sent to your email: {trusteeEmail} and phone: {maskedPhone}";
                                return RedirectToAction("verifyotp");
                               // return RedirectToAction("statussummary", "dashboard");
                            }
                            else
                            {
                                TempData["error"] = returnMsg;
                                return View("index");
                            }
                        }
                    }
                    else
                    {
                        string response = webportals.LoginForUnchangedPassword(PFno, password);
                        if (!string.IsNullOrEmpty(response))
                        {
                            string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                            string returnMsg = responseArr[0];
                            if (returnMsg == "SUCCESS")
                            {
                                string trusteeNo = responseArr[1];
                                string trusteeName = responseArr[2];
                                string trusteeEmail = responseArr[3];
                                

                                Session["trusteeNo"] = trusteeNo;
                                Session["trusteeName"] = trusteeName;
                                Session["trusteeEmail"] = trusteeEmail;
                         
                                return RedirectToAction("resetpassword", "login");
                            }
                            else
                            {
                                TempData["error"] = returnMsg;
                                return View("index");
                            }

                        }
                    }
                }
                else
                {
                    TempData["error"] = "Invalid PF No. ";
                    return View("index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("index");
            }
            return View();
        }

        public ActionResult VerifyOTP()
        {
            if (Session["trusteeNo"] == null) return View("index");
            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(OTP otp)
        {
            try
            {
                string generatedOtp = Session["OtpCode"] as string;
                DateTime? generatedAt = Session["OtpGeneratedAt"] as DateTime?;
                string otpFromUser = otp.OTPCode?.Trim();

                if (generatedOtp == null || generatedAt == null)
                {
                    TempData["error"] = "OTP session expired. Please login again.";
                    return RedirectToAction("Login");
                }

                if (generatedOtp.Equals(otpFromUser, StringComparison.OrdinalIgnoreCase) &&
                    DateTime.UtcNow <= generatedAt.Value.AddMinutes(5))
                {
                    // OTP valid
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (DateTime.UtcNow > generatedAt.Value.AddMinutes(5))
                {
                    TempData["error"] = "OTP has expired. Please request a new OTP.";
                    return RedirectToAction("VerifyOTP");
                }
                else
                {
                    TempData["error"] = "Invalid OTP. Please try again.";
                    return View("VerifyOTP");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("VerifyOTP");
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
        public ActionResult VerifyOTP1(OTP otp)
        {
            try
            {
                string generatedOtp = Session["otp"].ToString();
                string otpFromUser = otp.OTPCode.Trim();

                if (generatedOtp.ToLower() == otpFromUser.ToLower())
                {
                    return RedirectToAction("index", "dashboard");
                }
                else
                {
                    TempData["error"] = "Invalid OTP. Please try again later";
                    return View("verifyotp");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("verifyotp");
            }
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
        public ActionResult ResendOtp()
        {
            if (Session["trusteeNo"] == null)
            {
                TempData["error"] = "Please login first.";
                return RedirectToAction("Login");
            }

            string trusteeEmail = Session["trusteeEmail"]?.ToString();
            string PhoneNo = Session["PhoneNo"]?.ToString();
            string maskedPhone = PhoneNo.Length >= 12
  ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
  : PhoneNo;
            string trusteeName = Session["trusteeName"]?.ToString();

            string otp = GenerateOtp(6);
            Session["otp"] = otp;
            Session["OtpCode"] = otp;
            Session["OtpGeneratedAt"] = DateTime.UtcNow;

            string subject = "Telposta Trustee Portal OTP - Resend";
            string body = $"Dear {trusteeName}, your OTP for the Trustee Portal is {otp} . It is valid for 5 minutes. ";
            Components.SendEmailAlerts(trusteeEmail, subject, body);
            Components.SendSMSAlerts(PhoneNo, body);

            TempData["success"] = $"A new OTP has been sent to your email: {trusteeEmail} and phone: {maskedPhone}";
            return RedirectToAction("VerifyOTP");
        }
        public ActionResult ResetPassword(string email, string pensionerNo)
        {
            // If email is provided in query string, store it in session
            if (!string.IsNullOrEmpty(email))
            {
                Session["EmailAddress"] = email;
            }

            if (!string.IsNullOrEmpty(pensionerNo))
            {
                Session["trusteeNo"] = pensionerNo;
            }

            // Retrieve stored session values if they exist
            ViewBag.EmailAddress = Session["EmailAddress"] as string;
            ViewBag.PensionerNo = Session["trusteeNo"] as string;

            return View();
        }
       

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            try
            {
                if (Session["trusteeNo"] == null)
                {
                    TempData["Error"] = "Session expired. Please log in again.";
                    return RedirectToAction("index", "login");
                }
                string newPassword = reset.Password;
                string confirmPassword = reset.PasswordConfirmation;
                string pensionerNo = Session["trusteeNo"].ToString();

                string response = webportals.UpdateTrusteePassword(pensionerNo, newPassword);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response == "SUCCESS")
                    {
                        TempData["Success"] = "Password has been updated successfully";
                        return RedirectToAction("index", "login");
                    }
                    else
                    {
                        TempData["Error"] = "An error occured while updating your password. Please try again later.";
                        return RedirectToAction("resetpassword", "login");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View();
        }
        public ActionResult ForgotPassword(string email, string pensionerNo)
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ResetPassword reset)
        {
            try
            {

                string newPassword = GenerateRandomPassword(10);
                string trusteeNo = reset.PfNo;
                // string trusteeEmail = reset.Email;
                string response1 = webportals.GetTrusteeDetails(trusteeNo);
                if (response1 != null)
                {
                    string[] responseArr = response1.Split(strLimiters, StringSplitOptions.None);
                    Session["Name"] = responseArr[1];
                    Session["Email"] = responseArr[2];
                    Session["PhoneNo"] = responseArr[6];
                }
                string trusteeEmail = Session["Email"].ToString();
                string PhoneNo = Session["PhoneNo"].ToString();
                string maskedPhone = PhoneNo.Length >= 12
    ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
    : PhoneNo;
                string trusteeName = Session["Name"].ToString();
                    string response = Components.ObjNav.UpdateTrusteeAutoGenPassword(trusteeNo, newPassword);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response != "SUCCESS")
                    {
                        TempData["Error"] = "Failed to reset the password. Please try again.";
                        View("forgotpassword");
                    }

                }
                string subject = "Telposta Trustee Portal Password Reset";
                string body = $"Dear {trusteeName}, Use this password to log in: {newPassword} . Do not reply.";
                Components.SendEmailAlerts(trusteeEmail, subject, body);
                Components.SendSMSAlerts(PhoneNo, body);
                //return RedirectToAction("verifyotp");
                //SendPasswordResetLink(username);
                TempData["Success"] = $"Auto generated password has been sent to your email address: {trusteeEmail} and phone number: {maskedPhone}";
                //  return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("forgotpassword");
            }
            return View("index");
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] byteBuffer = new byte[1];

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(byteBuffer);
                    int index = byteBuffer[0] % validChars.Length;
                    password.Append(validChars[index]);
                }
            }

            return password.ToString();
        }
        [ChildActionOnly]
        public PartialViewResult Notification()
        {
            return PartialView("_Notification");
        }
    }
}