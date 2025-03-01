using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public ActionResult Index()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            return View();
        }

        public ActionResult MemberStatement(string pensionerNo, DateTime? startDate, DateTime? endDate)
        {
            
            

           

            pensionerNo = Session["pensionerNo"]?.ToString();
            try
            {
                var list = new List<PensionerStatement>();
                PensionerStatement PensionerStatement = new PensionerStatement();
                var periods = webportals.GetPayrollPeriods();
                
               
                if (!string.IsNullOrEmpty(periods))
                {

                    string[] periodsArr = periods.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    Array.Reverse(periodsArr);
                    foreach (string line in periodsArr)
                    {

                        string[] responseArr = line.Split(strLimiters, StringSplitOptions.None);
                        list.Add(new PensionerStatement()
                        {

                            StartDate = Convert.ToDateTime(responseArr[0]),
                            EndDate = Convert.ToDateTime(responseArr[0]),
                          
                        });
                    }
                    PensionerStatement.PayrollPeriods = list;


                    string fileName = pensionerNo.Replace("/", "");
                    string pdfFileName = $"PensionerStatement-{fileName}.pdf";
                    string path = "D:\\Portals\\TELPOSTA\\PensionPortal\\PensionPortal\\Downloads\\";
                    string path2 = "C:\\inetpub\\wwwroot\\Portals\\PensionPortal\\Downloads\\";
                    Console.WriteLine($"Start Date: {startDate}, End Date: {endDate}");

                    // Call the method with the validated dates
                    webportals.PensionerStatement(path2, fileName, pensionerNo, startDate.Value, endDate.Value);
                    webportals.PensionerStatement(path, fileName, pensionerNo, startDate.Value, endDate.Value);
                    //webportals.PensionerStatement(path, fileName, pensionerNo, DateTime.Parse("2024-01-01"), DateTime.Parse("2024-07-01"));

                    ViewBag.PdfUrl = Url.Content($"~/Downloads/{pdfFileName}");
                    return View(new PensionerStatement { PayrollPeriods = list });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }

            return View();
        }

        public ActionResult MemberStatement1(string pensionerNo, DateTime? startDate, DateTime? endDate)
        {



            if (startDate == null || endDate == null)
            {
                ViewBag.Error = "Both Start Date and End Date must be provided.";
                return View();
            }

            pensionerNo = Session["pensionerNo"]?.ToString();
            try
            {
                string returnstring = "";
                string fileName = pensionerNo.Replace(@"/", @"");
               // string pdfFileName = $"PensionerStatement-{fileName}.pdf";
               
                Console.WriteLine($"Start Date: {startDate}, End Date: {endDate}");

                // Call the method with the validated dates
                webportals.PensionerStatement1( pensionerNo, startDate.Value, endDate.Value, String.Format("PensionerStatement{0}.pdf", fileName), ref returnstring);
                // webportals.PensionerStatement1(pensionerNo, DateTime.Parse("2024-01-01"), DateTime.Parse("2024-07-01"));
                //  myPDF.Attributes.Add("src", ResolveUrl("~/Download/" + String.Format("PAYSLIP{0}.pdf
                byte[] bytes = Convert.FromBase64String(returnstring);
                string path = "D:\\Portals\\TELPOSTA\\PensionPortal\\PensionPortal\\Downloads";
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                FileStream stream = new FileStream(path, FileMode.CreateNew);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
                ViewBag.PdfUrl = Url.Content($"~/Downloads/" + String.Format("PensionerStatement{0}.pdf", fileName));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }

            return View();
        }



    }
}