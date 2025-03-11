using Newtonsoft.Json.Linq;
using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public ActionResult MemberStatement()
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
                return RedirectToAction("payperiods", "jobapplication");
            }
            return View(PensionerStatement);
        }
        public ActionResult GenerateMemberStatement(string pensionerNo, string pensionerStatus, DateTime? startDate, DateTime? endDate)
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

                webportals.GeneratePensionStatement(pensionerNo, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), pdfFileName, pensionerStatus);

                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("MemberStatement");
        }


        public ActionResult GenerateMemberStatement1(string pensionerNo,string pensionerStatus ,DateTime? startDate, DateTime? endDate)
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            if (startDate == null || endDate == null)
            {
                ViewBag.Error = "Both Start Date and End Date must be provided.";
                return RedirectToAction("MemberStatement");
            }


            try
            {
                pensionerNo = Session["pensionerNo"].ToString();
                pensionerStatus = Session["Status"].ToString();


               string fileName = pensionerNo.Replace(@"/", @"");
                string pdfFileName = $"Statement-{fileName}.pdf";
               
                Console.WriteLine($"Start Date: {startDate}, End Date: {endDate}");

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

                
               // webportals.GeneratePensionStatement(pensionerNo, Convert.ToDateTime("7/1/1999").ToString("yyyy-MM-dd"), Convert.ToDateTime("8/1/1999").ToString("yyyy-MM-dd"), pdfFileName, pensionerStatus);
                webportals.GeneratePensionStatement(pensionerNo, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), pdfFileName, pensionerStatus);


                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
               
             

                ViewBag.PdfUrl = Url.Content($"~/Downloads/" + $"PensionerStatement{fileName}.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return RedirectToAction("MemberStatement");
            }

            return RedirectToAction("MemberStatement");
        }
        public ActionResult LifeCertificate(string pensionerNo)
        {
            pensionerNo = Session["pensionerNo"]?.ToString();
            if (pensionerNo == null) return RedirectToAction("index", "login");
            string fileName = pensionerNo.Replace(@"/", @"");
            string pdfFileName = $"LifeCertificate-{fileName}.pdf";
          
            string path = Server.MapPath("~/Downloads/");
            string pdfFilePath = Path.Combine(path, pdfFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); // Create the Downloads folder if it doesn't exist
            }
            DateTime period = DateTime.Today;
           
            try
            {
                 webportals.LifeCertificate(path, pdfFileName, pensionerNo, period);
               // webportals.LifeCertificate(path2, fileName, pensionerNo, period);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                ViewBag.Error = "An error occurred while generating the life certificate.";
                return View();
            }

            //  ViewBag.filepath = Url.Content($"~/Downloads/{pdfFileName}").Replace("http://", "https://");
            if (System.IO.File.Exists(pdfFilePath))
            {
                // Set the URL for the PDF file
                ViewBag.PdfUrl = Url.Content($"~/Downloads/{pdfFileName}");
            }
            else
            {
                ViewBag.Error = "Life Certificate generation failed. File not found.";
            }
            return View();

        }


    }
}