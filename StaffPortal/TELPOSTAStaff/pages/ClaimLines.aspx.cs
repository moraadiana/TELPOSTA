﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class ClaimLines : System.Web.UI.Page
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        SqlDataAdapter adapter;
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

                LoadStaffDetails();
                LoadResponsibilityCenter();

                string query = Request.QueryString["query"];
                string approvalStatus = Request.QueryString["status"].Replace("%", " ");
                if (query == "new")
                {
                    MultiView1.SetActiveView(vwHeader);
                }
                else if (query == "old")
                {
                    string claimNo = Request.QueryString["ClaimNo"];
                    MultiView1.SetActiveView(vwLines);
                    lblLNo.Text = claimNo;
                    lblClaimNo.Text = claimNo;
                    LoadAdvanceTypes();
                    BindGridViewData(claimNo);
                    BindAttachedDocuments(claimNo);
                }

                if (approvalStatus == "Open" || approvalStatus == "Pending")
                {
                    btnApproval.Visible = true;
                    btnCancellApproval.Visible = false;
                    attachments.Visible = true;
                }
                else if (approvalStatus == "Pending Approval")
                {
                    btnApproval.Visible = false;
                    btnCancellApproval.Visible = true;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
                else
                {
                    btnApproval.Visible = false;
                    btnCancellApproval.Visible = false;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
            }
        }

        private void LoadStaffDetails()
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
                        lblDirectorate.Text = responseArr[2];
                        lblDepartment.Text = responseArr[1];
                    }
                }

                lblStaffNo.Text = staffNo;
                lblPayee.Text = staffName;
                lblRequester.Text = staffNo;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }



        private void LoadResponsibilityCenter()
        {
            try
            {
                //ddlResponsibilityCenter.Items.Clear();

                //string grouping = "S-CLAIMS";
                //string resCenters = webportals.GetResponsibilityCentres(grouping);
                //if (!string.IsNullOrEmpty(resCenters))
                //{
                //    string[] resCenterArr = resCenters.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);

                //    foreach (string rescenter in resCenterArr)
                //    {
                //        ddlResponsibilityCenter.Items.Add(new ListItem(rescenter));
                //    }
                //}
                //else
                //{
                //    ddlResponsibilityCenter.Items.Add(new ListItem("No responsibility centers available"));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                //ddlResponsibilityCenter.Items.Add(new ListItem("Error loading responsibility centers"));
            }
        }




        private void LoadAdvanceTypes()
        {
            try
            {
                ddlAdvancType.Items.Clear();
                string advanceTypes = webportals.GetAdvancetype(4);
                if (!string.IsNullOrEmpty(advanceTypes))
                {
                    string[] advanceTypeArr = advanceTypes.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string advanceType in advanceTypeArr)
                    {
                        string[] responseArr = advanceType.Split(strLimiters, StringSplitOptions.None);
                        ListItem li = new ListItem(responseArr[1], responseArr[0]);
                        ddlAdvancType.Items.Add(li);
                    }
                }

            }


            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string username = Session["username"].ToString();
                string department = lblDepartment.Text;
                //string directorate = lblDirectorate.Text;
                //string responsibilityCenter = ddlResponsibilityCenter.SelectedValue;
                string purpose = txtPurpose.Text;
                String AccNo = "BNK-0001";
                string travelType = ddlTravelType.SelectedValue;
                //string ApplicantType = DdApplicantType.SelectedItem.Text;
                int MyPettyType = 0;
                if (travelType == "Local")
                {
                    MyPettyType = 0;
                }
                else if (travelType == "International")
                {
                    MyPettyType = 1;
                }

                //if (string.IsNullOrEmpty(department))
                //{
                //    Message("Department cannot be null!");
                //    return;
                //}
                //if (string.IsNullOrEmpty(directorate))
                //{
                //    Message("Division cannot be null!");
                //    return;
                //}
                //if (string.IsNullOrEmpty(responsibilityCenter))
                //{
                //    Message("Responsibility center cannot be null!");
                //    return;
                //}
                if (purpose == "")
                {
                    Message("Purpose cannot be null!");
                    txtPurpose.Focus();
                    return;
                }
                if (purpose.Length > 200)
                {
                    Message("Purpose should be 200 characters and below!");
                    return;
                }
                //TODO:

                //string response = webportals.CreateClaimRequisitionHeader(username, travelType, purpose);//ClaimRequisitionCreate(department, directorate, responsibilityCenter, "", username, purpose, DateTime.Today,"");//
                //if (!string.IsNullOrEmpty(response))
                //{
                //    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                //    string returnMsg = responseArr[0];
                //    if (returnMsg == "SUCCESS")
                //    {
                //        string claimNo = responseArr[1];
                //        Message($"Claim number {claimNo} has been created successfully!");
                //        Session["ClaimNo"] = claimNo;
                //        BindGridViewData(claimNo);
                //        NewView();
                //    }
                //}
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void NewView()
        {
            try
            {
                MultiView1.SetActiveView(vwLines);
                string claimNo = Session["ClaimNo"].ToString();
                newLines.Visible = true;
                lbtnAddLine.Visible = false;
                lblLNo.Text = claimNo;
                lblClaimNo.Text = claimNo;
                LoadAdvanceTypes();
                BindGridViewData(claimNo);
                BindAttachedDocuments(claimNo);

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        private void BindAttachedDocuments(string claimNo)
        {
            try
            {
                // Call the AL web service method
                string docLines = webportals.GetDocumentlines(claimNo);

                // Check if the response is not empty or null
                if (!string.IsNullOrEmpty(docLines) && docLines != "No document lines")
                {
                    // Split the response by '[]' to separate each line
                    string[] lineItems = docLines.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);

                    // Create a DataTable to hold the parsed data for binding
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Document No");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("$systemCreatedAt");
                    dt.Columns.Add("SystemId");



                    // Loop through each line item
                    foreach (string item in lineItems)
                    {
                        // Split each line by '::' to get individual fields
                        string[] fields = item.Split(strLimiters, StringSplitOptions.None);

                        // Check if we have the correct number of fields to avoid errors
                        if (fields.Length == 4)
                        {
                            DataRow row = dt.NewRow();
                            row["No_"] = fields[0];
                            row["File Name"] = fields[1];
                            row["$systemCreatedAt"] = fields[2];
                            row["SystemId"] = fields[3];
                            dt.Rows.Add(row);
                        }
                    }

                    // Bind the DataTable to the GridView
                    gvAttachments.DataSource = dt;
                    gvAttachments.DataBind();
                }
                else
                {
                    // Handle the case where there are no imprest lines
                    gvLines.DataSource = null;
                    gvLines.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or show an error message as needed)
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void BindAttachedDocuments1(string claimNo)
        {
            try
            {
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spDocumentLines",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                command.Parameters.AddWithValue("@DocNo", "'" + claimNo + "'");
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvAttachments.DataSource = dt;
                gvAttachments.DataBind();
                connection.Close();

                foreach (GridViewRow row in gvAttachments.Rows)
                {
                    row.Cells[3].Text = row.Cells[3].Text.Split(' ')[0];
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void BindGridViewData(string claimNo)
        {
            try
            {
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spClaimLines",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection,
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                command.Parameters.AddWithValue("@ClmNo", "'" + claimNo + "'");
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvLines.DataSource = dt;
                gvLines.DataBind();
                connection.Close();

                foreach (GridViewRow row in gvLines.Rows)
                {
                    row.Cells[4].Text = Convert.ToDateTime(row.Cells[4].Text).ToShortDateString();
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "')</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        private void SuccessMessage(string message)
        {
            string page = "ClaimListing.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "'</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        protected void btnLine_Click(object sender, EventArgs e)
        {
            try
            {
                string claimNo = lblClaimNo.Text;
                string employeeNo = Session["username"].ToString();
                string advanceType = ddlAdvancType.SelectedValue;
                string amount = txtAmnt.Text;
                if (advanceType == "0")
                {
                    Message("Advance type cannot be null!");
                    return;
                }
                if (amount == "")
                {
                    Message("Amount cannot be empty!");
                    txtAmnt.Focus();
                    return;
                }
                //TODO:

                string claimLine = webportals.InsertClaimRequisitionLines(claimNo, Convert.ToDecimal(amount));//InsertClaimRequisitionLines(claimNo, advanceType, claimNo, advanceType, Convert.ToDecimal(amount), employeeNo);//
                if (!string.IsNullOrEmpty(claimLine))
                {
                    string[] strLimiters = new string[] { "::" };
                    string[] claimLinesArr = claimLine.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = claimLinesArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        Message("Line added successfully!");
                        txtAmnt.Text = string.Empty;
                        BindGridViewData(claimNo);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void lbtnAddLine_Click(object sender, EventArgs e)
        {
            string claimNo = string.Empty;
            if (Request.QueryString["ClaimNo"] == null)
            {
                claimNo = Session["ClaimNo"].ToString();
            }
            else
            {
                claimNo = Request.QueryString["ClaimNo"].ToString();
            }
            lblLNo.Text = claimNo;
            LoadAdvanceTypes();
            newLines.Visible = true;
            lbtnAddLine.Visible = false;
        }

        protected void lbtnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                string status = Request.QueryString["status"].ToString().Replace("%", " ");
                if (status == "Open" || status == "Pending")
                {
                    string[] args = new string[2];
                    string claimNo = lblClaimNo.Text;
                    args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                    string lineNo = args[0];
                    //string response = "";
                  //  string response = webportals.RemoveClaimRequisitionLines(Convert.ToInt32(lineNo));
                    //if (!string.IsNullOrEmpty(response))
                    //{
                    //    if (response == "SUCCESS")
                    //    {
                    //        Message("Line deleted successfully!");
                           BindGridViewData(claimNo);
                    //    }
                    //    else
                    //    {
                    //        Message("An error occured while removing line. Please try again later");
                    //        return;
                    //    }
                    //}
                }
                else
                {
                    Message("You can only edit an open document!");
                    return;
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                string claimNo = lblClaimNo.Text;
                if (gvLines.Rows.Count < 1)
                {
                    Message("Please add lines before sending for approval!");
                    return;
                }
                if (gvAttachments.Rows.Count < 1)
                {
                    Message("Please attach documents before sending for approval!");
                    return;
                }
                string msg = webportals.OnSendClaimRequisitionForApproval(claimNo);//ClaimRequisitionApprovalRequest(claimNo);//
                if (msg == "SUCCESS")
                {
                    SuccessMessage($"Claim number {claimNo} has been sent for approval successfuly!");
                }
                else
                {
                    Message("ERROR: " + msg);
                }
            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message);
                ex.Data.Clear();
            }
        }

        protected void btnCancellApproval_Click(object sender, EventArgs e)
        {
            try
            {
                string claimNo = lblClaimNo.Text;
                webportals.OnCancelClaimRequisition(claimNo);//CancelClaimRequisition(claimNo); // 
                SuccessMessage($"Claim number {claimNo} has been cancelled successfuly!");
            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message);
                ex.Data.Clear();
            }
        }

        protected void lbtnClose_Click(object sender, EventArgs e)
        {
            newLines.Visible = false;
            lbtnAddLine.Visible = true;
        }

       protected void lbtnAttach_Click(object sender, EventArgs e)
        {/* 
            try
            {
                if (fuClaimDocs.PostedFile != null)
                {
                    string DocumentNo = lblLNo.Text.Replace("/", "-"); // Replace slashes with dashes
                    string username = Session["username"].ToString();
                    string filePath = fuClaimDocs.PostedFile.FileName.Replace(" ", "-");
                    string fileName = fuClaimDocs.FileName.Replace(" ", "-");
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension == ".pdf" || fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                    {
                        string strPath = Server.MapPath("~/Uploads");
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                        }


                        string pathToUpload = Path.Combine(strPath, DocumentNo + fileName.ToUpper());

                        if (File.Exists(pathToUpload))
                        {
                            File.Delete(pathToUpload);
                        }
                        fuClaimDocs.SaveAs(pathToUpload);
                        webportals.SaveMemoAttchmnts(DocumentNo, pathToUpload, fileName.ToUpper(), username);

                        Stream fs = fuClaimDocs.PostedFile.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        webportals.RegFileUploadAtt(DocumentNo, fileName.ToUpper(), base64String, 52178720, "Claim Requisition");

                        BindAttachedDocuments(DocumentNo);
                        Message("Document uploaded successfully!");
                    }
                    else
                    {
                        Message("Please upload files with .pdf, .png, .jpg and .jpeg extensions only!");
                        return;
                    }
                }
                else
                {
                    Message("Please upload a file!");
                    return;
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }*/
        }
            protected void lbtnRemoveAttach_Click(object sender, EventArgs e)
        {
            try
            {
                //string documentNo = Session["PettyCashNo"].ToString();
                string documentNo = Request.QueryString["ClaimNo"];
                // string documentNo = lblLNo.Text;
                string[] args = new string[2];
                args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                string systemId = args[0];
                if (Components.ObjNav.DeleteDocumentAttachments(systemId))
                {
                    Message("Document deleted successfully!");
                    BindAttachedDocuments(documentNo);

                }
                else
                {
                    Message("An error occured while deleting document. Please try again later!");
                    return;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

       protected void lbtnRemoveAttach_Click1(object sender, EventArgs e)
        {/* 
            string status = Request.QueryString["status"].ToString().Replace("%", " ");
            if (status == "Open" || status == "Pending")
            {
                string[] args = new string[2];
                args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                string systemId = args[0];
                string documentNo = lblLNo.Text;
                string fileName = string.Empty;
                string documentDetails = webportals.GetAttachmentDetails(systemId);
                if (documentDetails != null)
                {
                    string[] documentsDetailsArr = documentDetails.Split(strLimiters, StringSplitOptions.None);
                    fileName = documentsDetailsArr[1].Split('.')[0];
                }

                string response = webportals.DeleteDocumentAttachment(systemId, fileName, documentNo);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg1 = responseArr[0];
                    string returnMsg2 = responseArr[1];
                    if (returnMsg1 == "SUCCESS" && returnMsg2 == "SUCCESS")
                    {
                        Message("Document deleted successfully.");
                        BindAttachedDocuments(documentNo);
                    }
                    else
                    {
                        Message("An error has occured. Please try again later.");
                        return;
                    }
                }
            }
            else
            {
                Message("You can only edit an open document!");
                return;
            }*/
        }
        }
}