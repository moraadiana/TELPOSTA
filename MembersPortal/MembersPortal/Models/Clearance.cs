using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class Clearance
    {
        public string ClearanceType { get; set; }
        public string BenefitCode { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> BenefitCodes { get; set; }
    }
}