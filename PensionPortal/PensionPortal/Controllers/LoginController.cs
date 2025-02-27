using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.Linq;
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
                bool isValid = webportals.CheckValidPensionerNo(PFno);
                Console.WriteLine($"CheckValidPensionerNo returned: {isValid}");
                if (webportals.CheckValidPensionerNo(PFno))
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
                            // string vendorVat = responseArr[4];

                            Session["pensionerNo"] = pensionerNo;
                            Session["pensionerName"] = pensionerName;
                            Session["pensionerEmail"] = pensionerEmail;
                            // Session["VendorVat"] = vendorVat;

                            //string otp = GenerateOtp(6);
                            //Session["otp"] = otp;

                            //string subject = "Telposta Pension Portal OTP";
                            //string body = $"{otp} is your OTP Code for Telposta Pension portal.";
                            //Components.SendEmailAlerts(pensionerEmail, subject, body);
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

        [ChildActionOnly]
        public PartialViewResult Notification()
        {
            return PartialView("_Notification");
        }
    }
}