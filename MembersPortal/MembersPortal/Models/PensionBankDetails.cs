using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class PensionBankDetails
    {
        public string BankAccName { get; set; }
        public string BankAccNo { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string Status { get; set; }
    }
}