using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class ContributionTypes
    {
        public decimal EEContribution { get; set; }
        public decimal EEUnreg { get; set; }
        public decimal ERUnregistered { get; set; }
        public decimal EERegistered { get; set; }
        public decimal ERRegistered { get; set; }
        public decimal EETransferIn { get; set; }
        public decimal ERTransferIn { get; set; }
        public decimal Arrears { get; set; }
        public decimal TotalContribution { get; set; }
        public decimal Balance { get; set; }

    }
}