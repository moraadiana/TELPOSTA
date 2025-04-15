using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrusteePortal.Models
{
    public class DashboardStatusViewModel
    {
        public List<StatusCount> MemberStatusCounts { get; set; }
        public List<StatusCount> PensionerStatusCounts { get; set; }
    }
}