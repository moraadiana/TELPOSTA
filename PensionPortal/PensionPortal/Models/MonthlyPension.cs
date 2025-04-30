using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PensionPortal.Models
{
    public class MonthlyPension
    {
        public string payPeriod {  get; set; }

        public string Amount { get; set; }
        public string Description { get; set; }
        //public int Year { get; set; }
        //public int Month { get; set; }

        public DateTime SortKey { get; set; }
    }
}