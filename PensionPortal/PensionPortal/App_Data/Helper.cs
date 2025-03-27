using PensionPortal.Models;
using PensionPortal.NAVWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PensionPortal
{
    public class Helper
    {
        private static Pension webportals = Components.ObjNav;
        private static string[] strLimiters = new string[] { "::" };
        private static string[] strLimiters2 = new string[] { "[]" };

        public static List<string> GetLifeCertPeriods()
        {
            var list = new List<string>();
            try
            {
                string result = webportals.GetLifeCertDates();
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
        public static List<PensionerStatement> GetPayrollPeriods1()
        {

            var list = new List<PensionerStatement>();
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
                        list.Add(new PensionerStatement()
                            {
                             StartDate = responseArr[0],
                             EndDate = responseArr[0]


                            }
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

        

    }
}