using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class Dashboard : System.Web.UI.Page
    {
        Staffportall webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                LoadEmployeeDetails();
                LoadEmployeeProfile();
            }
        }
        

       private void LoadEmployeeProfile()
       {
           try
           {
               string username = Session["username"].ToString();
                //string gender = webportals.GetStaffGender(username);
                //string imgPath = string.Empty;
                //if (gender == "Male") imgPath = "profile_m";
                //if (gender == "Female") imgPath = "profile_f";
                
                string virtualPath = $"~/profiles/{username}.png";
                string defaultVirtualPath = $"~/profiles/Default.png";

                string physicalPath = Server.MapPath(virtualPath);

                if (File.Exists(physicalPath))
                {
                    ImgProfilePic.ImageUrl = virtualPath;
                }
                else
                {
                    ImgProfilePic.ImageUrl = defaultVirtualPath;
                }
            }

                    
           catch (Exception ex)
           {
               ex.Data.Clear();
           }
       }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuProfilePic.HasFile)
            {
                Message("Please select a file");
                return;
            }

            // validate file type/size if you like
            string ext = Path.GetExtension(fuProfilePic.FileName).ToLower();
            if (ext != ".jpg" && ext != ".png" && ext != ".jpeg")
            {
                Message(" must be .jpg , .png or .jpeg file");
                return;
            }

            string path = Server.MapPath("~/Profiles/");
            string employeeNo = lblEmployeeNo.Text;
                //Session["trusteeNo"].ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filename = employeeNo + ".png";
            string filepath = path + filename;
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            fuProfilePic.SaveAs(filepath);
            LoadEmployeeProfile();
            
            webportals.UploadProfilePicture(employeeNo, filepath, "Profile pic");
            SuccessMessage("Profile updated sucessfully");
        }

        private void LoadEmployeeDetails()
       {
           try
           {
               string username = Session["username"].ToString();
               string response = webportals.GetStaffDetails(username);
               if (!string.IsNullOrEmpty(response))
               {
                   string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                   string returnMsg = responseArr[0];
                   if (returnMsg == "SUCCESS")
                   {
                       string staffNo = responseArr[1];
                       string gender = responseArr[2];
                       string idNumber = responseArr[3];
                       string email = responseArr[4];
                       string companyEmail = responseArr[5];
                       string phoneNumber = responseArr[6];
                       string citizenship = responseArr[7];
                       string postalCode = responseArr[8];
                       string postalAddress = responseArr[9];
                       string jobTitle = responseArr[10];
                       string staffName = Session["staffName"].ToString();

                       lblEmployeeNo.Text = staffNo;
                       lblGender.Text = gender;
                       lblIDNo.Text = idNumber;
                       lblEmail.Text = email;
                       lblEmailCompany.Text = companyEmail;
                       lblPhoneNo.Text = phoneNumber;
                       lblCitizenship.Text = citizenship;
                       lblPostalCode.Text = postalCode;
                       lblPostalAddress.Text = postalAddress;
                       lblDesignation.Text = jobTitle;
                       lblStaffName.Text = staffName;
                   }
               }
           }
           catch (Exception ex)
           {
               ex.Data.Clear();
           }
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