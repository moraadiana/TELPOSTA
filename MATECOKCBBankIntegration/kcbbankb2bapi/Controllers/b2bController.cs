using System;
using b2b.Respository;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace b2b.Controllers
{
    public class b2bController : ApiController
    {
        B2BNavService navService = new B2BNavService();

        /// <summary>
        /// account
        /// </summary>
        [HttpPost]
        [ResponseType(typeof(b2b.AccountValidation.RESPONSE.Response))]
        public IHttpActionResult testaccountvalidation(b2b.AccountValidation.REQUEST.Request request)
        {
            try
            {
                // **** serialize for log
                string strAccRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                UTILITIES.WriteLogOnFile(strAccRequest);
                // **** serialize for log
                // -- get the account details
                string strAccDetails = navService.GetAccountDetails(request.customerReference);
                        if (strAccDetails.Length.Equals(0))
                        {
                            // ** initialize response object
                            var accValResp = new b2b.AccountValidation.RESPONSE.Response
                            {
                                transactionID = request.requestId,
                                statusCode = 1,
                                statusMessage = "Tenant No: "+request.customerReference+" does not exist.",
                                CustomerName = "",
                                billAmount = 0,
                                currency = "",
                                billType = "",
                                creditAccountIdentifier = "",
                            };

                            // **** serialize for log
                            string strBalanceResponse = Newtonsoft.Json.JsonConvert.SerializeObject(accValResp);
                            UTILITIES.WriteLogOnFile(strBalanceResponse);
                            // **** serialize for log

                            return Ok(accValResp);
                        }
                        else
                        { 
                        string[] strAccDetailsArray = strAccDetails.Split(new string[] { ":::" }, StringSplitOptions.None);
                        b2b.Accounts.Account memberAccount = new b2b.Accounts.Account { AccountNumber = strAccDetailsArray[0], AccountName = strAccDetailsArray[1], Balance = strAccDetailsArray[2], BillType = strAccDetailsArray[3] };

                        // ** initialize response object
                        var accValResp = new b2b.AccountValidation.RESPONSE.Response
                        {
                            transactionID = request.requestId,
                            statusCode = 0,
                            statusMessage = "Success.",
                            CustomerName = memberAccount.AccountName,
                            billAmount = Convert.ToDecimal(memberAccount.Balance),
                            currency = "KES",
                            billType = memberAccount.BillType,
                            creditAccountIdentifier = "",
                        };
                        return Ok(accValResp);

                        // *** ***** *****
                    
                }
            }
            catch (Exception ex)
            {
                #region error handling
                // ** initialize response object
                var accValResp = new b2b.AccountValidation.RESPONSE.Response
                {
                    transactionID = request.requestId,
                    statusCode = 1,
                    statusMessage = "Fail: Error retrieving account details.",
                    CustomerName = "",
                    billAmount = 0,
                    currency = "",
                    billType = "",
                    creditAccountIdentifier = "",
                };
                // **** log for exception
                UTILITIES.Logexception(ex);
                // **** log for exception
                return Ok(accValResp);
                #endregion error handling
            }
        }

        /// <summary>
        /// advise
        /// </summary>
        [HttpPost]
        [ResponseType(typeof(b2b.PaymentAdvise.RESPONSE.PaymentAdviseResponse))]
        public IHttpActionResult testpaymentnotification(b2b.PaymentAdvise.REQUEST.Request request)
        {
            try
            {
                // **** serialize for log
                string strAccRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                UTILITIES.WriteLogOnFile(strAccRequest);
                // **** serialize for log
                // -- check if account exists
                string saccoAccountNo = navService.GetAccountDetails(request.customerReference);
                    if (saccoAccountNo.Length.Equals(0))
                    {
                        // --account not found
                        var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                        {
                            transactionID = request.transactionReference,
                            statusCode = "1",
                            statusMessage = "Tenant No: "+request.customerReference+" does not exist."
                        };
                        return Ok(paymentAdviseRespResponse);
                    }
                    else
                    {
                        // -- check if trans is dublicate 
                        if (navService.TransactionCodeExists(request.transactionReference))
                        {
                        // -- the transaction has already been processd
                        var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                        {
                            transactionID = request.transactionReference,
                            statusCode = "1",
                            statusMessage = "Duplicate transaction request."
                        };
                        return Ok(paymentAdviseRespResponse);
                        }
                        else
                        {

                            // -- queue request for posting
                            bool posted = navService.PostKCBTransaction(
                                             request.transactionReference
                                             , request.timestamp
                                             , Convert.ToDecimal(request.transactionAmount)
                                             , request.customerReference
                                             , request.customerName
                                             , request.channelCode
                                             , request.customerMobileNumber
                                             , request.organizationShortCode
                                             , Convert.ToDecimal(request.balance)
                                             , request.narration
                                             );

                            if (!posted)
                            {
                            // -- error occured                               
                            var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                            {
                                transactionID = request.transactionReference,
                                statusCode = "1",
                                statusMessage = "A severe error has occurred."
                            };
                            return Ok(paymentAdviseRespResponse);
                            }
                            else
                            {
                            // ** successful
                            var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                            {
                                transactionID = request.transactionReference,
                                statusCode = "0",
                                statusMessage = "Notification received successfully."
                            };
                            return Ok(paymentAdviseRespResponse);
                            }


                        }
                    }


            }
            catch (Exception ex)
            {

                #region error handler
                // -- error occured                               
                var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                {
                    transactionID = request.transactionReference,
                    statusCode = "1",
                    statusMessage = "A severe error has occurred."
                };
                // **** log for exception
                UTILITIES.Logexception(ex);
                // **** log for exception
                return Ok(paymentAdviseRespResponse);
                #endregion error handler

            }
        }
        [HttpPost]
        [ResponseType(typeof(b2b.PaymentAdvise.RESPONSE.PaymentAdviseResponse))]
        public IHttpActionResult disbursementnotification(b2b.PaymentAdvise.REQUEST.Disbursement request)
        {
            try
            {
                // **** serialize for log
                string strAccRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                UTILITIES.WriteLogOnFile(strAccRequest);
                // **** serialize for log
                // -- check if account exists
                
                        bool posted = navService.DisbursementNotification(
                                         request.ftReference
                                         , request.transactionDate
                                         , Convert.ToDecimal(request.amount)
                                         , request.transactionStatus
                                         , request.transactionMessage
                                         , request.beneficiaryAccountNumber
                                         , request.debitAccountNumber
                                         , request.beneficiaryName
                                         , request.transactionReference
                                         , request.merchantId
                                         );

                        if (!posted)
                        {
                            // -- error occured                               
                            var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                            {
                                transactionID = request.transactionReference,
                                statusCode = "1",
                                statusMessage = "A severe error has occurred."
                            };
                            return Ok(paymentAdviseRespResponse);
                        }
                        else
                        {
                            // ** successful
                            var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                            {
                                transactionID = request.transactionReference,
                                statusCode = "0",
                                statusMessage = "Notification received successfully."
                            };
                            return Ok(paymentAdviseRespResponse);
                        }


                    
                


            }
            catch (Exception ex)
            {

                #region error handler
                // -- error occured                               
                var paymentAdviseRespResponse = new b2b.PaymentAdvise.RESPONSE.Response
                {
                    transactionID = request.transactionReference,
                    statusCode = "1",
                    statusMessage = "A severe error has occurred."
                };
                // **** log for exception
                UTILITIES.Logexception(ex);
                // **** log for exception
                return Ok(paymentAdviseRespResponse);
                #endregion error handler

            }
        }
    }
}
