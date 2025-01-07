using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using MembersPortal.Models;

namespace MembersPortal.Controllers
{
    public class AuthenticationController : Controller
    {
/*
        public ActionResult Login()
        {
            var model = new LoginModel();

            return View("login", model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill the form correctly";
                return View(model);
            }

            try
            {
                var username = model.Username;
                var password = model.Password;
                var validMember = await Task.Run(() => Components.Portal.ValidMember(username));

                if (validMember == "SUCCESS")
                {
                    var login = Components.Portal.LoginMember(username, password);

                    if (login != "SUCCESS")
                    {
                        Session["username"] = username;
                        return RedirectToAction("dashboard", "dashboard", username);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "An Error occurred While Logging you In";
                        return View(model);
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "You are not a Valid Member";
                    return View(model);
                }

            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "There was a problem Creating Your account Try again Later!";
                ex.Data.Clear();
            }

            return View("login", model);
        }

        public ActionResult CreateAccount()
        {
            var model = new CreateAccount();

            return View("login", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount(CreateAccount model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill the form correctly";
                model = await FillAccount();
                ViewBag.ErrorMessage = "Please Fill the form correctly";
                return View(model);
            }
            try
            {
                model = await FillAccount();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was a problem Creating Your account Try again Later!";
                ex.Data.Clear();
            }

            return View("login", model);
        }

        private async Task<CreateAccount> FillAccount()
        {
            var accountList = new CreateAccount
            {
                BanksList = await GetBanks(),
                CountriesList = await GetCountries(),
                SponsorsList = await GetSponsors()
            };


            return accountList;
        }
        private async Task<List<SelectListItem>> GetBanks()
        {
            var bankList = new List<SelectListItem>();
            try
            {
                var response = await Task.Run(() => Components.Portal.GetBanks());

                if (!String.IsNullOrEmpty(response))
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(new string[] { "::" }, StringSplitOptions.None);

                        var items = new SelectListItem
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };

                        bankList.Add(items);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }



            return bankList;
        }

        private async Task<List<SelectListItem>> GetBankBranches(string bankCode)
        {
            var branchList = new List<SelectListItem>();
            try
            {
                var response = await Task.Run(() => Components.Portal.GetBankBranches(bankCode));

                if (!String.IsNullOrEmpty(response))
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(new string[] { "::" }, StringSplitOptions.None);

                        var items = new SelectListItem
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };

                        branchList.Add(items);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }



            return branchList;
        }


        private async Task<List<SelectListItem>> GetSponsors()
        {
            var sponsorList = new List<SelectListItem>();
            try
            {
                var response = await Task.Run(() => Components.Portal.GetSponsors());

                if (!String.IsNullOrEmpty(response))
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(new string[] { "::" }, StringSplitOptions.None);

                        var items = new SelectListItem
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };

                        sponsorList.Add(items);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }



            return sponsorList;
        }
        private async Task<List<SelectListItem>> GetCountries()
        {
            var countryList = new List<SelectListItem>();
            try
            {
                var response = await Task.Run(() => Components.Portal.GetCountries());

                if (!String.IsNullOrEmpty(response))
                {
                    var firstArr = response.Split(new string[] { "[]" }, StringSplitOptions.None);

                    foreach (var item in firstArr)
                    {
                        var secondArr = item.Split(new string[] { "::" }, StringSplitOptions.None);

                        var items = new SelectListItem
                        {
                            Text = secondArr[1],
                            Value = secondArr[0]
                        };

                        countryList.Add(items);

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }



            return countryList;
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }*/
    }
}