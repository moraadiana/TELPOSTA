using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class UserDetails
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string ID { get; set; }
        public string Class { get; set; }
        public string DOE { get; set; }
        public string DOJ { get; set; }
        public string DOR { get; set; }
        public string ContributionStatus { get; set; }
        public string Title { get; set; }
        public string MemberType { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

    }
}