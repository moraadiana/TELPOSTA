using MembersPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MembersPortal.viewmodel
{
    public class DashboardVM
    {
        public UserDetails UserDetails { get; set; }
        public List<Beneficiary> Beneficiary { get; set; } = new List<Beneficiary>();
        public ContributionTypes Contribution { get; set; }
        public string NoBeneficiaries { get; set; }
    }
}