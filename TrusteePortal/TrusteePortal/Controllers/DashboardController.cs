using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
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

        public ActionResult ChangeProfilePic(Account account)
        {
            if (Session["trusteeNo"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (account.profilePic != null && account.profilePic.ContentLength > 0)
            {
                string path = Server.MapPath("~/Profiles/");
                string trusteeNo = Session["trusteeNo"].ToString();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = Session["trusteeNo"].ToString() + ".png";
                string filepath = path + filename;
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                account.profilePic.SaveAs(filepath);
                // webportals.UploadProfilePicture(memberNo, filepath, "Profile pic");
            }
            return RedirectToAction("Index", "Dashboard");
        }

    }
}