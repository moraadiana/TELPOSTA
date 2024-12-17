using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class NOKRequest
    {
        public string LineNo { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Status { get; set; }
    }
}