﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelpostaMembersPortal.Models
{
    public class MemberStatement
    {
        [Required]
        //public string memberNo { get; set; }

        //[Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}