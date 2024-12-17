using MembersPortal.Models;
using MembersPortal.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MembersPortal.Controllers
{
    public class DahboardController : Controller
    {
        private readonly string[] delimiter1 = new string[] { "[]" };
        private readonly string[] delimiter2 = new string[] { "::" };

        public async Task<ActionResult> Dashboard()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var username = Session["username"].ToString();
            var details = new DashboardVM();
            try
            {
                details = await GetDetails(username);
                Session["firstname"] = details.UserDetails.Firstname;
                Session["secondname"] = details.UserDetails.MiddleName;
                Session["lastname"] = details.UserDetails.LastName;



            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return View(details);
        }

        private async Task<DashboardVM> GetDetails(string username)
        {
            var response = new DashboardVM();
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetUserDetails(username));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new UserDetails
                        {
                            Name = secondArr[0] + " " + secondArr[1] + " " + secondArr[2],
                            Firstname = secondArr[0],
                            MiddleName = secondArr[1],
                            LastName = secondArr[2],
                            Gender = secondArr[3],
                            DOB = secondArr[4],
                            ID = secondArr[5],
                            Class = secondArr[6],
                            DOE = secondArr[7],
                            DOJ = secondArr[8],
                            DOR = secondArr[9],
                            ContributionStatus = secondArr[10],
                            Title = secondArr[11],
                            MemberType = secondArr[12],

                        };

                        response.UserDetails = add;

                    }
                    var contributions = new ContributionTypes
                    {
                        EEContribution = await Task.Run(() => Components.Portal.GetEmployeeContribution(username)),
                        EEUnreg = await Task.Run(() => Components.Portal.GetEEUnregisteredContribution(username)),
                        ERUnregistered = await Task.Run(() => Components.Portal.GetERUnregisteredContribution(username)),
                        EERegistered = await Task.Run(() => Components.Portal.GetEERegisteredContribution(username)),
                        ERRegistered = await Task.Run(() => Components.Portal.GetERRegisteredContribution(username)),
                        EETransferIn = await Task.Run(() => Components.Portal.EETransferIn(username)),
                        ERTransferIn = await Task.Run(() => Components.Portal.ERTransferIn(username)),
                        Arrears = await Task.Run(() => Components.Portal.Arrears(username)),
                        TotalContribution = await Task.Run(() => Components.Portal.TotalContribution(username)),
                        Balance = await Task.Run(() => Components.Portal.BalanceLCY(username))
                    };

                    response.Contribution = contributions;
                    response.Beneficiary = await GetBeneficiaries(username);
                    response.NoBeneficiaries = response.Beneficiary.Count().ToString();
                }



            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        public async Task<ActionResult> MemberStatement(string ID)
        {
            if (ID == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var username = ID.ToString();
            var model = new List<MemberStmt>();
            try
            {
                model = await GetMemberStatement(username);


            }
            catch (Exception ex)
            {
                ex.Data.Clear();

            }
            return View(model);
        }
        public ActionResult GetMemberBenefits()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new Dates
            {
                Display = false
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult GetMemberBenefits(Dates model)
        {

            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("GetMemberBenefits", model);
            }

            var username = Session["username"].ToString();
            string fileName = username.Replace(@"/", @"");
            string pdfFileName = $"MemberBenefit-{fileName}.pdf";
            string path = "C:\\inetpub\\wwwroot\\JKUAT PENSION\\Downloads\\";
            DateTime startDate = Convert.ToDateTime(model.StartDate);

            DateTime endDate = Convert.ToDateTime(model.EndDate);
            try
            {
                Components.Portal.BenefitStatement(path, pdfFileName, username, startDate, endDate);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            ViewBag.PdfUrl = Url.Content($"~/Downloads/{pdfFileName}");

            return View();
        }
        public ActionResult GetMemberStatament(string ID)
        {
            if (ID == null)
            {
                return RedirectToAction("login", "authentication");
            }

            string fileName = ID.Replace(@"/", @"");
            string pdfFileName = $"MemberStatement-{fileName}.pdf";
            string path = "C:\\inetpub\\wwwroot\\JKUAT PENSION\\Downloads\\";
            try
            {
                var result = Components.Portal.MemberStatement(path, pdfFileName, ID);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            ViewBag.PdfUrl = Url.Content($"~/Downloads/{pdfFileName}");

            return View();
        }

        public async Task<ActionResult> EmploymentDetails(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return RedirectToAction("login", "authentication");
            }
            var list = new List<EmploymentHist>();

            try
            {
                var response = await Task.Run(() => Components.Portal.GetEmploymentHistory(key));
                if (response != null)
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var group in firstArr)
                    {
                        var secondArr = group.Split(new string[] { "::" }, StringSplitOptions.None);

                        var item = new EmploymentHist
                        {
                            Sponsor = secondArr[0],
                            StartDate = secondArr[1],
                            EndDate = secondArr[2]
                        };

                        list.Add(item);


                    }

                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return PartialView("_EmploymentDetails", list);

        }

        public async Task<ActionResult> DetailedLedger()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new List<DetailedLedgerEntries>();
            var username = Session["username"].ToString();
            try
            {
                model = await GetLedgerEntries(username);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return View(model);
        }


        private async Task<List<MemberStmt>> GetMemberStatement(string key)
        {
            var list = new List<MemberStmt>();

            try
            {
                var response = await Task.Run(() => Components.Portal.GetMemberStatement(key));
                if (response != null)
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var group in firstArr)
                    {
                        var secondArr = group.Split(new string[] { "::" }, StringSplitOptions.None);
                        if (secondArr[0] == "SUCCESS")
                        {
                            var item = new MemberStmt
                            {
                                EmploContrib = secondArr[1],
                                EEUnregistered = secondArr[2],
                                EmployeeContrib2 = secondArr[3],
                                EmployerUnregContrib = secondArr[4],
                                EEVoluntary = secondArr[5],
                                EAVCUnregContrib = secondArr[6],
                                Arrears = secondArr[7],
                                TransferIn = secondArr[8],
                                IntrestEarned = secondArr[9],
                                TotalContribution = secondArr[10],
                                TotalWithdrawal = secondArr[11],
                                Balance = secondArr[12]
                            };

                            list.Add(item);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return list;

        }
        public async Task<ActionResult> UpdateBeneficiary()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new Beneficiary();
            try
            {
                model.Relationships = Relationships();
                model.BanksList = await GetBanksList();

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }


            return View("updateBeneficiary", model);

        }

        [HttpPost]
        public async Task<ActionResult> UpdateBeneficiary(Beneficiary model)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please Fill The Form Correctly";

                return View(model);
            }


            var memberNo = Session["username"].ToString();
            DateTime DOB = Convert.ToDateTime(model.DOB);
            var firstName = model.FirstName;
            var otherName = model.MiddleName;
            var lastName = model.LastName;
            var email = model.Email;
            var bankCode = model.BankCode;
            var branchCode = model.BranchCode;
            var rlshp = model.Rlshp;
            var birthCert = model.BirthCertNo;
            var idNo = model.IDPassport;
            var bankAccName = model.BankAccName;
            var accNo = model.AccNo;
            var percbenefit = model.PercentageBenefit;
            var percPension = model.PercentagePension;


            var list = new string[] { otherName, email, bankAccName, bankCode, birthCert, idNo, accNo, percbenefit, percPension };


            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == null)
                {
                    list[i] = "";
                }
            }

            otherName = list[0];
            email = list[1];
            bankAccName = list[2];
            bankCode = list[3];
            birthCert = list[4];
            idNo = list[5];
            accNo = list[6];
            percbenefit = list[7];
            percPension = list[8];
            if (percbenefit == "")
            {
                percbenefit = "0";
            }
            if (percPension == "")
            {
                percPension = "0";
            }

            int PercentageBenefit = Convert.ToInt32(percbenefit);
            int PercentagePension = Convert.ToInt32(percPension);

            try
            {
                var response = await Task.Run(() => Components.Portal.AddBeneficiary(memberNo, firstName, lastName, otherName, DOB, email, rlshp,
                    birthCert, idNo, bankAccName, bankCode, branchCode, accNo, PercentageBenefit, PercentagePension));
                if (response != null)
                {
                    var arr = response.Split(delimiter2, StringSplitOptions.None);
                    if (arr[0] == "SUCCESS")
                    {
                        ViewBag.SuccessMessage = $"{firstName} {lastName} has been Added successfully";
                        model.Relationships = Relationships();
                        model.BanksList = await GetBanksList();
                        return View(model);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return View(model);
        }

        public ActionResult EditBeneficiaries(string lineNo, string name, string DOB)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new Beneficiary
            {
                Name = name,
                DOB = DOB,
                LineNo = lineNo
            };

            return PartialView("_EditBeneficiary", model);
        }

        public string RemoveBeneficiary(int id)
        {
            var result = string.Empty;
            return result;
        }

        public async Task<ActionResult> NextOfKinRequest()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var memberNo = Session["username"].ToString();
            var model = new List<NOKRequest>();
            try
            {
                model = await GetNOKRequests(memberNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return View(model);
        }
        public async Task<ActionResult> PensionBankingRequest()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var memberNo = Session["username"].ToString();
            var model = new List<PensionBankDetails>();
            try
            {
                model = await GetPensionBankingRequests(memberNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return View(model);
        }

        public async Task<ActionResult> UpdatePensionBankingDetails()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new PensionUpdateVM();
            var username = Session["username"].ToString();
            try
            {
                model.BankDetails = await GetPensioBankDetails(username);
                model.BanksList = await GetBanksList();
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePensionBankingDetails(PensionUpdateVM model)
        {

            var username = Session["username"].ToString();
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please Fill the form correctly!!";
                model.BankDetails = await GetPensioBankDetails(username);
                model.BanksList = await GetBanksList();
                return View(model);
            }
            try
            {
                var response = Components.Portal.UpdatePensionBankingDetails(username, model.PensionInput.BankAccName, model.PensionInput.Bank, model.PensionInput.Branch, model.PensionInput.BankAccNo);
                if (response != null)
                {
                    if (response == "SUCCESS")
                    {
                        ViewBag.SuccessMessage = "Successfully Submitted Your details";
                        model.BankDetails = await GetPensioBankDetails(username);
                        model.BanksList = await GetBanksList();
                        return View(model);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return View("UpdatePensionBankingDetails");
        }

        public ActionResult UpdateInformation()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new UserDetails();
            try
            {
                model = new UserDetails
                {
                    FirstName = Session["firstname"].ToString(),
                    MiddleName = Session["secondname"].ToString(),
                    LastName = Session["lastname"].ToString()
                };
            }
            catch (Exception ex)
            {

                ex.Data.Clear();
            }


            return View(model);
        }

        public async Task<ActionResult> InitiateClearance(string ID)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            var model = new Clearance();
            var username = ID;
            try
            {

                var exitNo = string.Empty;
                var result = Components.Portal.GetExitNo();

                if (!String.IsNullOrEmpty(result))
                {
                    exitNo = result;
                }
                Session["exitNo"] = exitNo;

                model.Types = ClearanceType();
                model.BenefitCodes = await GetBenefitsCodes();

            }
            catch (Exception ex)
            {
                ex.Data.Clear();

            }
            return PartialView("_ClearanceInitiate", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitClearance(Clearance model)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("login", "authentication");
            }
            if (String.IsNullOrEmpty(model.ClearanceType) || String.IsNullOrEmpty(model.BenefitCode))
            {
                model.Types = ClearanceType();
                model.BenefitCodes = await GetBenefitsCodes();
                ViewBag.SuccessMessage = "Fill the form Correctly";
                return PartialView("_ClearanceInitiate", model);

            }
            if (ModelState.IsValid)
            {
                var selectedType = Convert.ToInt32(model.ClearanceType);
                var benefitcode = model.BenefitCode;
                var exitNo = Session["exitNo"].ToString();
                var memberNo = Session["username"].ToString();

                try
                {
                    Components.Portal.Clearance(exitNo, memberNo, benefitcode, selectedType);

                    ViewBag.SuccessMessage = "Clearance Initiated Successfully";
                    return RedirectToAction("Dashboard");

                }
                catch (Exception ex)
                {
                    ex.Data.Clear();
                }


                return RedirectToAction("Dashboard");
            }

            model.Types = ClearanceType();
            model.BenefitCodes = await GetBenefitsCodes();
            return PartialView("_ClearanceInitiate", model);
        }

        private async Task<List<Beneficiary>> GetBeneficiaries(string username)
        {
            var response = new List<Beneficiary>();
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetBeneficiaries(username));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);
                        if (secondArr[0] == "SUCCESS")
                        {
                            var add = new Beneficiary
                            {
                                Name = secondArr[1] + " " + secondArr[2] + " " + secondArr[3],
                                Rlshp = secondArr[4],
                                DOB = secondArr[5],
                                Percentage = secondArr[6],
                                LineNo = ""
                            };

                            response.Add(add);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }


        public List<SelectListItem> ClearanceType()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem() { Text = "--Select--", Value = "" },
                new SelectListItem() { Text = "Principal", Value = "0" },
                new SelectListItem() { Text = "Beneficiary", Value = "1" },
                new SelectListItem() { Text = "ThirdParty", Value = "2" }
            };


            return list;
        }

        private List<SelectListItem> Relationships()
        {
            var list = new List<SelectListItem>();
            var rlsps = new string[] { "--Select--", "Son", "Daughter", "Wife", "Husband", "Mother", "Father", "Niece", "Nephew" };
            var counter = 0;
            foreach (string s in rlsps)
            {
                list.Add(new SelectListItem()
                {
                    Text = s,
                    Value = counter.ToString()
                });
                counter++;
            }

            return list;
        }
        private async Task<List<SelectListItem>> GetBenefitsCodes()
        {
            var response = new List<SelectListItem>();
            response.Add(new SelectListItem()
            {
                Text = "Select",
                Value = ""
            });
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetExitCodes());
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new SelectListItem
                        {
                            Text = secondArr[0],
                            Value = secondArr[1]
                        };

                        response.Add(add);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        private async Task<List<NOKRequest>> GetNOKRequests(string memberNo)
        {
            var response = new List<NOKRequest>();
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetNOKRequest(memberNo));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new NOKRequest
                        {
                            LineNo = secondArr[0],
                            FirstName = secondArr[1],
                            Surname = secondArr[2],
                            Status = secondArr[3]
                        };


                        response.Add(add);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        private async Task<List<PensionBankDetails>> GetPensionBankingRequests(string memberNo)
        {
            var response = new List<PensionBankDetails>();
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetPensionBankingChange(memberNo));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new PensionBankDetails
                        {
                            BankAccName = secondArr[0],
                            Bank = secondArr[2],
                            BankAccNo = MaskAccountNumber(secondArr[1]),
                            Status = secondArr[3]
                        };


                        response.Add(add);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }

        public string MaskAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 4)
                return accountNumber;

            int visibleDigits = 2;
            int maskLength = accountNumber.Length - (2 * visibleDigits);

            if (maskLength <= 0)
                return accountNumber;

            string firstPart = accountNumber.Substring(0, visibleDigits);
            string lastPart = accountNumber.Substring(accountNumber.Length - visibleDigits);
            string maskedPortion = new string('*', maskLength);

            return firstPart + maskedPortion + lastPart;
        }


        private async Task<List<DetailedLedgerEntries>> GetLedgerEntries(string username)
        {
            var response = new List<DetailedLedgerEntries>();
            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetDetailedLedgerEntries(username));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new DetailedLedgerEntries
                        {
                            Amount = secondArr[0],
                            ContributionPeriod = secondArr[1],
                            PDate = secondArr[2],
                            Sponsor = secondArr[3]
                        };

                        response.Add(add);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        private async Task<List<EmploymentHist>> GetEmploymentHstory(string username)
        {
            var response = new List<EmploymentHist>();

            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetEmploymentHistory(username));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var add = new EmploymentHist
                        {
                            Sponsor = secondArr[0],
                            StartDate = secondArr[1],
                            EndDate = secondArr[2]
                        };

                        response.Add(add);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        private async Task<PensionBankDetails> GetPensioBankDetails(string username)
        {
            var response = new PensionBankDetails();

            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetPensionAccountDetails(username));
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        if (secondArr.Length > 3)
                        {
                            var add = new PensionBankDetails
                            {
                                BankAccName = secondArr[0],
                                BankAccNo = secondArr[1],
                                Branch = secondArr[2],
                                Bank = secondArr[3]
                            };

                            response = add;
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }


        private async Task<List<SelectListItem>> GetBanksList()
        {
            var response = new List<SelectListItem>();
            response.Add(new SelectListItem()
            {
                Text = "--Select Bank--",
                Value = ""

            });

            try
            {
                var detailsResponse = await Task.Run(() => Components.Portal.GetBanks());
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var bank = new SelectListItem()
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };
                        response.Add(bank);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return response;
        }
        public JsonResult GetBankBranches(string bankcode)
        {
            var response = new List<SelectListItem>();

            try
            {
                var detailsResponse = Components.Portal.GetBankBranches(bankcode);
                if (detailsResponse != null)
                {
                    var firstArr = detailsResponse.Split(delimiter1, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(delimiter2, StringSplitOptions.None);

                        var bank = new SelectListItem()
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };
                        response.Add(bank);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


    }
}