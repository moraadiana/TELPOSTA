using Newtonsoft.Json.Linq;
using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace PensionPortal.Controllers
{
    public class StatementsController : Controller
    {
        //  private string connectionString = "your_database_connection_string"; // Update with your DB details
        
        private readonly string[] strLimiters2 = new string[] { "[]" };
        private readonly string[] strLimiters = new string[] { "::" };
        // GET: Dashboard

        Pension webportals = Components.ObjNav;
        private Helper _helper = new Helper();
        public ActionResult Index()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            return View();
        }
        public ActionResult PensionerStatement()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            PensionerStatement PensionerStatement = new PensionerStatement();
            try
            {
                var periods = Helper.GetPayrollPeriods();

                PensionerStatement.PayrollPeriods = periods;
               
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                //return RedirectToAction("payperiods", "jobapplication");
            }
            return View(PensionerStatement);
        }
        public ActionResult GeneratePensionerStatement(string pensionerNo, string pensionerStatus, DateTime? startDate, DateTime? endDate)
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            if (startDate == null || endDate == null)
            {
                TempData["Error"] = "Both Start Date and End Date must be provided.";
                return RedirectToAction("MemberStatement");
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

                webportals.GeneratePensionStatement(pensionerNo, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), pdfFileName);

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
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            LifeCertificate LifeCertificate = new LifeCertificate();
            try
            {
                var periods = Helper.GetLifeCertPeriods();

                LifeCertificate.LifeCertPeriods = periods;

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

            }
            return View(LifeCertificate);
        }
        public ActionResult GenerateLifeCertificate(string pensionerNo, DateTime period)
        {
            pensionerNo = Session["pensionerNo"]?.ToString();
            //string date = "8/31/2024";
           // period = Convert.ToDateTime(date);
            if (pensionerNo == null) return RedirectToAction("index", "login");

            try
            {
                string fileName = Regex.Replace(pensionerNo, @"[\/:*?""<>|]", "");
                string pdfFileName = $"LifeCertificate-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception("Resolved path is null or empty.");
                }

                string pdfFilePath = Path.Combine(path, pdfFileName);
                Debug.WriteLine($"Resolved path: {path}");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Delete(pdfFilePath);
                }

                webportals.LifeCertificate(path, pdfFileName, pensionerNo, period);
                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View("LifeCertificate");
        }

        public ActionResult GenerateLifeCertificate1(string pensionerNo, DateTime period)
        {
            pensionerNo = Session["pensionerNo"]?.ToString();
            if (pensionerNo == null) return RedirectToAction("index", "login");
           
           
            try
            {
                string date = "8/31/2024";
                period = Convert.ToDateTime(date);
                string fileName = pensionerNo.Replace(@"/", @"");
                string pdfFileName = $"LifeCertificate-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                string pdfFilePath = Path.Combine(path, pdfFileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); // Create the Downloads folder if it doesn't exist
                }

                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Delete(pdfFilePath);
                }

                webportals.LifeCertificate(path, pdfFileName, pensionerNo, period);
                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
               
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View("LifeCertificate");

        }


    }
}