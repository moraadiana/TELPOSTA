using System;
using System.Collections.Generic;
using System.Linq;
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
                string password = account.Password.Trim();
                bool isValid = webportals.CheckValidMemberNo(PFno);
                Console.WriteLine($"CheckValidMemberNo returned: {isValid}");
                if (webportals.CheckValidMemberNo(PFno))
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
                           // string vendorVat = responseArr[4];

                            Session["memberNo"] = memberNo;
                            Session["memberName"] = memberName;
                            Session["memberEmail"] = memberEmail;
                           // Session["VendorVat"] = vendorVat;

                            //string otp = GenerateOtp(6);
                            //Session["otp"] = otp;

                            //string subject = "Telposta Provident Fund Portal OTP";
                            //string body = $"{otp} is your OTP Code for Telposta Pension portal.";
                            //Components.SendEmailAlerts(memberEmail, subject, body);
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
                    TempData["error"] = "Invalid Vendor No. ";
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
        public ActionResult ResetPassword(string email , string memberNo)
        {
            Session["memberEmail"] = email;
            Session["memberNo"] = memberNo;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            try
            {
                string newPassword = reset.Password;
                string confirmPassword = reset.PasswordConfirmation;
                string memberNo = reset.memberNo;

                
                string response = webportals.ChangeMemberPassword(memberNo, newPassword);
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
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ResetPassword reset)
        {
            try
            {
                string username = reset.UserName;
                string memberNo = reset.memberNo;
                SendPasswordResetLink(username, memberNo);
                TempData["Success"] = "Please Follow the link sent to your email to set a password for your account.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("forgotpassword");
            }
            return View("forgotpassword");
        }
    }
}