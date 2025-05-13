using Newtonsoft.Json.Linq;
using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace PensionPortal.Controllers
{
    public class ReportController : Controller
    {

        private readonly string[] strLimiters2 = new string[] { "[]" };
        private readonly string[] strLimiters = new string[] { "::" };


        Pension webportals = Components.ObjNav;
        private Helper _helper = new Helper();
        public ActionResult Index()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            return View();
        }
        
        public ActionResult PensionerStatement()
        {
            if (Session["pensionerNo"] == null)
                return RedirectToAction("index", "login");

            var model = new PensionerStatement();
            try
            {
                model.PayrollPeriods = Helper.GetPayrollPeriods();

                ViewBag.PdfUrl = TempData["PdfUrl"];



            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
        public ActionResult GeneratePensionerStatement(string pensionerNo, string pensionerStatus, DateTime? startDate, DateTime? endDate)
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            if (startDate == null || endDate == null)
            {
                TempData["Error"] = "Both Start Date and End Date must be provided.";
                return RedirectToAction("PensionerStatement");
            }

            try
            {
                pensionerNo = Session["pensionerNo"].ToString();
                pensionerStatus = Session["Status"].ToString();
                string fileName = pensionerNo.Replace(@"/", @"");
                string pdfFileName = $"Statement-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pdfFilePath = Path.Combine(path, pdfFileName);
                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Delete(pdfFilePath);
                }
                string base64Pdf = webportals.GeneratePortalPensionStatement(pensionerNo, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), pdfFileName);

                if (string.IsNullOrWhiteSpace(base64Pdf))
                {
                    TempData["Error"] = "No data was returned for the pension statement.";
                    return RedirectToAction("PensionerStatement");
                }

                byte[] pdfBytes = Convert.FromBase64String(base64Pdf);
                System.IO.File.WriteAllBytes(pdfFilePath, pdfBytes);


                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("PensionerStatement");
        }


        public ActionResult LifeCertificate()
        {
            if (Session["pensionerNo"] == null)
                return RedirectToAction("index", "login");

            var model = new LifeCertificate();
            try
            {
                model.LifeCertPeriods = Helper.GetLifeCertPeriods();

                ViewBag.PdfUrl = TempData["PdfUrl"];
                ViewBag.Error = TempData["Error"];
                ViewBag.Success = TempData["Success"];



            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
        public ActionResult GenerateLifeCertificate(DateTime? period)
        {
            string pensionerNo = Session["pensionerNo"]?.ToString();
            if (pensionerNo == null)
                return RedirectToAction("index", "login");

            if (period == null || period == DateTime.MinValue)
            {
                TempData["Error"] = "Please select a valid Period to generate the Life Certificate.";
                return RedirectToAction("LifeCertificate");
            }

            try
            {
                string fileName = pensionerNo.Replace(@"/", @"");
                string pdfFileName = $"LifeCertificate-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                    throw new Exception("Resolved path is null or empty.");

                string pdfFilePath = Path.Combine(path, pdfFileName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(pdfFilePath))
                    System.IO.File.Delete(pdfFilePath);



                string response = webportals.LifeCertificate1(path, pdfFileName, pensionerNo, period.Value);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response == "SUCCESS")
                    {
                        TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
                    }
                    else
                    {
                        TempData["Error"] = "Failed to generate Life Certificate. Please try again later.";
                    }


                }
                else
                {
                    TempData["Error"] = $"You are not eligible for life certificate for selected period due to non-return of previous period life certificate.";
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("LifeCertificate");
        }

        public ActionResult BeneficiaryNomination()
        {
            string pensionerNo = Session["pensionerNo"]?.ToString();
            if (pensionerNo == null) return RedirectToAction("index", "login");

            try
            {
                string fileName = pensionerNo.Replace(@"/", @"");
                string pdfFileName = $"DependantForm-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                    throw new Exception("Resolved path is null or empty.");

                string pdfFilePath = Path.Combine(path, pdfFileName);
                Debug.WriteLine($"Resolved path: {path}");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(pdfFilePath))
                    System.IO.File.Delete(pdfFilePath);

                webportals.GenerateBeneficiaryReport(path, pdfFileName);
                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
                ViewBag.PdfUrl = TempData["PdfUrl"];


            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View();
        }

       
        [HttpPost]
       
        public ActionResult SubmitBeneficiaryNomination(HttpPostedFileBase AttachmentFile)
        {
            if (AttachmentFile == null || AttachmentFile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a file to upload.";
                return View("BeneficiaryNomination"); 
            }

            try
            {
                if (AttachmentFile == null || AttachmentFile.ContentLength == 0)
                {
                    ViewBag.Error = "Please select a file to upload.";
                    TempData["Error"] = "Please select a file to upload.";
                    //return View("BeneficiaryNomination");
                    return RedirectToAction("BeneficiaryNomination");
                }
                //string pensionerEmail = Session["pensionerEmail"].ToString();
                string pensionerNo = Session["pensionerNo"].ToString();
                string pensionerName = Session["pensionerName"].ToString();
                string fileName = Path.GetFileName(AttachmentFile.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                AttachmentFile.SaveAs(filePath);

                string subject = "Beneficiary Nomination Form Submission";
                string body = $"Dear Admin,<br/><br/>A new beneficiary nomination form has been submitted by {pensionerName} PF NO. {pensionerNo}.<br/><br/> This is sytem generated. Do not Reply.";

                Components.SendEmailAlertswithAttachment(subject, body, filePath);

                TempData["Success"] = "File uploaded and email sent successfully!";

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending email." + ex.Message;
            }

            return RedirectToAction("BeneficiaryNomination");
    
        }
        [HttpPost]

        public ActionResult SubmitLifeCertificate(HttpPostedFileBase AttachmentFile)
        {
            if (AttachmentFile == null || AttachmentFile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a file to upload.";
                return View("BeneficiaryNomination");
            }

            try
            {
                if (AttachmentFile == null || AttachmentFile.ContentLength == 0)
                {
                    ViewBag.Error = "Please select a file to upload.";
                    TempData["UploadError"] = "Please select a file to upload.";
                    return RedirectToAction("BeneficiaryNomination");
                }
               
                string pensionerNo = Session["pensionerNo"].ToString();
                string pensionerName = Session["pensionerName"].ToString();
                string fileName = Path.GetFileName(AttachmentFile.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                AttachmentFile.SaveAs(filePath);

                string subject = "Life Certificate Submission";
                string body = $"Dear Admin,<br/><br/>A new Life Certificate has been submitted by {pensionerName} PF NO. {pensionerNo}.<br/><br/> This is sytem generated. Do not Reply.";

                Components.SendEmailAlertswithAttachment(subject, body, filePath);

                TempData["Success"] = "File uploaded and email sent successfully!";

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending email." + ex.Message;
            }

            return RedirectToAction("LifeCertificate");

        }
        public ActionResult MonthlyPension()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var schedule = new List<MonthlyPension>();
            try
            {
                string username = Session["pensionerNo"].ToString();
                string pensionList = webportals.GetMonthlyPension(username);
                if (!string.IsNullOrEmpty(pensionList))
                {
                    string[] pensionListArr = pensionList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                    foreach (string pension in pensionListArr)
                    {
                        string[] response = pension.Split(new string[] { "::" }, StringSplitOptions.None);
                        if (response.Length == 2)
                        {
                            DateTime parsedDate;
                            string formattedDate = response[0].Trim();

                            if (DateTime.TryParseExact(formattedDate, "MM/dd/yy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                schedule.Add(new MonthlyPension()
                                {
                                    payPeriod = $"{parsedDate:MMMM yyyy}",
                                    Amount = response[1].Trim(),
                                    
                                    SortKey = parsedDate 
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            var sortedSchedule = schedule.OrderByDescending(x => x.SortKey).ToList();
            return View(sortedSchedule);

        }
        public ActionResult MonthlyDeductions()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var schedule = new List<MonthlyPension>();
            try
            {
                string username = Session["pensionerNo"].ToString();
                string pensionList = webportals.GetMonthlyPensionDeduction(username);
                if (!string.IsNullOrEmpty(pensionList))
                {
                    string[] pensionListArr = pensionList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                    foreach (string pension in pensionListArr)
                    {
                        string[] response = pension.Split(new string[] { "::" }, StringSplitOptions.None);
                        if (response.Length == 3)
                        {
                            DateTime parsedDate;
                            string formattedDate = response[0].Trim();

                            if (DateTime.TryParseExact(formattedDate, "MM/dd/yy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                schedule.Add(new MonthlyPension()
                                {
                                    payPeriod = $"{parsedDate:MMMM yyyy}",
                                    Amount = response[1].Trim(),
                                    Description = response[2].Trim(),
                                    SortKey = parsedDate 
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            var sortedSchedule = schedule.OrderByDescending(x => x.SortKey).ToList();
            return View(sortedSchedule);

        }

    }
}