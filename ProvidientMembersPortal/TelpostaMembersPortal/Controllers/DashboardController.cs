using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelpostaMembersPortal.Models;
using TelpostaMembersPortal.NAVWS;

namespace TelpostaMembersPortal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string[] strLimiters2 = new string[] { "[]" };
        private readonly string[] strLimiters = new string[] { "::" };
        // GET: Dashboard

        Portal webportals = Components.Portal;
        //string[] strLimiters = new string[] { "::" };
        public ActionResult Index()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");
            string username = Session["memberNo"].ToString();
            GetMemberData(username);
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("index", "login");
        }

        private void GetMemberData(string username)
        {
            try
            {
                string response = webportals.GetMemberProfileDetails(username);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    Session["Email"] = responseArr[0];
                    Session["PhoneNo"] = responseArr[1];
                    Session["Designation"] = responseArr[2];
                    Session["DOB"] = responseArr[3];
                    Session["EmploymentDate"] = responseArr[4];
                    Session["JoiningDate"] = responseArr[5];
                    Session["ID"] = responseArr[6];
                    Session["Gender"] = responseArr[7];
                    Session["Sponsor"] = responseArr[8];
                    Session["Salutation"] = responseArr[9];
                    Session["Status"] = responseArr[10];
                    Session["Balance"] = responseArr[12];
                    //Session["MemberName"]

                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        public ActionResult BeneficiaryList()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");

            var beneficiaries = new List<Beneficiaries>();
            try
            {
                string username = Session["memberNo"].ToString();
                string beneficiaryList = webportals.GetMemberBeneficiaries(username);
                if (!string.IsNullOrEmpty(beneficiaryList))
                {
                    string[] beneficiaryArr = beneficiaryList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string beneficiary in beneficiaryArr)
                    {
                        string[] response = beneficiary.Split(strLimiters, StringSplitOptions.None);
                        Beneficiaries memberbeneficiary = new Beneficiaries()
                        {
                            Name = response[0].Trim(),
                            DOB = response[1].Trim(),
                            Rlshp = response[2].Trim(),
                            PhoneNo = response[3].Trim(),
                            Email = response[4].Trim(),

                        };
                        beneficiaries.Add(memberbeneficiary);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            return View(beneficiaries);
        }
        public ActionResult News()
        {
            if (Session["memberNo"] == null) return RedirectToAction("index", "login");

            var news = new List<News>();
            try
            {
                string username = Session["memberNo"].ToString();
                string newsList = webportals.GetUpdates();
                if (!string.IsNullOrEmpty(newsList))
                {
                    string[] newsListArr = newsList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string newsupdate in newsListArr)
                    {
                        string[] response = newsupdate.Split(strLimiters, StringSplitOptions.None);
                        News News = new News()
                        {
                            Name = response[0].Trim(),
                            Description = response[1].Trim(),
                          

                        };
                        news.Add(News);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            return View(news);
        }
        public ActionResult ChangeProfilePic(Member member)
        {
            if (Session["memberNo"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (member.profilePic != null && member.profilePic.ContentLength > 0)
            {
                string path = Server.MapPath("~/Profiles/");
                string memberNo = Session["memberNo"].ToString();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = Session["memberNo"].ToString() + ".png";
                string filepath = path + filename;
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                member.profilePic.SaveAs(filepath);
               webportals.UploadProfilePicture(memberNo, filepath, "Profile pic");
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }

    

}

