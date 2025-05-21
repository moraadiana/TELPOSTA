using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Description;
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
                        string email = responseArr[3];
                        string phoneNo = responseArr[4];
                        Session["username"] = staffNo;
                          Session["staffName"] = staffName;
                          Session["email"] = email;
                        Session["phone"] = phoneNo;
                        string otp = GenerateOtp(6);
                        Session["otp"] = otp;
                        Session["OtpCode"] = otp;
                        Session["OtpGeneratedAt"] = DateTime.UtcNow;
                        string maskedPhone = phoneNo.Length >= 12
? phoneNo.Substring(0, 4) + "xxxxx" + phoneNo.Substring(phoneNo.Length - 2)
: phoneNo;
                        string subject = "Telposta Staff Portal OTP";

                        string body = $"Dear {staffName}, your OTP for the Staff Portal is {otp} . It is valid for 5 minutes. ";
                        Components.SentEmailAlerts(email, subject, body);
                        Components.SendSMSAlerts(phoneNo, body);
                        Message( $"An OTP has been sent to your email: {email} and phone: {maskedPhone}");
                        
                        Response.Redirect("~/pages/verifyotp.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();

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
                   //ex.Data.Clear();
                Message("An error occurred . Please try again.");
            }
          }

          private void LoginForUnchangedPassword(string username, string password)
          {
              try
              {
                  string response = webportals.LoginForUnchnagedPassword(username, password);
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
        public static string GenerateOtp(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
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
                string Name = webportals.getEmployeeName(username);
                string response1 = webportals.GetEmployeeDetails(username);
                if (!string.IsNullOrEmpty(response1))
                {
                    string[] responseArr = response1.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        string phoneNo = responseArr[10].ToString();
                        
                        Session["PhoneNo"] = phoneNo;
                        
                    }
                }
                string PhoneNo = Session["PhoneNo"].ToString();
                string maskedPhone = PhoneNo.Length >= 12
? PhoneNo.Substring(0, 4) + "xxxxx" + PhoneNo.Substring(PhoneNo.Length - 2)
: PhoneNo;

                //string staffPassword = GetStaffPassword(username);
                string subject = "Telposta Staff Portal Password Reset";
                //string body = $"Use this password to log into Telposta Staff portal .<br/> <br/>Auto generated Portal password: <strong>{newPassword}</strong> <br/> <br/>Do not reply to this email.";
                string body = $"Dear {Name}, Use this password to log in: {newPassword} . Do not reply.";
                Components.SentEmailAlerts(email, subject, body);

                Components.SendSMSAlerts(PhoneNo, body);
                Message($"Auto generated password has been sent to your email address: {email} and phone number : {maskedPhone}");
                return;


               
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
        private void Message(string message)
        {
            string strScript = $"<script>alert('{message}');</script>";
            Page.RegisterStartupScript("ClientScript", strScript);
        }
    }
}