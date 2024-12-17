using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Models
{
    public class CreateAccount
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Second Name")]
        public string SecondName { get; set; }
        [Required]
        [Display(Name = "Other Name")]
        public string OtherName { get; set; }
        [Required]
        [Display(Name = "ID Number")]
        public string ID { get; set; }
        [Display(Name = "KRA PIN")]
        public string KRA { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string SelectedGender { get; set; }
        [Required]
        [Display(Name = "Select Country")]
        public string SelectedCountry { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public string DOB { get; set; }
        [Required]
        [Display(Name = "Date Of Employment")]
        public string DOE { get; set; }
        [Required]
        [Display(Name = "Join Scheme Date")]
        public string DOJ { get; set; }
        [Required]
        [Display(Name = "Pensionable Date")]
        public string PensionableDate { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "Postal Address")]
        public string PostalAddress { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Bank Code")]
        public string SelectedBankCode { get; set; }
        [Required]
        [Display(Name = "Bank Branch Code")]
        public string SelectedBranchCode { get; set; }
        [Required]
        [Display(Name = "Bank Code")]
        public string BankAccount { get; set; }
        [Required]
        [Display(Name = "Bank Code")]
        public string NSSFNo { get; set; }
        [Required]
        [Display(Name = "Sponsor")]
        public string Sponsor { get; set; }

        public List<SelectListItem> BanksList { get; set; }
        public List<SelectListItem> BankBrachesList { get; set; }
        public List<SelectListItem> CountriesList { get; set; }
        public List<SelectListItem> SponsorsList { get; set; }

    }
}