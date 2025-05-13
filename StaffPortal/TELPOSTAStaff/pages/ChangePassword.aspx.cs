using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        Staffportall webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
        }
        protected void BtnResetPass_Click(object sender, EventArgs e)
        {
            string OldPass = txtOldPass.Text.ToString();
            string NewPass = TxtNewPass.Text.ToString();
            string ConfirmNewPass = TxtConfirmNewPass.Text.ToString();
            if (string.IsNullOrEmpty(OldPass))
            {
                Message("Please enter your old password");
                txtOldPass.Focus();
                return;
            }

            if (string.IsNullOrEmpty(NewPass))
            {
                Message("New Password cannot be null");
                TxtNewPass.Focus();
                return;
            }
            if (string.IsNullOrEmpty(ConfirmNewPass))
            {
                Message("Confirm Password cannot be null");
                TxtConfirmNewPass.Focus();
                return;
            }
            if (VerifyOldPass(OldPass) == false)
            {
                Message("Warning! Enter the correct old password!");
                txtOldPass.Focus();
                return;
            }
            if (NewPass != ConfirmNewPass)
            {
                Message("Your new passwords do not match!");
                return;
            }
            // Sp Reset password
            try
            {
                #region commented - using webservice
                if (ValidatePassword(NewPass) == false)
                {
                    Message("Password must be at least 6 characters, no more than 20 characters, and must include at least one upper case letter, one lower case letter, and one numeric digit.");
                }
                else
                {
                    string memberLoginInfo = webportals.UpdateStaffPassword(Session["username"].ToString(), NewPass);

                    if (!String.IsNullOrEmpty(memberLoginInfo))
                    {
                        string returnMsg = "";
                        string[] strdelimiters = new string[] { "::" };
                        string[] memberLoginInfo_arr = memberLoginInfo.Split(strdelimiters, StringSplitOptions.None);

                        returnMsg = memberLoginInfo_arr[0];
                        if (returnMsg == "SUCCESS")
                        {
                            Session.Abandon();
                            Session.Clear();
                            Session.Remove("username");
                            Session.RemoveAll();
                            SuccessMessage("Password successfully changed!");
                        }
                        else
                        {
                            Message("ERROR: Failed to change password!");
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private bool VerifyOldPass(string oldPass)
        {
            bool b = false;
            try
            {

                #region commented - using webservice
                string memberLoginInfo = webportals.VerifyCurrentPassword(Session["username"].ToString(), oldPass);

                if (!String.IsNullOrEmpty(memberLoginInfo))
                {
                    string returnMsg = "";
                    string[] strdelimiters = new string[] { "::" };
                    string[] memberLoginInfo_arr = memberLoginInfo.Split(strdelimiters, StringSplitOptions.None);

                    returnMsg = memberLoginInfo_arr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        b = true;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return b;
        }
        public bool ValidatePassword(string password)
        {
            bool r = false;
            string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$";
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    if (Regex.IsMatch(password, patternPassword))
                    {
                        r = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message.ToString());
            }
            return r;
        }
        private void Message(string message)
        {
            string strScript = $"<script>alert('{message}');</script>";
            Page.RegisterStartupScript("ClientScript", strScript);
        }
        [Obsolete]
        public void Message1(string strMsg)
        {
            string strScript = null;
            strScript = "<script>";
            strScript = strScript + "alert('" + strMsg + "');";
            strScript = strScript + "</script>";
            Page.RegisterStartupScript("ClientScript", strScript.ToString());
        }
        private void SuccessMessage(string message)
        {
            string page = "Dashboard.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "'</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }
        public void SuccessMessage1(string strMsg)
        {
            string strScript = null;
            string myPage = "Dashboard.aspx";
            strScript = "<script>";
            strScript = strScript + "alert('" + strMsg + "');";
            strScript = strScript + "window.location='" + myPage + "'";
            strScript = strScript + "</script>";
            Page.RegisterStartupScript("ClientScript", strScript.ToString());
        }
    }
}