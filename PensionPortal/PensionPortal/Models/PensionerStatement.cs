using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PensionPortal.Models
{
    public class PensionerStatement
    {
        [Required]
        public string StartDate { get; set; }


        [Required]
        public string EndDate { get; set; }
      
       // public string PayrollPeriod { get; set; }

        public string PdfUrl { get; set; }
        public List<PensionerStatement> PayrollPeriods { get; set; }
    }
}