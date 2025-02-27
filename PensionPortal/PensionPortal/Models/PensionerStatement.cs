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
        //public string memberNo { get; set; }

        //[Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}