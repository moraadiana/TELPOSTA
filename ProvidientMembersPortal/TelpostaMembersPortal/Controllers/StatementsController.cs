using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
        public ActionResult Index()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");
            return View();
        }

        public ActionResult MemberStatement(string memberNo, DateTime? startDate, DateTime? endDate)
        
        {
            if (Session["memberNo"] == null)
                return RedirectToAction("index", "login");
            memberNo = Session["memberNo"].ToString();
            DateTime defaultStartDate = DateTime.Now.AddMonths(-1);
            DateTime defaultEndDate = DateTime.Now;

            var model = new MemberStatement
            {
                StartDate = startDate ?? defaultStartDate,
                EndDate = endDate ?? defaultEndDate
            };

            //string membNo = Session["memberNo"].ToString();
            string fileName = memberNo.Replace(@"/", @"");
            string pdfFileName = $"MemberStatement-{fileName}.pdf";
            string path = "D:\\TELPOSTA\\TelpostaMembersPortal\\TelpostaMembersPortal\\Downloads\\";
            try
            {
                webportals.PensionerStatement1(path, fileName, memberNo, startDate.Value,endDate.Value);
            }
            catch (Exception ex)
            {
               // ex.Data.Clear();
                ViewBag.ErrorMessage = "An error occurred while generating the statement.";
                Console.WriteLine(ex.Message);
            }
            ViewBag.PdfUrl = Url.Content($"~/Downloads/{pdfFileName}");

            return View();





        }

    }
}
