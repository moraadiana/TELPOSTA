using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class DetailedLedgerEntries
    {
        public string Amount { get; set; }
        public string ContributionPeriod { get; set; }
        public string PDate { get; set; }
        public string Sponsor { get; set; }
    }
}