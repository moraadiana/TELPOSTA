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


        public static List<MemberStatement> GetPayrollPeriods()
        {
            var list = new List<MemberStatement>();
            try
            {
                string result = webportals.GetPayrollPeriods1();
                if (!string.IsNullOrEmpty(result))
                {
                    string[] resultsArr = result.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var period in resultsArr)
                    {
                        // Split further by '::' to separate StartDate and EndDate
                        var periodDetails = period.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (periodDetails.Length == 2)
                        {
                            var memberStatement = new MemberStatement
                            {
                                StartDate = periodDetails[0], // First part is StartDate
                                EndDate = periodDetails[1] // Second part is EndDate
                            };

                            list.Add(memberStatement); // Add the new MemberStatement to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return list;
        }

        public static List<string> GetPayrollPeriods1()
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

     

    }
}