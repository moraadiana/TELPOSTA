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
                // newLines.Visible = false;
                // LoadPassengers();
                string query = Request.QueryString["query"];

           //     string requestNo = Request.QueryString["RequestNo"];
                string approvalStatus = Request.QueryString["status"].Replace("%", " ");
                if (query == "new")
                {
                    MultiView1.SetActiveView(View1);
                }
                else if (query == "old")
                {
                    //if (requestNo != null)

                    // MultiView1.ActiveViewIndex = 1;
                    string requestNo = Request.QueryString["RequestNo"];
                    MultiView1.SetActiveView(View2);
                    BindGridviewData(requestNo);
                    // loadtraveldestinations();
                    //LoadPassengers();
                }
                //else
                //{
                //    //MultiView1.ActiveViewIndex = 0; View1
                //    MultiView1.SetActiveView(View1);
                //}

                if (approvalStatus == "Open" || approvalStatus == "Pending")
                {
                    lbtnSubmit.Visible = true;
                    lbtnAddLine.Visible = true;
                    
                }
                else if (approvalStatus == "Pending Approval")
                {
                    lbtnSubmit.Visible = true;
                    lbtnAddLine.Visible = false;
                    
                }
                else
                {
                    lbtnSubmit.Visible = false;
                    lbtnAddLine.Visible = false;
                    
                }
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
                        //lblUnitCode.Text = responseArr[2];
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
            if (!ValidateFields()) return; // Validate fields before proceeding

            try
            {
                string empNo = Session["username"].ToString();
                string username = Session["StaffName"].ToString().ToUpper().Replace(" ", ".");
                string travelType = ddlTravelType.SelectedValue;
                int type = Convert.ToInt32(travelType);
                string purpose = txtPurpose.Text;
                string reqNo = Session["RequestNo"]?.ToString() ?? string.Empty;
               
               // string description = txtDescription.Text.Trim();
                


                // string details = Components.ObjNav.CreateTransportRequest(reqNo, description, dateOfTravel, noOfDays, expectedReturnDate, from, destination, unitCode, department, createdBy);
                string details = webportals.CreateClaimRequisitionHeader(empNo, type, purpose);
                
                if(!string.IsNullOrEmpty(details))
                {
                    string[] responseArr = details.Split(strLimiters, StringSplitOptions.None);
                        string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        string requestNo = responseArr[1];
                        Message($"Transport Request number {requestNo} has been created successfully!");
                        Session["requestNo"] = requestNo;
                        MultiView1.ActiveViewIndex = 1;
                    }
                
                    else
                    {
                        Message($"error creating request");
                    }
                    // Switch to the second view
                    //MultiView1.ActiveViewIndex = 1;
                }

            }
            catch (Exception ex)
            {
                // Handle exception if needed
                ex.Data.Clear();
            }

        }

        private bool ValidateFields()
        {
            // Validate each field here
            if (string.IsNullOrWhiteSpace(ddlTravelType.SelectedValue))
            {
                Message("Travel Type is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(lblPayee.Text))
            {
                Message("Payee is required.");
                return false;
            }

           if (string.IsNullOrWhiteSpace(txtPurpose.Text))
            {
                Message("Purpose is required");
            }

            if (string.IsNullOrWhiteSpace(lblDepartment.Text))
            {
                Message("Department is required.");
                return false;
            }



            return true;
        }

        protected void lbtnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("TransportRequisitionListing.aspx"); // Go back to the first view
        }

        protected void lbnClose_Click(object sender, EventArgs e)
        {
            newLines.Visible = false;
           // lbnAddLine.Visible = true;
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
                string accomodation = ddlAccomodation.SelectedValue;
                int acc = Convert.ToInt32(accomodation);
                bool acco = Convert.ToBoolean(acc);
                string destinationType = ddlDestinationType.SelectedValue;
                int destType = Convert.ToInt32(destinationType);
                DateTime dateOfTravel;
                
                string days = txtNoOfDays.Text;
                int noOfDays = Convert.ToInt32(days);
                string department = lblDepartment.Text;
                string claimNo = Session["requestNo"]?.ToString() ?? Request.QueryString["RequestNo"];
                //decimal noOfDays = Convert.ToInt32(Days);

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
                //(claimNo: Text; EmpTrustee: option; staffNo: Text; destination: Text; accomodation: Boolean; startDate: Date; endDate: Date; days: Integer)
                //  procedure InsertTravelRequestLines(claimNo: Text; EmpTrustee: option; staffNo: Text; payee: Text; destination: Text; accomodation: Boolean; startDate: Date; endDate: Date; days: Integer; taxCode: Text) Message: Text

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
                       //txtPayee.Text = null;
                        //txtPhoneNo.Text = null;

                        BindGridviewData(claimNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Message("An error occurred while adding the passenger. Please try again.");

            }
        }

        private void HandlePassengerInsertionResult(string result, string reqNo)
        {
            if (!string.IsNullOrEmpty(result))
            {
                string[] resultParts = result.Split(new[] { "::" }, StringSplitOptions.None);
                string returnMsg = resultParts[0];

                if (returnMsg == "SUCCESS")
                {
                    Message("Passenger added successfully.");
                    //ddlPassengerNo.SelectedIndex = 0;
                    ddlStaffNo.Items.Clear();
                  //  txtPayee.Text = null;
                   // txtPhoneNo.Text = null;
                    BindGridviewData(reqNo);
                }
            }
        }

        private void BindGridviewData(string requestNo)
        {
            string lineData = Components.ObjNav.GetTravelRequestLines(requestNo);
            if (!string.IsNullOrEmpty(lineData) )
            {
               // var lineRows = lineData.Split("[ ]");
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

        private void Message(string message)
        {
            string strScript = $"<script>alert('{message}');</script>";
            Page.RegisterStartupScript("ClientScript", strScript);
        }

        protected void lbnAddLine_Click(Object sender, EventArgs e)
        {
            newLines.Visible = true;
          //  lbnAddLine.Visible = false;
            //LoadPassengers();
           
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
           // lblLNo.Text = claimNo;
           // LoadAdvanceTypes();
            newLines.Visible = true;
            lbtnAddLine.Visible = false;
        }

        protected void lbtnBack1_Click(object sender, EventArgs e)
        {
            Response.Redirect("TransportRequisitionListing.aspx");
        }


        protected void cancel(object sender, EventArgs e)
        {
            string message = "Are you sure you want to delete?";
            ClientScript.RegisterOnSubmitStatement(this.GetType(), "confirm", "return confirm('" + message + "');");
            string[] arg = new string[2];
            arg = (sender as LinkButton).CommandArgument.ToString().Split(';');
           // string passengerNo = arg[0];
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


            if (!CheckPassengersAdded(reqNo))
            {
                Message("Please add at least one passenger before submitting for approval.");
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
        private bool CheckPassengersAdded(string requestNo)
        {
            // Check if the GridView has any rows
            return gvLines.Rows.Count > 0;
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
                //  string requestNo= Session["requestNo"].ToString();
                string response = webportals.GetTravelDestination(staff);
                if (!string.IsNullOrEmpty(response))
                {

                    //   string[] passengersList = passengerList.Split('');
                    string[] responseList = response.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                ddlDestination.Items.Clear();

                    foreach (string destination in responseList)
                    {

                        string[] details = destination.Split(new string[] { "::" }, StringSplitOptions.None);
                        //if (details.Length == 3)
                        //{
                        string code = details[0];
                        string name = details[1];
                        //string passengerPhoneNo = details[2];

                        // Add new item to the dropdown list
                        System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem($"{code} - {name}", code);
                        ddlDestination.Items.Add(listItem);
                        ////txtName.Text = passengerName;
                        //txtPhoneNo.Text = passengerPhoneNo;
                        // }
                    }
                }

            
        }

        private void LoadPassengers()
        {
            string type = ddlEmployeeTrustee.SelectedValue;
            
            string staffList = Components.ObjNav.GetTravelStaff(Convert.ToInt32(type));

            if (!string.IsNullOrEmpty(staffList))
            {

             //   string[] passengersList = passengerList.Split('');
                string[] StaffsList = staffList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);


                ddlStaffNo.Items.Clear();

                foreach (string Stafflist in StaffsList)
                {

                    string[] details = Stafflist.Split(new string[] { "::" }, StringSplitOptions.None);
                    //if (details.Length == 3)
                    //{
                    string staffNo = details[0];
                    string staffName = details[1];
                    //string passengerPhoneNo = details[2];

                    // Add new item to the dropdown list
                    System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem($"{staffNo} - {staffName}", staffNo);
                    ddlStaffNo.Items.Add(listItem);
                    ////txtName.Text = passengerName;
                    //txtPhoneNo.Text = passengerPhoneNo;
                    // }
                    //loadtraveldestinations();
                }
            }
        }

    }
}