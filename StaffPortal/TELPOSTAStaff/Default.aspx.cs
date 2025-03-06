using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff
{
    public partial class Default : System.Web.UI.Page
    {
          readonly Staffportall webportals = Components.ObjNav;
          string[] strLimiters = new string[] { "::" };


          protected void Page_Load(object sender, EventArgs e)
          {
              txtusername.Focus();
          }

          private bool ValidStaffNo(string username)
          {
              bool valid = false;
              try
              {
                  string response = webportals.CheckValidStaffNo(username);
                  if (!string.IsNullOrEmpty(response))
                  {
                      if (response == "Valid")
                      {
                          valid = true;
                      }
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
              return valid;
          }

          private bool ChangedPassword(string username)
          {
              bool changed = false;
              try
              {
                  string response = webportals.CheckStaffPasswordChanged(username);
                  if (!string.IsNullOrEmpty(response))
                  {
                      if (response == "Yes")
                      {
                          changed = true;
                      }
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
              return changed;
          }

          private void LoginForChangedPassword(string username, string password)
          {
              try
              {
                  string response = webportals.StaffLogin(username, password);
                  if (!string.IsNullOrEmpty(response))
                  {
                      string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                      string returnMsg = responseArr[0];
                      if (returnMsg == "SUCCESS")
                      {
                          string staffNo = responseArr[1];
                          string staffName = responseArr[2];
                          Session["username"] = staffNo;
                          Session["staffName"] = staffName;
                        //Response.Redirect("Dashboard.aspx");
                        Response.Redirect($"pages/Dashboard.aspx?");
                      }
                      else
                      {
                          lblError.Text = returnMsg;
                          return;
                      }
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
          }

          private void LoginForUnchangedPassword(string username, string password)
          {
              try
              {
                  string response = webportals.LoginForUnchnagedPassword(username);
                  if (!string.IsNullOrEmpty(response))
                  {
                      string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                      string returnMsg = responseArr[0];
                      if (returnMsg == "SUCCESS")
                      {
                          string staffNo = responseArr[1];
                          string staffEmail = responseArr[2];
                          Response.Redirect($"ResetPassword.aspx?staffNo={staffNo}&email={staffEmail}");
                      }
                      else
                      {
                          lblError.Text = returnMsg;
                          return;
                      }
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
          }

          private string GetStaffEmail(string username)
          {
              string staffEmail = string.Empty;
              try
              {
                  string response = webportals.GetStaffEmail(username);
                  if (!string.IsNullOrEmpty(response))
                  {
                      string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                      if (responseArr[0] == null) staffEmail = responseArr[1];
                      else staffEmail = responseArr[0];
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
              return staffEmail;
          }

          private string GetStaffPassword(string username)
          {
              string password = string.Empty;
              try
              {
                  string response = webportals.GetStaffPassword(username);
                  if (!string.IsNullOrEmpty(response))
                  {
                      password = response;
                  }
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
              return password;
          }

          protected void lbtnLogin_Click(object sender, EventArgs e)
          {
              try
              {
                  string username = txtusername.Text.Trim();
                  string password = txtpassword.Text.Trim();

                  if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                  {
                      lblError.Text = "Username and password cannot be empty";
                      return;
                  }

                if (!ValidStaffNo(username))
                {
                    lblError.Text = "Invalid Staff No.";
                    return;
                }

                if (ChangedPassword(username))
                {
                    LoginForChangedPassword(username, password);
                }
                else
                {
                    LoginForUnchangedPassword(username, password);
                }
            }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
          }
       
          protected void lbtnForgot_Click(object sender, EventArgs e)
          {
              try
              {
                  string username = txtusername.Text.Trim();
                  if (string.IsNullOrEmpty(username))
                  {
                      lblError.Text = "Kindly input Staff Number";
                      txtusername.Focus();
                      return;
                  }

                if (!ValidStaffNo(username))
                {
                    lblError.Text = "Invalid Staff No";
                    return;
                }
                string newPassword = GenerateRandomPassword(10);
                string response = Components.ObjNav.UpdateStaffAutoGenPassword(username, newPassword);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response != "SUCCESS")
                    {
                        lblError.Text = "Failed to reset the password. Please try again.";
                        return;
                    }

                }
                string email = GetStaffEmail(username);
                //string staffPassword = GetStaffPassword(username);
                string subject = "Telposta Portal Password Reset";
                string body = $"Use this password to log into your portal.<br/><br/>Auto generated Portal password: <strong>{newPassword}</strong>. Login and change password <br/><br/>Do not reply to this email.";
                Components.SentEmailAlerts(email, subject, body);
                lblError.Text = $"Auto generated Portal password has been sent to your email address {email}";
                return;


                //  was before
                //string email = GetStaffEmail(username);
                //string staffPassword = GetStaffPassword(username);
                //string subject = "Telposta Portal Password Reset";
                //string body = $"Use this password to log into your portal.<br/><br/>Portal password: <strong>{staffPassword}</strong><br/><br/>Do not reply to this email.";
                //Components.SentEmailAlerts(email, subject, body);
                //lblError.Text = $"Portal password has been sent to your email address {email.ToUpper()}";
                //return;
              }
              catch (Exception ex)
              {
                  ex.Data.Clear();
              }
          }
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] byteBuffer = new byte[1];

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(byteBuffer);
                    int index = byteBuffer[0] % validChars.Length;
                    password.Append(validChars[index]);
                }
            }

            return password.ToString();
        }
    }
}