using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelpostaMembersPortal.Models
{
    public class Beneficiaries
    {
        public string Name { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Rlshp { get; set; }
        public string DOB { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        


       
    }
}