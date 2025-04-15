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
        public ActionResult StatusSummary()
        {
            var memberResult = webportals.GetMemberCountsByStatus();
            var pensionerResult = webportals.GetPensionerCountsByStatus();

            var memberStatusCounts = ParseStatusResult(memberResult);
            var pensionerStatusCounts = ParseStatusResult(pensionerResult);

            var dashboardViewModel = new DashboardStatusViewModel
            {
                MemberStatusCounts = memberStatusCounts,
                PensionerStatusCounts = pensionerStatusCounts
            };

            return View(dashboardViewModel);
        }

        private List<StatusCount> ParseStatusResult(string result)
        {
            var list = new List<StatusCount>();
            if (!string.IsNullOrEmpty(result))
            {
                var pairs = result.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2)
                    {
                        list.Add(new StatusCount
                        {
                            Status = parts[0],
                            Count = int.Parse(parts[1])
                        });
                    }
                }
            }
            return list;
        }

        public ActionResult StatusSummary1()
        {
            var memberResult = webportals.GetMemberCountsByStatus();
            var result = webportals.GetPensionerCountsByStatus(); // returns string like "Active=120;Deferred=45;..."
            var statusCounts = new List<StatusCount>();

            if (!string.IsNullOrEmpty(result))
            {
                var pairs = result.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2)
                    {
                        statusCounts.Add(new StatusCount
                        {
                            Status = parts[0],
                            Count = int.Parse(parts[1])
                        });
                    }
                }
            }

            return View(statusCounts);
        }



    }
}