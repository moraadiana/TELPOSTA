using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrusteePortal.NAVWS;

namespace TrusteePortal
{
    public class Helper
    {

        private static Trustee webportals = Components.ObjNav;
        private static string[] strLimiters = new string[] { "::" };
        private static string[] strLimiters2 = new string[] { "[]" };

        public static List<string> GetP9Periods()
        {
            var list = new List<string>();
            //try
            //{
            //    string result = webportals.GetPninePeriods();
            //    if (!string.IsNullOrEmpty(result))
            //    {
            //        string[] resultsArr = result.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);
            //        list.Add("-- Select Period--");
            //        list.AddRange(resultsArr); // Add all periods correctly
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex.Data.Clear();
            //}
           
            //to update when periods are set on system

            try
            {
                list.Add("-- Select Period -- ");
                list.Add("2025");
                list.Add("2024");
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return list;
        }
    }
}