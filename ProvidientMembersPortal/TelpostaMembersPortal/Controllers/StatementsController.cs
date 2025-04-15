using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using TelpostaMembersPortal.Models;
using TelpostaMembersPortal.NAVWS;

namespace TelpostaMembersPortal.Controllers
{
    public class StatementsController : Controller
    {
        //  private string connectionString = "your_database_connection_string"; // Update with your DB details

        private readonly string[] strLimiters2 = new string[] { "[]" };
        private readonly string[] strLimiters = new string[] { "::" };
        // GET: Dashboard

        Portal webportals = Components.Portal;
        //  private Helper _helper = new Helper();
        public ActionResult Index()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");

            return View();
        }
        
        public ActionResult MemberStatement()
        {
            if (Session["memberNo"] == null)
                return RedirectToAction("index", "login");

            var model = new MemberStatement();
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

        public ActionResult GenerateMemberStatement( DateTime? startDate, DateTime? endDate)
        {
            string memberNo = Session["memberNo"]?.ToString();
            if (memberNo == null)
                return RedirectToAction("index", "login");

            try
            {
                string fileName = memberNo.Replace(@"/", @"");
                string pdfFileName = $"Statement-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                    throw new Exception("Resolved path is null or empty.");

                string pdfFilePath = Path.Combine(path, pdfFileName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(pdfFilePath))
                    System.IO.File.Delete(pdfFilePath);

                //static date
                //  DateTime periodDate = Convert.ToDateTime("01/01/25");


                webportals.GenerateMemberStatement(path, pdfFileName, memberNo, Convert.ToDateTime(startDate));
               
                 TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
               

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("MemberStatement");
        }

        public ActionResult GenerateMemberStatement1(string memberNo, DateTime? startDate, DateTime? endDate)
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");
            if (startDate == null || endDate == null)
            {
                ViewBag.Error = "Both Start Date and End Date must be provided.";
                return RedirectToAction("MemberStatement");
            }

            memberNo = Session["memberNo"]?.ToString();

            if (string.IsNullOrEmpty(memberNo))
            {
                ViewBag.Error = "Member number is missing.";
                return RedirectToAction("MemberStatement");
            }

            try
            {
                string fileName = memberNo.Replace(@"/", @"");
                string pdfFileName = $"MemberStatement-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                    throw new Exception("Resolved path is null or empty.");

                string pdfFilePath = Path.Combine(path, pdfFileName);
                Debug.WriteLine($"Resolved path: {path}");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(pdfFilePath))
                    System.IO.File.Delete(pdfFilePath);

                webportals.GenerateMemberStatement(path,memberNo, pdfFileName, Convert.ToDateTime(startDate));
                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
                //ViewBag.PdfUrl = TempData["PdfUrl"];

                //ViewBag.PdfUrl = Url.Content($"~/Downloads/" + $"MemberStatement{fileName}.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return RedirectToAction("MemberStatement");
            }

            return RedirectToAction("MemberStatement");
        }

        public ActionResult BeneficiaryNomination()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");
            string memberNo = Session["memberNo"].ToString();

            try
            {
                string fileName = memberNo.Replace(@"/", @"");
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
                    
                    return RedirectToAction("BeneficiaryNomination");
                }
                string memberNo = Session["memberNo"].ToString();
                string memberName = Session["memberName"].ToString();
                string fileName = Path.GetFileName(AttachmentFile.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                AttachmentFile.SaveAs(filePath);

                string subject = "Beneficiary Nomination Form Submission";
                string body = $"Dear Admin,<br/><br/>A new beneficiary nomination form has been submitted by {memberName} PF No. {memberNo}.<br/><br/> This is sytem generated. Do not Reply.";

                Components.SendEmailAlertswithAttachment(subject, body, filePath);

                TempData["Success"] = "File submitted successfully!";

            }
            catch (Exception ex)
            {
                // ViewBag.Error = "Error sending email: " + ex.Message;
                TempData["Error"] = "Error sending email." + ex.Message;
            }

            return RedirectToAction("BeneficiaryNomination");

        }

    }
}

