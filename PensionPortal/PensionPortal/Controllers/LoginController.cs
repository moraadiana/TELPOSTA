using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace PensionPortal.Controllers
{
    public class LoginController : Controller
    {


        Pension webportals = Components.ObjNav;
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
                string phoneNo = account.PhoneNo;
                bool isValid = webportals.CheckValidPensionerNo(PFno);
                Console.WriteLine($"CheckValidPensionerNo returned: {isValid}");
                bool changedpwd = false;
               
                if (webportals.CheckValidPensionerNo(PFno))
                {
                    string responsepwd = webportals.CheckPensionerPasswordChanged(PFno);
                    if (!string.IsNullOrEmpty(responsepwd))
                    {
                        if (responsepwd == "Yes")
                        {
                            changedpwd = true;
                        }
                    }
                    if (changedpwd == true)
                    {
                        string response = webportals.CheckPensionerLogin(PFno, password);
                        if (!string.IsNullOrEmpty(response))
                        {
                            string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                            string returnMsg = responseArr[0];
                            if (returnMsg == "SUCCESS")
                            {
                                string pensionerNo = responseArr[2];
                                string pensionerName = responseArr[3];
                                string pensionerEmail = responseArr[4];
                                string pensionerPhoneNo = responseArr[4];

                                Session["pensionerNo"] = pensionerNo;
                                Session["pensionerName"] = pensionerName;
                                Session["pensionerEmail"] = pensionerEmail;
                                Session["pensionerPhoneNo"] = pensionerPhoneNo;
                                // Session["VendorVat"] = vendorVat;

                                string otp = GenerateOtp(6);
                                Session["otp"] = otp;

                                string subject = "Telposta Pension Portal OTP";
                                string body = $"{otp}";
                                Components.SendEmailAlerts(pensionerEmail, subject, body);
                                Components.SendSMSAlerts(pensionerPhoneNo, subject, body) ;
                                return RedirectToAction("verifyotp");



                                //return RedirectToAction("index", "dashboard");
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
                                string pensionerNo = responseArr[1];
                                string pensionerName = responseArr[2];
                                string pensionerEmail = responseArr[3];
                                // string vendorVat = responseArr[4];

                                Session["pensionerNo"] = pensionerNo;
                                Session["pensionerName"] = pensionerName;
                                Session["pensionerEmail"] = pensionerEmail;
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
            if (Session["pensionerNo"] == null) return View("index");
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
        public ActionResult ResetPassword(string email, string pensionerNo)
        {
            // If email is provided in query string, store it in session
            if (!string.IsNullOrEmpty(email))
            {
                Session["EmailAddress"] = email;
            }

            if (!string.IsNullOrEmpty(pensionerNo))
            {
                Session["pensionerNo"] = pensionerNo;
            }

            // Retrieve stored session values if they exist
            ViewBag.EmailAddress = Session["EmailAddress"] as string;
            ViewBag.PensionerNo = Session["pensionerNo"] as string;

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            try
            {
                if (Session["pensionerNo"] == null)
                {
                    TempData["Error"] = "Session expired. Please log in again.";
                    return RedirectToAction("index", "login");
                }
                string newPassword = reset.Password;
                string confirmPassword = reset.PasswordConfirmation;
                string pensionerNo = Session["pensionerNo"].ToString();

                string response = webportals.UpdatePensionerPassword(pensionerNo, newPassword);
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
                string pensionerNo = reset.PfNo;
                string pensionerEmail= reset.Email;
                //string email = Components.ObjNav.GetPensionerEmail(pensionerNo);
                string response = Components.ObjNav.UpdatePensionerAutoGenPassword(pensionerNo, newPassword);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response != "SUCCESS")
                    {
                        TempData["Error"] = "Failed to reset the password. Please try again.";
                        View("forgotpassword");
                    }

                }
                string subject = "Telposta Pension Portal Password Reset";
                string body = $"Use this password to log into Telposta Pension Portal.<br/><br/>Auto generated Portal password: <strong>{newPassword}</strong> <br/> <br/>Do not reply to this email.";
                Components.SendEmailAlerts(pensionerEmail, subject, body);
                //return RedirectToAction("verifyotp");
                //SendPasswordResetLink(username);
                TempData["Success"] = $"Auto generated password has been sent to your email address {pensionerEmail}";
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