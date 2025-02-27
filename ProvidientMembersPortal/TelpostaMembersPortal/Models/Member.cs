using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelpostaMembersPortal.Models
{
    public class Member
    {
        public string PFno { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }

        public string DOB { get; set; }
        public string ID { get; set; }
        public string MemberType { get; set; }

        public string Status { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Designation { get; set; }

    }
}