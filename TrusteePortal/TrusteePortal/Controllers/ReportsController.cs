using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using TrusteePortal.Models;
using TrusteePortal.NAVWS;

namespace TrusteePortal.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        Trustee webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pnine()
        {
            if (Session["trusteeNo"] == null)
                return RedirectToAction("index", "login");

            var model = new Pnine();
            try
            {
                ViewBag.PdfUrl = TempData["PdfUrl"];
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
        
        public ActionResult GenerateP9Report(DateTime period)
        {
            string trusteeNo = Session["trusteeNo"]?.ToString();
            if (trusteeNo == null)
                return RedirectToAction("index", "login");

            try
            {
                string fileName = trusteeNo.Replace(@"/", @"");
                string pdfFileName = $"P9-{fileName}.pdf";

                string path = Server.MapPath("~/Downloads/");
                if (string.IsNullOrEmpty(path))
                    throw new Exception("Resolved path is null or empty.");

                string pdfFilePath = Path.Combine(path, pdfFileName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(pdfFilePath))
                    System.IO.File.Delete(pdfFilePath);

                string periodDate = Convert.ToString(period);
                if (string.IsNullOrEmpty(periodDate))
                {
                    TempData["Error"] = "Select period";
                }
                
               

               webportals.GenerateP9(trusteeNo, period, path, pdfFileName);
               

                TempData["PdfUrl"] = Url.Content($"~/Downloads/{pdfFileName}");

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Pnine");
        }
    }
}