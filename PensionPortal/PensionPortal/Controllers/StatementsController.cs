using Newtonsoft.Json.Linq;
using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
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

        public ActionResult  GenerateMemberStatement1(string payrollPeriod)
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            if (string.IsNullOrEmpty(payrollPeriod))
            {
                TempData["Error"] = "Please select a payroll period.";
                return RedirectToAction("MemberStatement");
            }
           
            // Initialize the model
           
            try
            {
                string pensionerNo = Session["pensionerNo"]?.ToString();
                string[] dates = payrollPeriod.Split('-');
                string startDate = dates[0].Trim();
                string endDate = dates[1].Trim();

                var pdfUrl = _helper.GeneratePensionerStatement(pensionerNo, startDate, endDate);
                if (string.IsNullOrEmpty(pdfUrl))
                {

                    TempData["Error"] = "Failed to generate the statement.";
                    return RedirectToAction("MemberStatement");

                }
                var model = new PensionerStatement
                {
                    PayrollPeriods = Helper.GetPayrollPeriods(),
                    PayrollPeriod = payrollPeriod,
                    PdfUrl = pdfUrl
                };

                return View("MemberStatement", model);
                // Call the method with the validated dates

                // return View(new PensionerStatement { PayrollPeriods = list });

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }

            return View();
        }
        public ActionResult GenerateMemberStatement(string pensionerNo, DateTime? startDate, DateTime? endDate)
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            if (startDate == null || endDate == null)
            {
                ViewBag.Error = "Both Start Date and End Date must be provided.";
                return RedirectToAction("MemberStatement");
            }

            pensionerNo = Session["pensionerNo"]?.ToString();

            if (string.IsNullOrEmpty(pensionerNo))
            {
                ViewBag.Error = "Pensioner number is missing.";
                return RedirectToAction("MemberStatement");
            }

            try
            {
                string returnstring = "";
                string fileName = pensionerNo.Replace(@"/", @"");
                string path = "D:\\Portals\\TELPOSTA\\PensionPortal\\PensionPortal\\Downloads";

                Console.WriteLine($"Pensioner No: {pensionerNo}");
                Console.WriteLine($"Start Date: {startDate.Value.ToString("yyyy-MM-dd")}, End Date: {endDate.Value.ToString("yyyy-MM-dd")}");
               // .Value.ToString("yyyy-MM-dd"
                // Ensure directory exists
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Ensure file does not exist before generating the statement
                string filePath = Path.Combine(path, $"PensionerStatement{fileName}.pdf");
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Call the web service to generate the statement
                webportals.PensionerStatement(path, fileName, pensionerNo,Convert.ToDateTime( startDate.Value.ToString("yyyy-MM-dd")), Convert.ToDateTime(endDate.Value.ToString("yyyy-MM-dd")));

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

            if (string.IsNullOrEmpty(pensionerNo))
            {
                ViewBag.Error = "Pensioner number is missing.";
                return RedirectToAction("lifecertificate");
            }
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
                 webportals.LifeCertificate(path, fileName, pensionerNo, period);
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