using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PensionPortal.Models
{
    public class LifeCertReport
    {
        public string Period { get; set; }
        public string Status { get; set; }

        public DateTime SortKey { get; set; }
    }
}