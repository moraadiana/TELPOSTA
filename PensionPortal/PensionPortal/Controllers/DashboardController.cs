﻿using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace PensionPortal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string[] strLimiters2 = new string[] { "[]" };
        private readonly string[] strLimiters = new string[] { "::" };
        // GET: Dashboard

        Pension webportals = Components.ObjNav;
        //string[] strLimiters = new string[] { "::" };
        public ActionResult Index()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");
            string username = Session["pensionerNo"].ToString();
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
                string response = webportals.GetPensionerProfileDetails(username);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    Session["Email"] = responseArr[0];
                    Session["PhoneNo"] = responseArr[1];
                  //  Session["Designation"] = responseArr[2];
                    Session["DOB"] = responseArr[2];
                    Session["EmploymentDate"] = responseArr[3];
                    Session["JoiningDate"] = responseArr[4];
                    Session["ID"] = responseArr[5];
                    Session["Gender"] = responseArr[6];
                    Session["Sponsor"] = responseArr[7];
                    Session["Salutation"] = responseArr[8];
                    Session["Status"] = responseArr[9];
                    Session["AccountNo"] = responseArr[10];
                    Session["Name"] = responseArr[11];
                   
                   Session["SuspensionReason"] = responseArr[12];
                    Session["SuspensionDate"] = responseArr[13];
                    Session["Employer"] = responseArr[14]; 
                    Session["RetirementDate"] = responseArr[15];
                    Session["PensionerType"] = responseArr[16];
                    Session["CeaseDate"] = responseArr[17];
                    Session["BankName"] = responseArr[18];
                    Session["BranchName"] = responseArr[19];
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
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var beneficiaries = new List<Beneficiaries>();
            try
            {
                string username = Session["pensionerNo"].ToString();
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
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var news = new List<News>();
            try
            {
                string username = Session["pensionerNo"].ToString();
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
        public ActionResult PaySchedule()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var schedule = new List<PaySchedule>();
            try
            {
                string username = Session["pensionerNo"].ToString();
                string newsList = webportals.GetPaySchedule();
                if (!string.IsNullOrEmpty(newsList))
                {
                    string[] newsListArr = newsList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string newsupdate in newsListArr)
                    {
                        string[] response = newsupdate.Split(strLimiters, StringSplitOptions.None);
                        PaySchedule PaySchedule = new PaySchedule()
                        {
                            payDays = response[0].Trim(),
                            Months = response[1].Trim(),


                        };
                        schedule.Add(PaySchedule);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            return View(schedule);
        }

        public ActionResult MonthlyPension()
        {
            if (Session["pensionerNo"] == null) return RedirectToAction("index", "login");

            var schedule = new List<MonthlyPension>();
            try
            {
                string username = Session["pensionerNo"].ToString();
                string pensionList = webportals.GetMonthlyPension(username);
                if (!string.IsNullOrEmpty(pensionList))
                {
                    string[] pensionListArr = pensionList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    //foreach (string pension in pensionListArr)
                    //{
                    //    string[] response = pension.Split(strLimiters, StringSplitOptions.None);
                    //    MonthlyPension MonthlyPension = new MonthlyPension()
                    //    {
                    //        payPeriod = response[0].Trim(),
                    //        Amount = response[1].Trim(),


                    //    };
                    //    schedule.Add(MonthlyPension);
                    //}

                    foreach (string pension in pensionListArr)
                    {
                        string[] response = pension.Split(new string[] { "::" }, StringSplitOptions.None);
                        if (response.Length == 2)
                        {
                            DateTime parsedDate;
                            string formattedDate = response[0].Trim();

                            // Parse the date
                            if (DateTime.TryParseExact(formattedDate, "MM/dd/yy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                schedule.Add(new MonthlyPension()
                                {
                                    payPeriod = $"{parsedDate:MMMM yyyy}",
                                    Amount = response[1].Trim(),
                                    SortKey = parsedDate // Sorting key
                                });
                            }
                        }
                    }
                }
            
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            // return View(schedule);
            // return View(schedule.OrderByDescending(x => x.payPeriod).ToList());
            var sortedSchedule = schedule.OrderByDescending(x => x.SortKey).ToList();
            return View(sortedSchedule);

        }
    }
}