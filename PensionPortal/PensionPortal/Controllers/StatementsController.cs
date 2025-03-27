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
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            PensionerStatement PensionerStatement = new PensionerStatement();
            try
            {
                var periods = Helper.GetPayrollPeriods();

                PensionerStatement.PayrollPeriods = periods;
                ViewBag.PdfUrl = TempData["PdfUrl"];

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
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
            if (Session["pensionerNo"] == null)
                return RedirectToAction("index", "login");

            var model = new LifeCertificate();
            try
            {
                model.LifeCertPeriods = Helper.GetLifeCertPeriods();
               
                ViewBag.PdfUrl = TempData["PdfUrl"];



            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
         public ActionResult GenerateLifeCertificate(DateTime period)
         {
            string pensionerNo = Session["pensionerNo"]?.ToString();
            if (pensionerNo == null)
             return RedirectToAction("index", "login");

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

                //static date
              //  DateTime periodDate = Convert.ToDateTime("01/01/25");
                

             string response = webportals.LifeCertificate1(path,pdfFileName, pensionerNo, period);
                if (!string.IsNullOrEmpty(response))
                {
                    if(response=="SUCCESS")
                    {
                        TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
                    }
                   
                   
                }
              
                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");
               
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




    }
}