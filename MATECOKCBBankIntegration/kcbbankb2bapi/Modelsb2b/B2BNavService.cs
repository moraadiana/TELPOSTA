using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using b2b.NAVWS;
using System.Net;
using System.Configuration;


namespace b2b
{
    using b2b;
    public class B2BNavService
    {
        //private b2b.NAVWS.CloudPESALIVE1 coopNavService = new b2b.NAVWS.CloudPESALIVE1();
        private b2b.NAVWS.KCBBankIntergration KCBNavService = new b2b.NAVWS.KCBBankIntergration();
        //private b2b.B2BNavService;
        //consumers
        //private b2b.;
       // Consumers con=new
        private NetworkCredential cd;

        public B2BNavService()
        {
            string navurl = ConfigurationManager.AppSettings["nvurl"];

            string navdomain = ConfigurationManager.AppSettings["nvdomain"];
            string navuser = ConfigurationManager.AppSettings["nvuser"];
            string navpass = ConfigurationManager.AppSettings["nvpass"];
            

            this.cd = new NetworkCredential(navuser, navpass, navdomain);

            this.KCBNavService.Url = navurl;
            this.KCBNavService.PreAuthenticate = true;
            this.KCBNavService.Credentials = (ICredentials)this.cd;
        }

        public string GetAccountDetails (string accNo)
        {
            string result = "";

            try
            {
                return KCBNavService.GetAccountDetails(accNo);
            }

            catch (Exception ex)
            {
                UTILITIES.Logexception(ex);
                ex.Data.Clear();
                return result;
            }
        }
       
        public bool TransactionCodeExists(string transcode)
        {
            bool result = false;
            try
            {
                return KCBNavService.TransactionCodeExists(transcode);
            }
            catch (Exception ex)
            {
                UTILITIES.Logexception(ex);
                ex.Data.Clear();
                return result;
            }
        }
        
        public bool PostKCBTransaction(

              string TransactionReferenceCode
            , string TransactionDate
            , decimal TotalAmount
            , string AccountNumber
            , string AccountName
            , string channelCode
            , string msisdn
            ,string shortCode
            ,decimal accbal
            , string narration
            )
        {
            bool rst;

            rst = KCBNavService.PostKCBTransaction(
                  TransactionReferenceCode
                , TransactionDate
                , TotalAmount
                , AccountNumber
                , AccountName
                , channelCode
                , msisdn
                , shortCode
                , accbal
                , narration);

            return rst;
        }
    }
}