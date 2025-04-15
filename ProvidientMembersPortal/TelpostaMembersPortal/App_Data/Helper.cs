using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TelpostaMembersPortal.Models;
using TelpostaMembersPortal.NAVWS;

namespace TelpostaMembersPortal
{
    public class Helper
    {
        private static Portal webportals = Components.Portal;
        private static string[] strLimiters = new string[] { "::" };
        private static string[] strLimiters2 = new string[] { "[]" };

        public static List<MemberStatement> GetPayrollPeriods1()
        {

            var list = new List<MemberStatement>();
            // var periods = webportals.GetPayrollPeriods();
            try
            {
                string result = webportals.GetPayrollPeriods();
                if (!string.IsNullOrEmpty(result))
                {
                    string[] resultsArr = result.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in resultsArr)
                    {
                        string[] responseArr = str.Split(strLimiters, StringSplitOptions.None);
                        //list.Add("-- Select Period--");
                        list.Add(new MemberStatement()
                        {
                            StartDate = responseArr[0],
                            EndDate = responseArr[0]


                        }
                        //string result = webportals.GetLifeCertDates();
                        //if (!string.IsNullOrEmpty(result))
                        //{
                        //    string[] resultsArr = result.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);
                        //    list.Add("-- Select Period--");
                        //    list.AddRange(resultsArr); // Add all periods correctly
                        //}
                       );

                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return list;


        }

        public static List<string> GetPayrollPeriods()
        {
            var list = new List<string>();
            try
            {
                string result = webportals.GetPayrollPeriods();
                if (!string.IsNullOrEmpty(result))
                {
                    string[] resultsArr = result.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);
                    list.Add("-- Select Period--");
                    list.AddRange(resultsArr); // Add all periods correctly
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return list;
        }

        public string GeneratePensionerStatement(string pensionerNo, string startDate, string endDate)
        {
            try
            {
                // Generate a unique file name
                string fileName = pensionerNo.Replace("/", "");
                string pdffileName = $"PensionerStatement-{fileName}.pdf";
                string path = "D:\\Portals\\TELPOSTA\\PensionPortal\\PensionPortal\\Downloads\\";
                string filePath = Path.Combine(path, fileName);
                //PensionerStatement PensionerStatement = new PensionerStatement();

                //startDate = PensionerStatement.StartDate;
                //endDate = PensionerStatement.EndDate;
                // Ensure directory exists
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Call the AL procedure via NAV web service
                // webportals.PensionerStatement(path, fileName, pensionerNo, startDate,endDate);

                // Return the file path (ensure it's accessible via web)
                return "/GeneratedStatements/" + fileName;
            }
            catch
            {
                return null;
            }
        }

    }
}