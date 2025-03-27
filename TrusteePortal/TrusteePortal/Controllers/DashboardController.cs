using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrusteePortal.Models;
using TrusteePortal.NAVWS;

namespace TrusteePortal.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        Trustee webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        public ActionResult Index()
        {
            if (Session["trusteeNo"] == null) return RedirectToAction("index", "login");
            string username = Session["trusteeNo"].ToString();
            GetPensionerData(username);
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("index", "login");
        }

        private void GetPensionerData(string username)
        {
            try
            {
                string response = webportals.GetTrusteeDetails(username);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    Session["Name"] = responseArr[1];
                    Session["Email"] = responseArr[2];
                    Session["Category"] = responseArr[3];
                    Session["Designation"] = responseArr[4];
                    Session["Contact"] = responseArr[5];
                    Session["phoneNo"] = responseArr[6];
                    //Session["bankName"] = responseArr[7];
                    //Session["BranchName"] = responseArr[8];

                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
       

    }
}