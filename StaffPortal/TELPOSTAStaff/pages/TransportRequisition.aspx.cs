using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;



namespace TELPOSTAStaff.pages
{
    public partial class TransportRequisition : System.Web.UI.Page
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
                    return;
                }
                LoadStaffDepartmentDetails();
                LoadResponsibilityCenter();
                LoadVendorAccountNo();

                string query = Request.QueryString["query"];
                string approvalStatus = Request.QueryString["status"].Replace("%", " ");
                if (query == "new")
                {
                    MultiView1.SetActiveView(View1);
                }
                else if (query == "old")
                {
                    string requestNo = Request.QueryString["RequestNo"];
                    string accomodationProvided = Request.QueryString["accomodation"];

                    if (accomodationProvided == "Yes")
                    {
                        MultiView1.SetActiveView(View2);
                        BindGridviewData2(requestNo);
                    }
                    else
                    {
                        MultiView1.SetActiveView(View3);
                        BindGridviewData(requestNo);
                    }
                }

                if (approvalStatus == "Open" || approvalStatus == "Pending")
                {
                    lbtnSubmit.Visible = true;
                    lbtnAddLine.Visible = true;
                    lbnAddAccomodation.Visible = true;


                }
                else if (approvalStatus == "Pending Approval")
                {
                    lbtnSubmit.Visible = false;
                    lbtnAddLine.Visible = false;
                    lbnAddAccomodation.Visible = false;

                }
                else
                {
                    lbtnSubmit.Visible = false;
                    lbtnAddLine.Visible = false;
                    lbnAddAccomodation.Visible = false;

                }
            }
        }
        private void LoadVendorAccountNo()
        {
            try
            {
                ddlAccountNo.Items.Clear();
                string accountNoList = webportals.GetVendors();
                if (!string.IsNullOrEmpty(accountNoList))
                {

                    string[] AccountList = accountNoList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                    ddlAccountNo.Items.Clear();

                    foreach (string account in AccountList)
                    {

                        string[] details = account.Split(new string[] { "::" }, StringSplitOptions.None);

                        string No = details[0];
                        string Name = details[1];
                        System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem($"{No} - {Name}", No);
                        
                        ddlAccountNo.Items.Add(listItem);

                    }
                }
            }
            catch (Exception ex)
            {
                Message("An error occurred . Please try again.");

            }
        }
        protected void lbtnToLines_Click(object sender, EventArgs e)
        {
            string requestNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
            if (gvLines1.Rows.Count < 1)
            {
                Message("Please add atleast one accomodation lines ");
            }
            MultiView1.SetActiveView(View3);
            BindGridviewData(requestNo);
            



        }
        private void LoadResponsibilityCenter()
        {
            try
            {
                ddlResponsibilityCenter.Items.Clear();

                string resCenters = webportals.GetAllResponsibilityCentres();
                if (!string.IsNullOrEmpty(resCenters))
                {
                    string[] resCenterArr = resCenters.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string rescenter in resCenterArr)
                    {
                        ddlResponsibilityCenter.Items.Add(new ListItem(rescenter));
                    }
                }
                else
                {
                    ddlResponsibilityCenter.Items.Add(new ListItem("No responsibility centers available"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                ddlResponsibilityCenter.Items.Add(new ListItem("Error loading responsibility centers"));
            }
        }
        private void LoadStaffDepartmentDetails()
        {
            try
            {
                string staffNo = Session["username"].ToString();
                string staffName = Session["StaffName"].ToString();
                string response = webportals.GetStaffDepartmentDetails(staffNo);
                if (!string.IsNullOrEmpty(response))
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        lblDepartment.Text = responseArr[1];
                    }
                    else
                    {
                        Message("An error occured while loading details. Please try again later.");
                        return;
                    }
                    lblStaffNo.Text = staffNo;
                    lblPayee.Text = staffName;
                    lblRequester.Text = staffNo;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
      
        
        protected void lbtnNext_Click(object sender, EventArgs e)
        {

            try
            {
                string empNo = Session["username"].ToString();
                string username = Session["StaffName"].ToString().ToUpper().Replace(" ", ".");
                string travelType = ddlTravelType.SelectedValue;
                int type = Convert.ToInt32(travelType);
                if (string.IsNullOrEmpty(travelType))
                {
                    Message("Travel Type must have a value");
                    return;

                }
                string purpose = txtPurpose.Text;
                string reqNo = Session["RequestNo"]?.ToString() ?? string.Empty;
                string resCenter = ddlResponsibilityCenter.SelectedValue;
                string accomodation = ddlCompanyAccommodation.SelectedValue;
                if (string.IsNullOrEmpty(resCenter) || resCenter == "--Select--")
                {
                    Message("Responsiblity Center Type must have a value");
                    return;

                }

                if (string.IsNullOrEmpty(accomodation) || accomodation == "--Select--")
                {
                    throw new Exception("Please select an accommodation preference.");
                }
                if (string.IsNullOrEmpty(purpose))
                {
                    Message("Purpose must have a value");
                    return;

                }
               
                int acc = Convert.ToInt32(accomodation);
                bool accomodationProvided = Convert.ToBoolean(acc);

               
                string details = webportals.CreateTravelRequisitionHeader(empNo, type, purpose, resCenter, accomodationProvided);
                
                if(!string.IsNullOrEmpty(details))
                {
                    string[] responseArr = details.Split(strLimiters, StringSplitOptions.None);
                        string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        string requestNo = responseArr[1];
                        Message($"Transport Request number {requestNo} has been created successfully!");
                        Session["requestNo"] = requestNo;
                        Session["accomodation"]  = accomodation;
                        
                        if (Convert.ToInt32(accomodation) == 1)
                        {
                            MultiView1.SetActiveView(View2);
                        }
                        else
                        {
                            MultiView1.SetActiveView(View3);
                        }
                    }
                
                    else
                    {
                        Message($"error creating request");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

        }
        protected void btnAddAccommodation_Click(object sender, EventArgs e)
        {
            try
            {

                string accountType = ddlAccountType.SelectedValue;
                if (string.IsNullOrEmpty(accountType) || accountType == "-- Select -- ")
                {
                    Message("Account Type must have a value");
                    return;

                }
                int type = Convert.ToInt32(accountType);
                string AccountNo = ddlAccountNo.SelectedValue;
                string description = txtDescription.Text;
               
                string ReqNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
               
                if (string.IsNullOrEmpty(AccountNo) )
                {
                    Message("Account No must have a value");
                    return;

                }
                if (string.IsNullOrEmpty(description))
                {
                    Message("Description must have a value");
                    return;

                }
                string amount = txtAmount.Text;

                if (string.IsNullOrEmpty(amount))
                {
                    Message("Ammount must have a value");
                    return;

                }

                decimal parsedAmount;
                if (!decimal.TryParse(amount, out parsedAmount))
                {
                    Message("Please enter a valid numeric value for the amount.");
                    return; 
                }

                if (parsedAmount <= 0)
                {
                    Message("Amount must be greater than zero.");
                    return; 
                }
                string result = Components.ObjNav.InsertAccomodationLines(ReqNo, type, AccountNo, description, Convert.ToDecimal(amount)); //, 

                if (!String.IsNullOrEmpty(result))
                {
                    string returnMsg = "";
                    string[] strdelimiters = new string[] { "::" };
                    string[] result_arr = result.Split(strdelimiters, StringSplitOptions.None);

                    returnMsg = result_arr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        Message("Line added successfully.");

                        BindGridviewData2(ReqNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Message("An error occurred while adding the line. Please try again.");

            }
        }


        protected void lbtnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("TransportRequisitionListing.aspx");
        }

        protected void lbnClose_Click(object sender, EventArgs e)
        {
            newLines.Visible = false;
            accomodationLines.Visible = false;
           
        }
        
        protected void btnLine_Click(object sender, EventArgs e)
        {
            try
            {
                string EmpTrustee = ddlEmployeeTrustee.SelectedValue;
                int trustee = Convert.ToInt32(EmpTrustee);
                string staffNo= ddlStaffNo.SelectedValue;
                string staffDetails = ddlStaffNo.SelectedItem.Text;

                string[] details = staffDetails.Split(new string[] { " - " }, StringSplitOptions.None);
                string payee = details.Length > 1 ? details[1] : string.Empty;
                string destination = ddlDestination.SelectedValue;
                string accomodation = ddlAccommodation.SelectedValue;
                int acc = Convert.ToInt32(accomodation);
                bool acco = Convert.ToBoolean(acc);
                string destinationType = ddlDestinationType.SelectedValue;
                int destType = Convert.ToInt32(destinationType);
                DateTime dateOfTravel;
                
                string days = txtNoOfDays.Text;
                int noOfDays = Convert.ToInt32(days);
                string department = lblDepartment.Text;
                string claimNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
                
                if (string.IsNullOrEmpty(EmpTrustee))
                {
                    Message("Employee/Trustee must have a value");
                    return;
                }
                if (string.IsNullOrEmpty(staffNo))
                {
                    Message("Staff Number must have a value");
                    return;
                }
                if (string.IsNullOrEmpty(destination))
                {
                    Message("Destination must have a value");
                    return;
                }
                if (string.IsNullOrEmpty(accomodation))
                {
                    Message("Accomodation must have a value");
                    return;
                }
                if (string.IsNullOrEmpty(destinationType))
                {
                    Message("Destination Type must have a value");
                    return;
                }
                if (!DateTime.TryParse(txtDateOfTravel.Text, out dateOfTravel))
                {
                    Message("Invalid date of travel. Please enter a valid date.");
                    return;
                }
                if ( noOfDays <= 0)
                {
                    Message("Number of days must be a positive number.");
                    return;
                }

                DateTime expectedReturnDate = dateOfTravel.AddDays((double)noOfDays);

                

                if (string.IsNullOrEmpty(claimNo))
                {
                    Message("Error: No requisition selected.");
                    return;
                }
                if (string.IsNullOrEmpty(days))
                {
                    Message("Days cannot be null");
                    return;
                }

                string result = Components.ObjNav.InsertTravelRequestLines(claimNo, trustee, staffNo, destination, acco, dateOfTravel, expectedReturnDate, noOfDays, destType); //, 
                

                if (!String.IsNullOrEmpty(result))
                {
                    string returnMsg = "";
                    string[] strdelimiters = new string[] { "::" };
                    string[] result_arr = result.Split(strdelimiters, StringSplitOptions.None);

                    returnMsg = result_arr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        Message("Line added successfully.");
                        ddlStaffNo.SelectedIndex = 0;
                       
                        BindGridviewData(claimNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Message("An error occurred while adding the passenger. Please try again.");

            }
        }

        private void BindGridviewData(string requestNo)
        {
            string lineData = Components.ObjNav.GetTravelRequestLines(requestNo);
            if (!string.IsNullOrEmpty(lineData) )
            {
                string[] lineItems = lineData.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                var lineList = new DataTable();
                lineList.Columns.Add("Trustee/Employee");
                lineList.Columns.Add("StaffNo");
                lineList.Columns.Add("Payee");
                lineList.Columns.Add("DestinationType");
                lineList.Columns.Add("Destination");
                lineList.Columns.Add("IncludeAccomodation");
                lineList.Columns.Add("StartDate");
                lineList.Columns.Add("NoofDays" );
                lineList.Columns.Add("EndDate");
                lineList.Columns.Add("Description");
                lineList.Columns.Add("TaxCode");
                lineList.Columns.Add("Line No_");

                foreach (var row in lineItems)
                {
                    var columns = row.Split(new[] { "::" }, StringSplitOptions.None);
                    if (columns.Length == 12)
                    {
                        string accomodation = columns[5] == "0" ? "Yes" : columns[5] == "1" ? "No" : "Unknown";
                        lineList.Rows.Add(columns[0], columns[1], columns[2], columns[3], columns[4], accomodation, columns[6], columns[7], columns[8], columns[9], columns[10], columns[11]);
                    }
                }

                gvLines.DataSource = lineList;
                gvLines.DataBind();
            }
            else
            {
                gvLines.DataSource = null;
                gvLines.DataBind();
            }
        }
        private void BindGridviewData2(string requestNo)
        {
            string lineData = Components.ObjNav.GetAccomodationLines(requestNo);
            if (!string.IsNullOrEmpty(lineData))
            {
                string[] lineItems = lineData.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                DataTable lineList = new DataTable();
                lineList.Columns.Add("Account Type");
                lineList.Columns.Add("Account No");
                lineList.Columns.Add("Account Name");
                lineList.Columns.Add("Description");
                lineList.Columns.Add("Amount");
                lineList.Columns.Add("Line No_");
                foreach (string item in lineItems)
                {
                    string[] fields = item.Split(strLimiters, StringSplitOptions.None);

                    if (fields.Length >= 5)
                    {
                        DataRow row = lineList.NewRow();
                        row["Account Type"] = fields[0];
                        row["Account No"] = fields[1];
                        row["Account Name"] = fields[2];
                        row["Description"] = fields[3];
                        row["Amount"] = fields[4];
                        row["Line No_"] = fields[5];
                        lineList.Rows.Add(row);
                    }
                    else
                    {
                        Console.WriteLine("Skipping invalid record: " + item);
                    }
                }

                gvLines1.DataSource = lineList;
                gvLines1.DataBind();
            }
            else
            {
                gvLines1.DataSource = null;
                gvLines1.DataBind();
            }
        }
        protected void lbtnRemove_Click(object sender, EventArgs e)
        {
            string approvalStatus = Request.QueryString["status"].Replace("%", " ");
            if (approvalStatus != "Open" || approvalStatus != "Pending")
            {
                Message("You can only delete line when status is open or pending");

            }
            string message = "Are you sure you want to delete?";
            ClientScript.RegisterOnSubmitStatement(this.GetType(), "confirm", "return confirm('" + message + "');");
            
            string[] arg = new string[2];
            arg = (sender as LinkButton).CommandArgument.ToString().Split(';');
            string lineNo = arg[0];
            string reqNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
            try
            {
                Components.ObjNav.RemoveAccomodationLines(Convert.ToInt32(lineNo));
                Message("Deleted successfully");
                BindGridviewData2(reqNo);

            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message.ToString());
                ex.Data.Clear();
            }
        }
        protected void lbtnAddAccomodation_Click(object sender, EventArgs e)
        {
            accomodationLines.Visible = true;
            lbnAddAccomodation.Visible = true;
        }

        private void Message(string message)
        {
            string strScript = $"<script>alert('{message}');</script>";
            Page.RegisterStartupScript("ClientScript", strScript);
        }

       
        protected void lbtnAddLine_Click(object sender, EventArgs e)
        {
            string requestNo = string.Empty;
            if (Request.QueryString["requestNo"] == null)
            {
                requestNo = Session["requestNo"].ToString();
            }
            else
            {
                requestNo = Request.QueryString["requestNo"].ToString();
            }
            newLines.Visible = true;
            lbtnAddLine.Visible = false;
        }

        protected void cancel(object sender, EventArgs e)
        {
            string approvalStatus = Request.QueryString["status"].Replace("%", " ");
            if (approvalStatus != "Open" || approvalStatus != "Pending")
            {
                Message("You can only delete line when status is open or pending");

            }
            string message = "Are you sure you want to delete?";
            ClientScript.RegisterOnSubmitStatement(this.GetType(), "confirm", "return confirm('" + message + "');");
            string[] arg = new string[2];
            arg = (sender as LinkButton).CommandArgument.ToString().Split(';');
           
            string lineNo = arg[0];
            string reqNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
            try
            {
               Components.ObjNav.RemoveClaimRequisitionLines(Convert.ToInt32(lineNo));
                Message("Deleted successfully");
                BindGridviewData(reqNo);

            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message.ToString());
                ex.Data.Clear();
            }
        }
        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {

            string reqNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];

            if(gvLines.Rows.Count < 1)
            {
                Message("Please add at least one travel line before submitting for approval.");
                return;
            }

            try
            {
                string resultMessage = Components.ObjNav.OnSendTravelRequestForApproval(reqNo);
                if (resultMessage == "SUCCESS")
                {
                    ShowMessageAndRedirect("Your request has been sent for approval.", "TransportRequisitionListing.aspx");
                }
                else
                {
                    ShowMessageAndRedirect(resultMessage, "TransportRequisitionListing.aspx");
                }

            }
            catch (Exception ex)
            {
                ShowMessageAndRedirect("ERROR: " + ex.Message, "TransportRequisitionListing.aspx");

            }

        }
        private void ShowMessageAndRedirect(string message, string redirectUrl)
        {
            string script = $"alert('{message}'); window.location='{redirectUrl}';";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }
        
        protected void ddlDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadtraveldestinations();
        }
        protected void ddlStaffNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadPassengers();  
                
            }
            catch (Exception ex)
            {
                Message("Error loading passengers: " + ex.Message);
            }
        }

        private void loadtraveldestinations()
        {
            
                string staff = ddlStaffNo.SelectedValue;
                string response = webportals.GetTravelDestination(staff);
                if (!string.IsNullOrEmpty(response))
                {

                    string[] responseList = response.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                ddlDestination.Items.Clear();

                    foreach (string destination in responseList)
                    {

                        string[] details = destination.Split(new string[] { "::" }, StringSplitOptions.None);
                        
                        string code = details[0];
                        string name = details[1];
                        System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem($"{code} - {name}", code);
                        ddlDestination.Items.Add(listItem);
                        
                    }
                }

        }

        private void LoadPassengers()
        {
            string type = ddlEmployeeTrustee.SelectedValue;
            
            string staffList = Components.ObjNav.GetTravelStaff(Convert.ToInt32(type));

            if (!string.IsNullOrEmpty(staffList))
            {
                string[] StaffsList = staffList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                ddlStaffNo.Items.Clear();

                foreach (string Stafflist in StaffsList)
                {

                    string[] details = Stafflist.Split(new string[] { "::" }, StringSplitOptions.None);
                 
                    string staffNo = details[0];
                    string staffName = details[1];
                    System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem($"{staffNo} - {staffName}", staffNo);
                    ddlStaffNo.Items.Add(listItem);
                }
            }
        }

    }
}