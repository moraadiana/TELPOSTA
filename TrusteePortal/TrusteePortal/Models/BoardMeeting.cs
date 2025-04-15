using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrusteePortal.Models
{
    public class BoardMeeting
    {
        //public string Code {  get; set; }
        //Message += 'SUCCESS' + '::' + BoardRegister."Board Meeting"  + '::' + format(BoardRegister."Posting Date") + '::' + format(BoardRegister."Meeting Type" )+ '::'+ BoardRegister.Description ;
        public string MeetingNo { get; set; }

        public string Date { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }


    }
}