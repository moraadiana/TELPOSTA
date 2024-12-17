using MembersPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.viewmodel
{
    public class PensionUpdateVM
    {
        public PensionBankDetails BankDetails { get; set; }
        public PensionBankDetails PensionInput { get; set; }
        public List<SelectListItem> BanksList { get; set; }
        public List<SelectListItem> BankBranchList { get; set; }
    }
}