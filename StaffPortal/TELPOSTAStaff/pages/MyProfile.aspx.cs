using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class MyProfile : System.Web.UI.Page
    {
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        Staffportall webportals = Components.ObjNav;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");

                }
                LoadEmployeeDetails();

            }
        }

        private void LoadEmployeeDetails()
        {
            string username = Session["username"].ToString();

            string response = webportals.GetEmployeeDetails(username);
            if (!string.IsNullOrEmpty(response))
            {
                string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                string returnMsg = responseArr[0];
                if (returnMsg == "SUCCESS")
                {
                    
                    txtFirstName.Text = responseArr[1].ToString();
                    txtMiddleName.Text = responseArr[2].ToString();
                    txtLastName.Text = responseArr[3].ToString();
               
                    string gender = responseArr[4].Trim();
                    if (ddlGender.Items.FindByText(gender) != null)
                    {
                        ddlGender.ClearSelection();
                        ddlGender.Items.FindByText(gender).Selected = true;
                    }
                    lblDoB.Text = responseArr[5].ToString();
                    
                    string maritalStatus = responseArr[6].Trim();
                    if (ddlMaritalStatus.Items.FindByText(maritalStatus) != null)
                    {
                        ddlMaritalStatus.ClearSelection();
                        ddlMaritalStatus.Items.FindByText(maritalStatus).Selected = true;
                    }
                    txtReligion.Text = responseArr[7].ToString();
                    txtTribe.Text = responseArr[8].ToString();
                    txtEmail.Text = responseArr[9].ToString();
                    txtPhoneNo.Text = responseArr[10].ToString();
                    txtCounty.Text = responseArr[11].ToString();
                    lblID.Text = responseArr[13].ToString();
                    txtAddress.Text = responseArr[14].ToString();
                    // ddlTitle.Text = responseArr[15].ToString();
                    string empTitle = responseArr[15].Trim();
                    if (ddlTitle.Items.FindByText(empTitle) != null)
                    {
                        ddlTitle.ClearSelection();
                        ddlTitle.Items.FindByText(empTitle).Selected = true;
                    }


                    lblEmpno.Text = responseArr[16].ToString();

                }
                else

                {
                    Message("No data found");
                }

            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string username = Session["username"].ToString();
            string firstName = txtFirstName.Text;
            string middleName = txtMiddleName.Text;
            string lastName = txtLastName.Text;
            string email = txtEmail.Text;
            string phoneNo = txtPhoneNo.Text;
            string address = txtAddress.Text;
            string gender = ddlGender.Text;
            string DOB = lblDoB.Text;
            string maritalStatus = ddlMaritalStatus.Text;
            string religion = txtReligion.Text;
            string tribe = txtTribe.Text;
            string county = txtCounty.Text;
            string idNo = lblID.Text;
            string title = ddlTitle.Text;

            // Convert gender, marital status, and title to integers (if required by the AL procedure)
            int genderInt = Convert.ToInt32(gender);
            int maritalStatusInt = Convert.ToInt32(maritalStatus);
            int titleInt = Convert.ToInt32(title);
            DateTime DoB = Convert.ToDateTime(DOB);

            string response = webportals.UpdateEmployeeDetails(username, firstName, middleName,
                lastName, genderInt, DoB, maritalStatusInt, religion, tribe, email, phoneNo, county, idNo, address, titleInt);
            if (response == "Success")
            {
                SuccessMessage("Details updated successfully.");

            }
            else
            {
                Message("Failed to update details: ");

            }
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            txtFirstName.ReadOnly = false;
            txtLastName.ReadOnly = false;
            txtMiddleName.ReadOnly = false;
           
            txtReligion.ReadOnly = false;
            txtTribe.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtPhoneNo.ReadOnly = false;
            txtCounty.ReadOnly = false;
            txtAddress.ReadOnly = false;
            
        }
        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "');</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        private void SuccessMessage(string message)
        {
            string page = "dashboard.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "';</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }
    }
}