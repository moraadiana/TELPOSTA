using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelpostaMembersPortal.Models
{
    public class MemberStatement
    {
        [Required]

        public string StartDate { get; set; }

        public string EndDate { get; set; }
        //public string PayrollPeriod { get; set; }

        public string PdfUrl { get; set; }
        public List<Config> PayrollPeriods { get; set; }
       // public List<MemberStatement> PayrollPeriods { get; set; }

    }
}