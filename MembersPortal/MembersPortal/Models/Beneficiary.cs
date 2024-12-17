using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class Beneficiary
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
        public string Percentage { get; set; }
        public string LineNo { get; set; }
        public string BirthCertNo { get; set; }
        public string Email { get; set; }
        public string IDPassport { get; set; }
        public string BankAccName { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        [DataType(DataType.Text)]
        public string AccNo { get; set; }
        public string PercentageBenefit { get; set; }
        public string PercentagePension { get; set; }


        public List<SelectListItem> Relationships { get; set; }
        public List<SelectListItem> BanksList { get; set; }
        public List<SelectListItem> BankBranchList { get; set; }
    }
}