using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PensionPortal.Models
{
    public class LifeCertificate
    {
        [Required]
        public string Period { get; set; }

        public string PdfUrl { get; set; }
        public List<LifeCertificate>LifeCertPeriods{ get; set; }
    }
}