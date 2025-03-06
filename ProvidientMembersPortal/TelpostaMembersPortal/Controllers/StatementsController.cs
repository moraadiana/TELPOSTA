using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");
            MemberStatement MemberStatement = new MemberStatement();
            try
            {
                var periods = Helper.GetPayrollPeriods();

                MemberStatement.PayrollPeriods = periods;

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("payperiods", "jobapplication");
            }
            return View(MemberStatement);
        }


        public ActionResult GenerateMemberStatement(string memberNo, DateTime? startDate, DateTime? endDate)
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
                string returnstring = "";
                string fileName = memberNo.Replace(@"/", @"");
                string pdfFileName = $"Statement-{fileName}.pdf";

                DateTime startDateOnly = startDate.Value.Date;
                string startingDate = startDateOnly.ToString("yyyy-MM-dd");
                DateTime endDateOnly = endDate.Value.Date;
                string endingDate = endDateOnly.ToString("yyyy-MM-dd");

                Console.WriteLine($"Pensioner No: {memberNo}");

                string path = Server.MapPath("~/Downloads/");
                string pdfFilePath = Path.Combine(path, pdfFileName);
                //Console.WriteLine($"Start Date: {startDateOnly}, End Date: {endDateOnly}");
                Console.WriteLine($"Start Date: {startingDate}, End Date: {endingDate}");



                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Delete(pdfFilePath);
                }


                //webportals.GeneratePensionStatement(pensionerNo, Convert.ToDateTime("7/1/1999").ToString("yyyy-MM-dd"), Convert.ToDateTime("8/1/1999").ToString("yyyy-MM-dd"), pdfFileName);

                byte[] bytes = Convert.FromBase64String(returnstring);
                //string path = HostingEnvironment.MapPath("~/Download/" + $"PAYSLIP{filename}.pdf");


                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                FileStream stream = new FileStream(path, FileMode.CreateNew);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
                //  myPDF.Attributes.Add("src", ResolveUrl("~/Download/" + String.Format("PAYSLIP{0}.pdf", filename)));

                ViewBag.PdfUrl = Url.Content($"~/Downloads/" + $"MemberStatement{fileName}.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return RedirectToAction("MemberStatement");
            }

            return RedirectToAction("MemberStatement");
        }
       

        }


}

