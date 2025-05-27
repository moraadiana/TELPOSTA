using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TelpostaMembersPortal.Models;
using TelpostaMembersPortal.NAVWS;

namespace TelpostaMembersPortal.Controllers
{
    public class LoginController : Controller
    {

        Portal webportals = Components.Portal;
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
                bool isValid = webportals.CheckValidMemberNo(PFno);
                Console.WriteLine($"CheckValidMemberNo returned: {isValid}");
                bool changedpwd = false;

                if (webportals.CheckValidMemberNo(PFno))
                {
                    string responsepwd = webportals.CheckMemberPasswordChanged(PFno);
                    if (!string.IsNullOrEmpty(responsepwd))
                    {
                        if (responsepwd == "Yes")
                        {
                            changedpwd = true;
                        }
                    }
                    if (changedpwd == true)
                    {
                        string response = webportals.CheckMemberLogin(PFno, password);
                        if (!string.IsNullOrEmpty(response))
                        {
                            string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                            string returnMsg = responseArr[0];
                            if (returnMsg == "SUCCESS")
                            {
                                string memberNo = responseArr[2];
                                string memberName = responseArr[3];
                                string memberEmail = responseArr[4];
                                string PhoneNo = responseArr[5];

                                Session["memberNo"] = memberNo;
                                Session["memberName"] = memberName;
                                Session["memberEmail"] = memberEmail;
                                Session["PhoneNo"] = PhoneNo;

                                string otp = GenerateOtp(6);
                                Session["otp"] = otp;
                                Session["OtpCode"] = otp;
                                Session["OtpGeneratedAt"] = DateTime.UtcNow;
                                string maskedPhone = PhoneNo.Length >= 12
                             ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
                             : PhoneNo;

                                string subject = "Telposta Provident Fund Portal OTP";

                                string body = $"Dear {memberName}, your OTP for the Provident Fund Portal is {otp} . It is valid for 5 minutes. ";
                                //Components.SendEmailAlerts(memberEmail, subject, body);
                                //Components.SendSMSAlerts(PhoneNo, body);
                                //TempData["success"] = $"An OTP has been sent to your email: {memberEmail} and phone: {maskedPhone}";
                                //return RedirectToAction("verifyotp");
                                return RedirectToAction("index", "dashboard");
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
                                string memberNo = responseArr[1];
                                string memberName = responseArr[2];
                                string memberEmail = responseArr[3];
                                // string vendorVat = responseArr[4];

                                Session["memberNo"] = memberNo;
                                Session["memberName"] = memberName;
                                Session["memberEmail"] = memberEmail;
                                // Response.Redirect($"ResetPassword.aspx?staffNo={staffNo}&email={staffEmail}");
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
            if (Session["memberNo"] == null) return View("index");
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
        public void SendPasswordResetLink(string email, string memberNo)
        {
            try
            {
                //Session["memberEmail"] = email;
               
             string memberName = webportals.GetMemberName(memberNo);
                string subject = "Telposta Provident Fund Portal Password Reset";
                string body = $"Hello {memberName};" +
                    $"<br/><br/>" +
                    $"Please follow the link below to reset your Telposta Provident Fund Portal Account Password." +
                    $"<br/><br/>" +
                    $"<a href='{String.Format(@"{0}://{1}/login/resetpassword?", Request.Url.Scheme, Request.Url.Authority)}'>Click here.</a>" +
                    $"<br/><br/>" +
                    $"Regards, Administrator";
                Components.SendEmailAlerts(email, subject, body);
                TempData["Success"] = "Check your email to reset password";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        [ChildActionOnly]
        public PartialViewResult Notification()
        {
            return PartialView("_Notification");
        }
        public ActionResult ResendOtp()
        {
            if (Session["memberNo"] == null)
            {
                TempData["error"] = "Please login first.";
                return RedirectToAction("Login");
            }

            string memberEmail = Session["memberEmail"]?.ToString();
            string PhoneNo = Session["PhoneNo"]?.ToString();
            string memberName = Session["memberName"]?.ToString();
            string maskedPhone = PhoneNo.Length >= 12
 ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
 : PhoneNo;
            string otp = GenerateOtp(6);
            Session["otp"] = otp;
            Session["OtpCode"] = otp;
            Session["OtpGeneratedAt"] = DateTime.UtcNow;

            string subject = "Telposta Provident Fund Portal OTP - Resend";
            string body = $"Dear {memberName}, your OTP for the Provident Fund Portal is {otp} . It is valid for 5 minutes. ";
            Components.SendEmailAlerts(memberEmail, subject, body);
            Components.SendSMSAlerts(PhoneNo, body);

            TempData["success"] = $"A new OTP has been sent to your email: {memberEmail} and phone: {maskedPhone}";
            return RedirectToAction("VerifyOTP");
        }
        public ActionResult ResetPassword(string email, string memberNo)
        {
            // If email is provided in query string, store it in session
            if (!string.IsNullOrEmpty(email))
            {
                Session["memberEmail"] = email;
            }

            if (!string.IsNullOrEmpty(memberNo))
            {
                Session["memberNo"] = memberNo;
            }

            // Retrieve stored session values if they exist
            ViewBag.memberEmail = Session["memberEmail"] as string;
            ViewBag.memberNo = Session["memberNo"] as string;

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            try
            {
                if (Session["memberNo"] == null)
                {
                    TempData["Error"] = "Input PF No.";
                    return RedirectToAction("index", "login");
                }
                string newPassword = reset.Password;
                string confirmPassword = reset.PasswordConfirmation;
                string memberNo = Session["memberNo"].ToString();

                string response = webportals.UpdateMemberPassword(memberNo, newPassword);
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
        public ActionResult ForgotPassword(string email, string memberNo)
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ResetPassword reset)
        {
            try
            {

                string newPassword = GenerateRandomPassword(10);
                string memberNo = reset.memberNo;
                //string memberEmail = reset.Email;
                string response1 = webportals.GetMemberProfileDetails(memberNo);
                if (response1 != null)
                {
                    string[] responseArr = response1.Split(strLimiters, StringSplitOptions.None);
                    Session["Email"] = responseArr[0];
                    Session["PhoneNo"] = responseArr[1];
                    Session["MmemberName"] = responseArr[1];
                }
                    //string email = Components.ObjNav.GetPensionerEmail(pensionerNo);
                    string PhoneNo = Session["PhoneNo"].ToString();
                string maskedPhone = PhoneNo.Length >= 12
  ? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
  : PhoneNo;
                string memberEmail = Session["Email"].ToString();
                string memberName = Session["memberName"].ToString();

                    string response = webportals.UpdateMemberAutoGenPassword(memberNo, newPassword);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response != "SUCCESS")
                    {
                        TempData["Error"] = "Failed to reset the password. Please try again.";
                        View("forgotpassword");
                    }

                }
                string subject = "Telposta Provident Fund Portal Password Reset";
                string body = $"Dear Member, Use this password to log in: {newPassword}. Do not reply.";
                Components.SendEmailAlerts(memberEmail, subject, body);
                Components.SendSMSAlerts(PhoneNo, body);
                //return RedirectToAction("verifyotp");
                //SendPasswordResetLink(username);
                TempData["Success"] = $"Auto generated password has been sent to your email address {memberEmail} and phone number: {maskedPhone}";
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
    }
}